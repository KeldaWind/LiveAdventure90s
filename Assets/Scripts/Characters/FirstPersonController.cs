using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] string horizontalAxis = "FirstPersonHorizontalAxis";
    [SerializeField] float minimumAxisValueToConsider = 0.25f;

    // Update is called once per frame
    void Update()
    {
        UpdateLateralMovementValues(Input.GetAxis(horizontalAxis));
    }

    [Header("Horizontal Movement")]
    [SerializeField] float maxHorizontalSpeed = 5f;

    public void UpdateLateralMovementValues(float input)
    {
        input = (Mathf.Abs(input) > minimumAxisValueToConsider) ? Mathf.Sign(input) : 0;

        transform.position += Vector3.right * input * maxHorizontalSpeed * Time.deltaTime;
    }
}
