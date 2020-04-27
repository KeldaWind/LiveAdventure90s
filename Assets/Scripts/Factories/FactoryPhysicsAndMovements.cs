using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FactoryPhysicsAndMovements
{
    public static float GetRemainingTravelledDistanceIfStopMovingNow(float currentSpeed, float desceleration)
    {
        return (currentSpeed * currentSpeed)/(2 * Mathf.Abs(desceleration));
    }
}
