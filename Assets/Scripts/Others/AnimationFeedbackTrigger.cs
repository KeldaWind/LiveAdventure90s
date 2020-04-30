using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFeedbackTrigger : MonoBehaviour
{
    [SerializeField] AudioManager.Sound[] soundsToPlay = new AudioManager.Sound[0];
    [SerializeField] string[] fxsToPlay = new string[0];
    [SerializeField] Transform positionToPlay = default;


    public void PlayFeedback()
    {
        if (soundsToPlay.Length > 0)
        {
            AudioManager.Sound sound = soundsToPlay[Random.Range(0, soundsToPlay.Length)];
            AudioManager.PlaySound(sound);
        }

        if (fxsToPlay.Length > 0)
        {
            string fxTag = fxsToPlay[Random.Range(0, fxsToPlay.Length)];
            Transform tr = positionToPlay ? positionToPlay : transform;
            FxManager.Instance.PlayFx(fxTag, positionToPlay.position, positionToPlay.rotation, positionToPlay.lossyScale);
        }
    }
}
