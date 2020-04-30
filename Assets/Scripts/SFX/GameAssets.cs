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

    [Header("Music")]
    [SerializeField] AudioSource ambianceMusicSource = default;
    [SerializeField] float ambianceMusicVolume = 1f;
    [SerializeField] AudioSource winMusicSource = default;
    [SerializeField] float winMusicVolume = 1f;
    [SerializeField] float winCrossFadeDuration = 0.5f;
    [SerializeField] AudioSource loseMusicSource = default;
    [SerializeField] float loseMusicVolume = 1f;
    [SerializeField] float loseCrossFadeDuration = 0.5f;

    TimerSystem crossFadeTimer = new TimerSystem();
    AudioSource crossFadingToSource = default;
    float crossFadingToSourceVolume = 1f;

    public void PlayAmbianceMusic()
    {
        if (ambianceMusicSource)
        {
            ambianceMusicSource.volume = ambianceMusicVolume;
            ambianceMusicSource.Play();
        }
        if (winMusicSource)
        {
            winMusicSource.Stop();
        }
        if (loseMusicSource)
        {
            loseMusicSource.Stop();
        }
    }

    public void PlayWinMusic()
    {
        crossFadingToSource = winMusicSource;
        crossFadingToSource.volume = 0;
        crossFadingToSourceVolume = winMusicVolume;
        crossFadingToSource.Play();

        crossFadeTimer = new TimerSystem();
        crossFadeTimer.ChangeTimerValue(winCrossFadeDuration);
        crossFadeTimer.StartTimer();
    }

    public void PlayLoseMusic()
    {
        crossFadingToSource = loseMusicSource;
        crossFadingToSource.volume = 0;
        crossFadingToSourceVolume = loseMusicVolume;
        crossFadingToSource.Play();

        crossFadeTimer = new TimerSystem();
        crossFadeTimer.ChangeTimerValue(loseCrossFadeDuration);
        crossFadeTimer.StartTimer();
    }

    private void Update()
    {
        if (!crossFadeTimer.TimerOver)
        {
            crossFadeTimer.UpdateTimer();
            if (ambianceMusicSource)
            {
                ambianceMusicSource.volume = Mathf.Lerp(ambianceMusicVolume, 0, crossFadeTimer.GetTimerCoefficient);
            }

            if (crossFadingToSource)
            {
                crossFadingToSource.volume = Mathf.Lerp(0, crossFadingToSourceVolume, crossFadeTimer.GetTimerCoefficient);
            }

            if (crossFadeTimer.TimerOver)
            {
                ambianceMusicSource.Stop();
            }
        }
    }
}
