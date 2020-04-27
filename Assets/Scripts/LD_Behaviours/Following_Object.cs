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

    public bool isObjectFallingOutOfRange;
    public bool canObjectMove = true;

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
                if (canObjectMove)
                {
                    if (!IsThereAnObstacleInThisDirection(Vector3.up * -1f))
                    {
                        MoveToThisDirection(-1f);
                    }
                }
            }
            else
            {
                MoveObject();
            }
        }
        else
        {
            //Debug.Log("IN Range");

            if (Mathf.Abs(diff) > followingMinRange)
            {
                MoveObject();
            }
        }
    }

    void MoveObject()
    {
        if (canObjectMove)
        {
            if (!IsThereAnObstacleInThisDirection(Vector3.up * Math.Sign(diff)))
            {
                MoveToThisDirection(Math.Sign(diff));
            }
        }
    }

    void MoveToThisDirection(float direction)
    {
        float newYPos = objectPos.position.y + direction * plateformSpeed * Time.deltaTime;

        objectPos.position = new Vector3(objectPos.position.x, newYPos, objectPos.position.z);
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
