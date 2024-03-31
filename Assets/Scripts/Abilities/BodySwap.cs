using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * TODO: Add in a cooldown when swapping back
 * This script implements the BodySwap feature
 * Should ONLY be attached to Player instances
 * If attached to anything else, the script will break
 * Also some stuff, like child names, is hard coded here
 * I might update that later on if need be
 * 
 */

public class BodySwap : MonoBehaviour
{
    //Player reference
    [SerializeField] BasePlayer player;
    //Debug log param
    [SerializeField]
    private bool debug = false;
    //Fields that can be changed
    public float distanceFromPlayer = 30f;
    public float amountOfSecondsToLookAtFor = GameManager.timeToLook;
    public float cooldown = GameManager.timeSwapCooldown;
    //Private fields for bookkeeping
    public float _timeLookingAt = 0.0f;
    private GameObject _prevObjectHit;
    private float elapsedTime = 0.0f;
    [SerializeField] private Animator vhsEffect;
    public bool lookingRobot;
    [SerializeField]
    private AudioSource bodySwapSound;

    // Start is called before the first frame update
    void Start()
    {
        vhsEffect = GameObject.Find("VHS").GetComponent<Animator>();
    }

    //Used for debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Camera cam = GameObject.Find("Camera").GetComponent<Camera>();
        Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);
        Gizmos.DrawRay(forwardRay.origin, forwardRay.direction * distanceFromPlayer);
    }

    /* Maybe something to add in the future: Cone of los for body-swapping */

    //Gets the game object currently being targeted by a raycast
    //May return null if no game object is being targeted
    private GameObject GetGameObjectBeingLookedAt()
    {
        //Raycast out from the camera
        Camera cam = GameObject.Find("Camera").GetComponent<Camera>();
        Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);

        RaycastHit hit;

        //If something is in the player's path within a set amount of unity units...
        if (Physics.Raycast(forwardRay, out hit, distanceFromPlayer))
        {
            //Object hit
            GameObject objectHit = hit.transform.gameObject;
            if(debug)
                Debug.Log("Object Hit -> " + objectHit);
            //Return the object hit
            return objectHit;
        }
        //Return a new game object if nothing is hit
        //Avoids a null reference exception and
        //also hides the game object from the scene
        else
        {
            GameObject temp = new GameObject();
            temp.hideFlags = HideFlags.HideAndDontSave;
            return temp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Update the robot the playe is currently in
        //Debug.Log("Current Detection Level -> " + GameManager.GetDetectionLevel());
        //Every frame the player hasn't body swaped, increase detection level
        GameManager.IncreaseDetectionLevel();


        lookingRobot = false; //For Ui


        //Don't really like this method, but it should work for now
        //Will likely be altered later on
        if (GameManager.bodySwapOnCD)
        {
            BodySwapCooldown(cooldown);
        }
        //A body swap cannot be performed if
        //the cooldown is still in effect
        else
        {
            //Object in the player's current path
            GameObject currObjectHit = GetGameObjectBeingLookedAt();
            //If it is another robot that is not the hunter, then swap bodies
            if (currObjectHit.GetComponent<BaseRobot>() != null && currObjectHit.GetComponent<BaseEnemy>() == null)
            {
                //Increment time looking at if prevObject is
                //a robot and the same as the current robot
                if (_prevObjectHit.Equals(currObjectHit))
                {
                    lookingRobot = true;
                    if(debug)
                        Debug.Log("Time looking at -> " + _timeLookingAt);
                    _timeLookingAt += Time.deltaTime;
                }
                //Otherwise, set _timeLookingAt to 0
                else
                {
                    lookingRobot = false;
                    _timeLookingAt = 0.0f;
                }
                bool canBodySwap = _timeLookingAt >= amountOfSecondsToLookAtFor;
                //If the player presses q and they have
                //been looking at the robot long enough,
                //then perform the body swap
                if (canBodySwap && Input.GetKeyDown(KeyCode.Q))
                {
                    lookingRobot = false;
                    DoBodySwap(currObjectHit);
                }
            }

            //Store the previous object hit
            _prevObjectHit = currObjectHit;
        }

        //Update robot the player is in
        GameManager.robotInhabited = this.tag;
        if(debug)
            Debug.Log("Robot inhabited: " + GameManager.robotInhabited);
    }

    void DisablePlayerScripts()
    {
        player.GetComponent<BasePlayer>().enabled = false;
        player.GetComponent<PlayerAndCameraMovement>().enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<BodySwap>().enabled = false;
        player.GetComponent<ActivateST>().enabled = false;
        player.GetComponent<ActivateET>().enabled = false;
    }

    //Disables robot body parts (one transfering into)
    void DisableRobotPlayerBodyParts(GameObject robot)
    {
        Transform bodyParts = robot.transform.Find("BodyParts");
        foreach (Transform bodyPart in bodyParts)
        {
            bodyPart.gameObject.SetActive(false);
        }
    }

    //Enables player's body parts (shows their current state)
    void EnablePlayerBodyParts()
    {
        Transform bodyParts = this.transform.Find("BodyParts");
        foreach (Transform bodyPart in bodyParts)
        {
            bodyPart.gameObject.SetActive(true);
        }
    }

    void EnableRobotPlayerScripts(GameObject robot)
    {
        //This effectively makes that robot the player
        //Enables all player related scripts on the robot
        robot.GetComponent<BasePlayer>().enabled = true;
        robot.GetComponent<PlayerAndCameraMovement>().enabled = true;
        robot.GetComponent<CharacterController>().enabled = true;
        robot.GetComponent<SwapBack>().enabled = true;
        if (robot.tag == "Brutus")
        {
            robot.GetComponent<Destroy>().enabled = true;
            robot.GetComponent<Flashbang>().enabled = true;
        }
        if (robot.tag == "Tubo")
        {
            robot.GetComponent<Flashbang>().enabled = true;
        }
    }

    void DecreaseFootstepSound(GameObject robot)
    {
        robot.transform.Find("Footstep").GetComponent<AudioSource>().volume = 0.25f;
    }

    void DisableRobotPlayerAI(GameObject robot)
    {
        robot.GetComponent<AI>().enabled = false;
        robot.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
    }

    void DisablePlayerCam()
    {
        //Deactivate SAR, play an animation

        //Anim to be added by the appropriate team member
        Debug.Log(player);
        //Disable the player's camera and orientation of that camera
        Transform playerCam = gameObject.transform.Find("PlayerCam");
        //This is done by disabling all children of the player cam, but not the player cam itself
        //so we can find the player cam later on
        foreach (Transform child in playerCam)
        {
            child.gameObject.SetActive(false);
        }
    }

    void EnablePlayerRobotCam(GameObject robot)
    {
        vhsEffect.SetTrigger("BodySwap");
        //Enable the robot to transfer into's features
        Transform robotCam = robot.GetComponent<Transform>().Find("RobotCam");
        //Enable all children of the robot camera game object
        //This is done so we can always find the robot camera children
        foreach (Transform child in robotCam)
        {
            child.gameObject.SetActive(true);
        }

        //Move the camera to the robot
        GameObject.Find("Camera").transform.position = Vector3.Lerp(
            GameObject.Find("Camera").transform.position, 
            robot.GetComponent<PlayerAndCameraMovement>().virtualCam.transform.position, 
            Time.deltaTime
        );
    }

    private void DoBodySwap(GameObject robot)
    {
        //Start CD after the swap
        GameManager.swapBackOnCD = true;
        //Reset detection level on body swap
        GameManager.ResetDetectionLevel();
        //Sequence of events required for a body swap
        DisablePlayerCam();
        DisablePlayerScripts();
        EnablePlayerRobotCam(robot);
        EnablePlayerBodyParts();
        EnableRobotPlayerScripts(robot);
        DisableRobotPlayerAI(robot);
        DisableRobotPlayerBodyParts(robot);
        DecreaseFootstepSound(robot);
        bodySwapSound.Play();
    }

    private void BodySwapCooldown(float cooldown)
    {
        Debug.Log("Elapsed time = " + elapsedTime);
        if (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            GameManager.bodySwapOnCD = false;
            elapsedTime = 0.0f;
            return;
        }
    }
}
