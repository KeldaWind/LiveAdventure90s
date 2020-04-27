using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    float startZPosition = default;
    private void Start()
    {
        startZPosition = transform.position.z;
    }

    void Update()
    {
        Vector3 newPos = GameManager.Instance.GetCameraWorldPosition;
        newPos.z = startZPosition;
        transform.position = newPos;
    }
}
