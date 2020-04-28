using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour
{
    UnityEvent OnCollectEvent;


    private void OnTriggerEnter(Collider other)
    {
        OnCollectEvent.Invoke();
        gameObject.SetActive(false);
    }
}
