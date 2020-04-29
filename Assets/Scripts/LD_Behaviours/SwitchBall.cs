using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBall : MonoBehaviour
{

    [Header("Ball Movement")]
    public Vector3 currentHolderPos;
    public ObjectHolder_Event[] trailPositions;

    public float keyBallSpeed = 10f;
    private Transform objectPos;

    public float followingMinRange = 0.35f;

    public AnimationCurve accelerationCurve;
    private float accelerationModifier = 0f;
    public float accelerationCurveSpeed = 1f;

    [Header("Object Behaviour")]
    public bool isObjectFallingOutOfRange;
    public bool isBallMoving;
    private bool isAccelerating;

    private float lowestTrailPos;
    private float highestTrailPos;



    private void Awake()
    {
        objectPos = this.GetComponent<Transform>();

        SetNewTrailReferencePositions();
    }

    private void Update()
    {
        float diff = GameManager.Instance.GetCameraWorldPosition.y - objectPos.localPosition.y;


        if (Mathf.Abs(diff) > followingMinRange)
        {
            MoveKeyBallOnTrail(Mathf.Sign(diff));
        }
        else
        {
            isAccelerating = false;
            isBallMoving = false;
        }
    }

    void MoveKeyBallOnTrail(float direction)
    {
        if (!isAccelerating && !isBallMoving)
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

    void MoveToThisDirection(float direction)
    {
        float newYPos = Mathf.Clamp(objectPos.position.y + direction * keyBallSpeed * accelerationCurve.Evaluate(accelerationModifier) * Time.deltaTime, lowestTrailPos, highestTrailPos);

        objectPos.position = new Vector3(objectPos.position.x, newYPos, objectPos.position.z);

        isBallMoving = true;
    }

    float GetLowestTrailPos(ObjectHolder_Event[] trail)
    {
        float currentMin = trail[0].transform.localPosition.y;

        for (int i = 1; i < trail.Length; i++)
        {
            if (currentMin > trail[i].transform.localPosition.y)
                currentMin = trail[i].transform.localPosition.y;
        }

        return currentMin;
    }

    float GetHighestTrailPos(ObjectHolder_Event[] trail)
    {
        float currentMax = trail[0].transform.localPosition.y;

        for (int i = 1; i < trail.Length; i++)
        {
            if (currentMax < trail[i].transform.localPosition.y)
                currentMax = trail[i].transform.localPosition.y;
        }

        return currentMax;
    }

    public void SetNewTrailReferencePositions()
    {
        lowestTrailPos = GetLowestTrailPos(trailPositions);
        highestTrailPos = GetHighestTrailPos(trailPositions);
    }
}
