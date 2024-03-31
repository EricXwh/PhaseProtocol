using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/** Only entered from AIChasingAfterSeeingPlayer */
public class AIChasingAfterPlayerBreaksLOS : MonoBehaviour
{
    private static float elapsedTime = 0.0f;
    //Modify from here
    private static float destroyRadius = 10.0f;
    //Destroys objects within a certain radius of the robot
    private static void DestroyWallsInRadius(NavMeshAgent agent)
    {
        Collider[] withinDestroyRadius = Physics.OverlapSphere(agent.transform.position, destroyRadius);
        foreach (Collider collider in withinDestroyRadius)
        {
            Debug.Log("Colliders exist");
            //The hunter must be able to break the wall (an enum is used for setting this)
            if (collider.GetComponent<DestructibleWall>() != null &&
                (collider.GetComponent<DestructibleWall>().robotsWhichCanDestroyWallOption == DestructibleWall.RobotsWhichCanDestroyWall.Hunter ||
                 collider.GetComponent<DestructibleWall>().robotsWhichCanDestroyWallOption == DestructibleWall.RobotsWhichCanDestroyWall.HunterAndBrutus))
            {
                collider.GetComponent<DestructibleWall>().BreakWall();
                //Destroy(collider.gameObject);
            }
        }
    }

    //Alter state according to current features of the environment
    private static void EnterWanderingState(NavMeshAgent agent, AI aiAgent)
    {
        aiAgent.StartCoroutine(GameManager.MakePlayerInvincibleForSecs(2.0f));
        aiAgent.evasion.Play();
        Debug.Log("Cannot still see player, entering Wandering");
        //Increment anger level after chasing sequence ends
        GameManager.angerLevel++;
        //Reset detection level to 0
        GameManager.ResetDetectionLevel();
        Debug.Log("Curr anger level = " + GameManager.angerLevel);
        aiAgent.aiState = AI.AiState.Wandering;
        //Make sure to set a new destination for the agent
        AIWandering.GotoNextPoint(agent, aiAgent.patrolPoints);
    }

    public static void GetTarget(NavMeshAgent agent, Transform target, bool canSeePlayer, float timeToKeepChasingFor, bool isFlashbanged, AI aiAgent)
    {
        //Destroy any walls in range
        DestroyWallsInRadius(agent);
        //Always set the detection level to the max in this state
        GameManager.SetDetectionLLevelToThreshold();
        //If flashbanged, enter flashbanged state
        if (isFlashbanged)
        {
            aiAgent.aiState = AI.AiState.Flashbanged;
        }
        //Agent is stopped (seperate from flashbang and cooldown logic) if a body swap or swap back occured
        if ((GameManager.bodySwapOnCD || GameManager.swapBackOnCD))
        {
            aiAgent.aiState = AI.AiState.Stopped;
        }
        Debug.Log("Time to keep chasing for -> " + timeToKeepChasingFor);

        //If player is seen by the agent at any point in this state,
        //reenter ChasingAfterSeeingPlayer
        if (canSeePlayer)
        {
            elapsedTime = 0.0f;
            aiAgent.aiState = AI.AiState.ChasingAfterSeeingPlayer;
        }
        
        //If the time to keep chasing for has not passed, keep chasing the player
        //after they have broken line of sight
        if (elapsedTime <= timeToKeepChasingFor)
        {
            Debug.Log("Time elapsed after LOS broken -> " + elapsedTime);
            agent.destination = target.transform.position;
            elapsedTime += Time.deltaTime;
        }
        //Otherwise, alter the state accordingly
        else
        {
            Debug.Log("Stops chasing after -> " + elapsedTime);
            //Set elapsed time to 0, alter state
            elapsedTime = 0.0f;
            EnterWanderingState(agent, aiAgent);
        }
    }
}
