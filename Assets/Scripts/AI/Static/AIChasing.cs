using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Being kept just in case, but for now this is unused
public class AIChasing : MonoBehaviour
{
    /*
    //Modify from here
    private static float destroyRadius = 5.0f;

    //State where AI follows the player (without having seen them beforehand)
    private static void FollowingBeforeSeeingPlayer(NavMeshAgent agent, Transform target, bool playerBeingLookedAt)
    {
        //Destroy objects within radius
        DestroyWallsInRadius(agent);
        //Set player as destination
        agent.destination = target.position;
        //Player has been seen, transition to state where
        //AI follows player but has seen them at least once
        if (playerBeingLookedAt)
        {
            Debug.Log("Player Seen");
            aiChasingSubstate = AIChasingSubstates.FollowingAfterSeeingPlayer;
        }
    }

    //State where AI follows the player (after having seen them once)
    private static void FollowingAfterSeeingPlayer(NavMeshAgent agent, Transform target, bool playerBeingLookedAt, AI aiInstance)
    {
        DestroyWallsInRadius(agent);
        //If player breaks line of sight, stop following player
        //Else, continue following player
        if (!playerBeingLookedAt)
        {
            Debug.Log("Line of sight broken. Going back to wandering");
            //Destination will be set according to this state's rules
            aiInstance.aiState = AI.AiState.Wandering;
        }
        else
        {
            //Keep following the player (the update in destination is not entirely necessary though
            agent.destination = target.position;
        }
        //Need to set anger bar to zero here, whenever that is implemented
    }

    //Destroys objects within a certain radius of the robot
    private static void DestroyWallsInRadius(NavMeshAgent agent)
    {
        Collider[] withinDestroyRadius = Physics.OverlapSphere(agent.transform.position, destroyRadius);
        foreach (Collider collider in withinDestroyRadius)
        {
            Debug.Log("Colliders exist");
            if (collider.GetComponent<DestructibleWall>() != null)
            {
                Destroy(collider.gameObject);
            }
        }
    }
    //Gets the target for the AI
    public static void GetTarget(NavMeshAgent agent, Transform target, bool playerBeingLookedAt, AI aiInstance)
    {
        //Do something according to the current state
        switch (aiChasingSubstate)
        {
            case AIChasingSubstates.FollowingBeforeSeeingPlayer:
                FollowingBeforeSeeingPlayer(agent, target, playerBeingLookedAt);
                break;
            case AIChasingSubstates.FollowingAfterSeeingPlayer:
                FollowingAfterSeeingPlayer(agent, target, playerBeingLookedAt, aiInstance);
                break;
            case AIChasingSubstates.BreakWall:
                BreakWall(agent, target);
                break;
            case AIChasingSubstates.Jump:
                Jump(agent, target);
                break;
            case AIChasingSubstates.Sprint:
                Sprint(agent, target);
                break;
            default:
                Debug.Log("Error. There is no current AI Chasing State");
                break;
        }
    }
    */


    /*
    //State where AI breaks wall in front of it
    private static void BreakWall(NavMeshAgent agent, Transform target)
    {
        //If there is a destructable wall in front of the agent, destroy it
        if (target.GetComponent<DestructibleWall>() != null)
        {
            Destroy(target);
        }
        //This is very much editable
    }

    //State where AI jumps over the obstacle in front of it
    private static void Jump(NavMeshAgent agent, Transform target)
    {
        //This may be handled by navmesh links, but I still think something
        //may be needed here anyways
        return;
    }

    //State where AI sprints across obstacle in front of it
    private static void Sprint(NavMeshAgent agent, Transform target)
    {
        //Need to wait for sprint mechanic to be implemented to implement this
        return;
    }
    */
}
