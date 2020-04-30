using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterFlower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform rendererTr = default;
    [SerializeField] Transform idleTr = default;
    [SerializeField] Transform leftAttackTr = default;
    [SerializeField] Transform rightAttackTr = default;
    Transform currentAttackTr = default;
    [SerializeField] float transitionCoeff = 0.3f;
    /*[SerializeField] float transitionDuration = 0.2f;
    [SerializeField] float transitionBackSpeed = 0.5f;
    [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);*/

    public Vector3 GetCurrentShootPosition { get { if (rendererTr) return rendererTr.position; else return transform.position; } }

    [Header("Global Movements Parameters")]
    [SerializeField] float maxHorizontalSpeed = 10f;
    [SerializeField] float horizontalAcceleration = 50f;
    [SerializeField] float horizontalBraking = 100f;
    float currentHorizontalSpeed = 0f;

    [SerializeField] float maxVerticalSpeed = 10f;
    [SerializeField] float verticalAcceleration = 50f;
    [SerializeField] float verticalBraking = 100f;
    float currentVerticalSpeed = 0f;

    [SerializeField] float stopSpeed = 2f;
    [SerializeField] float stopDistance = 0.1f;


    Transform currentGlobalTargetTr = default;
    public void SetCurrentGlobalTarget(Transform target, ShootDirection direction)
    {
        currentGlobalTargetTr = target;
        currentAttackTr = direction == ShootDirection.Left ? leftAttackTr : rightAttackTr;
    }

    bool isShooting = false;
    public void SetShooting()
    {
        isShooting = true;
    }

    public void ResetShooting()
    {
        isShooting = false;
    }

    private void Start()
    {
        transform.SetParent(null);
    }

    void Update()
    {
        if (!currentGlobalTargetTr)
            return;

        UpdateHorizontalValues();
        UpdateVerticalValues();

        UpdateMovement();

        if (isShooting)
        {
            rendererTr.transform.position = Vector3.Lerp(rendererTr.position, currentAttackTr.position, transitionCoeff);
        }
        else
        {
            rendererTr.transform.position = Vector3.Lerp(rendererTr.position, idleTr.position, transitionCoeff);
        }
    }

    public void UpdateHorizontalValues()
    {
        float horDiff = currentGlobalTargetTr.position.x - transform.position.x;

        if (Mathf.Abs(horDiff) < stopDistance && Mathf.Abs(currentHorizontalSpeed) < stopSpeed)
        {
            currentHorizontalSpeed = 0;
        }
        else
        {
            float inUseAcceleration = Mathf.Sign(horDiff) * (Mathf.Sign(horDiff) == Mathf.Sign(currentHorizontalSpeed) ? horizontalAcceleration : horizontalBraking);
            currentHorizontalSpeed = Mathf.Clamp(currentHorizontalSpeed + Time.deltaTime * inUseAcceleration, -maxHorizontalSpeed, maxHorizontalSpeed);
        }
    }

    public void UpdateVerticalValues()
    {
        float verDiff = currentGlobalTargetTr.position.y - transform.position.y;

        if (Mathf.Abs(verDiff) < stopDistance && Mathf.Abs(currentVerticalSpeed) < stopSpeed)
        {
            currentVerticalSpeed = 0;
        }
        else
        {
            float inUseAcceleration = Mathf.Sign(verDiff) * (Mathf.Sign(verDiff) == Mathf.Sign(currentVerticalSpeed) ? verticalAcceleration : verticalBraking);
            currentVerticalSpeed = Mathf.Clamp(currentVerticalSpeed + Time.deltaTime * inUseAcceleration, -maxVerticalSpeed, maxVerticalSpeed);
        }
    }

    public void UpdateMovement()
    {
        transform.position += new Vector3(currentHorizontalSpeed, currentVerticalSpeed, 0) * Time.deltaTime;
    }
}
