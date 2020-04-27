using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] KeyCode jetpackInput = KeyCode.JoystickButton2;

    // Update is called once per frame
    void Update()
    {
        UpdateJetpackValues(Input.GetKey(jetpackInput));

        UpdateMovement();
    }

    #region Global Movement
    public void UpdateMovement()
    {
        transform.position += (Vector3.up * currentJetpackVerticalSpeed) * Time.deltaTime;
    }
    #endregion

    #region Jetpack
    [Header("Jetpack")]
    [SerializeField] float jetpackMaxUpSpeed = 10f;
    [SerializeField] float jetpackMaxDownSpeed = -10f;
    [SerializeField] float jetpackUpAcceleration = 20f;
    [SerializeField] float jetpackGravity = -10f;
    float currentJetpackVerticalSpeed = 0;

    public void UpdateJetpackValues(bool isJetpackInputDown)
    {
        float currentVerticalAcceleration = jetpackGravity + (isJetpackInputDown ? jetpackUpAcceleration : 0);

        currentJetpackVerticalSpeed = Mathf.Clamp(currentJetpackVerticalSpeed + currentVerticalAcceleration * Time.deltaTime, jetpackMaxDownSpeed, jetpackMaxUpSpeed);

        //Debug.Log(currentJetpackVerticalSpeed);
    }
    #endregion
}
