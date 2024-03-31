using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateET : MonoBehaviour
{
    [SerializeField] BasePlayer player;
    //dynamic fields
    public float distanceFromPlayer = 10f;
    public KeyCode abilityKey = KeyCode.R;
    public bool debug = false;
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
        if (debug)
            Debug.Log("Object faced -> " + thisObjectFaced);
        //check if the current object faced is an exit terminal
        if (thisObjectFaced.GetComponent<ExitTerminal>() != null) {
            if(ActivateST.allActivated) {
                //if ability key pressed, change scene
                if (Input.GetKeyDown(abilityKey)) {
                    //Reset detection level
                    GameManager.ResetDetectionLevel();
                    //Reset anger level
                    GameManager.angerLevel = 0;
                    //Reset player health to max
                    GameManager.ResetHPToMax();
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    //SceneManager.LoadScene("Zone0");
                    Debug.Log("EXIT TERMINAL ACTIVATED");
                }
            }
        }
    }
}
