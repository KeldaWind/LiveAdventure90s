using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxObject : MonoBehaviour
{
    [SerializeField] ParticleSystem[] particlesToPlay = new ParticleSystem[0];

    private void Update()
    {
        CheckForDestroy();
    }

    public void PlayFx()
    {
        bool atLeastOnePlayed = false;

        foreach(ParticleSystem fx in particlesToPlay)
        {
            if (fx)
            {
                fx.Play();
                atLeastOnePlayed = true;
            }
        }

        if (!atLeastOnePlayed)
            DestroyFx();
    }

    public void DestroyFx()
    {
        Destroy(gameObject);
    }

    public void CheckForDestroy()
    {
        bool canDestroy = true;
        foreach (ParticleSystem fx in particlesToPlay)
        {
            if(fx.particleCount > 0 || fx.isPlaying)
            {
                canDestroy = false;
                break;
            }
        }

        if (canDestroy)
            DestroyFx();
    }
}
