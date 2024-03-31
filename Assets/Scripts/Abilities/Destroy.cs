using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] BasePlayer player;
    //dynamic fields
    public float distanceFromPlayer = 10f;
    public KeyCode abilityKey = KeyCode.R;
    public GameObject Destroyed;

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
            Debug.Log("Object in destroy path -> " + objectFacing);
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
        //check if the current object faced is a destructable wall
        if (thisObjectFaced.GetComponent<DestructibleWall>() != null) {
            //if ability key pressed, destroy this wall
            if (Input.GetKeyDown(abilityKey)) {
                thisObjectFaced.GetComponent<DestructibleWall>().BreakWall();
                //Instantiate(Destroyed);
                //Destroy(thisObjectFaced);
            }
        }
    }
}
