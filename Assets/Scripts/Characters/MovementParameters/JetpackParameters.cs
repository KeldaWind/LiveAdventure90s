﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LiveAd/Characters/Jetpack Parameters", fileName = "JetpackParameters")]
public class JetpackParameters : ScriptableObject
{
    [SerializeField] float jetpackMaxUpSpeed = 10f;
    public float GetJetpackMaxUpSpeed => jetpackMaxUpSpeed;


    [SerializeField] float jetpackMaxDownSpeed = -10f;
    public float GetJetpackMaxDownSpeed => jetpackMaxDownSpeed;


    [SerializeField] float jetpackUpAcceleration = 20f;
    public float GetJetpackUpAcceleration => jetpackUpAcceleration;


    [SerializeField] float jetpackGravityWhenGoingUp = -10f;
    public float GetJetpackGravityWhenGoingUp => jetpackGravityWhenGoingUp;


    [SerializeField] float jetpackGravityWhenGoingDown = -10f;
    public float GetJetpackGravityWhenGoingDown => jetpackGravityWhenGoingDown; 


    [SerializeField] float outOfBoundsAcceleration = 150f;
    public float GetOutOfBoundsAcceleration => outOfBoundsAcceleration;


    [SerializeField] float outOfBoundsMaxSpeed = 10f;
    public float GetOutOfBoundsMaxSpeed => outOfBoundsMaxSpeed;
}
