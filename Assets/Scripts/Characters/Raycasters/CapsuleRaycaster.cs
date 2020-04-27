using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleRaycaster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CapsuleCollider selfCollider = default;
    [SerializeField] Transform selfTr = default;
    [SerializeField] LayerMask checkMask = default;

    public CharacterCollisionFlags flags;

    private void Start()
    {
        flags.Reset();
    }

    public float RaycastHorizontal(float distance)
    {
        RaycastHit hit = new RaycastHit();

        float radius = selfCollider.radius;
        float height = selfCollider.height;
        Vector3 centerOffset = selfCollider.center;
        float capsulePointsOffset = Mathf.Clamp((selfCollider.height / 2) - selfCollider.radius, 0, selfCollider.height/2);

        if(Physics.CapsuleCast(
            selfTr.position + centerOffset + selfTr.up * capsulePointsOffset, 
            selfTr.position + centerOffset - selfTr.up * capsulePointsOffset,
            radius, selfTr.right, out hit, Mathf.Abs(distance), checkMask))
        {
            Debug.DrawRay(selfCollider.ClosestPoint(hit.point), -hit.normal, Color.red);

            /*float startPoint = selfTr.position.x + boxCollider.size.x * 0.5f * Mathf.Sign(distance);
            float newDistance = Mathf.Sign(distance) * Mathf.Abs(hit.point.x - startPoint);

            if (distance < 0) flags.left = true;
            if (distance > 0) flags.right = true;

            CollisionReceiver receiver = hit.collider.GetComponent<CollisionReceiver>();
            if (receiver != null)
            {
                if (distance < 0)
                    receiver.OnCollidedFromRight?.Invoke();
                if (distance > 0)
                    receiver.OnCollidedFromLeft?.Invoke();
            }

            return newDistance;*/
        }

        /*if (hit.collider != null)
        {
            float startPoint = selfTr.position.x + boxCollider.size.x * 0.5f * Mathf.Sign(distance);
            float newDistance = Mathf.Sign(distance) * Mathf.Abs(hit.point.x - startPoint);

            if (distance < 0) flags.left = true;
            if (distance > 0) flags.right = true;

            CollisionReceiver receiver = hit.collider.GetComponent<CollisionReceiver>();
            if (receiver != null)
            {
                if (distance < 0)
                    receiver.OnCollidedFromRight?.Invoke();
                if (distance > 0)
                    receiver.OnCollidedFromLeft?.Invoke();
            }

            return newDistance;
        }*/

        if (distance < 0) flags.left = false;
        if (distance > 0) flags.right = false;

        return distance;
    }

    /*public float RaycastVertical(float distance)
    {
        RaycastHit2D hit = new RaycastHit2D();

        Vector2 actualSize = new Vector2(boxCollider.size.x * selfTr.lossyScale.x, boxCollider.size.y * selfTr.lossyScale.y) * skinWidthMultiplier;

        hit = Physics2D.BoxCast(selfTr.position, actualSize, 0, Vector2.up * Mathf.Sign(distance), Mathf.Abs(distance), layerMask);

        if (hit.collider != null)
        {
            float startPoint = selfTr.position.y + boxCollider.size.y * 0.5f * Mathf.Sign(distance);
            float newDistance = Mathf.Sign(distance) * Mathf.Abs(hit.point.y - startPoint);

            if (distance < 0) flags.below = true;
            if (distance > 0) flags.above = true;

            CollisionReceiver receiver = hit.collider.GetComponent<CollisionReceiver>();
            if (receiver != null)
            {
                if (distance < 0)
                    receiver.OnCollidedFromAbove?.Invoke();
                if (distance > 0)
                    receiver.OnCollidedFromBelow?.Invoke();
            }

            return newDistance;
        }

        if (distance < 0) flags.below = false;
        if (distance > 0) flags.above = false;

        return distance;
    }*/
}
