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
    [SerializeField] BoxCollider selfCollider = default;
    [SerializeField] Rigidbody selfBody = default;
    [SerializeField] float skinWidthMultiplier = 0.99f;
    [SerializeField] float ceilingAndGroundCheckDistance = 0.1f;
    [SerializeField] float maxSpeedBoostDuration = 0.1f;

    TimerSystem boostTimerSystem = default;
    float currentBoostSpeed = default;

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


    float currentVerticalSpeed = 0f;
    public Vector3 GetCurrentVerticalMovementSpeed => Vector3.up * currentBoostSpeed;
    ThirdPersonController onPlatformThirdPersonCharacter = default;

    bool avoidSound = false;
    public IEnumerator AvoidSoundOnStartCoroutine()
    {
        avoidSound = true;
        yield return new WaitForSeconds(0.2f);
        avoidSound = false;
    }

    private void Awake()
    {
        objectPos = this.GetComponent<Transform>();
        canObjectMove = true;
        boostTimerSystem = new TimerSystem();
        boostTimerSystem.ChangeTimerValue(maxSpeedBoostDuration);
        StartCoroutine(AvoidSoundOnStartCoroutine());
    }

    private void Update()
    {
        UpdateTargetSpeed();
        UpdatePhysics();
    }

    private void FixedUpdate()
    {
        //UpdatePhysics();


        /*if (canObjectMove)
            CompareObjectAndCameraPositions();*/
    }

    float lastDirection = 0;
    public void UpdateTargetSpeed()
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
            if (Mathf.Sign(accelerationModifier) != Mathf.Sign(diff))
            {
                accelerationModifier = 0;
            }

            if (Mathf.Abs(accelerationModifier) < 1f)
            {
                accelerationModifier += Time.deltaTime * accelerationCurveSpeed * Mathf.Sign(diff);
                if (Mathf.Abs(accelerationModifier) > 1f)
                    accelerationModifier = Mathf.Sign(diff);
            }
        }
        else
        {
            accelerationModifier = 0;
        }


        currentVerticalSpeed = plateformSpeed * Mathf.Sign(accelerationModifier) * accelerationCurve.Evaluate(Mathf.Abs(accelerationModifier));



        if(currentVerticalSpeed > currentBoostSpeed)
        {
            boostTimerSystem.StartTimer();
            currentBoostSpeed = currentVerticalSpeed;
        }
        else if(currentVerticalSpeed < currentBoostSpeed)
        {
            boostTimerSystem.UpdateTimer();
            if (boostTimerSystem.TimerOver)
            {
                boostTimerSystem.StartTimer();
                currentBoostSpeed = currentVerticalSpeed;
            }
        }
    }

    public void UpdatePhysics()
    {
        CheckForGround();
        CheckForCeiling();

        selfBody.velocity = Vector3.up * currentVerticalSpeed;

        return;

        #region OLD - Custom Physic
        float movementDistance = currentVerticalSpeed * Time.deltaTime;
        float initialMovementDistance = movementDistance;
        Vector3 movement = Vector3.up * movementDistance;

        Vector3 actualSize = 
            new Vector3(selfCollider.size.x * objectPos.lossyScale.x, 
            selfCollider.size.y * objectPos.lossyScale.y, 
            selfCollider.size.z * objectPos.lossyScale.z) * skinWidthMultiplier;

        #region V1 - Multi Hit
        /*RaycastHit[] hits = Physics.BoxCastAll(objectPos.position + selfCollider.center,
            actualSize * 0.5f,
            movement,
            objectPos.rotation,
            Mathf.Abs(movementDistance),
            blockingElementsLayerMask, 
            QueryTriggerInteraction.Ignore);        

        float startPoint = objectPos.position.y + selfCollider.center.y + selfCollider.size.y * 0.5f * Mathf.Sign(movementDistance);
        
        foreach (RaycastHit hit in hits)
        {
            Collider hitCollider = hit.collider;

            if (hitCollider == selfCollider)
                continue;

            Rigidbody hitBody = hitCollider.GetComponent<Rigidbody>();

            if (hitBody)
            {
                float initialDifference = Mathf.Sign(movementDistance) * Mathf.Abs(hit.point.y - startPoint);
                float remainingDistance = movementDistance - initialDifference;

                //Vector3 startPos = new Vector3(hit.point.x + 0.1f, startPoint + initialDifference);
                //Debug.DrawRay(startPos, Vector3.up * movementDistance, Color.red, Time.deltaTime);
                //Debug.DrawRay(hit.point, hit.normal * initialDifference, Color.green, 0.5f);
                //Debug.DrawRay(hit.point, -hit.normal * remainingDistance, Color.magenta, 0.5f);

                print(remainingDistance);
                // Après, il faut essayer de faire bouger l'objet de la distance restante 
                float reallyTravelledDistance = TryToMoveHitObjectWithDistance(hitBody, hitCollider, remainingDistance);
                float newDistance = initialDifference + reallyTravelledDistance;

                if (Mathf.Abs(newDistance) < Mathf.Abs(movementDistance))
                {
                    movementDistance = newDistance;
                }
            }
            else
            {
                float newDistance = Mathf.Sign(movementDistance) * Mathf.Abs(hit.point.y - startPoint);

                if (Mathf.Abs(newDistance) < Mathf.Abs(movementDistance))
                {
                    movementDistance = newDistance;
                } 
            }
        }*/
        #endregion

        #region V2 - Single Hit
        /*RaycastHit hit = new RaycastHit();
        Physics.BoxCast(objectPos.position + selfCollider.center,
            actualSize * 0.5f,
            movement,
            out hit,
            objectPos.rotation,
            Mathf.Abs(movementDistance),
            blockingElementsLayerMask,
            QueryTriggerInteraction.Ignore);

        Collider hitCollider = hit.collider;
        if (hitCollider)
        {
            float startPoint = objectPos.position.y + selfCollider.center.y + selfCollider.size.y * 0.5f * Mathf.Sign(movementDistance);

            Rigidbody hitBody = hitCollider.GetComponent<Rigidbody>();

            if (hitBody)
            {
                float initialDifference = Mathf.Sign(movementDistance) * Mathf.Abs(hit.point.y - startPoint);
                float remainingDistance = movementDistance - initialDifference;

                //Vector3 startPos = new Vector3(hit.point.x + 0.1f, startPoint + initialDifference);
                //Debug.DrawRay(startPos, Vector3.up * movementDistance, Color.red, Time.deltaTime);
                //Debug.DrawRay(hit.point, hit.normal * initialDifference, Color.green, 0.5f);
                //Debug.DrawRay(hit.point, -hit.normal * remainingDistance, Color.magenta, 0.5f);

                print(remainingDistance);
                // Après, il faut essayer de faire bouger l'objet de la distance restante 
                float reallyTravelledDistance = TryToMoveHitObjectWithDistance(hitBody, hitCollider, remainingDistance);
                float newDistance = initialDifference + reallyTravelledDistance;

                if (Mathf.Abs(newDistance) < Mathf.Abs(movementDistance))
                {
                    movementDistance = newDistance;
                }
            }
            else
            {
                float newDistance = Mathf.Sign(movementDistance) * Mathf.Abs(hit.point.y - startPoint);

                if (Mathf.Abs(newDistance) < Mathf.Abs(movementDistance))
                {
                    movementDistance = newDistance;
                }
            }
        }*/
        #endregion

        movement = Vector3.up * movementDistance;

        objectPos.Translate(movement);
        #endregion
    }

    public void CheckForGround()
    {
        if (CheckForDirection(Vector3.down) && currentVerticalSpeed < 0)
        {
            currentVerticalSpeed = 0;
            accelerationModifier = 0;
        }
    }

    public void CheckForCeiling()
    {
        if (CheckForDirection(Vector3.up) && currentVerticalSpeed > 0)
        {
            currentVerticalSpeed = 0;
            accelerationModifier = 0;
        }
    }

    public bool CheckForDirection(Vector3 direction)
    {
        Vector3 actualSize = new Vector3(selfCollider.size.x * transform.lossyScale.x, selfCollider.size.y * transform.lossyScale.y, selfCollider.size.z * transform.lossyScale.z) * skinWidthMultiplier;
        bool hitSomething = Physics.BoxCast(transform.position + selfCollider.center, actualSize * 0.5f, direction, transform.rotation, ceilingAndGroundCheckDistance, blockingElementsLayerMask);

        return hitSomething;
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

    // OLD
    /*
    public float TryToMoveHitObjectWithDistance(Rigidbody objectBody, Collider objectCollider, float startDistance)
    {
        /// Returns the distance travelled by the object in order to know how much can the object move
        float finalMoveDistance = startDistance;
        Vector3 movement = Vector3.up * finalMoveDistance;
        print(startDistance);

        Transform objectTr = objectBody.transform;
        BoxCollider objectBoxCollider = objectCollider as BoxCollider;

        if (!objectBoxCollider)
            return finalMoveDistance;

        Vector3 actualSize =
            new Vector3(objectBoxCollider.size.x * objectTr.lossyScale.x,
            objectBoxCollider.size.y * objectTr.lossyScale.y,
            objectBoxCollider.size.z * objectTr.lossyScale.z) * skinWidthMultiplier;

        RaycastHit hit = new RaycastHit();
            Physics.BoxCast(objectPos.position + selfCollider.center,
            actualSize * 0.5f,
            movement,
            objectPos.rotation,
            Mathf.Abs(finalMoveDistance),
            blockingElementsLayerMask,
            QueryTriggerInteraction.Ignore);

        if(hit.collider != null)
        {

        }
        else
        {
            objectTr.Translate(movement);
        }

        return finalMoveDistance;
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
        if (!IsThereAnObstacleInThisDirection(Vector3.up * direction).collider)
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

    RaycastHit IsThereAnObstacleInThisDirection(Vector3 direction)
    {
        RaycastHit result = new RaycastHit();
        float maxDistance = 0.2f;

        Physics.BoxCast(objectPos.position,
            objectPos.lossyScale * 0.5f,
            direction,
            out result,
            Quaternion.identity,
            maxDistance,
            blockingElementsLayerMask);

        return result;

        if (result.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }*/
}
