using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Get Components")]
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI outOfFrameWarning;
    public TextMeshProUGUI outOfFrameTimer;
    public DamageableEntity herosLife;

    public int maxOutOfFrameTime;
    private int currentOutOfFrameTime;

    private bool isHeroOutOfFrame;


    private void Awake()
    {
        Instance = this;

        //herosLife.OnLifeAmountChanged += RefreshLife;
        herosLife.OnLifeAmountChanged += RefreshLife;
    }

    private void Update()
    {
        if (isHeroOutOfFrame)
        {
            RefreshOutOfFrameTimer();

            if (!GameManager.Instance.IsPlayerOutOfFrame())
            {
                InFrame();
            }
        }
        else
        {
            if(GameManager.Instance.IsPlayerOutOfFrame())
            {
                RefreshOutOfFrameWarning();
            }
        }
    }

    void RefreshLife(int amount, int current)
    {
        string currentText = current.ToString();
        lifeText.text = currentText;
    }

    void RefreshOutOfFrameWarning()
    {
        outOfFrameWarning.gameObject.SetActive(true);
        currentOutOfFrameTime = maxOutOfFrameTime;
        outOfFrameTimer.gameObject.SetActive(true);
        isHeroOutOfFrame = true;
    }

    void RefreshOutOfFrameTimer()
    {
        currentOutOfFrameTime -= (int)Time.deltaTime;

        string timerText = currentOutOfFrameTime.ToString();
        outOfFrameTimer.text = timerText;

        if (currentOutOfFrameTime < 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    void InFrame()
    {
        outOfFrameWarning.gameObject.SetActive(false);
        outOfFrameTimer.gameObject.SetActive(true);
        isHeroOutOfFrame = false;
    }
}
