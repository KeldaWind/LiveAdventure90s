using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThirdPersonCharacter : ProjectileBase
{
    [Header("Player Projectile")]
    [SerializeField] SphereCollider projectileCollider = default;
    [SerializeField] float projectileSpeed = 20f;
    Vector3 beforeMovementPosition = Vector3.zero;

    [Header("Feedbacks")]
    [SerializeField] AudioManager.Sound defaultDestroySound = AudioManager.Sound.H_ImpactShootWall;

    public override void ShootProjectile(Vector3 direction, GameObject instigator)
    {
        base.ShootProjectile(direction, instigator);
        selfBody.velocity = shootDirection * projectileSpeed;
    }

    public override void UpdateTrajectory()
    {
        
    }

    public override void HandleCollision(Collider collider, Collision collision)
    {
        if (shootInstigator == collider.gameObject)
            return;

        GameObject hitObject = collider.gameObject;

        DamageableEntity hitDamageableEntity = hitObject.GetComponent<DamageableEntity>();
        if (hitDamageableEntity)
        {
            if (hitDamageableEntity.GetDamageTag != damageTag && damageTag != DamageTag.Environment)
                hitDamageableEntity.ReceiveDamage(projectileDamages, gameObject);
        }
        else
        {
            AudioManager.PlaySound(defaultDestroySound);
        }

        DestroyProjectile();
    }
}
