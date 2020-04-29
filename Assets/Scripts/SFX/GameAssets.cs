using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private void Awake()
    {
        if (_i && _i != this)
        {
            Destroy(this);
        }
        else
        {
            _i = this;
        }
    }

    private static GameAssets _i;

    public static GameAssets i => _i;

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public AudioManager.Sound sound;
        public AudioClip AudioClip;
    }

}
