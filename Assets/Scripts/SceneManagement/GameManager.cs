using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * 
 * A set of factory/static methods used for keeping track
 * of values stored in multiple classes
 * 
 */
public class GameManager : MonoBehaviour
{
    //Class variables used for game management
    private static float detectionLevel;
    private static float detectionLevelThreshold;
    private static bool isFlashbanged;
    public static bool flashbangOnCD;
    public static int angerLevel;
    public static int[] subterminalPositionsZone1;
    public static int[] subterminalPositionsZone2;
    public static int[] subterminalPositionsZone3;
    public static string robotInhabited;
    public static float volume;
    public static float sensitivity;
    //Class variables for cooldown management
    public static bool swapBackOnCD;
    public static bool bodySwapOnCD;
    //Class var indicating if the dash is on cd
    public static bool scamperDashOnCD;
    //Health is shared between robots and the player, hence one value for it exists
    public static int playerHP = 300;

    //Class vars for swapping
    public static float timeToLook;
    public static float timeSwapCooldown;

    // These values are set at the start of the game, do not change
    // except for in certain scripts
    void Start()
    {
        detectionLevel = 0.0f;
        detectionLevelThreshold = 20.0f;
        volume = .5f;
        //sensitivity = 1f;
        isFlashbanged = false;
        flashbangOnCD = false;
        //Zone 1 subterminal positions
        subterminalPositionsZone1 = new int[2];
        subterminalPositionsZone1[0] = Random.Range(0, 2);
        subterminalPositionsZone1[1] = Random.Range(2, 4);
        //Zone 2 subterminal positions
        subterminalPositionsZone2 = new int[2];
        subterminalPositionsZone2[0] = Random.Range(0, 2);
        subterminalPositionsZone2[1] = Random.Range(2, 4);
        //Zone 3 subterminal positions
        subterminalPositionsZone3 = new int[4];
        subterminalPositionsZone3[0] = Random.Range(0, 2);
        subterminalPositionsZone3[1] = Random.Range(2, 4);
        subterminalPositionsZone3[2] = Random.Range(4, 6);
        subterminalPositionsZone3[3] = Random.Range(6, 8);
        Debug.Log("Front subterminal zone 1 position -> " + subterminalPositionsZone1[0]);
        Debug.Log("Back subterminal zone 1 position -> " + subterminalPositionsZone1[1]);
        Debug.Log("Platform Subterminal Location (Zone 2) -> " + subterminalPositionsZone2[0]);
        Debug.Log("Ground Subterminal Location (Zone 2) -> " + subterminalPositionsZone2[1]);
        robotInhabited = "Player";
        swapBackOnCD = false;
        bodySwapOnCD = false;
        scamperDashOnCD = false;
        //MAking the two values below slightly less than their actual so that the bar does not fill too early
        timeToLook = 0.98f;
        timeSwapCooldown = 4.98f;
        playerHP = 300;
        sensitivity = 1.0f;
    }

    //Increments detection level
    public static void IncreaseDetectionLevel()
    {
        detectionLevel = detectionLevel + Time.deltaTime;
    }

    private void Update()
    {
        Debug.Log("Volume: " + volume);
        AudioListener.volume = volume;
        Debug.Log("Detection Level -> " + detectionLevel);
    }

    //Decrements detection level
    public static void ResetDetectionLevel()
    {
        detectionLevel = 0.0f;
        Debug.Log("New detection level: " + detectionLevel);
    }

    public static float GetDetectionLevel()
    {
        return detectionLevel;
    }

    public static void ResetHPToMax()
    {
        playerHP = 300;
    }

    public static void SetDetectionLLevelToThreshold()
    {
        detectionLevel = detectionLevelThreshold;
    }

    public static float GetAngerLevel()
    {
        return angerLevel;
    }

    //Return Max Detection Level
    public static float GetDetectionLevelThreshold()
    {
        return detectionLevelThreshold;
    }

    public static bool DetectionLevelAboveThreshold()
    {
        return detectionLevel >= detectionLevelThreshold;
    }

    public static bool IsFlashBanged()
    {
        return isFlashbanged;
    }

    public static void SetIsFlashbanged(bool flashbanged)
    {
        isFlashbanged = flashbanged;
    }

    public static IEnumerator MakePlayerInvincibleForSecs(float invincibilityTime)
    {
        //Find current player (by default, just make this the original player to avoid NPE's)
        BasePlayer robotInhabited = GameObject.Find("Player").GetComponent<BasePlayer>();
        foreach (BasePlayer basePlayer in FindObjectsOfType<BasePlayer>())
        {
            if (basePlayer.enabled)
            {
                Debug.Log("Player found -> " + basePlayer);
                robotInhabited = basePlayer;
            }
        }
        robotInhabited.GetComponent<PlayerHealth>().setIsInvincible(true);
        //Disable all the player's colliders for a time exceot the character controller
        //This effectively makes them invisible and they can't take damage because the character
        //controller isn't considered a collider
        foreach (Collider c in robotInhabited.GetComponents<Collider>())
        {
            //The exception is the character controller
            //which is not susceptible to raycasts and needed to move the player
            if (c.GetType() != typeof(CharacterController))
            {
                Debug.Log("Collider type: " + c);
                c.enabled = false;
            }
        }
        Debug.Log("Invincibility started");
        yield return new WaitForSeconds(invincibilityTime);
        //Reenable the colliders after invincibility ends
        foreach (Collider c in robotInhabited.GetComponents<Collider>())
        {
            c.enabled = true;
        }
        robotInhabited.GetComponent<PlayerHealth>().setIsInvincible(false);
        Debug.Log("Invincibility ended");
    }

    /*
    //When Zone1 is loaded, this is relevant
    public void Update()
    {
        Debug.Log("Zone 1 Initialized? -> " + zone1Initialized);
        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Zone1")) && !zone1Initialized)
        {
            //Generate the subterminal in the specificed position
            Instantiate(exitTerminal, terminalPosition, Quaternion.identity);
            Debug.Log("Scene 1 being loaded");
            //Zone 1 subterminals should never be initialized again
            zone1Initialized = true;
        }
    }
    */
}
