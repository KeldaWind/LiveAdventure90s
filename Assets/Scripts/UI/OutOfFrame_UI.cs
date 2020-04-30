using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OutOfFrame_UI : MonoBehaviour
{
    [Header("Get Components")]
    public TextMeshProUGUI outOfFrameWarning;
    public TextMeshProUGUI outOfFrameTimer;

    public RectTransform herosUpIndicator;
    public RectTransform herosDownIndicator;
    private RectTransform currentIndicator;
    

    private bool isHeroOutOfFrame;
    private float currentOutOfFrameTime;

    TimerSystem outOfRangeTimerSystem = new TimerSystem();


    private void Start()
    {
        outOfRangeTimerSystem = new TimerSystem(UIManager.Instance.maxOutOfFrameTime, null);

        SetUpWarning();
    }

    void Update()
    {
        if (isHeroOutOfFrame)
        {
            if (GameManager.Instance.herosDirection.y > 0)
            {
                herosDownIndicator.gameObject.SetActive(true);
                currentIndicator = herosDownIndicator;
            }
            else
            {
                herosUpIndicator.gameObject.SetActive(true);
                currentIndicator = herosUpIndicator;
            }

            RefreshOutOfFrameTimer();

            if (!GameManager.Instance.IsPlayerOutOfFrame())
            {
                InFrame();
            }
        }
        else
        {
            herosDownIndicator.gameObject.SetActive(false);
            herosUpIndicator.gameObject.SetActive(false);

            if (GameManager.Instance.IsPlayerOutOfFrame())
            {
                //print("HELP");
                RefreshOutOfFrameWarning();
            }
        }
    }


    void RefreshOutOfFrameWarning()
    {
        outOfFrameWarning.gameObject.SetActive(true);
        //currentOutOfFrameTime = UIManager.Instance.maxOutOfFrameTime + 1;
        outOfFrameTimer.gameObject.SetActive(true);
        isHeroOutOfFrame = true;

        outOfRangeTimerSystem.StartTimer();
        StartWarningFeedback();
    }

    void RefreshOutOfFrameTimer()
    {
        if (outOfRangeTimerSystem.TimerOver)
            return;

        //currentOutOfFrameTime = currentOutOfFrameTime - Time.deltaTime;
        outOfRangeTimerSystem.UpdateTimer();
        UpdateWarningFeedback();

        if (outOfRangeTimerSystem.TimerOver)
        {
            GameManager.Instance.GameOver();
            outOfFrameTimer.text = "OUT";
            return;
        }

        ShowHeroDirection();

        int current = ((int)outOfRangeTimerSystem.GetTimerCounter) + 1;
        string timerText = current.ToString();
        outOfFrameTimer.text = timerText;

        /*if (outOfRangeTimer.TimerOver)
        {
            GameManager.Instance.GameOver();
        }*/
    }

    void InFrame()
    {
        outOfFrameWarning.gameObject.SetActive(false);
        outOfFrameTimer.gameObject.SetActive(false);
        isHeroOutOfFrame = false;
        //currentOutOfFrameTime = UIManager.Instance.maxOutOfFrameTime + 1;
    }

    void ShowHeroDirection()
    {
        Vector2 viewPos = WorldToCanvasPosition(transform.parent.GetComponent<RectTransform>(), Camera.main, GameManager.Instance.herosPos);

        if (GameManager.Instance.herosDirection.y < 0)
        {
            currentIndicator.anchoredPosition3D = new Vector3(viewPos.x, -120, 0);
        }
        else
        {
            currentIndicator.anchoredPosition3D = new Vector3(viewPos.x, -25, 0);
        }
    }

    private Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
    {
        Vector2 temp = camera.WorldToViewportPoint(position);

        temp.x *= canvas.sizeDelta.x;
        temp.y *= canvas.sizeDelta.y;

        temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
        temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

        return temp;
    }

    #region Feedbacks
    [Header("Lose Sight Feedbacks")]
    [SerializeField] AudioManager.Sound warningSound = AudioManager.Sound.C_HeroSreenExit_loop;
    [SerializeField] int loseSightSoundCountPerSerie = 3;
    [SerializeField] float minTimeBetweenWarningSounds = 0.15f;
    [SerializeField] float maxTimeBetweenWarningSounds = 0.05f;
    [SerializeField] AnimationCurve betweenWarningSoundsCurve = AnimationCurve.Linear(0, 0, 1, 1);

    TimerSystem warningSerieTimer = new TimerSystem();

    [SerializeField] float minTimeBetweenWarningSeries = 0.3f;
    [SerializeField] float maxTimeBetweenWarningSeries = 0.15f;
    [SerializeField] AnimationCurve betweenWarningSeriesCurve = AnimationCurve.Linear(0, 0, 1, 1);

    TimerSystem betweenWarningSerieTimer = new TimerSystem();

    public void SetUpWarning()
    {
        warningSerieTimer = new TimerSystem(minTimeBetweenWarningSounds, EndWarningSerie, loseSightSoundCountPerSerie, PlayWarningSound);
        betweenWarningSerieTimer = new TimerSystem(minTimeBetweenWarningSeries, StartWarningSerie);
    }

    public void StartWarningFeedback()
    {
        StartWarningSerie();
    }

    public void UpdateWarningFeedback()
    {
        if (!warningSerieTimer.TimerOver)
        {
            warningSerieTimer.UpdateTimer();
        }
        else
        {
            if (!betweenWarningSerieTimer.TimerOver)
            {
                betweenWarningSerieTimer.UpdateTimer();
            }
        }
    }

    public void StartWarningSerie()
    {
        float coeff = betweenWarningSoundsCurve.Evaluate(outOfRangeTimerSystem.GetTimerCoefficient);
        warningSerieTimer.ChangeTimerValue(Mathf.Lerp(minTimeBetweenWarningSounds, maxTimeBetweenWarningSounds, coeff));
        warningSerieTimer.StartTimer();

        //print("Between Sounds : " + Mathf.Lerp(minTimeBetweenWarningSounds, maxTimeBetweenWarningSounds, coeff));
    }

    public void EndWarningSerie()
    {
        float coeff = betweenWarningSeriesCurve.Evaluate(outOfRangeTimerSystem.GetTimerCoefficient);
        betweenWarningSerieTimer.ChangeTimerValue(Mathf.Lerp(minTimeBetweenWarningSeries, maxTimeBetweenWarningSeries, coeff));
        betweenWarningSerieTimer.StartTimer();

        //print("Between Series : " + Mathf.Lerp(minTimeBetweenWarningSeries, maxTimeBetweenWarningSeries, coeff));
    }

    public void PlayWarningSound()
    {
        AudioManager.PlaySound(warningSound);
    }
    #endregion
}
