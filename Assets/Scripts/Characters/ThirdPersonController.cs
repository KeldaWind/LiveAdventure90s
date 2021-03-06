﻿using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Win")]
    [SerializeField] GameObject fireworksOnWin = default;
    [SerializeField] float fireworksDownDistance = 3;

    [Header("Inputs")]
    [SerializeField] string horizontalAxis = "ThirdPersonHorizontalAxis";
    [SerializeField] float minimumAxisValueToConsiderHorizontalMovement = 0.25f;
    [SerializeField] KeyCode jumpingGamepadInput = KeyCode.JoystickButton0;
    [SerializeField] KeyCode jumpingKeyboardInput = KeyCode.Space;
    [SerializeField] KeyCode shootingGamepadInput = KeyCode.JoystickButton2;
    [SerializeField] KeyCode shootingKeyboardInput = KeyCode.E;

    public bool GetJumpKeyDown => (Input.GetKeyDown(jumpingGamepadInput) || Input.GetKeyDown(jumpingKeyboardInput));
    public bool GetJumpKey => (Input.GetKey(jumpingGamepadInput) || Input.GetKey(jumpingKeyboardInput));
    public bool GetJumpKeyUp => (Input.GetKeyUp(jumpingGamepadInput) || Input.GetKeyUp(jumpingKeyboardInput));

    float currentHorizontalInput = 0f;
    bool isShootingInputDown = false;

    [Header("Collisions")]
    [SerializeField] Rigidbody selfBody = default;
    [SerializeField] BoxCollider selfCollider = default;
    [SerializeField] LayerMask movementsCheckMask = default;
    [SerializeField] float movementThreshold = 0.01f;
    [SerializeField] float skinWidthMultiplier = 0.99f;
    bool isOnGround = false;

    private void Start()
    {
        jumpDurationSystem = new TimerSystem(jumpMaxDuration, EndJumping);
        lateJumpDurationSystem = new TimerSystem(lateJumpDelay, null);
        extentJumpDurationSystem = new TimerSystem(extentJumpDelay, null);

        shootingFrequenceSystem = new FrequenceSystem(bulletsPerSecond);
        shootingFrequenceSystem.SetUp(CheckForShootAgain);
        shootingFrequenceSystem.Stop();

        SetUpLifeSystem();

        // characterRenderer.material = normalMaterial;
        SetUpRenderer();

        /*walkStepFrequenceSystem = new FrequenceSystem(stepFeedbackPerSecond);
        walkStepFrequenceSystem.SetUp(PlayFootFeedbackSound);*/

        //AudioManager.PlayAmbianceMusic();
        charaFlower.SetCurrentGlobalTarget(currentShootDirection == ShootDirection.Left ? leftShootPosition : rightShootPosition, currentShootDirection);
    }

    void Update()
    {
        UpdateShooting();
        HandleInputs();
        UpdateHorizontalMovementValues(currentHorizontalInput);
        UpdateVerticalMovementValues();
        UpdateRecovering();
        UpdateAnimatorValues();

        /*if (Input.GetKeyDown(KeyCode.D))
        {
            characterAnimator.SetBool("IsDead", true);
        }*/

        /*if (Input.GetKeyDown(KeyCode.W))
        {
            AudioManager.PlayWinMusic();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            AudioManager.PlayLoseMusic();
        }*/
    }

    private void FixedUpdate()
    {
        
        UpdatePhysics();
    }

    bool won = false;
    public void Win()
    {
        won = true;
        characterAnimator.SetBool("Won", true);
        lifeSystem.SetImmuneToDamages();

        isShootingInputDown = false;
        currentHorizontalInput = 0;

        if (fireworksOnWin)
        {
            Instantiate(fireworksOnWin, new Vector3(0, transform.position.y - fireworksDownDistance, 0), Quaternion.identity);
        }
    }
    public void HandleInputs()
    {
        if(dead || won)
        {
            return;
        }

        if ((Input.GetKeyDown(shootingGamepadInput) || Input.GetKeyDown(shootingKeyboardInput)))
        {
            isShootingInputDown = true;
            if (!IsStunned)
                StartShooting();
        }
        else if ((Input.GetKeyUp(shootingGamepadInput) || Input.GetKeyUp(shootingKeyboardInput)))
        {
            isShootingInputDown = false;
        }

        if (IsStunned)
        {
            currentHorizontalInput = 0;
            return;
        }

        currentHorizontalInput = Input.GetAxis(horizontalAxis);
        currentHorizontalInput = (Mathf.Abs(currentHorizontalInput) > minimumAxisValueToConsiderHorizontalMovement) ? Mathf.Sign(currentHorizontalInput) : 0;

        if (currentHorizontalInput != 0)
        {
            ShootDirection previous = currentShootDirection;
            currentShootDirection = currentHorizontalInput > 0 ? ShootDirection.Right : ShootDirection.Left;
            if(previous != currentShootDirection)
            {
                charaFlower.SetCurrentGlobalTarget(currentShootDirection == ShootDirection.Left ? leftShootPosition : rightShootPosition, currentShootDirection);
            }
        }

        if (GetJumpKeyDown)
        {
            if(IsOnGround || CanLateJump)
                StartJumping();
            else
                StartExtentJumpDelay();
        }
        else if (GetJumpKeyUp && isJumping)
        {
            EndJumping();
        }        
    }

    #region Global Movement - OLD
    /* void Move(Vector3 movement)
     {
         Vector3 initialMovement = movement;

         if (movement.x != 0)
             movement.x = boxRaycaster.RaycastHorizontal(movement.x);
         else
             boxRaycaster.ResetLastHorizontalHitResult();
         if (Mathf.Abs(movement.x) < movementThreshold)
             movement.x = 0;

         if (movement.y != 0)
             movement.y = boxRaycaster.RaycastVertical(movement.y);
         else
             boxRaycaster.ResetLastVerticalHitResult();
         if (Mathf.Abs(movement.y) < movementThreshold)
             movement.y = 0;

         if (movement.x > 0) boxRaycaster.flags.left = false;
         if (movement.x < 0) boxRaycaster.flags.right = false;
         if (movement.y > 0) boxRaycaster.flags.below = false;
         if (movement.y < 0) boxRaycaster.flags.above = false;

         if (initialMovement != movement)
         {
             if (boxRaycaster.flags.right && initialMovement.x > 0)
             {
                 currentHorizontalSpeed = 0;
             }
             else if (boxRaycaster.flags.left && initialMovement.x < 0)
             {
                 currentHorizontalSpeed = 0;
             }

             if (initialMovement.y < 0 && IsOnGround && currentVerticalSpeed < 0)
             {
                 currentVerticalSpeed = 0;
             }

             if (initialMovement.y > 0 && boxRaycaster.flags.above)
             {
                 currentVerticalSpeed = 0;
             }
         }

         transform.Translate(movement);
     }*/
    #endregion

    #region Global Movement
    public void UpdatePhysics()
    {
        CheckForGround();
        if (currentVerticalSpeed > 0)
            CheckForCeiling();

        Vector3 movementBoost = Vector3.zero;

        selfBody.velocity = new Vector3(currentHorizontalSpeed, currentVerticalSpeed, 0) + movementBoost;

        /*if (IsWalking)
            walkStepFrequenceSystem.UpdateFrequence();*/
    }

    RaycastHit currentGround = new RaycastHit();
    Following_Plateform currentStepingOnFollowingPlatform = default;
    float previousPlatformHeight = 0;
    public void CheckForGround()
    {
        if (currentStepingOnFollowingPlatform)
        {
            float newHeight = currentStepingOnFollowingPlatform.transform.position.y;
            float diff = newHeight - previousPlatformHeight;
            transform.position += Vector3.up * diff;
            previousPlatformHeight = newHeight;

            // Patch for repeat landing bug
            if (currentVerticalSpeed < 2f)
                currentVerticalSpeed = 0;
        }

        bool startIsOnGround = isOnGround;

        Vector3 actualSize = new Vector3(selfCollider.size.x * transform.lossyScale.x, selfCollider.size.y * transform.lossyScale.y, selfCollider.size.z * transform.lossyScale.z) * skinWidthMultiplier;

        Collider previousCollider = currentGround.collider;
        RaycastHit hit = new RaycastHit();
        isOnGround = Physics.BoxCast(
            transform.position + selfCollider.center, 
            actualSize * 0.5f, 
            Vector3.down, 
            out hit, 
            transform.rotation, 
            onGroundCheckDistance, 
            movementsCheckMask);

        if (isOnGround && currentVerticalSpeed < 0)
        {
            currentVerticalSpeed = 0;
            CheckForExtentJump();
        }

        if (startIsOnGround && startIsOnGround != isOnGround)
        {
            StartLateJumpDelay();
            characterAnimator.SetBool("IsGrounded", isOnGround);
        }

        if (isOnGround && startIsOnGround != isOnGround)
        {
            PlayOnLandedFeedback();
            characterAnimator.SetBool("IsGrounded", isOnGround);
        }

        if (IsOnGround)
        {
            //print("YES");
            if (previousCollider != hit.collider)
            {
                Following_Plateform previousFollowingPlatform = currentStepingOnFollowingPlatform;

                HandleCollision(null, hit.collider);
                currentGround = hit;
                currentStepingOnFollowingPlatform = currentGround.collider.GetComponent<Following_Plateform>();
                if (currentStepingOnFollowingPlatform)
                {
                    previousPlatformHeight = currentStepingOnFollowingPlatform.transform.position.y;
                    currentStepingOnFollowingPlatform.SetCharacterOn(this);
                }
                else
                {
                    if (previousFollowingPlatform)
                    {
                        previousFollowingPlatform.SetCharacterOn(null);
                    }
                }
            }
        }
        else
        {
            //print("NO");
            currentGround = new RaycastHit();
            if (currentStepingOnFollowingPlatform)
            {
                currentStepingOnFollowingPlatform.SetCharacterOn(null);
                currentStepingOnFollowingPlatform = null;
            }
        }
    }

    public void CheckForCeiling()
    {
        Vector3 actualSize = new Vector3(selfCollider.size.x * transform.lossyScale.x, selfCollider.size.y * transform.lossyScale.y, selfCollider.size.z * transform.lossyScale.z) * skinWidthMultiplier;
        bool hitCeiling = Physics.BoxCast(transform.position + selfCollider.center, actualSize * 0.5f, Vector3.up, transform.rotation, ceilingCheckDistance, movementsCheckMask);
        if (hitCeiling)
            currentVerticalSpeed = 0;
    }
    #endregion

    #region Horizontal Movement
    [Header("Horizontal Movement")]
    [SerializeField] float maxHorizontalSpeed = 5f;
    [SerializeField] float horizontalAcceleration = 20f;
    [SerializeField] float horizontalAccelerationTurningBack = 20f;
    [SerializeField] float horizontalDeceleration = 20f;
    [SerializeField] float airControl = 0.75f;
    [SerializeField] float airDrag = 0.1f;
    float currentHorizontalSpeed = 0f;

    public void UpdateHorizontalMovementValues(float input)
    {
        if (input == 0)
        {
            if (IsStunned)
                return;

            currentHorizontalSpeed = Mathf.Clamp(
                currentHorizontalSpeed - Mathf.Sign(currentHorizontalSpeed) * horizontalDeceleration * Time.deltaTime * (IsOnGround ? 1 : airDrag),
                currentHorizontalSpeed > 0 ? 0 : currentHorizontalSpeed,
                currentHorizontalSpeed < 0 ? 0 : currentHorizontalSpeed);
        }
        else
        {
            float usedAcceleration = Mathf.Sign(currentHorizontalSpeed) == input ? horizontalAcceleration : horizontalAccelerationTurningBack;
            currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed + usedAcceleration * Time.deltaTime * input * (IsOnGround ? 1 : airControl),
                -maxHorizontalSpeed, maxHorizontalSpeed);
        }
    }
    #endregion

    #region Jump and Gravity
    [Header("Jump and Gravity")]
    [SerializeField] float jumpForce = 20.0f;
    [SerializeField] float jumpMaxDuration = 0.3f;
    [SerializeField] float verticalGravity = -9.81f;
    [SerializeField] float maxVerticalDownVelocity = -20.0f;
    [SerializeField] float onGroundCheckDistance = 0.1f;
    [SerializeField] float ceilingCheckDistance = 0.05f;

    float currentVerticalSpeed = 0f;
    TimerSystem jumpDurationSystem = new TimerSystem();
    bool isJumping = false;
    //public bool IsOnGround => boxRaycaster.flags.below == true;
    public bool IsOnGround => isOnGround;

    public void UpdateVerticalMovementValues()
    {
        UpdateJumpAssists();
        if (!isJumping)
        {

            currentVerticalSpeed = Mathf.Clamp(currentVerticalSpeed + verticalGravity * Time.deltaTime, maxVerticalDownVelocity, currentVerticalSpeed);
        }
        else
        {
            jumpDurationSystem.UpdateTimer();
        }
    }

    public void ExpulsePlayerFromPlatform(float force)
    {
        if (currentVerticalSpeed < force)
            currentVerticalSpeed = force;
    }

    public void StartJumping()
    {
        isJumping = true;
        currentVerticalSpeed = jumpForce;

        if (currentStepingOnFollowingPlatform)
        {
            float platformSpeed = currentStepingOnFollowingPlatform.GetCurrentVerticalMovementSpeed.y;
            if (platformSpeed > 0)
            {
                currentVerticalSpeed += platformSpeed;
            }
        }

        jumpDurationSystem.StartTimer();

        if (CanLateJump)
            InterruptLateJumpDelay();

        if (CanExtentJump)
        {
            InterruptExtentJumpDelay();
            if (!GetJumpKey)
                EndJumping();
        }

        PlayJumpFeedback();
    }

    public void EndJumping()
    {
        isJumping = false;
    }

    #region Jump Assists
    [Header("Jump Assists")]
    [Tooltip("Duration during which the jump input is consider before jumping")]
    [SerializeField] float extentJumpDelay = 0.2f;
    [Tooltip("Duration during which the player can still jump after getting down a ledge without jumping")]
    [SerializeField] float lateJumpDelay = 0.2f;

    TimerSystem extentJumpDurationSystem = new TimerSystem();
    TimerSystem lateJumpDurationSystem = new TimerSystem();

    public bool CanExtentJump => !extentJumpDurationSystem.TimerOver;
    public bool CanLateJump => !lateJumpDurationSystem.TimerOver;

    public void UpdateJumpAssists()
    {
        if (!lateJumpDurationSystem.TimerOver)
        {
            lateJumpDurationSystem.UpdateTimer();
        }

        if (!extentJumpDurationSystem.TimerOver)
        {
            extentJumpDurationSystem.UpdateTimer();
        }
    }

    public void StartLateJumpDelay()
    {
        lateJumpDurationSystem.StartTimer();
    }

    public void InterruptLateJumpDelay()
    {
        lateJumpDurationSystem.EndTimer();
    }

    public void StartExtentJumpDelay()
    {
        extentJumpDurationSystem.StartTimer();
    }

    public void InterruptExtentJumpDelay()
    {
        extentJumpDurationSystem.EndTimer();
    }

    public void CheckForExtentJump()
    {
        if (CanExtentJump)
        {
            StartJumping();
        }
    }
    #endregion
    #endregion

    #region Shooting
    [Header("Shooting")]
    [SerializeField] ProjectileBase projectilePrefab = default;
    [SerializeField] Transform leftShootPosition = default;
    [SerializeField] Transform rightShootPosition = default;
    [SerializeField] float shootRandomAngle = 8f;
    [SerializeField] float shootRecoil = 5f;
    ShootDirection currentShootDirection = ShootDirection.Right;

    [SerializeField] float bulletsPerSecond = 10f;
    FrequenceSystem shootingFrequenceSystem = default;

    public System.Action OnPlayerShotProjectile = default;

    [SerializeField] ThirdPersonCharacterFlower charaFlower = default;

    public void StartShooting()
    {
        if (shootingFrequenceSystem.IsStopped && !dead && !won)
        {
            ShootProjectile();
            shootingFrequenceSystem.Resume();
        }
    }

    public void UpdateShooting()
    {
        if (!shootingFrequenceSystem.IsStopped)
        {
            shootingFrequenceSystem.UpdateFrequence();
        }
    }

    public void ShootProjectile()
    {
        float randomAngle = Random.Range(-shootRandomAngle, shootRandomAngle);
        Vector3 shootDirection = currentShootDirection == ShootDirection.Right ? Vector3.right : Vector3.left;
        shootDirection = Quaternion.Euler(0, 0, randomAngle) * shootDirection;

        //Vector3 shootPosition = (currentShootDirection == ShootDirection.Right ? rightShootPosition : leftShootPosition).position;
        Vector3 shootPosition = charaFlower.GetCurrentShootPosition;        
        
        //Quaternion shootRotation = (currentShootDirection == ShootDirection.Right ? rightShootPosition : leftShootPosition).rotation;
        Quaternion shootRotation = Quaternion.Euler(0, 0, currentShootDirection == ShootDirection.Right ? 0 : 180);
        ProjectileBase newProjectile = Instantiate(projectilePrefab, shootPosition, shootRotation);
        newProjectile.ShootProjectile(shootDirection, gameObject);

        currentHorizontalSpeed += shootRecoil * (currentShootDirection == ShootDirection.Right ? -1 : 1);

        PlayShootFeedback();

        charaFlower.SetShooting();
        OnPlayerShotProjectile?.Invoke();
    }

    public void CheckForShootAgain()
    {
        if (isShootingInputDown && !IsStunned && !dead && !won)
            ShootProjectile();
        else
        {
            shootingFrequenceSystem.Stop();
            shootingFrequenceSystem.ResetFrequence();
            charaFlower.ResetShooting();
        }
    }
    #endregion

    #region Collisions
    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision, collision.collider);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (checkCollisionAgain)
        {
            HandleCollision(collision, collision.collider);

            checkCollisionAgain = false;
        }
    }

    public void HandleCollision(Collision collision, Collider hitCollider)
    {
        EnemyBase hitEnemy = hitCollider.GetComponent<EnemyBase>();
        if (hitEnemy)
        {
            lifeSystem.ReceiveDamage(hitEnemy.GetMeleeDamages, hitEnemy.gameObject);
        }
        else
        {
            Laser_Behaviour hitLaser = hitCollider.GetComponentInParent<Laser_Behaviour>();
            if (hitLaser)
            {
                bool alreadyRecovering = IsRecovering;
                lifeSystem.ReceiveDamage(hitLaser.laserDamages, hitLaser.gameObject);
                //hitLaser.DeactivateForDuration(onDamagedRecoveringDuration);

                #region Handle knockback direction and laser deactivation
                if (!alreadyRecovering)
                {
                    if (collision != null)
                    {
                        Vector3 averagePosition = Vector3.zero;
                        Vector3 averageNormal = Vector3.zero;
                        float count = collision.contactCount;
                        foreach (ContactPoint point in collision.contacts)
                        {
                            averagePosition += point.point;
                            averageNormal += point.normal;
                        }
                        averagePosition /= count;
                        averageNormal /= count;

                        float horizontalDot = Vector3.Dot(Vector3.right, averageNormal);
                        float verticalDot = Vector3.Dot(Vector3.up, averageNormal);

                        currentHorizontalSpeed = onDamagedHorizontalSpeed *  Mathf.Sign(horizontalDot);
                        currentVerticalSpeed = onDamagedVerticalSpeed * Mathf.Sign(verticalDot);

                        if(verticalDot > 0.5f)
                        {
                            hitLaser.DeactivateForDuration(onDamagedRecoveringDuration);
                        }
                    }
                    else
                    {

                        hitLaser.DeactivateForDuration(onDamagedRecoveringDuration);
                    }
                }
                #endregion
            }
        }
    }
    #endregion

    #region Life Management and Debug
    [Header("Life System")]
    [SerializeField] DamageableEntity lifeSystem = default;
    [SerializeField] float onDamagedHorizontalSpeed = 6f;
    [SerializeField] float onDamagedVerticalSpeed = 12f;
    [SerializeField] float onDamagedStunDuration = 0.1f;
    [SerializeField] float onDamagedRecoveringDuration = 0.5f;
    [SerializeField] float recoveringBlinkingFrequence = 10f;

    public DamageableEntity GetLifeSystem => lifeSystem;
    TimerSystem stunTimer = new TimerSystem();
    TimerSystem recoveringTimer = new TimerSystem();

    bool IsStunned => !stunTimer.TimerOver;
    bool IsRecovering => !recoveringTimer.TimerOver;

    public void SetUpLifeSystem()
    {
        lifeSystem.OnReceivedDamages = OnReceivedDamages;
        lifeSystem.OnLifeReachedZero = Die;

        stunTimer = new TimerSystem(onDamagedStunDuration, null);
        recoveringTimer = new TimerSystem(onDamagedRecoveringDuration, EndRecover);
    }

    public void UpdateRecovering()
    {
        if (IsStunned)
            stunTimer.UpdateTimer();

        if (IsRecovering)
        {
            recoveringTimer.UpdateTimer();
            if (IsRecovering)
            {
                float blinkingCoeff = Mathf.Cos(recoveringTimer.GetTimerCounter * Mathf.PI * 2 * recoveringBlinkingFrequence);
                /*characterRenderer.material = blinkingCoeff > 0 ? blinkingMaterial : normalMaterial;*/
                foreach (RendererWithBaseMaterial parameters in rendererWithMaterials)
                {
                    parameters.renderer.material = blinkingCoeff > 0 ? blinkingMaterial : parameters.normalMtl;
                }
            }
            else
            {
                foreach (RendererWithBaseMaterial parameters in rendererWithMaterials)
                {
                    parameters.renderer.material = parameters.normalMtl;
                }
            }
        }
    }

    bool checkCollisionAgain = false;
    public void EndRecover()
    {
        lifeSystem.ResetCanReceiveDamages();
        checkCollisionAgain = true;

        if (currentGround.collider)
            HandleCollision(null, currentGround.collider);

        if (isShootingInputDown)
        {
            StartShooting();
        }
    }

    public System.Action OnCharacterReceivedDamage = default;
    public void OnReceivedDamages(int delta, int remainingLife, GameObject damageInstigator)
    {
        //print("Remaining life : " + remainingLife);
        if (isJumping)
        {
            EndJumping();
            if (!jumpDurationSystem.TimerOver)
                jumpDurationSystem.EndTimer();
        }

        float xOffset = transform.position.x - damageInstigator.transform.position.x;
        currentHorizontalSpeed = onDamagedHorizontalSpeed * Mathf.Sign(xOffset);
        currentVerticalSpeed = onDamagedVerticalSpeed;

        lifeSystem.SetImmuneToDamages();

        stunTimer.StartTimer();
        recoveringTimer.StartTimer();

        PlayDamagedFeedback();
        OnCharacterReceivedDamage?.Invoke();
    }

    bool dead = false;
    public void Die()
    {
        if (dead)
            return;
        dead = true;
        isShootingInputDown = false;
        currentHorizontalInput = 0;

        characterAnimator.SetBool("IsDead", true);
        GameManager.Instance.GameOver();
        //print("I'm die");
    }
    #endregion

    [Header("Rendering")]
    [SerializeField] Renderer[] characterRenderers = new Renderer[0];
    [SerializeField] Material normalMaterial = default;
    [SerializeField] Material blinkingMaterial = default;
    [SerializeField] Animator characterAnimator = default;

    struct RendererWithBaseMaterial { public Renderer renderer; public Material normalMtl; }
    RendererWithBaseMaterial[] rendererWithMaterials = new RendererWithBaseMaterial[0];
    public void SetUpRenderer()
    {
        rendererWithMaterials = new RendererWithBaseMaterial[characterRenderers.Length];
        for (int i =0; i < characterRenderers.Length; i++)
        {
            Renderer render = characterRenderers[i];
            RendererWithBaseMaterial parameters = new RendererWithBaseMaterial();
            parameters.renderer = render;
            parameters.normalMtl = render.material;
            rendererWithMaterials[i] = parameters;
        }
    }

    float minSpeedToAnimateMovement = 5f;
    public void UpdateAnimatorValues()
    {
        characterAnimator.SetBool("IsMoving", Mathf.Abs(currentHorizontalSpeed) > minSpeedToAnimateMovement);
        characterAnimator.SetFloat("VerticalSpeed",  isOnGround ? 0 : currentVerticalSpeed);
        characterAnimator.transform.localRotation = Quaternion.Euler(0, currentShootDirection == ShootDirection.Left ? -90 : 90, 0);
    }

    [Header("Feedbacks")]
    [SerializeField] string damagedFxTag = "PlaceHolder";
    [SerializeField] AudioManager.Sound damagedSound = AudioManager.Sound.H_HeroTakeDamage;
    [SerializeField] string shootFxTag = "PlaceHolderShoot";
    [SerializeField] AudioManager.Sound shootSound = AudioManager.Sound.H_GunShoot;
    [SerializeField] string landingFxTag = "PlaceHolderShoot";
    [SerializeField] AudioManager.Sound landingSound = AudioManager.Sound.H_HeroJumpLanding;
    [SerializeField] string jumpFxTag = "PlaceHolderShoot";
    [SerializeField] AudioManager.Sound jumpSound = AudioManager.Sound.H_HeroJump;
    [SerializeField] string walkFxTag = "PlaceHolderShoot";
    [SerializeField] float stepFeedbackPerSecond = 2.5f;

    public void PlayDamagedFeedback()
    {
        AudioManager.PlaySound(damagedSound);
        FxManager.Instance.PlayFx(damagedFxTag, transform.position + Vector3.up, Quaternion.identity, Vector3.one * 3);
    }

    public void PlayShootFeedback()
    {
        Vector3 pos = charaFlower.GetCurrentShootPosition;
        Quaternion rot = Quaternion.Euler(0, currentShootDirection == ShootDirection.Left ? 180 : 0, 0);

        AudioManager.PlaySound(shootSound);
        FxManager.Instance.PlayFx(shootFxTag, pos, rot, Vector3.one);
    }

    public void PlayOnLandedFeedback()
    {
        AudioManager.PlaySound(landingSound);
        FxManager.Instance.PlayFx(landingFxTag, transform.position, Quaternion.identity, Vector3.one);
    }

    public void PlayJumpFeedback()
    {
        AudioManager.PlaySound(jumpSound);
        FxManager.Instance.PlayFx(jumpFxTag, transform.position, Quaternion.identity, Vector3.one);
        characterAnimator.SetTrigger("StartJump");
    }

    public void PlayFootFeedbackSound()
    {
        WalkingDirection direction = CurrentWalkingDirection;

        // FEEDBACK : PLAY WALK SOUND 
        FxManager.Instance.PlayFx(walkFxTag, transform.position, Quaternion.Euler(0, 0, direction == WalkingDirection.Right ? 135 : 45), Vector3.one);
    }

    FrequenceSystem walkStepFrequenceSystem = new FrequenceSystem(1);

    #region Values for animation
    public WalkingDirection CurrentWalkingDirection => IsWalking ? (currentHorizontalSpeed > 0f ? WalkingDirection.Right : WalkingDirection.Left) : WalkingDirection.Neutral;
    public bool IsWalking => IsOnGround && Mathf.Abs(currentHorizontalSpeed) > 0f;
    #endregion

    public void Respawn(Transform respawnPos, ShootDirection direction)
    {
        dead = false;
        characterAnimator.SetBool("IsDead", dead);

        transform.position = new Vector3(respawnPos.position.x, respawnPos.position.y, transform.position.z);
        currentShootDirection = direction;
        charaFlower.SetCurrentGlobalTarget(currentShootDirection == ShootDirection.Left ? leftShootPosition : rightShootPosition, currentShootDirection);
        lifeSystem.ResetLife();
    }
}

public enum ShootDirection
{
    Right, Left
}

public enum WalkingDirection
{
    Left, Neutral, Right
}