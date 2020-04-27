using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionReceiver : MonoBehaviour
{
    public UnityEvent OnCollidedFromRight, OnCollidedFromLeft, OnCollidedFromAbove, OnCollidedFromBelow;
}
