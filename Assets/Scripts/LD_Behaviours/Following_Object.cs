using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;

public class Following_Object : MonoBehaviour
{
    [Header("Plateform Movement")]
    private Transform objectPos;

    private float diff;
    private float plateformSpeed = 10f;
    public float followingMinRange = 0.35f;
    public float followingMaxRange = 2f;
    [SerializeField] private float accelerationModifier;

    public bool isObjectFallingOutOfRange;
    public bool canObjectMove = true;
    private bool isGoingUp;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isAccelerating;


    public AnimationCurve accelerationCurve;

    public LayerMask blockingElementsLayerMask;

    [SerializeField] private bool inCameraRange;



    private void Awake()
    {
        objectPos = this.GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        CompareObjectAndCameraPositions();
    }

    /// <summary>
    /// Compare the object and the camera Y position
    /// </summary>
    void CompareObjectAndCameraPositions()
    {
        //Get magnitude
        diff = GameManager.Instance.GetCameraWorldPosition.y - objectPos.localPosition.y;

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
                isGoingUp = false;
                isAccelerating = false;
                isMoving = false;
            }
        }
    }

    void MoveObject(float direction)
    {
        if (canObjectMove)
        {
            if (!IsThereAnObstacleInThisDirection(Vector3.up * direction))
            {
                if (Math.Sign(diff) > 0)
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
                    accelerationModifier += Time.deltaTime * 1f;
                }
                else
                {
                    accelerationModifier = 1f;
                }

                MoveToThisDirection(Math.Sign(diff));
            }
            else
            {
                isGoingUp = false;
                isAccelerating = false;
                isMoving = false;
            }
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
            return true;
        else
            return false;
    }
}
