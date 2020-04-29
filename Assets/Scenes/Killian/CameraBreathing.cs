using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraBreathing : MonoBehaviour
{
    Camera gameCamera;
    public bool move; 

    [Header("SlowShake")]
    float duration = 500;
    public Vector3 strength;
    public int vibrato;
    public float randomness;
    public bool fadeOut;

    [Header("LightRoll")]
    public bool roll;
    float rollDuration = 500;
    public Vector3 rollStrength;
    public int rollVibrato;
    public float rollRandomness;
    bool rollFadeOut = false;

    void Start()
    {
        gameCamera = GetComponent<Camera>();
        if (move)
        {
            gameCamera.DOShakePosition(duration, strength, vibrato, randomness, fadeOut).SetLoops(-1);
        }

        if(roll)
        {
            gameCamera.DOShakeRotation(rollDuration, rollStrength, rollVibrato, rollRandomness, rollFadeOut).SetLoops(-1);
        }

    }
}
