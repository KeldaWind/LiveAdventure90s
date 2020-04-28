using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Behaviour : MonoBehaviour
{
    [Header("Getting Components")]
    public LineRenderer lineRenderer;
    public Vector3 startPos;
    public GameObject targetPos;

    private Vector3 impactPos;




    private void Awake()
    {
        lineRenderer.SetPosition(0, startPos);
    }

    private void Update()
    {
        FindNextTarget();
    }

    public void SetLaserState(bool value)
    {
        lineRenderer.enabled = value;
    }

    void FindNextTarget()
    {
        Vector3 direction = (targetPos.transform.position - lineRenderer.GetPosition(0)).normalized;
        RaycastHit hit;

        Physics.Raycast(lineRenderer.GetPosition(0), direction, out hit, Mathf.Infinity);

        if (hit.collider != null)
            impactPos = hit.point;
        lineRenderer.SetPosition(1, impactPos);

        if (IsCollidingWithPlayer(hit))
        {
            //Receive Damage ??
        }
    }

    bool IsCollidingWithPlayer(RaycastHit results)
    {
        if (results.collider == null)
            return false;

        if (results.collider.gameObject.tag == "Player")
            return true;
        else
            return false;
    }
}
