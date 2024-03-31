using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * Performs a swap back to SAR
 * Note that this should ONLY be attached to robots other than SAR and Hunter
 * Helper methods used for readability as easier editing
 * 
 */
public class SwapBack : MonoBehaviour
{
    //Player instance to swap back to
    [SerializeField] BasePlayer player;
    [SerializeField] float cooldown = 0.75f;
    private float elapsedTime = 0.0f;
    [SerializeField] private Animator vhsEffect;
    [SerializeField] private AudioSource swapBackSound;

    // Start is called before the first frame update
    void Start()
    {
        vhsEffect = GameObject.Find("VHS").GetComponent<Animator>();
    }

    //Enable player cam
    void EnablePlayerCam()
    {
        Transform playerCam = player.gameObject.transform.Find("PlayerCam");
        //Debug.Log(playerCam);
        //Renabling children must be done via foreach
        //Not true for disabling though
        foreach (Transform child in playerCam)
        {
            child.gameObject.SetActive(true);
        }
        //Move the camera back to the player
        GameObject.Find("Camera").transform.position = Vector3.Lerp(
            GameObject.Find("Camera").transform.position,
            player.GetComponent<PlayerAndCameraMovement>().virtualCam.transform.position,
            Time.deltaTime
        );
    }

    //Disable robot cam
    void DisableRobotCam()
    {
        //Disabling parent disables all children
        Transform robotCam = gameObject.transform.Find("RobotCam");
        //Disable only the children of the parent
        //That way we can always find the parent object
        foreach (Transform child in robotCam)
        {
            child.gameObject.SetActive(false);
        }
    }

    //Renable all player scripts
    void EnablePlayerScripts()
    {
        player.GetComponent<BasePlayer>().enabled = true;
        player.GetComponent<PlayerAndCameraMovement>().enabled = true;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<BodySwap>().enabled = true;
        player.GetComponent<ActivateST>().enabled = true;
        player.GetComponent<ActivateET>().enabled = true;
    }

    //Disable the player's body parts
    void DisablePlayerBodyParts()
    {
        Transform bodyParts = player.transform.Find("BodyParts");
        foreach (Transform bodyPart in bodyParts)
        {
            bodyPart.gameObject.SetActive(false);
        }
    }

    //Disable features of the player enabled for the robot
    void DisableRobotPlayerScripts()
    {
        this.GetComponent<BasePlayer>().enabled = false;
        this.GetComponent<PlayerAndCameraMovement>().enabled = false;
        this.GetComponent<CharacterController>().enabled = false;
        this.GetComponent<SwapBack>().enabled = false;
        if (tag == "Brutus")
        {
            GetComponent<Destroy>().enabled = false;
            GetComponent<Flashbang>().enabled = false;
        }
        if (tag == "Tubo")
        {
            GetComponent<Flashbang>().enabled = false;
        }
    }

    //Enable robot body parts
    void EnableRobotBodyParts()
    {
        Transform bodyParts = this.transform.Find("BodyParts");
        foreach (Transform bodyPart in bodyParts)
        {
            bodyPart.gameObject.SetActive(true);
        }
    }

    //Enable the AI on the robot
    void EnableRobotAI()
    {
        this.GetComponent<AI>().enabled = true;
        this.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
    }

    //Make footstep sound louder
    void IncreaseFootstepSound()
    {
        transform.Find("Footstep").GetComponent<AudioSource>().volume = 1.0f;
    }

    //Swaps player back to original body
    public void DoSwapBack()
    {
        //The object this is attached to should be a robot
        if (GetComponent<BaseRobot>())
        {
            //Reset detection level
            GameManager.ResetDetectionLevel();
            //Perform all necessary parts of the swap back in order
            DisableRobotCam();
            EnablePlayerCam();
            DisableRobotPlayerScripts();
            EnablePlayerScripts();
            EnableRobotAI();
            DisablePlayerBodyParts();
            EnableRobotBodyParts();
            IncreaseFootstepSound();
            vhsEffect.SetTrigger("BodySwap");
            swapBackSound.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Increase detection level for every frame you haven't body swapped
        GameManager.IncreaseDetectionLevel();
        if (GameManager.swapBackOnCD)
        {
            Debug.Log("SwapBack on CD");
            SwapBackCD(cooldown);
        }
        else
        {
            Debug.Log("SwapBack not on CD");
            //Debug.Log("Detection level while in another body -> " + GameManager.GetDetectionLevel());
            bool qPressed = Input.GetKeyDown(KeyCode.Q);
            //On Q press, swap back
            if (qPressed)
            {
                //Body swap on CD
                GameManager.bodySwapOnCD = true;
                //Start cooldown
                Debug.Log("Placeholder");
                DoSwapBack();
                Debug.Log("Cooldown initiated? -> " + GameManager.bodySwapOnCD);
            }
        }
        //Update robot the player is in
        GameManager.robotInhabited = this.tag;
        Debug.Log("Robot inhabited: " + GameManager.robotInhabited);
    }

    private void SwapBackCD(float cooldown)
    {
        Debug.Log("Elapsed time = " + elapsedTime);
        if (elapsedTime < cooldown)
        {
            elapsedTime += Time.deltaTime;
        }
        else
        {
            GameManager.swapBackOnCD = false;
            elapsedTime = 0.0f;
            return;
        }
    }
}
