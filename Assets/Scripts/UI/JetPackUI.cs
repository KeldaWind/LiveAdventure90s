using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JetPackUI : MonoBehaviour
{
    public Image jetpackGauge;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        RefreshJetpackGauge();
        
    }

    void RefreshJetpackGauge()
    {
        jetpackGauge.fillAmount = GameManager.Instance.firstPersonController.GetJetpackJaugeCoefficient;
    }
}
