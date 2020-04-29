using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;

public class Following_Plateform : MonoBehaviour
{
    [Header("Object Movement")]
    public float plateformSpeed = 10f;
    private Transform objectPos;

    public float followingMinRange = 0.35f;
    public float followingMaxRange = 2f;

    public AnimationCurve accelerationCurve;
    private float accelerationModifier = 0f;
    public float accelerationCurveSpeed = 1f;

    public float maxBoostTime = 0.1f;
    private float currentBoostTimer = 0f;

    public LayerMask blockingElementsLayerMask;

    [Header("Object Behaviour")]
    public bool isObjectFallingOutOfRange;
    private bool isGoingUp;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool canObjectMove;
    private bool isAccelerating;
    [HideInInspector] public bool isBoostingJump;
    public float plateformVelocity;



    private void Awake()
    {
        objectPos = this.GetComponent<Transform>();
        canObjectMove = true;
    }

    private void FixedUpdate()
    {
        if (canObjectMove)
            CompareObjectAndCameraPositions();
    }

    /// <summary>
    /// Compare the object and the camera Y position
    /// </summary>
    void CompareObjectAndCameraPositions()
    {
        //Get magnitude
        float diff = GameManager.Instance.GetCameraWorldPosition.y - objectPos.localPosition.y;

        if (Mathf.Abs(diff) > followingMaxRange)
        {
            //Debug.Log("Out of Range");

            if (isObjectFallingOutOfRange)
            {
                MoveObject(-1f);
            }
            else
            {
                MoveObject(Math.Sign(diff));
            }
        }
        else
        {
            //Debug.Log("IN Range");

            if (Mathf.Abs(diff) > followingMinRange)
            {
                MoveObject(Math.Sign(diff));
            }
            else
            {
                CelesteBoostJumpTiming();

                isGoingUp = false;

                isAccelerating = false;
                isMoving = false;
            }
        }
    }

    void CelesteBoostJumpTiming()
    {
        if (isGoingUp && !isBoostingJump)
        {
            //plateformVelocity = 
            currentBoostTimer = 0;
            isBoostingJump = true;
        }
        else if (currentBoostTimer < maxBoostTime)
        {
            currentBoostTimer += Time.deltaTime;
        }
        else
        {
            isBoostingJump = false;
        }
    }

    void MoveObject(float direction)
    {
        if (!IsThereAnObstacleInThisDirection(Vector3.up * direction))
        {
            isBoostingJump = false;

            if (direction > 0)
                isGoingUp = true;
            else
                isGoingUp = false;


            if (!isAccelerating && !isMoving)
            {
                accelerationModifier = 0;
                isAccelerating = true;
            }
            else if (accelerationModifier < 1)
            {
                accelerationModifier += Time.deltaTime * accelerationCurveSpeed;
            }
            else
            {
                accelerationModifier = 1f;
            }

            MoveToThisDirection(Math.Sign(direction));
        }
        else
        {
            CelesteBoostJumpTiming();

            isGoingUp = false;
            isAccelerating = false;
            isMoving = false;
        }
    }

    void MoveToThisDirection(float direction)
    {
        float newYPos = objectPos.position.y + direction * plateformSpeed * accelerationCurve.Evaluate(accelerationModifier) * Time.deltaTime;

        objectPos.position = new Vector3(objectPos.position.x, newYPos, objectPos.position.z);

        isMoving = true;
    }

    bool IsThereAnObstacleInThisDirection(Vector3 direction)
    {
        RaycastHit result;
        float maxDistance = 0.2f;

        Physics.BoxCast(objectPos.position,
            objectPos.lossyScale * 0.5f,
            direction,
            out result,
            Quaternion.identity,
            maxDistance,
            blockingElementsLayerMask);

        if (result.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
