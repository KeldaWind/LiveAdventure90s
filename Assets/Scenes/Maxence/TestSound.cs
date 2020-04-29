using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{

    public AudioClip AudioClipTest;

    public void TestSoundF()
    {
        //AudioManager.PlaySound2(AudioClipTest);
        AudioManager.PlaySound(AudioManager.Sound.THEME_YouWin);
    }
}
