using System.Collections;
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


    [SerializeField] float outOfBoundsUpAcceleration = 150f;
    public float GetOutOfBoundsUpAcceleration => outOfBoundsUpAcceleration;


    [SerializeField] float outOfBoundsMaxUpSpeed = 10f;
    public float GetOutOfBoundsMaxUpSpeed => outOfBoundsMaxUpSpeed;


    [SerializeField] float outOfBoundsDownAcceleration = 150f;
    public float GetOutOfBoundsDownAcceleration => outOfBoundsDownAcceleration;


    [SerializeField] float outOfBoundsMaxDownSpeed = 10f;
    public float GetOutOfBoundsMaxDownSpeed => outOfBoundsMaxDownSpeed;
}
