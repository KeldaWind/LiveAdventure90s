using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Life_UI : MonoBehaviour
{
    [Header("Get Components")]
    public GameObject lifePointPrefab;
    private List<Image> lifePoints;

    public Sprite enableSprite;
    public Sprite unenableSprite;

    public float lifeIconSpace = 10f;
    private float lifeIconSizeref;
    private int maxLife;


    private void Awake()
    {
        lifeIconSizeref = lifePointPrefab.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
    }

    public void SetLife(int currentLife)
    {
        lifePoints = new List<Image>();
        maxLife = currentLife;

        for (int i = 0; i < currentLife; i++)
        {
            GameObject newPoint = Instantiate(lifePointPrefab, this.transform);
            RectTransform rectPoint = newPoint.GetComponent<RectTransform>();
            Image pointImage = newPoint.GetComponent<Image>();
            pointImage.sprite = enableSprite;
            pointImage.color = new Color32(255,255,255,255);

            Vector3 newPos;
            newPos = new Vector3(0, ((rectPoint.sizeDelta.y * i) + (lifeIconSpace * i)), 0);

            rectPoint.GetComponent<RectTransform>().anchoredPosition3D = newPos;

            lifePoints.Add(pointImage);
        }
    }

    public void RefreshLife(int amount, int current)
    {
        for (int i = 0; i < current; i++)
        {
            lifePoints[i].sprite = enableSprite;
            lifePoints[i].color = new Color32(255,255,255,255);
        }

        for (int i = current; i < maxLife; i++)
        {
            lifePoints[i].sprite = unenableSprite;
            lifePoints[i].color = new Color32(170, 170, 170, 255);
        }

        SetLife(current);
    }

}
