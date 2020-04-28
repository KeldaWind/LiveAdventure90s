using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public Action<int, int> OnLifeAmountChanged;

    public Action<int, int> OnReceivedDamages;

    public Action OnLifeReachedZero;


    public static LifeManager Instance;


    private void Awake()
    {
        Instance = this;
    }
}
