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

        lifeSystem.OnReceivedDamages += PlayHitFeedback;
        lifeSystem.OnLifeReachedZero += Die;
        //enemyRenderer.material = normalMaterial;

        SetUpRenderer();
        //rendererParent.rotation = Quaternion.Euler(0, currentShootDirection == ShootDirection.Right ? 0 : 180, 0);
    }

    ShootDirection currentShootDirection = ShootDirection.Right;
    private void Update()
    {
        ShootDirection previous = currentShootDirection;
        currentShootDirection = GetShootDirection;
        if (currentShootDirection != previous)
        {
            print("Change");
            rendererParent.rotation = Quaternion.Euler(0, currentShootDirection  == ShootDirection.Right ? rightRotation : leftRotation, 0);
        }
        
        UpdateBehavior();
    }

    bool detectedPlayer;
    ThirdPersonController _thirdPersonCharacter = default;
    public void DetectFirstPersonCharacter(ThirdPersonController thirdPersonCharacter)
    {
        detectedPlayer = true;
        _thirdPersonCharacter = thirdPersonCharacter;
        rendererParent.rotation = Quaternion.Euler(0, currentShootDirection == ShootDirection.Right ? rightRotation : leftRotation, 0);

        if (shootFrequenceSystem.IsStopped)
        {
            StartShooting();
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

    [Header("Life")]
    [SerializeField] DamageableEntity lifeSystem = default;
    [SerializeField] float hitFeedbackDuration = 0.1f;

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
        /*enemyRenderer.material = hitMaterial;*/
        foreach (RendererWithBaseMaterial parameters in rendererWithMaterials)
        {
            parameters.renderer.material = hitMaterial;
        }
        yield return new WaitForSeconds(hitFeedbackDuration);
        //enemyRenderer.material = normalMaterial;
        foreach (RendererWithBaseMaterial parameters in rendererWithMaterials)
        {
            parameters.renderer.material = parameters.normalMtl;
        }
    }

    [Header("Rendering")]
    [SerializeField] Transform rendererParent = default;
    [SerializeField] Renderer[] enemyRenderers = default;
    [SerializeField] Material normalMaterial = default;
    [SerializeField] Material hitMaterial = default;
    [SerializeField] float rightRotation = 60;
    [SerializeField] float leftRotation = 120;

    bool pendingKill = false;
    public virtual void Die()
    {
        if (pendingKill)
            return;

        pendingKill = true;

        PlayDeathFeedback();

        Destroy(gameObject);
    }

    [Header("Melee damaging")]
    [SerializeField] int meleeDamages = 2;
    public int GetMeleeDamages => meleeDamages;

    #region Shoot
    [Header("Shooting")]
    [SerializeField] ProjectileBase enemyProjectilePrefab = default;
    [SerializeField] Animator enemyAnimator = default;
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

    public void StartShooting()
    {
        enemyAnimator.SetTrigger("Shoot");
    }

    public void ShootProjectile()
    {
        ShootDirection shootDirectionEnum = currentShootDirection;
        Vector3 shootPosition = shootDirectionEnum == ShootDirection.Right ? rightShootPosition.position : leftShootPosition.position;
        Vector3 shootDirection = shootDirectionEnum == ShootDirection.Right ? Vector3.right : Vector3.left;

        shootDirection = Quaternion.Euler(0, 0, shootAngle * (shootDirectionEnum == ShootDirection.Right ? 1 : -1)) * shootDirection;

        /*print("SHOOT");
        Debug.DrawRay(shootPosition, shootDirection * 5.0f, Color.magenta, 0.25f);*///
        ProjectileBase newProj = Instantiate(enemyProjectilePrefab, shootPosition, Quaternion.identity);
        newProj.ShootProjectile(shootDirection, gameObject);

        PlayShootFeedback();
    }

    public void CheckIfShoot()
    {
        if (detectedPlayer)
            StartShooting();
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
                return (_thirdPersonCharacter.transform.position.x - transform.position.x > 0 ? ShootDirection.Right : ShootDirection.Left);
            }
            return currentShootDirection;
        }
    }
    #endregion

    [Header("Feedbacks")]
    [SerializeField] string shootFxTag = "PlaceHolderShoot";
    [SerializeField] AudioManager.Sound shootSound = AudioManager.Sound.E_EnnemyThrow;
    [SerializeField] string damagedFxTag = "PlaceHolder";
    [SerializeField] AudioManager.Sound damagedSound = AudioManager.Sound.H_ImpactShootEnemie;
    [SerializeField] string deathFxTag = "PlaceHolder";
    [SerializeField] AudioManager.Sound deathSound = AudioManager.Sound.E_EnnemyDeath;

    public void PlayDamagedFeedback()
    {
        AudioManager.PlaySound(damagedSound);
        FxManager.Instance.PlayFx(damagedFxTag, transform.position + Vector3.up, Quaternion.identity, Vector3.one);
    }

    public void PlayShootFeedback()
    {        
        Transform source = GetShootDirection == ShootDirection.Left ? leftShootPosition : rightShootPosition;

        AudioManager.PlaySound(shootSound);
        FxManager.Instance.PlayFx(shootFxTag, source.position, source.rotation, Vector3.one);
    }

    public void PlayDeathFeedback()
    {
        AudioManager.PlaySound(deathSound);
        FxManager.Instance.PlayFx(deathFxTag, transform.position, Quaternion.identity, Vector3.one);
    }

    struct RendererWithBaseMaterial { public Renderer renderer; public Material normalMtl; }
    RendererWithBaseMaterial[] rendererWithMaterials = new RendererWithBaseMaterial[0];
    public void SetUpRenderer()
    {
        rendererWithMaterials = new RendererWithBaseMaterial[enemyRenderers.Length];
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            Renderer render = enemyRenderers[i];
            RendererWithBaseMaterial parameters = new RendererWithBaseMaterial();
            parameters.renderer = render;
            parameters.normalMtl = render.material;
            rendererWithMaterials[i] = parameters;
        }
    }
}
