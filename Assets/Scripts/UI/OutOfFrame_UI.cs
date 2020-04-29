using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OutOfFrame_UI : MonoBehaviour
{
    [Header("Get Components")]
    public TextMeshProUGUI outOfFrameWarning;
    public TextMeshProUGUI outOfFrameTimer;
    public RectTransform herosDirectionIndicator;

    private bool isHeroOutOfFrame;
    private float currentOutOfFrameTime;


    void Update()
    {
        if (isHeroOutOfFrame)
        {
            herosDirectionIndicator.gameObject.SetActive(true);
            RefreshOutOfFrameTimer();

            if (!GameManager.Instance.IsPlayerOutOfFrame())
            {
                InFrame();
            }
        }
        else
        {
            herosDirectionIndicator.gameObject.SetActive(false);

            if (GameManager.Instance.IsPlayerOutOfFrame())
            {
                RefreshOutOfFrameWarning();
            }
        }
    }


    void RefreshOutOfFrameWarning()
    {
        outOfFrameWarning.gameObject.SetActive(true);
        currentOutOfFrameTime = UIManager.Instance.maxOutOfFrameTime + 1;
        outOfFrameTimer.gameObject.SetActive(true);
        isHeroOutOfFrame = true;
    }

    void RefreshOutOfFrameTimer()
    {
        currentOutOfFrameTime = currentOutOfFrameTime - Time.deltaTime;
        ShowHeroDirection();

        int current = (int)currentOutOfFrameTime;
        string timerText = current.ToString();
        outOfFrameTimer.text = timerText;

        if (currentOutOfFrameTime < 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    void InFrame()
    {
        outOfFrameWarning.gameObject.SetActive(false);
        outOfFrameTimer.gameObject.SetActive(false);
        isHeroOutOfFrame = false;
        currentOutOfFrameTime = UIManager.Instance.maxOutOfFrameTime + 1;
    }

    void ShowHeroDirection()
    {
        Quaternion newRotation = Quaternion.LookRotation(GameManager.Instance.herosDirection, Vector3.forward);
        herosDirectionIndicator.rotation = newRotation;

        if (GameManager.Instance.herosDirection.y < 0)
            herosDirectionIndicator.anchoredPosition = new Vector3(0, 250, 0);
        else
            herosDirectionIndicator.anchoredPosition = new Vector3(0, -260, 0);
    }

    void AnimIndicator()
    {

    }
}
