using System;
using System.Collections;
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

    bool avoidSound = false;

    [Header("FX")]
    public ParticleSystem attractionFX;


    public IEnumerator AvoidSoundOnStartCoroutine()
    {
        avoidSound = true;
        yield return new WaitForSeconds(0.2f);
        avoidSound = false;
    }

    private void Awake()
    {
        objectPos = this.GetComponent<Transform>();

        SetNewTrailReferencePositions();

        StartCoroutine(AvoidSoundOnStartCoroutine());
    }

    float lastDirection = 0;
    private void Update()
    {
        float diff = GameManager.Instance.GetCameraWorldPosition.y - objectPos.localPosition.y;
        if (lastDirection != Mathf.Sign(diff))
        {
            lastDirection = Mathf.Sign(diff);
            if (diff > 0)
                PlayGoingUpSound();
            else
                PlayGoingDownSound();
        }

        if (Mathf.Abs(diff) > followingMinRange)
        {
            MoveKeyBallOnTrail(Mathf.Sign(diff));
        }
        else
        {
            isAccelerating = false;
            isBallMoving = false;
        }

        if(GameManager.Instance.GetCameraWorldPosition.y > lowestTrailPos && GameManager.Instance.GetCameraWorldPosition.y < highestTrailPos)
        {
            UIManager.Instance.OnPointerInteraction();
        }
        else
        {
            UIManager.Instance.OnNormalInteraction();
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

        if (!isBallMoving)
        {
            isBallMoving = true;
            attractionFX.Play();
        }
        else if (isBallMoving && objectPos.position.y <= lowestTrailPos || objectPos.position.y >= highestTrailPos)
        {
            isBallMoving = false;
            attractionFX.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
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

        for (int i = 0; i < trailPositions.Length; i++)
        {
            trailPositions[i].transform.position = new Vector3(this.transform.position.x, trailPositions[i].transform.position.y, trailPositions[i].transform.position.z);
        }
    }

    [Header("Feedbacks")]
    [SerializeField] AudioManager.Sound onStartGoingUpSound = AudioManager.Sound.LD_evelvatorActive;
    [SerializeField] AudioManager.Sound onStartGoingDownSound = AudioManager.Sound.LD_elevatorDisable;

    public void PlayGoingUpSound()
    {
        if (avoidSound)
            return;

        AudioManager.PlaySound(onStartGoingUpSound);
    }

    public void PlayGoingDownSound()
    {
        if (avoidSound)
            return;

        AudioManager.PlaySound(onStartGoingDownSound);
    }
}
