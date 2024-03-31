using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChasingBeforeSeeingPlayer : MonoBehaviour
{
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

    //State where AI follows the player (without having seen them beforehand)
    private static void AlterState(NavMeshAgent agent, Transform target, bool canSeePlayer, bool isFlashbanged, AI aiAgent)
    {
        //Destroy objects within radius
        DestroyWallsInRadius(agent);
        //If flashbanged, enter flashbanged state
        if (isFlashbanged)
        {
            aiAgent.aiState = AI.AiState.Flashbanged;
        }
        //Agent is stopped (seperate from flashbang and cooldown logic) if a body swap or swap back occured
        if (GameManager.bodySwapOnCD || GameManager.swapBackOnCD)
        {
            aiAgent.aiState = AI.AiState.Stopped;
        }
        //Set player as destination
        agent.destination = target.position;
        //Player has been seen, transition to state where
        //AI follows player but has seen them at least once
        if (canSeePlayer)
        {
            Debug.Log("Player Seen");
            aiAgent.aiState = AI.AiState.ChasingAfterSeeingPlayer;
        }
    }

    public static void GetTarget(NavMeshAgent agent, Transform target, bool canSeePlayer, bool isFlashbanged, AI aiAgent)
    {
        //Always set the detection level to the max in this state
        GameManager.SetDetectionLLevelToThreshold();
        AlterState(agent, target, canSeePlayer, isFlashbanged, aiAgent);
    }
}
