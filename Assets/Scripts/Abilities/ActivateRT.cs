using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateRT : MonoBehaviour
{
    /*
    public GameObject player;
    public static GameObject checkpoint = null;
    private Vector3 respawnPoint;

    //dynamic fields
    public float distanceFromPlayer = 5f;
    public KeyCode abilityKey = KeyCode.R;
    
    //called once at game start
    void Start(){
        player = GameObject.FindWithTag("Player");
    }

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

    public void checkpointRespawn() {
        player.transform.position = checkpoint.transform.position + new Vector3(2,0,2);
        player.transform.rotation = checkpoint.transform.rotation;
        PlayerHealth.isDead = false;
    }

    //called once per frame
    void Update() {
        GameObject thisObjectFaced = getObjectFaced();
        //check if the current object faced is an exit terminal
        if (thisObjectFaced.GetComponent<RespawnTerminal>() != null) {
            //set respawn point when close
            if (Input.GetKeyDown(abilityKey)) {
                Debug.Log("checkpoint set");
                checkpoint = thisObjectFaced;
            }
            
        }

        if ((checkpoint != null) && PlayerHealth.isDead) {
            checkpointRespawn();
        }
    }
    */
}
