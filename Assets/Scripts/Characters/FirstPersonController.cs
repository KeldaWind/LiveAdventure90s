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

    public bool GetJetpackUpInput => gameOver ? false : (Input.GetKey(jetpackGamepadInput) || Input.GetKey(jetpackKeyboardInput) || Input.GetKey(jetpackGamepadAltInput));
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

    public float GetJetpackJaugeCoefficient => Mathf.Clamp(currentJetpackVerticalSpeed, 0, jetpackMaxUpSpeed) / jetpackMaxUpSpeed;

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
                float distance = transform.position.y - bottomBound.position.y;
                if (transform.position.y < bottomBound.position.y || Mathf.Abs(distance) < standByDistanceFromBottom)
                {
                    return JetpackBoundsState.TooLow;
                }
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

    public float GetLowDistance
    {
        get
        {
            if (bottomBound)
            {
                float distance = transform.position.y - bottomBound.position.y;
                if (transform.position.y < bottomBound.position.y || Mathf.Abs(distance) < standByDistanceFromBottom)
                {
                    return distance;
                }
            }
            if (thirdPersonController && maxDistanceFromThirdPersonCharacter != 0)
            {
                float distance = transform.position.y - thirdPersonController.transform.position.y;
                if (Mathf.Abs(distance) > maxDistanceFromThirdPersonCharacter || Mathf.Abs(distance) < standByDistanceFromBottom)
                    return distance;
            }
            return standByDistanceFromBottom;
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

    [Header("Jetpack Stand By")]
    [SerializeField] float standByDistanceFromBottom = 2f;
    [SerializeField] float standByAcceleration = 5f;
    public void UpdateJetpackValues(bool isJetpackInputDown)
    {
        if (/*!gameOver*/true)
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
                            float distanceFromLow = GetLowDistance;
                            if (Mathf.Abs(distanceFromLow) > standByDistanceFromBottom)
                            {
                                currentVerticalAcceleration = outOfBoundsUpAcceleration;
                                currentMaxUpSpeed = isJetpackInputDown ? currentMaxUpSpeed : outOfBoundsUpMaxSpeed;
                            }
                            else
                            {
                                currentVerticalAcceleration = isJetpackInputDown ? currentMaxUpSpeed : standByAcceleration * -Mathf.Sign(distanceFromLow);
                            }
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
        else
        {
            if (currentJetpackVerticalSpeed == 0)
                return;

            float accelerationDirection = -Mathf.Sign(currentJetpackVerticalSpeed);
            float min = accelerationDirection < 0 ? 0 : currentJetpackVerticalSpeed;
            float max = accelerationDirection > 0 ? 0 : currentJetpackVerticalSpeed;

            print(accelerationDirection * gameOverDeceleration * Time.deltaTime);
            currentJetpackVerticalSpeed = 
                Mathf.Clamp(currentJetpackVerticalSpeed + accelerationDirection * gameOverDeceleration * Time.deltaTime, min, max);
        }

        #region Manage Pitch
        float targetPitchValue = 0;
        if(currentJetpackVerticalSpeed > 0)
        {
            targetPitchValue = Mathf.Lerp(0, maxPitchWhenGoingUp, goingUpPitchCurve.Evaluate((Mathf.Abs(currentJetpackVerticalSpeed) / jetpackMaxUpSpeed) * pitchUpMultiplicator));
        }
        else
        {
            targetPitchValue = Mathf.Lerp(0, -maxPitchWhenGoingDown, goingDownPitchCurve.Evaluate((Mathf.Abs(currentJetpackVerticalSpeed) / Mathf.Abs(jetpackMaxDownSpeed)) * pitchDownMultiplicator));
        }
        #endregion

        currentPitch = Mathf.Lerp(currentPitch, targetPitchValue, pitchChangingCoeff);
        transform.rotation = Quaternion.Euler(currentPitch, 0, 0);

        UpdateSound();
    }
    #endregion

    [Header("Jetpack Camera Movements")]
    [SerializeField] float maxPitchWhenGoingUp = 3f;
    [SerializeField] float pitchUpMultiplicator = 1f;
    [SerializeField] AnimationCurve goingUpPitchCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float maxPitchWhenGoingDown = 3f;
    [SerializeField] float pitchDownMultiplicator = 1f;
    [SerializeField] AnimationCurve goingDownPitchCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float pitchChangingCoeff = 0.3f;
    float currentPitch = 0f;

    [Header("Feedbacks")]
    [SerializeField] AudioSource jetpackOnSoundSource = default;
    [SerializeField] float maxJetpackOnVolume = 1f;
    [SerializeField] AudioSource jetpackOffSoundSource = default;
    [SerializeField] float maxJetpackOffVolume = 1f;

    [SerializeField] float jetpackSoundTransitionDuration = 0.5f;
    float currentJetpackSoundTransitionCoeff = 0f;
    float previousSoundDirection = 0f;

    public void UpdateSound()
    {
        float currentSoundDirection = -1;
        JetpackBoundsState boundsState = GetJetpackBoundsState;
        switch (boundsState)
        {
            case JetpackBoundsState.TooLow:
                currentSoundDirection = 1;
                break;
            case JetpackBoundsState.Neutral:
                currentSoundDirection = GetJetpackUpInput ? 1 : -1;
                break;
            case JetpackBoundsState.TooHigh:
                currentSoundDirection = -1;
                break;
        }

        currentJetpackSoundTransitionCoeff = Mathf.Clamp(currentJetpackSoundTransitionCoeff + Time.deltaTime * currentSoundDirection, 0, jetpackSoundTransitionDuration);

        jetpackOnSoundSource.volume = maxJetpackOnVolume * (currentJetpackSoundTransitionCoeff / jetpackSoundTransitionDuration);

        if(currentSoundDirection < 0 && previousSoundDirection > 0)
        {
            jetpackOffSoundSource.volume = maxJetpackOffVolume * (currentJetpackSoundTransitionCoeff / jetpackSoundTransitionDuration);
            jetpackOffSoundSource.Play();
        }

        previousSoundDirection = currentSoundDirection;
    }

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

    bool gameOver = false;
    float gameOverDeceleration = 12f;
    public void SetGameOver()
    {
        gameOver = true;
    }
}

public enum JetpackBoundsState { TooLow, Neutral, TooHigh }
public enum JetpackVersion { Version1, Version2 }
