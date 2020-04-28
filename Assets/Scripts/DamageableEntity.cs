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

    public Action<int, int> OnLifeAmountChanged;

    public Action<int, int> OnReceivedDamages;

    public Action OnLifeReachedZero;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentLife = maxLife;
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

    public void ReceiveDamage(int amount)
    {
        amount = Mathf.Abs(amount);

        LoseLife(amount);

        OnReceivedDamages?.Invoke(-amount, currentLife);
    }

    public void LifeReachedZero()
    {
        OnLifeReachedZero?.Invoke();
    }
}

public enum DamageTag { Player, Enemy, Environment }