using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Main Characters")]
    [SerializeField] ThirdPersonController thirdPersonController = default;
    [SerializeField] FirstPersonController firstPersonController = default;

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

        firstPersonController.SetFirstPersonRef(thirdPersonController);
    }

    #region Important Values
    public Vector3 GetCameraWorldPosition => firstPersonController.GetCameraWorldPosition;
    #endregion
}
