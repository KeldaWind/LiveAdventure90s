﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;


    private void Awake()
    {
        Instance = this;
    }
}