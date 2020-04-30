using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Transform respawnPos = default;
    [SerializeField] ShootDirection respawnDirection = ShootDirection.Right;

    public Transform GetRespawnTransform => respawnPos ? respawnPos : transform;
    public ShootDirection GetRespawnDirection => respawnDirection;

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController character = other.GetComponent<ThirdPersonController>();
        if (character)
        {
            GameManager.Instance.PassCheckpoint(this);
        }
    }
}
