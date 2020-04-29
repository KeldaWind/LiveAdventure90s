using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JetPackUI : MonoBehaviour
{
    public Image jetpackGauge;
    private float maxVelocity;
    private float currentVelocity;


    private void Update()
    {
        RefreshJetpackGauge();
    }

    void RefreshJetpackGauge()
    {
        jetpackGauge.fillAmount = currentVelocity / maxVelocity;
    }
}
