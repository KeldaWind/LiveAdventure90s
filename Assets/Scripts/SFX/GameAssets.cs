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

        GenerateDictionary();
    }

    private static GameAssets _i;

    public static GameAssets i => _i;

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public AudioManager.Sound sound;
        public AudioClip AudioClip;
        public ParticleSystem.MinMaxCurve volumeMultiplier;
        public ParticleSystem.MinMaxCurve pitch;
    }

    Dictionary<AudioManager.Sound, SoundAudioClip> soundsDictionary = new Dictionary<AudioManager.Sound, SoundAudioClip>();
    public void GenerateDictionary()
    {
        soundsDictionary = new Dictionary<AudioManager.Sound, SoundAudioClip>();

        foreach (SoundAudioClip audioClip in soundAudioClipArray)
        {
            if (!soundsDictionary.ContainsKey(audioClip.sound) && audioClip.AudioClip)
            {
                soundsDictionary.Add(audioClip.sound, audioClip);
            }
        }
    }

    public SoundAudioClip GetAudioClip(AudioManager.Sound soundType)
    {
        if (soundsDictionary.ContainsKey(soundType))
            return soundsDictionary[soundType];
        else
            return null;
    }
}
