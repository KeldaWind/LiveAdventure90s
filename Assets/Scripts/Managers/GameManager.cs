﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float frameRange;
    public Vector3 herosDirection;
    public Vector3 herosPos;

    [SerializeField] int targetFrameRate = 60;

    [Header("Main Characters")]
    [SerializeField] ThirdPersonController thirdPersonController = default;
    public FirstPersonController firstPersonController = default;

    [Header("Level Bounds")]
    [SerializeField] Transform bottomBound = default;
    [SerializeField] Transform topBound = default;

    public Action OnEndOfGameEvent;


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

        OnEndOfGameEvent += firstPersonController.SetGameOver;
        thirdPersonController.GetLifeSystem.OnLifeReachedZero += GameOver;

        AudioManager.PlayAmbianceMusic();
    }

    public bool IsPlayerOutOfFrame()
    {
        float distance = firstPersonController.gameObject.transform.localPosition.y - thirdPersonController.gameObject.transform.localPosition.y;
        herosDirection = (firstPersonController.gameObject.transform.localPosition - thirdPersonController.gameObject.transform.localPosition).normalized;
        herosPos = thirdPersonController.gameObject.transform.position;

        if (Math.Abs(distance) > frameRange)
        {

            return true;
        }

        return false;
    }

    public void GameOver()
    {
        OnEndOfGameEvent?.Invoke();
        UIManager.Instance.PlayLoseAnim();
        AudioManager.PlayLoseMusic();
        StartCoroutine(TimeBeforeRestart(UIManager.Instance.GetLoseAnimationDuration()));
    }

    IEnumerator TimeBeforeRestart(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Victory()
    {
        OnEndOfGameEvent?.Invoke();
        AudioManager.PlayWinMusic();
        UIManager.Instance.PlayWinAnim();
    }

    #region Important Values
    public Vector3 GetCameraWorldPosition => firstPersonController.GetCameraWorldPosition;
    #endregion
}
