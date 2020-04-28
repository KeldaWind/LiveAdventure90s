using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] int targetFrameRate = 60;

    [Header("Main Characters")]
    [SerializeField] ThirdPersonController thirdPersonController = default;
    [SerializeField] FirstPersonController firstPersonController = default;

    [Header("Level Bounds")]
    [SerializeField] Transform bottomBound = default;
    [SerializeField] Transform topBound = default;

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

        Application.targetFrameRate = targetFrameRate;

        firstPersonController.SetThirdPersonRef(thirdPersonController);
        firstPersonController.SetUpBounds(bottomBound, topBound);
    }

    #region Important Values
    public Vector3 GetCameraWorldPosition => firstPersonController.GetCameraWorldPosition;
    #endregion
}
