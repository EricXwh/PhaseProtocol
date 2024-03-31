using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIWandering : MonoBehaviour
{
    private static bool debug = false;
    //Gets the next point for the AI to go to
    public static void GotoNextPoint(NavMeshAgent agent, Vector3[] points)
    {
        if (debug)
            Debug.Log("Entered");
        //Do nothing if there are no points to traverse between
        if (points.Length == 0)
        {
            return;
        }

        //Vector3 currDestination = agent.destination;
        int newDestinationPoint = Random.Range(0, points.Length);

        //If the new destination is the same as the current one
        //then calculate a new destination until it is different from the
        //current one
        while (newDestinationPoint == agent.GetComponent<AI>().nextPatrolPointDestination)
        {
            newDestinationPoint = Random.Range(0, points.Length);
            if (agent.GetComponent<BaseEnemy>() == null)
            {
                Debug.Log("New destination -> " + newDestinationPoint);
                Debug.Log("Old destination -> " + agent.GetComponent<AI>().nextPatrolPointDestination);
            }
        }

        //Set the destination
        agent.destination = points[newDestinationPoint];
        agent.GetComponent<AI>().nextPatrolPointDestination = newDestinationPoint;
        Debug.Log("Exits");
    }

    private static bool IsHunter(NavMeshAgent agent)
    {
        return agent.GetComponent<BaseEnemy>() != null;
    }

    //Cases when the state should change (for the hunter)
    static void AlterState(NavMeshAgent agent, bool canSeePlayer, bool isFlashbanged, AI aiAgent)
    {
        //If flashbanged, enter flashbanged state
        if (isFlashbanged && IsHunter(agent))
        {
            aiAgent.aiState = AI.AiState.Flashbanged;
        }
        //Agent is stopped (seperate from flashbang and cooldown logic) if a body swap or swap back occured
        if ((GameManager.bodySwapOnCD || GameManager.swapBackOnCD) && IsHunter(agent))
        {
            aiAgent.aiState = AI.AiState.Stopped;
        }
        //If the player can be seen...
        //May be a good idea to put a small timer on this
        if (IsHunter(agent) && canSeePlayer)
        {
            GameManager.SetDetectionLLevelToThreshold();
            aiAgent.aiState = AI.AiState.ChasingAfterSeeingPlayer;
            aiAgent.detection.Play();
        }
        //If detection level has reached appropriate level and player has not already been seen...
        if (IsHunter(agent) && GameManager.DetectionLevelAboveThreshold() && !canSeePlayer)
        {
            GameManager.SetDetectionLLevelToThreshold();
            aiAgent.aiState = AI.AiState.ChasingBeforeSeeingPlayer;
            aiAgent.detection.Play();
        }
    }

    public static void GetTarget(NavMeshAgent agent, Vector3[] points, bool canSeePlayer, bool isFlashbanged, AI aiAgent)
    {
        //Check if the state should be altered
        AlterState(agent, canSeePlayer, isFlashbanged, aiAgent);
        //Goes to the next point under the following conditions
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextPoint(agent, points);
        }
    }
}
