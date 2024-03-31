using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStopped : MonoBehaviour
{
    //In this state, the AI will be disabled for a set
    //period of time
    private static float elapsedTime = 0.0f;
    //Is this the first run (for performance)?
    private static bool firstRun = true;

    //Change state accordingly after stun period
    //Agent can enter every other state EXCEPT
    //ChasingBeforeSeeingPLayer here
    private static void DetermineNextState(NavMeshAgent agent, Transform target, bool canSeePlayer, AI aiAgent)
    {
        //Start playing footsteps again
        aiAgent.GetComponent<AudioSource>().Play();
        firstRun = true;
        Debug.Log("Aspects of new state. Can see player? -> " + canSeePlayer);
        //If player is still within FOV, AI chases
        if (canSeePlayer)
        {
            Debug.Log("Stop ended, now chasing");
            GameManager.SetDetectionLLevelToThreshold();
            aiAgent.detection.Play();
            aiAgent.aiState = AI.AiState.ChasingAfterSeeingPlayer;
        }
        //If player is outside of FOV, default to wandering
        //If affected by a flashbang, I think that the state entered after
        //breaking line of sight in ChasingAfterSeeingPlayer should be
        //bypassed. This can be changed at a later time though
        else
        {
            Debug.Log("Stop ended, now wandering");
            aiAgent.aiState = AI.AiState.Wandering;
            //Make sure to set a new destination for the agent
            AIWandering.GotoNextPoint(agent, aiAgent.patrolPoints);
            GameManager.ResetDetectionLevel();
            aiAgent.evasion.Play();
        }
    }

    public static void GetTarget(NavMeshAgent agent, Transform target, float timeStopped, bool canSeePlayer, AI aiAgent)
    {
        //Reset the detection level each time, it will be set to full again
        //if a chasing state is entered
        GameManager.ResetDetectionLevel();
        //Change eye color on first run and disable colliders
        if (firstRun)
        {
            firstRun = false;
            aiAgent.ChangeEyeColorToFlashbangEyes(agent);
            aiAgent.disableColliders = true;
            //Do not play this after the player is hit
            //and reset the value if it is true on the first run
            if (!aiAgent.justHitPlayer)
            {
                aiAgent.flashBangHit.PlayDelayed(0.5f);
            }
            else
            {
                aiAgent.justHitPlayer = false;
            }
            //Stop any foot step sounds while active
            aiAgent.GetComponent<AudioSource>().Stop();
        }
        if (elapsedTime >= timeStopped)
        {
            //Let the agent move again
            agent.isStopped = false;
            //Elapsed time goes to 0
            elapsedTime = 0.0f;
            //Change eye color back to normal on this run
            aiAgent.ChangeEyeColorToRegularEyes(agent);
            //Exit this state
            DetermineNextState(agent, target, canSeePlayer, aiAgent);
        }
        //Increase time passed and make sure agent is stopped
        else
        {
            Debug.Log("Amount of seconds frozen so far -> " + elapsedTime);
            //Stop agent
            agent.isStopped = true;
            Debug.Log("Stun timer at -> " + elapsedTime);
            elapsedTime += Time.deltaTime;
        }
    }
}
