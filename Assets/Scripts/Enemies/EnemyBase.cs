using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private void Start()
    {
        shootFrequenceSystem = new FrequenceSystem(projectilesPerSecond);
        shootFrequenceSystem.SetUp(CheckIfShoot);
        shootFrequenceSystem.Stop();
    }

    private void Update()
    {
        UpdateBehavior();
    }

    bool detectedPlayer;
    ThirdPersonController _thirdPersonCharacter = default;
    public void DetectFirstPersonCharacter(ThirdPersonController thirdPersonCharacter)
    {
        detectedPlayer = true;
        _thirdPersonCharacter = thirdPersonCharacter;

        if (shootFrequenceSystem.IsStopped)
        {
            ShootProjectile();
            shootFrequenceSystem.Resume();
        }
    }
    public void LosePlayer()
    {
        detectedPlayer = false;
    }

    public virtual void UpdateBehavior()
    {
        UpdateShooting();
    }

    #region Shoot
    [Header("Shooting")]
    [SerializeField] ProjectileBase enemyProjectilePrefab = default;
    [SerializeField] float projectilesPerSecond = 0.5f;
    [SerializeField] float shootAngle = 45f;
    [SerializeField] Transform rightShootPosition = default;
    [SerializeField] Transform leftShootPosition = default;

    FrequenceSystem shootFrequenceSystem = default;
    
    public void UpdateShooting()
    {
        if (!shootFrequenceSystem.IsStopped)
            shootFrequenceSystem.UpdateFrequence();
    }

    public void ShootProjectile()
    {
        ShootDirection shootDirectionEnum = GetShootDirection;
        Vector3 shootPosition = shootDirectionEnum == ShootDirection.Right ? rightShootPosition.position : leftShootPosition.position;
        Vector3 shootDirection = shootDirectionEnum == ShootDirection.Right ? Vector3.right : Vector3.left;

        shootDirection = Quaternion.Euler(0, 0, shootAngle * (shootDirectionEnum == ShootDirection.Right ? 1 : -1)) * shootDirection;

        /*print("SHOOT");
        Debug.DrawRay(shootPosition, shootDirection * 5.0f, Color.magenta, 0.25f);*///
        ProjectileBase newProj = Instantiate(enemyProjectilePrefab, shootPosition, Quaternion.identity);
        newProj.ShootProjectile(shootDirection);
    }

    public void CheckIfShoot()
    {
        if (detectedPlayer)
            ShootProjectile();
        else
            shootFrequenceSystem.Stop();
    }

    //[SerializeField]
    ShootDirection GetShootDirection
    {
        get
        {
            if (_thirdPersonCharacter != null)
            {
                lastShootDirection = (_thirdPersonCharacter.transform.position.x - transform.position.x > 0 ? ShootDirection.Right : ShootDirection.Left);
            }
            return lastShootDirection;
        }
    }
    ShootDirection lastShootDirection = ShootDirection.Left;
    #endregion
}
