using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyBarrel : ProjectileBase
{
    public override void Update()
    {
        if (mustBeDestroyed)
        {
            DestroyProjectile();
            return;
        }

        UpdateVerticalMovementValues();
        base.Update();
    }

    [Header("Physics and Movements")]
    [SerializeField] BoxCollider barrelCollider = default;
    [SerializeField] BoxRaycaster barrelRaycaster = default;
    [SerializeField] float gravity = -30f;
    [SerializeField] float maxVerticalDownSpeed = -30f;
    [SerializeField] float movementThreshold = 0.001f;
    [SerializeField] float onGroundCheckDistance = 0.1f;
    float currentHorizontalSpeed = 0f;
    float currentVerticalSpeed = 0f;

    Vector3 beforeMovementPosition = Vector3.zero;

    bool IsOnGround => barrelRaycaster.flags.below;

    [Header("Barrel")]
    [SerializeField] float shootForce = 10f;
    bool mustBeDestroyed = false;

    public override void ShootProjectile(Vector3 direction)
    {
        base.ShootProjectile(direction);

        currentHorizontalSpeed = direction.x * shootForce;
        currentVerticalSpeed = direction.y * shootForce;
    }

    public override void HandleCollision(Collider hitCollider, RaycastHit hit)
    {
        if (mustBeDestroyed)
            return;

        print("Barrel Collision : " + hitCollider.gameObject.name);
        DamageableEntity hitDamageableEntity = hitCollider.GetComponent<DamageableEntity>();
        if (hitDamageableEntity)
        {
            if (hitDamageableEntity.GetDamageTag != damageTag)
            {
                hitDamageableEntity.ReceiveDamage(projectileDamages);
                mustBeDestroyed = true;
            }
        }
    }

    public override void UpdateTrajectory()
    {
        Vector3 movement = new Vector3(currentHorizontalSpeed, currentVerticalSpeed, 0) * Time.deltaTime;
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
        }
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
        else
        {
            barrelRaycaster.CheckForGroundBelow(onGroundCheckDistance);
        }
    }
}
