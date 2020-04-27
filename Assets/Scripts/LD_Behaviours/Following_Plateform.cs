using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;

public class Following_Plateform : MonoBehaviour
{
    [Header("Plateform Movement")]
    public Transform plateformPos;
    public Transform cameraRef;
    public BoxCollider plateformCollid;

    private float diff;
    private float plateformSpeed = 10f;
    private float moveRange = 0.3f;

    public LayerMask layerMask;

    [SerializeField] private bool inCameraRange;


    private void FixedUpdate()
    {
        ComparePlaterformAndCameraPositions();
    }

    void ComparePlaterformAndCameraPositions()
    {
        //Get magnitude
        diff = cameraRef.localPosition.y - plateformPos.localPosition.y;

        if (Mathf.Abs(diff) > moveRange)
        {
            if (!IsThereAnObstacleInThisDirection(Vector3.up * Math.Sign(diff)))
            {
                MoveToThisDirection();
            }
        }
    }

    void MoveToThisDirection()
    {
        float newYPos = plateformPos.position.y + Math.Sign(diff) * plateformSpeed * Time.deltaTime;

        plateformPos.position = new Vector3(plateformPos.position.x, newYPos, plateformPos.position.z);
    }

    bool IsThereAnObstacleInThisDirection(Vector3 direction)
    {
        RaycastHit result;
        float maxDistance = 0.2f;

        Physics.BoxCast(plateformPos.position,
            plateformPos.lossyScale * 0.5f,
            direction,
            out result,
            Quaternion.identity,
            maxDistance,
            layerMask);

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
