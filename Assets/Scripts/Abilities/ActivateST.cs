using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class ActivateST : MonoBehaviour
{
    //dynamic fields
    public float distanceFromPlayer = 10f;
    public KeyCode abilityKey = KeyCode.R;
    public SubTerminal[] subterminalList;
    public List<SubTerminal> tempList = new List<SubTerminal>();
    public static bool allActivated = false;
    //These fields are exclusivly for the subterminal randomziation in zone 1
    [SerializeField]
    private Vector3[] subterminalSpawnLocationsZone1;
    [SerializeField]
    private Vector3[] subterminalSpawnLocationsZone2;
    [SerializeField]
    private Vector3[] subterminalSpawnLocationsZone3;
    [SerializeField]
    public GameObject subterminal;
    [SerializeField]
    public AudioSource activateSound;

    //called once at game start
    void Start(){
        Debug.Log("Number of Robots with this Script: " + FindObjectsOfType<ActivateST>().Length);
        //If in zone 1, instantiate the subterminal into the scene at a randomzed point
        //If we are in zone 1, randomize subterminal location
        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Zone1 (Final)")))
        {
            foreach (int subterminal in GameManager.subterminalPositionsZone1)
            {
                GameObject.Find("SubTerminalComponents (" + subterminal + ")").SetActive(false);
            }
        }
        //If in zone 2, set subterminals at predetermined points to be 
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Zone2")))
        {
            foreach (int subterminal in GameManager.subterminalPositionsZone2)
            {
                GameObject.Find("SubTerminalComponents (" + subterminal + ")").SetActive(false);
            }
        }
        //If in zone 3, set subterminals at predetermined points
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Zone3")))
        {
            foreach (int subterminal in GameManager.subterminalPositionsZone3)
            {
                GameObject.Find("SubTerminalComponents (" + subterminal + ")").SetActive(false);
            }
        }
        subterminalList = GameObject.FindObjectsOfType<SubTerminal>();
        tempList = subterminalList.ToList();
        Debug.Log("list: " + tempList);
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

    //called once per frame
    void Update() {
        Debug.Log("Number of Subterminals -> " + tempList.Count);
        GameObject thisObjectFaced = getObjectFaced();
        Debug.Log("Subterminal Faced? -> " + thisObjectFaced);
        Debug.Log("All activated? -> " + allActivated);
        //check if the current object faced is an exit terminal
        if (thisObjectFaced.GetComponent<SubTerminal>() != null) {
            //if ability key pressed, change scene
            if (Input.GetKeyDown(abilityKey)) {
                Debug.Log("Interacted with subterminal");
                //remove object from array
                if (tempList.Contains(thisObjectFaced.GetComponent<SubTerminal>())) {
                    //Play subterminal activation sound
                    activateSound.Play();
                    //Change the materials array associated with the subterminal
                    //by making a whole new array, beacuse meshrenderer.materials returns
                    //a copy of the materials array rather than the materials array itself (so because Unity is dumb)
                    SubTerminal subTerminal = thisObjectFaced.GetComponent<SubTerminal>();
                    Transform monitor = subTerminal.transform.Find("Monitor");
                    Material[] originalMats = monitor.GetComponent<MeshRenderer>().materials;
                    Material[] newMats = new Material[originalMats.Length];

                    for (int i = 0; i < originalMats.Length; i++)
                    {
                        if (i != 1)
                        {
                            newMats[i] = originalMats[i];
                        }
                        else
                        {
                            newMats[i] = subTerminal.usedColor;
                        }
                    }

                    //Replace old mats with new ones
                    monitor.GetComponent<MeshRenderer>().materials = newMats;

                    //Disable text on subterminal after activation
                    thisObjectFaced.transform.Find("Subterminal Text").gameObject.SetActive(false);
                    thisObjectFaced.transform.Find("Subterminal #").gameObject.SetActive(false);

                    //Remove subterminal from out subterminals list
                    Debug.Log("Exists");
                    tempList.Remove(thisObjectFaced.GetComponent<SubTerminal>());
                }
            }
        }
        // check if list empty
        if(tempList.Count == 0) {
            Debug.Log("No sub terminals");
            allActivated = true;
            //Change the materials array associated with the exit terminal
            //by making a whole new array, beacuse meshrenderer.materials returns
            //a copy of the materials array rather than the materials array itself (so because Unity is dumb)
            GameObject exitTerminal = GameObject.Find("ExitTerminal");
            Transform monitor = exitTerminal.transform.Find("Monitor");
            Material[] originalMats = monitor.GetComponent<MeshRenderer>().materials;
            Material[] newMats = new Material[originalMats.Length];

            for (int i = 0; i < originalMats.Length; i++)
            {
                if (i != 1)
                {
                    newMats[i] = originalMats[i];
                }
                else
                {
                    newMats[i] = exitTerminal.GetComponent<ExitTerminal>().terminalActiveColor;
                }
            }

            //Disable exit terminal text after all subterminals have been activated
            exitTerminal.transform.Find("Subterminal Text").gameObject.SetActive(false);
            exitTerminal.transform.Find("Subterminal #").gameObject.SetActive(false);

            //Replace old mats with new ones
            monitor.GetComponent<MeshRenderer>().materials = newMats;
        }
    }
}
