using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public UnityEvent OnCollectEvent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            OnCollectEvent?.Invoke();
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
