using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] KeyCode jetpackGamepadInput = KeyCode.JoystickButton5;
    [SerializeField] KeyCode jetpackGamepadAltInput = KeyCode.JoystickButton6;
    [SerializeField] KeyCode jetpackKeyboardInput = KeyCode.UpArrow;
    /*[SerializeField] KeyCode jetpackDownGamepadInput = KeyCode.JoystickButton6;
    [SerializeField] KeyCode jetpackDownKeyboardInput = KeyCode.DownArrow;*/

    public bool GetJetpackUpInput => Input.GetKey(jetpackGamepadInput) || Input.GetKey(jetpackKeyboardInput) || Input.GetKey(jetpackGamepadAltInput);
    //public bool GetJetpackDownInput => Input.GetKey(jetpackDownGamepadInput) || Input.GetKey(jetpackDownKeyboardInput);

    [Header("Important References")]
    [SerializeField] Camera cameramanCamera = default;
    [SerializeField] float maxDistanceFromThirdPersonCharacter = 0f;
    ThirdPersonController thirdPersonController = default;
    public Vector3 GetCameraWorldPosition => cameramanCamera.transform.position;

    public void SetThirdPersonRef(ThirdPersonController reference)
    {
        thirdPersonController = reference;
    }

    private void Start()
    {
        SetUpJetpack();
    }

    void Update()
    {
        UpdateJetpackValues(GetJetpackUpInput);

        UpdateMovement();
    }

    #region Global Movement
    public void UpdateMovement()
    {
        transform.position += (Vector3.up * currentJetpackVerticalSpeed) * Time.deltaTime;

        //transform.position += (Vector3.right * currentAutoFollowHorizontalSpeed) * Time.deltaTime;
    }
    #endregion

    #region Vertical Jetpack
    [Header("Jetpack Version")]
    [SerializeField] JetpackVersion inUseVersion = JetpackVersion.Version1;
    [Header("Jetpack 1")]
    [SerializeField] JetpackParameters jetpackParameters = default;
    [SerializeField] float jetpackMaxUpSpeed = 10f;
    [SerializeField] float jetpackMaxDownSpeed = -10f;
    [SerializeField] float jetpackUpAcceleration = 20f;
    [SerializeField] float jetpackGravityWhenGoingUp = -10f;
    [SerializeField] float jetpackGravityWhenGoingDown = -10f;
    [SerializeField] float outOfBoundsUpAcceleration = 128f;
    [SerializeField] float outOfBoundsUpMaxSpeed = 10f;
    [SerializeField] float outOfBoundsDownAcceleration = 128f;
    [SerializeField] float outOfBoundsDownMaxSpeed = 10f;
    float currentJetpackVerticalSpeed = 0;

    /*[Header("Jetpack 2")]
    [SerializeField]*/
    
    Transform bottomBound = default;
    Transform topBound = default;
    public JetpackBoundsState GetJetpackBoundsState
    {
        get
        {
            if (bottomBound)
            {
                if (transform.position.y < bottomBound.position.y)
                    return JetpackBoundsState.TooLow;
            }
            if(topBound)
            {
                if (transform.position.y > topBound.position.y)
                    return JetpackBoundsState.TooHigh;
            }

            if (thirdPersonController && maxDistanceFromThirdPersonCharacter != 0)
            {
                float distance = transform.position.y - thirdPersonController.transform.position.y;
                if (Mathf.Abs(distance) > maxDistanceFromThirdPersonCharacter)
                    return distance > 0 ? JetpackBoundsState.TooHigh : JetpackBoundsState.TooLow;
            }

            return JetpackBoundsState.Neutral;
        }
    }

    public void SetUpBounds(Transform bottom, Transform top)
    {
        bottomBound = bottom;
        topBound = top;
    }

    public void SetUpJetpack()
    {
        if (jetpackParameters)
        {
            jetpackMaxUpSpeed = jetpackParameters.GetJetpackMaxUpSpeed;
            jetpackMaxDownSpeed = jetpackParameters.GetJetpackMaxDownSpeed;
            jetpackUpAcceleration = jetpackParameters.GetJetpackUpAcceleration;
            jetpackGravityWhenGoingUp = jetpackParameters.GetJetpackGravityWhenGoingUp;
            jetpackGravityWhenGoingDown = jetpackParameters.GetJetpackGravityWhenGoingDown;
            outOfBoundsUpAcceleration = jetpackParameters.GetOutOfBoundsUpAcceleration;
            outOfBoundsUpMaxSpeed = jetpackParameters.GetOutOfBoundsMaxUpSpeed;
            outOfBoundsDownAcceleration = jetpackParameters.GetOutOfBoundsDownAcceleration;
            outOfBoundsDownMaxSpeed = jetpackParameters.GetOutOfBoundsMaxDownSpeed;
        }
    }

    public void UpdateJetpackValues(bool isJetpackInputDown)
    {
        switch (inUseVersion)
        {
            case JetpackVersion.Version1:
                #region V1
                JetpackBoundsState boundsState = GetJetpackBoundsState;
                float currentMaxUpSpeed = jetpackMaxUpSpeed;
                float currentMaxDownSpeed = jetpackMaxDownSpeed;
                float currentVerticalAcceleration = 0;

                switch (boundsState)
                {
                    case JetpackBoundsState.TooLow:
                        currentVerticalAcceleration = outOfBoundsUpAcceleration;
                        currentMaxUpSpeed = isJetpackInputDown ? currentMaxUpSpeed : outOfBoundsUpMaxSpeed;
                        break;

                    case JetpackBoundsState.Neutral:
                        currentVerticalAcceleration = isJetpackInputDown ? jetpackUpAcceleration : (currentJetpackVerticalSpeed > 0 ? jetpackGravityWhenGoingUp : jetpackGravityWhenGoingDown);
                        break;

                    case JetpackBoundsState.TooHigh:
                        currentVerticalAcceleration = -outOfBoundsDownAcceleration;
                        currentMaxDownSpeed = -outOfBoundsDownMaxSpeed;
                        break;
                }
                currentJetpackVerticalSpeed = Mathf.Clamp(currentJetpackVerticalSpeed + currentVerticalAcceleration * Time.deltaTime, currentMaxDownSpeed, currentMaxUpSpeed);
                #endregion
                break;
            case JetpackVersion.Version2:
                break;
        }
    }
    #endregion

    #region Horizontal Auto Follow
    /*[Header("Horizontal Auto Follow")]
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
    }*/
    #endregion 
}

public enum JetpackBoundsState { TooLow, Neutral, TooHigh }
public enum JetpackVersion { Version1, Version2 }
