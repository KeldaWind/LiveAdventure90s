using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Life_UI : MonoBehaviour
{
    [Header("Get Components")]
    public TextMeshProUGUI lifeText;


    public void SetLife(int maxLife)
    {
        string currentText = maxLife.ToString();
        lifeText.text = currentText;
    }

    public void RefreshLife(int amount, int current)
    {
        string currentText = current.ToString();
        lifeText.text = currentText;
    }
}
