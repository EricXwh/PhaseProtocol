using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChasingAfterSeeingPlayer : MonoBehaviour
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

    //Method for after the agent sees the player
    private static void AlterState(NavMeshAgent agent, Transform target, bool canSeePlayer, bool isFlashbanged, AI aiAgent)
    {
        Debug.Log("Is Player Being Seen? -> " + canSeePlayer);
        DestroyWallsInRadius(agent);
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
        //If player breaks line of sight, enter another state
        //before wandering
        //Else, continue following player without entering that state yet
        if (!canSeePlayer)
        {
            Debug.Log("Line of sight broken. Continue chasing after LOS broken");
            //Destination will be set according to this state's rules
            aiAgent.aiState = AI.AiState.ChasingAfterPlayerBreaksLOS;
        }
        else
        {
            //Keep following the player (the update in destination is not entirely necessary though
            agent.destination = target.position;
        }
    }

    public static void GetTarget(NavMeshAgent agent, Transform target, bool canSeePlayer, bool isFlashbanged, AI aiAgent)
    {
        //Always set the detection level to the max in this state
        GameManager.SetDetectionLLevelToThreshold();
        AlterState(agent, target, canSeePlayer, isFlashbanged, aiAgent);
    }
}
