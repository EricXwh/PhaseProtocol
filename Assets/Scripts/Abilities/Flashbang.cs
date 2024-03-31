using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Only attached this to a Tubo robot instance */
public class Flashbang : MonoBehaviour
{
    [SerializeField]
    private float flashbangMaxDistance = 50.0f;
    [SerializeField]
    private float flashbangCD = 5.0f;
    [SerializeField]
    private float fovDegrees = 90.0f;
    [SerializeField]
    private BaseEnemy hunter;
    [SerializeField]
    private Object hunterrobot;
    private float elapsedTime = 0.0f;
   

    //Return if the player is seeing the hunter
    private bool CanSeeHunter()
    {
        //Raycast out from the camera
        Camera cam = GameObject.Find("Camera").GetComponent<Camera>();
        Ray forwardRay = new Ray(cam.transform.position, cam.transform.forward);

        RaycastHit hit;

        //If something is in the player's path within a set amount of unity units...
        if (Physics.Raycast(forwardRay, out hit, flashbangMaxDistance))
        {
            //Object hit
            GameObject objectHit = hit.transform.gameObject;
            Debug.Log("Object Hit -> " + objectHit);
            if (objectHit.GetComponent<BaseEnemy>() != null)
            {
                return true;
            }
        }
        return false;
        //This is a legacy version
        /*
        //Raycast out from the camera's center
        Camera cam = GameObject.Find("Camera").GetComponent<Camera>();

        //The direction of the ray (also I am aware the target is always the hunter)
        Vector3 rayDirection = hunter.transform.position - cam.transform.position;
        RaycastHit hit;

        Debug.Log(rayDirection);

        //Detection within a certain radius
        if (Vector3.Angle(rayDirection, cam.transform.forward) <= fovDegrees * 0.5f)
        {
            //Determine if the player is within view
            if (Physics.Raycast(cam.transform.position, rayDirection, out hit, flashbangMaxDistance))
            {
                Debug.Log("Flashbang object hit -> " + hit.transform.tag);
                Debug.DrawRay(cam.transform.position, cam.transform.forward * flashbangMaxDistance, Color.green);
                //If yes, return the AI component of the base enemy
                if (hit.transform.GetComponent<BaseEnemy>() != null)
                {
                    return true;
                }
                //Indicates hunter is not being seen
                else
                {
                    return false;
                }
            }
        }
        //Debug.LogError("CanSeePlayer() at invalid state.");
        //Error out
        return false;
        */
    }

    //Check if a flashbang succeeds
    private bool FlashbangSucceeds()
    {
        //Flashbang succeeds when...
            //1.) Player presses R
            //2.) Player can see hunter
            //3.) The hunter is not already flashbanged
            //4.) The flashbang is not on cooldown
        bool pressedR = Input.GetKeyDown(KeyCode.R);
        return pressedR && CanSeeHunter() && !GameManager.IsFlashBanged() && !GameManager.flashbangOnCD;
    }

    //Simplifies another boolean test
    private bool FlashbangShouldGoOffCD()
    {
        return elapsedTime >= flashbangCD && GameManager.flashbangOnCD;
    }

    private void Update()
    {
        //If the flashbang is on CD (see AIFlashbang for the trigger)
        //then do nothing
        if (GameManager.flashbangOnCD)
        {
            Debug.Log("Cooldown engaged, time passed -> " + elapsedTime);
            elapsedTime += Time.deltaTime;
        }
        //If elapsed time exceeds flashbang CD
        //
        if (FlashbangShouldGoOffCD())
        {
            Debug.Log("Cooldown ended");
            elapsedTime = 0.0f;
            GameManager.flashbangOnCD = false;
        }
        //If flashbang succeeds and flashbang is not on CD, engage hunter flashbanged state
        if (FlashbangSucceeds())
        {
            //Trigger hunter's flashbang state indirectly
            GameManager.SetIsFlashbanged(true);

        }
    }
}
