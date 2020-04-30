using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessAnimTrigger_PlayerDamaged : PostProcessAnimTrigger
{
    [SerializeField] ThirdPersonController thirdPersonController;

    private void OnEnable()
    {
        thirdPersonController.OnCharacterReceivedDamage += PlayPostProcessAnim;
        //subscribe
    }

    private void OnDisable()
    {

        thirdPersonController.OnCharacterReceivedDamage -= PlayPostProcessAnim;
        //unsubscribe
    }
}
