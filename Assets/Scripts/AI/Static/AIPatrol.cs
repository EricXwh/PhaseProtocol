using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPatrol : MonoBehaviour
{
    //Gets the next point for the AI to go to (predetermined
    static void GotoNextPoint(NavMeshAgent agent, Vector3[] points)
    {
        //Do nothing if there are no points to traverse between
        if (points.Length == 0)
        {
            return;
        }

        //Set the destination (predetermined)
        agent.destination = points[agent.GetComponent<AI>().nextPatrolPointDestination];
        //Increment the desitantion for the next go around
        agent.GetComponent<AI>().nextPatrolPointDestination = (agent.GetComponent<AI>().nextPatrolPointDestination + 1) % points.Length;
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
    public static void GetTarget (NavMeshAgent agent, Vector3[] points, bool canSeePlayer, bool isFlashbanged, AI aiAgent)
    {
        AlterState(agent, canSeePlayer, isFlashbanged, aiAgent);
        //Goes to the next point under the following conditions
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Debug.Log(agent.destination);
            GotoNextPoint(agent, points);
        }
    }
}
