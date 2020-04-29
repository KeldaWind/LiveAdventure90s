using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Laser_Behaviour : MonoBehaviour
{
    [Header("Getting Components")]
    public LineRenderer lineRenderer;
    public GameObject startPos;
    public GameObject targetPos;

    public BoxCollider laserCollider;

    private bool isActive = true;

    private Vector3 impactPos;




    private void Awake()
    {
        lineRenderer.SetPosition(0, startPos.transform.localPosition);
    }

    private void Update()
    {
        if (isActive)
            FindNextTarget();
    }

    public void SetLaserState(bool value)
    {
        isActive = value;
        lineRenderer.enabled = value;

        if (value)
            laserCollider.enabled = true;
        else
            laserCollider.enabled = false;
    }

    void SetLaserCollider(float magnitude, Vector3 direction)
    {
        Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);
        laserCollider.transform.localScale = new Vector3(0.5f, 0.5f, magnitude);
        laserCollider.transform.rotation = newRotation;
        laserCollider.transform.position = startPos.transform.localPosition + (direction * (magnitude * 0.5f));
    }

    void FindNextTarget()
    {
        float magnitude = (targetPos.transform.localPosition - startPos.transform.localPosition).magnitude;
        Vector3 direction = (targetPos.transform.localPosition - startPos.transform.localPosition).normalized;
        RaycastHit hit;

        Physics.Raycast(startPos.transform.localPosition, direction, out hit, Mathf.Infinity);

        if (hit.collider != null)
            impactPos = hit.point;
        lineRenderer.SetPosition(1, impactPos);


        SetLaserCollider(magnitude, direction);

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
