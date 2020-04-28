using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoxRaycaster : MonoBehaviour
{
    RaycastHit lastHorizontalHitResult = default;
    RaycastHit lastVerticalHitResult = default;

    [Header("References")]
    [SerializeField] BoxCollider selfCollider = default;
    [SerializeField] Transform selfTr = default;
    [SerializeField] LayerMask checkMask = default;
    [SerializeField] float skinWidthMultiplier = 0.99f;

    public CharacterCollisionFlags flags;

    private void Start()
    {
        flags.Reset();
    }

    public float RaycastHorizontal(float distance)
    {
        RaycastHit hit = new RaycastHit();

        Vector3 actualSize = new Vector3(selfCollider.size.x * selfTr.lossyScale.x * skinWidthMultiplier, selfCollider.size.y * selfTr.lossyScale.y, selfCollider.size.z * selfTr.lossyScale.z);

        if (Physics.BoxCast(selfTr.position + selfCollider.center, actualSize * 0.5f, Vector3.right * Mathf.Sign(distance), out hit, selfTr.rotation, Mathf.Abs(distance), checkMask))
        {
            float startPoint = selfTr.position.x + selfCollider.center.x + selfCollider.size.x * 0.5f * Mathf.Sign(distance);
            float newDistance = Mathf.Sign(distance) * Mathf.Abs(hit.point.x - startPoint);

            if (distance < 0) flags.left = true;
            if (distance > 0) flags.right = true;

            lastHorizontalHitResult = hit;

            return newDistance;
        }

        lastHorizontalHitResult = hit;

        if (distance < 0) flags.left = false;
        if (distance > 0) flags.right = false;

        return distance;
    }

    public float RaycastVertical(float distance)
    {
        RaycastHit hit = new RaycastHit();

        Vector3 actualSize = new Vector3(selfCollider.size.x * selfTr.lossyScale.x, selfCollider.size.y * selfTr.lossyScale.y * skinWidthMultiplier, selfCollider.size.z * selfTr.lossyScale.z);

        if (Physics.BoxCast(selfTr.position + selfCollider.center, actualSize * 0.5f, Vector3.up * Mathf.Sign(distance), out hit, selfTr.rotation, Mathf.Abs(distance), checkMask))
        {
            float startPoint = selfTr.position.y + selfCollider.center.y + selfCollider.size.y * 0.5f * Mathf.Sign(distance);
            float newDistance = Mathf.Sign(distance) * Mathf.Abs(hit.point.y - startPoint);

            if (distance < 0) { flags.below = true; OnLanded?.Invoke(); }
            if (distance > 0) flags.above = true;

            return newDistance;
        }

        if (distance < 0) flags.below = false;
        if (distance > 0) flags.above = false;

        return distance;
    }

    public Action OnLanded;

    public bool CheckForGroundBelow(float checkDistance)
    {
        Vector3 actualSize = new Vector3(selfCollider.size.x * selfTr.lossyScale.x, selfCollider.size.y * selfTr.lossyScale.y, selfCollider.size.z * selfTr.lossyScale.z) * skinWidthMultiplier;
        bool groundBelow = Physics.BoxCast(selfTr.position + selfCollider.center, actualSize * 0.5f, Vector3.down, selfTr.rotation, checkDistance, checkMask);

        if (!groundBelow)
            flags.below = false;
        
        return groundBelow;
    }
}
