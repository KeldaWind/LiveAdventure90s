using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    protected Vector3 shootDirection = Vector3.right;
    bool shot = false;
    [Header("Base Projectile Parameters")]
    [SerializeField] protected LayerMask checkMask = default;
    [SerializeField] protected int projectileDamages = 1;
    [SerializeField] protected DamageTag damageTag = DamageTag.Player;
    public DamageTag GetDamageTag => damageTag;

    public virtual void ShootProjectile(Vector3 direction)
    {
        shootDirection = direction;
        shot = true;
    }

    public abstract void UpdateTrajectory();

    public abstract void HandleCollision(Collider hitCollider, RaycastHit hit);

    public virtual void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    public virtual void Update()
    {
        if (shot)
            UpdateTrajectory();
    }
}
