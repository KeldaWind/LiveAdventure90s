using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessAnimTrigger_Shoot : PostProcessAnimTrigger
{
    [SerializeField] ThirdPersonController thirdPersonController;

    private void OnEnable()
    {
        thirdPersonController.OnPlayerShotProjectile += PlayPostProcessAnim;
        //subscribe
    }

    private void OnDisable()
    {

        thirdPersonController.OnPlayerShotProjectile -= PlayPostProcessAnim;
        //unsubscribe
    }
}
