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
    public int laserDamages = 1;

    private bool laserIsActive = true;

    private Vector3 impactPos;

    public void DeactivateForDuration(float duration)
    {
        deactivationTimer = new TimerSystem(duration, EndDeactivation);
        deactivationTimer.StartTimer();

        laserCollider.enabled = false;
    }
    TimerSystem deactivationTimer = new TimerSystem();

    public void UpdateDeactivation()
    {
        deactivationTimer.UpdateTimer();
    }

    public void EndDeactivation()
    {
        if (laserIsActive)
            laserCollider.enabled = true;
    }



    private void Awake()
    {
        lineRenderer.SetPosition(0, startPos.transform.localPosition);
    }

    private void Update()
    {
        if (laserIsActive)
            FindNextTarget();

        if (!deactivationTimer.TimerOver)
            UpdateDeactivation();
    }

    public void SetLaserState(bool value)
    {
        if (laserIsActive == value)
            return;

        laserIsActive = value;
        lineRenderer.enabled = value;

        if (value)
        {
            laserCollider.enabled = true;
            PlayLaserEnabling();
        }
        else
        {
            laserCollider.enabled = false;
            PlayLaserDisabling();
        }
    }

    void SetLaserCollider(float magnitude, Vector3 direction)
    {
        Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);
        laserCollider.transform.localScale = new Vector3(0.5f, 0.5f, magnitude);
        laserCollider.transform.rotation = newRotation;
        laserCollider.transform.position = startPos.transform.position + (direction * (magnitude * 0.5f));
    }

    void FindNextTarget()
    {
        float magnitude = (targetPos.transform.localPosition - startPos.transform.localPosition).magnitude;
        Vector3 direction = (targetPos.transform.localPosition - startPos.transform.localPosition).normalized;
        RaycastHit hit;

        Physics.Raycast(startPos.transform.position, direction, out hit, Mathf.Infinity);

        if (hit.collider != null)
            impactPos = hit.point;

        Vector3 newDestination = new Vector3(impactPos.x - this.transform.localPosition.x, impactPos.y - this.transform.localPosition.y, impactPos.z - this.transform.localPosition.z);
        lineRenderer.SetPosition(1, newDestination);


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

    [Header("Feedbacks")]
    [SerializeField] AudioManager.Sound onLaserEnabled = AudioManager.Sound.LD_LaserActive;
    [SerializeField] AudioManager.Sound onLaserDisabled = AudioManager.Sound.LD_LaserDisable;

    public void PlayLaserEnabling()
    {
        AudioManager.PlaySound(onLaserEnabled);
    }

    public void PlayLaserDisabling()
    {
        AudioManager.PlaySound(onLaserDisabled);
    }
}
