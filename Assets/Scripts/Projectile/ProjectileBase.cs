using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    bool pendingKill = false;

    protected Vector3 shootDirection = Vector3.right;
    bool shot = false;
    protected GameObject shootInstigator = default;
    [Header("Base Projectile Parameters")]
    //[SerializeField] protected LayerMask checkMask = default;
    [SerializeField] protected Rigidbody selfBody = default;
    [SerializeField] protected int projectileDamages = 1;
    [SerializeField] protected DamageTag damageTag = DamageTag.Player;
    public DamageTag GetDamageTag => damageTag;

    public virtual void ShootProjectile(Vector3 direction, GameObject instigator)
    {
        shootDirection = direction;
        shot = true;
        shootInstigator = instigator;
    }

    public abstract void UpdateTrajectory();

    public abstract void HandleCollision(Collider collider, Collision collision);

    public virtual void DestroyProjectile()
    {
        if (pendingKill)
            return;

        pendingKill = true;
        Destroy(gameObject);
    }

    public virtual void Update()
    {
        if (shot)
            UpdateTrajectory();
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.collider, collision);
    }
}
