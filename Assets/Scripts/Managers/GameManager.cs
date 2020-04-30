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

    [Header("Pause")]
    [SerializeField] KeyCode pauseJoystickInput = KeyCode.JoystickButton7;
    [SerializeField] KeyCode pauseKeyboardInput = KeyCode.KeypadEnter;

    bool GetPauseInputDown => Input.GetKeyDown(pauseJoystickInput) || Input.GetKeyDown(pauseKeyboardInput);

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
    }

    public void Start()
    {
        AudioManager.PlayAmbianceMusic();
    }

    private void Update()
    {
        if (!won && !gameOver)
        {
            if (GetPauseInputDown)
            {
                if (paused)
                    UnPauseGame();
                else
                    PauseGame();
            }
        }
        else
        {
            if (GetPauseInputDown)
            {
                if (restarting)
                    return;

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                restarting = true;
            }
        }
    }
    bool restarting = false;

    bool paused = false;
    public void PauseGame()
    {
        if (!gameOver)
        {
            Time.timeScale = 0.0f;
            UIManager.Instance.ShowPausePanel();
            paused = true;
        }
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1.0f;
        UIManager.Instance.HidePausePanel();
        paused = false;
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

    bool gameOver = false;
    public void GameOver()
    {
        OnEndOfGameEvent?.Invoke();
        UIManager.Instance.PlayLoseAnim();
        AudioManager.PlayLoseMusic();
        StartCoroutine(TimeBeforeRestart(UIManager.Instance.GetLoseAnimationDuration()));
    }

    IEnumerator TimeBeforeRestart(float duration)
    {
        gameOver = true;
        yield return new WaitForSeconds(duration);
        if (GetCurrentCheckpoint)
            RespawnOnLastCheckpoint();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameOver = false;
    }

    bool won = false;
    float waitTime = 2f;
    public void Victory()
    {
        OnEndOfGameEvent?.Invoke();
        AudioManager.PlayWinMusic();
        UIManager.Instance.PlayWinAnim();
        thirdPersonController.Win();
        firstPersonController.Win();
        won = true;
        StartCoroutine(WinCoroutine());
    }

    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        UIManager.Instance.ShowEnd();
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

        if (passedCheckpoint.Count > 1)
            UIManager.Instance.PlayCheckpointPassFeedback();
    }

    public void RespawnOnLastCheckpoint()
    {
        thirdPersonController.Respawn(GetCurrentCheckpoint.GetRespawnTransform, GetCurrentCheckpoint.GetRespawnDirection);
        firstPersonController.Respawn(GetCurrentCheckpoint.GetRespawnTransform);
    
        AudioManager.PlayAmbianceMusic();
        UIManager.Instance.ShowAllUI();
    }
    #endregion

    #region Important Values
    public Vector3 GetCameraWorldPosition => firstPersonController.GetCameraWorldPosition;
    #endregion
}
