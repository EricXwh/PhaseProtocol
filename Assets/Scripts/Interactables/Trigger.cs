using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private bool triggerActive = false;

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            triggerActive = true;
        }
    }

    public void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            triggerActive = false;
        }
    }

    public void Update() {
        if (triggerActive && Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("interacted"); //change scene
        }
    }

}
