using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DamageableEntity : MonoBehaviour
{
    [Header("Life parameters")]
    [SerializeField] DamageTag damageTag = DamageTag.Player;
    public DamageTag GetDamageTag => damageTag;
    [SerializeField] int maxLife = 10;
    int currentLife = 10;
    bool canReceiveDamages = true;
    public void SetImmuneToDamages()
    {
        canReceiveDamages = false;
    }

    public void ResetCanReceiveDamages()
    {
        canReceiveDamages = true;
    }

    public Action<int> OnDamageableEntitySetUp;

    public Action<int, int> OnLifeAmountChanged;

    public Action<int, int, GameObject> OnReceivedDamages;

    public Action OnLifeReachedZero;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentLife = maxLife;
        OnDamageableEntitySetUp?.Invoke(currentLife);
    }

    public void ResetLife()
    {
        currentLife = maxLife;
        OnLifeAmountChanged?.Invoke(0, currentLife);
    }

    void LoseLife(int amount)
    {
        amount = Mathf.Abs(amount);

        currentLife = Mathf.Clamp(currentLife - amount, 0, maxLife);

        OnLifeAmountChanged?.Invoke(-amount, currentLife);

        if (currentLife <= 0)
            LifeReachedZero();
    }

    public void RecoverLife(int amount)
    {
        amount = Mathf.Abs(amount);
        currentLife = Mathf.Clamp(currentLife + amount, 0, maxLife);

        OnLifeAmountChanged?.Invoke(amount, currentLife);
    }

    public void ReceiveDamage(int amount, GameObject damageInstigator)
    {
        if (!canReceiveDamages)
            return;

        amount = Mathf.Abs(amount);

        LoseLife(amount);

        OnReceivedDamages?.Invoke(-amount, currentLife, damageInstigator);
    }

    public void LifeReachedZero()
    {
        OnLifeReachedZero?.Invoke();
    }
}

public enum DamageTag { Player, Enemy, Environment }