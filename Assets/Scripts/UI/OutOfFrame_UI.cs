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
        Vector2 viewPos = WorldToCanvasPosition(transform.parent.GetComponent<RectTransform>(), Camera.main, GameManager.Instance.herosPos);

        if (GameManager.Instance.herosDirection.y < 0)
        {
            currentIndicator.anchoredPosition3D = new Vector3(viewPos.x, -145, 0);
        }
        else
        {
            currentIndicator.anchoredPosition3D = new Vector3(viewPos.x, -45, 0);
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
}
