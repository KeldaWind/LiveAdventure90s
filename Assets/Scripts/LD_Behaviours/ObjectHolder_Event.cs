using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ObjectHolder_Event : MonoBehaviour
{
    public UnityEvent OnBallArrival;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "KeyBall")
        {
            OnBallArrival?.Invoke();
        }
    }
}
