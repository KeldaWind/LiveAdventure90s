using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    private float score;

    public Action onScoreUpdate;




    private void RaiseScore(int amount)
    {
        score += amount;
    }

    void RefreshScoreDisplayed()
    {
        Debug.Log(score);
    }
}
