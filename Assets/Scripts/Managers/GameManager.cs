using System;
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
        RespawnOnLastCheckpoint();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Victory()
    {
        OnEndOfGameEvent?.Invoke();
        AudioManager.PlayWinMusic();
        UIManager.Instance.PlayWinAnim();
    }

    #region Checkpoints
    List<Checkpoint> passedCheckpoint = new List<Checkpoint>();
    public Checkpoint GetCurrentCheckpoint {
        get
        {
            if (passedCheckpoint.Count > 0)
                return passedCheckpoint[passedCheckpoint.Count - 1];
            else
                return null;
        } 
    }

    public void PassCheckpoint(Checkpoint cp)
    {
        if (passedCheckpoint.Contains(cp))
            return;

        passedCheckpoint.Add(cp);
    }

    public void RespawnOnLastCheckpoint()
    {
        thirdPersonController.Respawn(GetCurrentCheckpoint.GetRespawnTransform, GetCurrentCheckpoint.GetRespawnDirection);
        firstPersonController.Respawn(GetCurrentCheckpoint.GetRespawnTransform);
    
        AudioManager.PlayAmbianceMusic();
    }
    #endregion

    #region Important Values
    public Vector3 GetCameraWorldPosition => firstPersonController.GetCameraWorldPosition;
    #endregion
}
