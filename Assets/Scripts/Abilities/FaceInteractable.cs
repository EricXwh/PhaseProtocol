using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceInteractable : MonoBehaviour
{
    [SerializeField] BasePlayer player;
    //dynamic fields
    public float distanceFromPlayer = 30f;
    public KeyCode abilityKey = KeyCode.R;
    
    //called once at game start
    void Start(){}

    GameObject getObjectFaced() {
        //Raycast out from the camera's center
        Camera cam = GameObject.Find("Camera").GetComponent<Camera>();
        Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);

        RaycastHit hit;

        //if raycast hits an object a certain distance away from the player, return that object
        if (Physics.Raycast(forwardRay, out hit, distanceFromPlayer)) {
            GameObject objectFacing = hit.transform.gameObject;
            return objectFacing;
        }
        //if raycast hits no object, return a new game object to avoid null referencing
        else {
            GameObject temp = new GameObject();
            temp.hideFlags = HideFlags.HideAndDontSave;
            return temp;
        }
    }

    //called once per frame
    void Update() {
        GameObject thisObjectFaced = getObjectFaced();
        //check if the current object faced is an interactable object
        if (thisObjectFaced.GetComponent<InteractableObject>() != null) {
            //log the name of object to console
            Debug.Log(thisObjectFaced);
            if (Input.GetKeyDown(abilityKey)) {
                Debug.Log("Ability activated!");
            }
        }
    }
}
