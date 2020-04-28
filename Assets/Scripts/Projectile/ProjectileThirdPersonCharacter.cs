using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThirdPersonCharacter : ProjectileBase
{
    [Header("Player Projectile")]
    [SerializeField] SphereCollider projectileCollider = default;
    [SerializeField] float projectileSpeed = 20f;
    Vector3 beforeMovementPosition = Vector3.zero;

    public override void UpdateTrajectory()
    {
        Vector3 nextMovement = shootDirection * projectileSpeed * Time.deltaTime;

        RaycastHit hitOnWay = CheckForObjectOnTrajectory(nextMovement);

        if (!hitOnWay.collider)
        {
            transform.position += nextMovement;
        }
    }

    public RaycastHit CheckForObjectOnTrajectory(Vector3 nextMovement)
    {
        RaycastHit hitOnWay = new RaycastHit();

        if (Physics.SphereCast(transform.position + projectileCollider.center, projectileCollider.radius, nextMovement, out hitOnWay, nextMovement.magnitude, checkMask))
        {
            HandleCollision(hitOnWay.collider, hitOnWay);
        }
        return hitOnWay;
    }

    public override void HandleCollision(Collider hitCollider, RaycastHit hit)
    {
        Debug.DrawRay(hit.point, hit.normal, Color.red, 0.2f);
        DestroyProjectile();
    }
}
