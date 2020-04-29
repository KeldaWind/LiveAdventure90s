using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyBarrel : ProjectileBase
{
    private void Start()
    {
        barrelRenderer.material = normalMaterial;
        lifeSystem.OnLifeReachedZero += DestroyProjectile;
        lifeSystem.OnReceivedDamages += PlayHitFeedback;
    }

    public override void Update()
    {
        UpdateVerticalMovementValues();
        base.Update();
    }

    private void FixedUpdate()
    {
        UpdatePhysics();
    }

    [Header("Physics and Movements")]
    [SerializeField] BoxCollider barrelCollider = default;
    [SerializeField] LayerMask movementsCheckMask = default;
    [SerializeField] float gravity = -30f;
    [SerializeField] float maxVerticalDownSpeed = -30f;
    [SerializeField] float skinWidthMultiplier = 0.99f;
    [SerializeField] float onGroundCheckDistance = 0.1f;
    float currentHorizontalSpeed = 0f;
    float currentVerticalSpeed = 0f;
    bool isOnGround = false;

    Vector3 beforeMovementPosition = Vector3.zero;

    bool IsOnGround => isOnGround;

    [Header("Barrel")]
    [SerializeField] float shootForce = 10f;
    [SerializeField] DamageableEntity lifeSystem = default;

    [Header("Rendering")]
    [SerializeField] Renderer barrelRenderer = default;
    [SerializeField] Material normalMaterial = default;
    [SerializeField] Material hitMaterial = default;
    [SerializeField] float hitFeedbackDuration = 0.05f;

    public override void ShootProjectile(Vector3 direction, GameObject instigator)
    {
        base.ShootProjectile(direction, instigator);

        currentHorizontalSpeed = direction.x * shootForce;
        currentVerticalSpeed = direction.y * shootForce;
    }

    public override void HandleCollision(Collider collider, Collision collision)
    {
        GameObject hitObject = collider.gameObject;

        if (shootInstigator == hitObject)
            return;

        bool mustDestroy = false;
        bool preventDestroyOnWall = false;

        ProjectileBase hitProjectile = hitObject.GetComponent<ProjectileBase>();
        if (hitProjectile)
        {
            preventDestroyOnWall = true;
        }

        if (!preventDestroyOnWall && collision != null)
        {
            Vector3 averageNormal = Vector3.zero;
            ContactPoint[] points = collision.contacts;
            foreach(ContactPoint point in points)
            {
                averageNormal += point.normal;
            }
            averageNormal /= points.Length;

            if (Mathf.Abs(Vector3.Dot(averageNormal, Vector3.up)) < 0.5f)
            {
                mustDestroy = true;
            }
        }

        DamageableEntity hitDamageableEntity = hitObject.GetComponent<DamageableEntity>();
        if (hitDamageableEntity)
        {
            if (hitDamageableEntity.GetDamageTag != damageTag && damageTag != DamageTag.Environment)
                hitDamageableEntity.ReceiveDamage(projectileDamages, gameObject);

            mustDestroy = true;
        }

        if (mustDestroy)
        {
            DestroyProjectile();
        }
    }

    public override void UpdateTrajectory()
    {
        #region OLD
        /*Vector3 movement = new Vector3(currentHorizontalSpeed, currentVerticalSpeed, 0) * Time.deltaTime;
        Vector3 initialMovement = movement;

        if (movement.x != 0)
            movement.x = barrelRaycaster.RaycastHorizontal(movement.x);
        else
            barrelRaycaster.ResetLastHorizontalHitResult();
        if (Mathf.Abs(movement.x) < movementThreshold)
            movement.x = 0;

        if (movement.y != 0)
            movement.y = barrelRaycaster.RaycastVertical(movement.y);
        else
            barrelRaycaster.ResetLastVerticalHitResult();
        if (Mathf.Abs(movement.y) < movementThreshold)
            movement.y = 0;

        if (movement.x > 0) barrelRaycaster.flags.left = false;
        if (movement.x < 0) barrelRaycaster.flags.right = false;
        if (movement.y > 0) barrelRaycaster.flags.below = false;
        if (movement.y < 0) barrelRaycaster.flags.above = false;

        if (initialMovement != movement)
        {
            if (initialMovement.y < 0 && IsOnGround && currentVerticalSpeed < 0)
            {
                currentVerticalSpeed = 0;
            }
        }

        transform.Translate(movement);

        if (barrelRaycaster.GetLastHorizontalHitResult.collider)
        {
            HandleCollision(barrelRaycaster.GetLastHorizontalHitResult.collider, barrelRaycaster.GetLastHorizontalHitResult);
            mustBeDestroyed = true;
        }
        if (barrelRaycaster.GetLastVerticalHitResult.collider && barrelRaycaster.GetLastHorizontalHitResult.collider != barrelRaycaster.GetLastVerticalHitResult.collider)
        {
            HandleCollision(barrelRaycaster.GetLastVerticalHitResult.collider, barrelRaycaster.GetLastVerticalHitResult);
        }

        if (mustBeDestroyed)
        {
            DestroyProjectile();
        }*/
        #endregion
    }

    public void UpdateVerticalMovementValues()
    {
        if (!IsOnGround)
        {
            if (currentVerticalSpeed > maxVerticalDownSpeed)
            {
                currentVerticalSpeed = Mathf.Clamp(currentVerticalSpeed + gravity * Time.deltaTime, maxVerticalDownSpeed, currentVerticalSpeed);
            }
        }
    }

    public void UpdatePhysics()
    {
        CheckForGround();

        selfBody.velocity = new Vector3(currentHorizontalSpeed, currentVerticalSpeed, 0);
    }

    public void CheckForGround()
    {
        bool startOnGround = isOnGround;

        Vector3 actualSize = new Vector3(barrelCollider.size.x * transform.lossyScale.x, barrelCollider.size.y * transform.lossyScale.y, barrelCollider.size.z * transform.lossyScale.z) * skinWidthMultiplier;

        RaycastHit hit = new RaycastHit();
        isOnGround = Physics.BoxCast(transform.position + barrelCollider.center, actualSize * 0.5f, Vector3.down, out hit, transform.rotation, onGroundCheckDistance, movementsCheckMask);

        if (isOnGround && currentVerticalSpeed < 0)
            currentVerticalSpeed = 0;

        if (!startOnGround && isOnGround != startOnGround)
        {
            HandleCollision(hit.collider, null);
        }
    }

    public void PlayHitFeedback(int delta, int remainingLife, GameObject damageInstigator)
    {
        if (remainingLife > 0)
        {
            StartCoroutine(HitFeedbackCoroutine());
            PlayDamagedFeedback();
        }
    }
    public IEnumerator HitFeedbackCoroutine()
    {
        barrelRenderer.material = hitMaterial;
        yield return new WaitForSeconds(hitFeedbackDuration);
        barrelRenderer.material = normalMaterial;
    }

    [Header("Barrel Feedbacks")]
    [SerializeField] string damagedFxTag = "PlaceHolder";

    public void PlayDamagedFeedback()
    {
        // FEEDBACK : PLAY DAMAGED SOUND 
        FxManager.Instance.PlayFx(damagedFxTag, transform.position + Vector3.up, Quaternion.identity, Vector3.one * 0.5f);
    }
}
