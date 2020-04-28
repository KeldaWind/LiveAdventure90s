using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemyBarrel : ProjectileBase
{
    public override void Update()
    {
        base.Update();
    }

    [Header("Physics")]
    [SerializeField] BoxRaycaster barrelRaycaster = default;
    [SerializeField] float gravity = 30f;
    [SerializeField] float maxVerticalDownSpeed = 30f;

    public override void HandleCollision(RaycastHit hit)
    {

    }

    public override void UpdateTrajectory()
    {

    }
}
