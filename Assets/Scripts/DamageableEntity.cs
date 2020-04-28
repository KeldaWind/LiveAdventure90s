using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableEntity : MonoBehaviour
{
    [Header("Life parameters")]
    private int maxLife;
    private int currentLife;



    void LoseLife(int amount)
    {
        currentLife -= amount;

        LifeManager.Instance.OnLifeAmountChanged.Invoke(amount, currentLife);

        if (currentLife <= 0)
            LifeManager.Instance.OnLifeReachedZero.Invoke();
    }

    public void RecoverLife(int amount)
    {
        int newLife = currentLife + amount;

        LifeManager.Instance.OnLifeAmountChanged.Invoke(amount, currentLife);

        if (newLife == maxLife)
            return;

        if(newLife > maxLife)
        {
            int newAmount = amount - (newLife - maxLife);
            currentLife += newAmount;
        }
        else
        {
            currentLife = newLife;
        }
    }

    public void ReceiveDamage(int amount)
    {
        LoseLife(amount);

        LifeManager.Instance.OnReceivedDamages.Invoke(amount, currentLife);
    }
}
