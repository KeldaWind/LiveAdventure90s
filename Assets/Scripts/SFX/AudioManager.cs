using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    public enum Sound
    {
        C_CameraBallKeep,/**/
        C_HeroSreenExit_loop,
        C_JetpackShutdown,//
        C_JetpackUp_loop,//
        C_CameraBallDrop,/**/
        C_CameraBallLock,/**/
        E_EnnemyDeath,//
        E_EnnemyThrow,//
        E_EnnemyProjectileDestroy,//
        E_EnnemyProjectileLanding,//
        H_HeroTakeDamage,//
        H_GunShoot,//
        H_HeroJump,//
        H_HeroJumpLanding,//
        H_HeroHeal,/**/
        H_ImpactShootWall,//
        H_ImpactShootEnemie,//
        H_ImpactShootEnemieProjectile,//
        H_HeroTakeBall,/**/
        H_HeroPoseBall,/**/
        H_HeroRecupCollectible,//
        LD_evelvatorActive,
        LD_RailActive,
        LD_elevatorDisable,
        LD_RailDisable,
        LD_LaserActive,
        LD_LaserDisable,
        LD_DoorOpen,
        LD_DoorClose,
        Other_CameraOn,
        Other_CameraShutdown,
        THEME_AmbianceLevel_loop,
        THEME_TitleScreen_loop,
        THEME_GameOver,
        THEME_YouWin,
        None
    }


    //Exemple :  AudioManager.PlaySound(AudioManager.Sound.THEME_YouWin);
    public static void PlaySound(Sound sound)
    {
        if (!GameAssets.i)
        {
            Debug.LogWarning("NO AUDIO MANAGER ON SCENE");
            return;
        }

        GameAssets.SoundAudioClip soundParameters = GameAssets.i.GetAudioClip(sound);

        if (soundParameters == null)
            return;

        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        float volume = 1f;
        switch (soundParameters.volumeMultiplier.mode)
        {
            case ParticleSystemCurveMode.Constant:
                if (soundParameters.volumeMultiplier.constant > 0)
                    volume = soundParameters.volumeMultiplier.constant;
                break;
            case ParticleSystemCurveMode.TwoConstants:
                volume = Random.Range(soundParameters.volumeMultiplier.constantMin, soundParameters.volumeMultiplier.constantMax);
                break;
        }

        float pitch = 1f;
        switch (soundParameters.pitch.mode)
        {
            case ParticleSystemCurveMode.Constant:
                if (soundParameters.pitch.constant > 0)
                    pitch = soundParameters.pitch.constant;
                break;

            case ParticleSystemCurveMode.TwoConstants:
                pitch = Random.Range(soundParameters.pitch.constantMin, soundParameters.pitch.constantMax);
                break;
        }

        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(soundParameters.AudioClip);

        SoundObject soundObj = soundGameObject.AddComponent<SoundObject>();
        soundObj.SetAudioSource(audioSource);
    }

    /*public static void PlaySound2(AudioClip AudioClipTest)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(AudioClipTest);

    }*/


    /*private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray)
        {
            if(soundAudioClip.sound == sound)
            {
                return soundAudioClip.AudioClip;
            }
        }
        Debug.LogError("Sound" + sound + "not found!");
        return null;
    }*/
}
