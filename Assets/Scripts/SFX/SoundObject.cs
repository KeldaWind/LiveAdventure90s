using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    AudioSource _audioSource = default;

    public void SetAudioSource(AudioSource audioSource)
    {
        _audioSource = audioSource;
    }

    public void DestroyAudio()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            DestroyAudio();
        }
    }
}
