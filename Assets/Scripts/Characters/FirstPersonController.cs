using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] KeyCode jetpackGamepadInput = KeyCode.JoystickButton2;
    [SerializeField] KeyCode jetpackKeyboardInput = KeyCode.UpArrow;

    [Header("Important References")]
    [SerializeField] Camera cameramanCamera = default;
    ThirdPersonController thirdPersonController = default;
    public Vector3 GetCameraWorldPosition => cameramanCamera.transform.position;

    public void SetFirstPersonRef(ThirdPersonController reference)
    {
        thirdPersonController = reference;
    }

    void Update()
    {
        UpdateJetpackValues(Input.GetKey(jetpackGamepadInput) || Input.GetKey(jetpackKeyboardInput));

        UpdateMovement();
    }

    #region Global Movement
    public void UpdateMovement()
    {
        transform.position += (Vector3.up * currentJetpackVerticalSpeed) * Time.deltaTime;

        transform.position += (Vector3.right * currentAutoFollowHorizontalSpeed) * Time.deltaTime;
    }
    #endregion

    #region Vertical Jetpack
    [Header("Jetpack")]
    [SerializeField] float jetpackMaxUpSpeed = 10f;
    [SerializeField] float jetpackMaxDownSpeed = -10f;
    [SerializeField] float jetpackUpAcceleration = 20f;
    [SerializeField] float jetpackGravityWhenGoingUp = -10f;
    [SerializeField] float jetpackGravityWhenGoingDown = -10f;
    float currentJetpackVerticalSpeed = 0;

    public void UpdateJetpackValues(bool isJetpackInputDown)
    {
        float currentVerticalAcceleration = isJetpackInputDown ? jetpackUpAcceleration : (currentJetpackVerticalSpeed > 0 ? jetpackGravityWhenGoingUp : jetpackGravityWhenGoingDown);

        currentJetpackVerticalSpeed = Mathf.Clamp(currentJetpackVerticalSpeed + currentVerticalAcceleration * Time.deltaTime, jetpackMaxDownSpeed, jetpackMaxUpSpeed);
    }
    #endregion

    #region Horizontal Auto Follow
    [Header("Horizontal Auto Follow")]
    [SerializeField] float maxHorizontalSpeed = 5f;
    [SerializeField] float hozitontalAcceleration = 10f;
    [SerializeField] float hozitontalBackAcceleration = 20f;
    [SerializeField] float hozitontalDeceleration = 15f;
    [SerializeField] float minimumTargetDistanceToMoveToward = 0.1f;
    [SerializeField] float reachableDistanceStopCoefficient = 2f;
    float currentAutoFollowHorizontalSpeed = 0;

    public void UpdateAutoFollowValues()
    {
        if (!thirdPersonController)
            return;

        float signedDistanceWithTarget = thirdPersonController.transform.position.x - transform.position.x;

        bool mustMoveTowardTarget = Mathf.Abs(signedDistanceWithTarget) > minimumTargetDistanceToMoveToward;
        if (mustMoveTowardTarget)
        {
            float nextTravelledDistance = currentAutoFollowHorizontalSpeed * Time.deltaTime;
            if (Mathf.Abs(signedDistanceWithTarget) < Mathf.Abs(nextTravelledDistance) * reachableDistanceStopCoefficient)
                mustMoveTowardTarget = false;
        }

        float autoInputValue = mustMoveTowardTarget ? Mathf.Sign(signedDistanceWithTarget) : 0;

        if (autoInputValue == 0)
        {
            if (currentAutoFollowHorizontalSpeed != 0)
            {
                currentAutoFollowHorizontalSpeed -= Mathf.Sign(currentAutoFollowHorizontalSpeed) * hozitontalDeceleration * Time.deltaTime;
                currentAutoFollowHorizontalSpeed =
                    Mathf.Clamp(currentAutoFollowHorizontalSpeed,
                    currentAutoFollowHorizontalSpeed > 0 ? 0 : currentAutoFollowHorizontalSpeed,
                    currentAutoFollowHorizontalSpeed < 0 ? 0 : currentAutoFollowHorizontalSpeed);
            }
        }
        else
        {
            currentAutoFollowHorizontalSpeed = 
                Mathf.Clamp(currentAutoFollowHorizontalSpeed + autoInputValue * Time.deltaTime * 
                (Mathf.Sign(currentAutoFollowHorizontalSpeed) != Mathf.Sign(signedDistanceWithTarget) ? hozitontalBackAcceleration : hozitontalAcceleration), 
                -maxHorizontalSpeed, maxHorizontalSpeed);
        }

        //print(currentAutoFollowHorizontalSpeed);
    }
    #endregion 
}
