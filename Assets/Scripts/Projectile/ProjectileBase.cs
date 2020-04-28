using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    protected Vector3 shootDirection = Vector3.right;
    bool shot = false;
    [Header("Base Projectile Parameters")]
    [SerializeField] protected LayerMask checkMask = default;

    public virtual void ShootProjectile(Vector3 direction)
    {
        shootDirection = direction;
        shot = true;
    }

    public abstract void UpdateTrajectory();

    public abstract void HandleCollision(RaycastHit hit);

    public virtual void Update()
    {
        UpdateTrajectory();
    }
}
