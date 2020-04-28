using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
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
    [SerializeField] BoxCollider selfCollider = default;
    [SerializeField] BoxRaycaster boxRaycaster = default;
    [SerializeField] float movementThreshold = 0.01f;

    private void Start()
    {
        jumpDurationSystem = new TimerSystem(jumpMaxDuration, EndJumping);
        lateJumpDurationSystem = new TimerSystem(lateJumpDelay, null);
        extentJumpDurationSystem = new TimerSystem(extentJumpDelay, null);

        shootingFrequenceSystem = new FrequenceSystem(bulletsPerSecond);
        shootingFrequenceSystem.SetUp(CheckForShootAgain);
        shootingFrequenceSystem.Stop();

        boxRaycaster.OnLanded = CheckForExtentJump;
    }

    void Update()
    {
        UpdateShooting();
        HandleInputs();
        UpdateHorizontalMovementValues(currentHorizontalInput);
        UpdateVerticalMovementValues();

        Vector3 movement = new Vector3(currentHorizontalSpeed, currentVerticalSpeed, 0) * Time.deltaTime;

        Move(movement);
    }

    public void HandleInputs()
    {
        currentHorizontalInput = Input.GetAxis(horizontalAxis);
        currentHorizontalInput = (Mathf.Abs(currentHorizontalInput) > minimumAxisValueToConsiderHorizontalMovement) ? Mathf.Sign(currentHorizontalInput) : 0;

        if (currentHorizontalInput != 0)
        {
            currentShootDirection = currentHorizontalInput > 0 ? ShootDirection.Right : ShootDirection.Left;
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

        /*if(CanExtentJump && (GetJumpKeyUp))
        {
            InterruptExtentJumpDelay();
        }*/

        if ((Input.GetKeyDown(shootingGamepadInput) || Input.GetKeyDown(shootingKeyboardInput)))
        {
            isShootingInputDown = true;
            StartShooting();
        }
        else if ((Input.GetKeyUp(shootingGamepadInput) || Input.GetKeyUp(shootingKeyboardInput)))
        {
            isShootingInputDown = false;
        }
    }

    #region Global Movement
    void Move(Vector3 movement)
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

        /*if (boxRaycaster.GetLastHorizontalHitResult.collider)
        {
            ProjectileBase hitProjectile = boxRaycaster.GetLastHorizontalHitResult.collider.GetComponent<ProjectileBase>();
            if (hitProjectile != null)
            {
                hitProjectile.HandleCollision(selfCollider, new RaycastHit());
            }
        }
        if (boxRaycaster.GetLastVerticalHitResult.collider && boxRaycaster.GetLastHorizontalHitResult.collider != boxRaycaster.GetLastVerticalHitResult.collider)
        {
            ProjectileBase hitProjectile = boxRaycaster.GetLastVerticalHitResult.collider.GetComponent<ProjectileBase>();
            if (hitProjectile != null)
            {
                hitProjectile.HandleCollision(selfCollider, new RaycastHit());
            }
        }*/

        transform.Translate(movement);
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

    float currentVerticalSpeed = 0f;
    TimerSystem jumpDurationSystem = new TimerSystem();
    bool isJumping = false;
    public bool IsOnGround => boxRaycaster.flags.below == true;

    public void UpdateVerticalMovementValues()
    {
        UpdateJumpAssists();
        if (!isJumping)
        {
            if (IsOnGround)
            {
                boxRaycaster.CheckForGroundBelow(onGroundCheckDistance);
                if (!IsOnGround)
                    StartLateJumpDelay();
            }
            else
                currentVerticalSpeed = Mathf.Clamp(currentVerticalSpeed + verticalGravity * Time.deltaTime, maxVerticalDownVelocity, currentVerticalSpeed);
        }
        else
        {
            jumpDurationSystem.UpdateTimer();
        }
    }

    public void StartJumping()
    {
        isJumping = true;
        currentVerticalSpeed = jumpForce;
        jumpDurationSystem.StartTimer();

        if (CanLateJump)
            InterruptLateJumpDelay();

        if (CanExtentJump)
        {
            InterruptExtentJumpDelay();
            if (!GetJumpKey)
                EndJumping();
        }
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
    ShootDirection currentShootDirection = ShootDirection.Right;

    [SerializeField] float bulletsPerSecond = 10f;
    FrequenceSystem shootingFrequenceSystem = default;

    public void StartShooting()
    {
        if (shootingFrequenceSystem.IsStopped)
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
        Vector3 shootPosition = (currentShootDirection == ShootDirection.Right ? rightShootPosition : leftShootPosition).position;
        Quaternion shootRotation = (currentShootDirection == ShootDirection.Right ? rightShootPosition : leftShootPosition).rotation;
        ProjectileBase newProjectile = Instantiate(projectilePrefab, shootPosition, shootRotation);
        newProjectile.ShootProjectile(currentShootDirection == ShootDirection.Right ? Vector3.right : Vector3.left);
    }

    public void CheckForShootAgain()
    {
        if (isShootingInputDown)
            ShootProjectile();
        else
        {
            shootingFrequenceSystem.Stop();
            shootingFrequenceSystem.ResetFrequence();
        }
    }
    #endregion
}

public enum ShootDirection
{
    Right, Left
}
