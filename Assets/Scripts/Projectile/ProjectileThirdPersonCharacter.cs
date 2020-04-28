using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThirdPersonCharacter : ProjectileBase
{
    [Header("Player Projectile")]
    [SerializeField] SphereCollider projectileCollider = default;
    [SerializeField] float projectileSpeed = 20f;

    public override void UpdateTrajectory()
    {
        Vector3 nextMovement = shootDirection * projectileSpeed * Time.deltaTime;

        RaycastHit hitOnWay = CheckForObjectOnTrajectory(nextMovement);

        if (hitOnWay.collider != null)
        {
            HandleCollision(hitOnWay);
        }
        else
        {
            transform.position += nextMovement;
        }
    }

    public RaycastHit CheckForObjectOnTrajectory(Vector3 nextMovement)
    {
        RaycastHit hitOnWay = new RaycastHit();

        if (Physics.SphereCast(transform.position + projectileCollider.center, projectileCollider.radius, nextMovement, out hitOnWay, nextMovement.magnitude, checkMask))
        {

        }
        return hitOnWay;
    }

    public override void HandleCollision(RaycastHit hit)
    {
        Destroy(gameObject);
        Debug.DrawRay(hit.point, hit.normal, Color.red, 0.2f);
    }
}
