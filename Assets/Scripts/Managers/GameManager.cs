using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Main Characters")]
    [SerializeField] FirstPersonController firstPersonController = default;
    [SerializeField] ThirdPersonController thirdPersonController = default;

    private void Awake()
    {
        if(Instance && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        thirdPersonController.SetFirstPersonRef(firstPersonController);
    }

    #region Important Values
    public Vector3 GetCameraWorldPosition => thirdPersonController.GetCameraWorldPosition;
    #endregion
}
