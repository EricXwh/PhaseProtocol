using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class AI : MonoBehaviour
{
    /*

    This class is responsible for:
        -Determining what 'State' the AI is in.
        -Selects the correct AI function based on the AI's state.

    This class offsets calculations to its attached NavMeshAgent.

    */

    public enum AiState
    {
        ChasingBeforeSeeingPlayer,
        ChasingAfterSeeingPlayer,
        ChasingAfterPlayerBreaksLOS,
        Patrol,
        Wandering,
        Flashbanged,
        Stopped
    }

    //Hunter only: Flashbang eye color
    [SerializeField]
    private Material flashbangEyes; //Resources.Load("HunterMats/Flashbang Eyes.mat", typeof(Material)) as Material;
    //Hunter only: Regular eye color
    [SerializeField]
    private Material regularEyes; //Resources.Load("HunterMats/Material.002.mat", typeof(Material)) as Material;
    //Hunter only: Spawn point to warp to on respawn
    [SerializeField]
    private Vector3 spawnPoint;
    [SerializeField]
    public float speed = 3.0f;
    [SerializeField]
    private float patrolSpeed = 5.0f;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float minimumDistance = 5.0f;
    [SerializeField]
    public Vector3[] patrolPoints;
    [SerializeField]
    private float speedMultiplier = 2.0f;
    //The reason I am making this static is because it needs
    //to be known and modified by other classes
    //Moreover, it would not make much sense to have other classes have
    //a reference to the AI script if they are not a part of the AI itself
    [SerializeField]
    public AiState aiState = AiState.Patrol;
    [SerializeField]
    private float fovDegrees = 135.0f;
    [SerializeField]
    private float viewDistance = 100.0f;
    private NavMeshAgent agent;
    [SerializeField]
    private float timeStunned = 5.0f;
    [SerializeField]
    private float timeToKeepChasingFor = 2.0f;
    [SerializeField]
    private float speedGoingOverConveyors = 50.0f;
    [SerializeField]
    public float speedIncreaseAfterSeeingPlayer = 3.0f;
    [SerializeField]
    private float initialAcceleration = 250.0f;
    [SerializeField]
    private float accelerationIncreaseFactor = 5.0f;
    [SerializeField]
    private float initialAngularSpeed = 120.0f;
    [SerializeField]
    private float angularSpeedIncreaseFactor = 120.0f;
    [SerializeField]
    public AudioSource evasion;
    [SerializeField]
    public AudioSource detection;
    [SerializeField]
    public AudioSource flashBangHit;
    public bool justHitPlayer;


    public int nextPatrolPointDestination = 0;

    private Vector3 previousPosition;
    private Vector3 currentPosition;

    [SerializeField]
    private float timeToDisableCollidersFor;
    public bool disableColliders;

    Animator animator;
    [SerializeField]
    private bool debug = false;
    // Start is called before the first frame update
    void Start()
    {
        //Setting some variables
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        //Needs to be 0 for patrolling to work
        agent.stoppingDistance = 0.0f;
        agent.acceleration = initialAcceleration;
        agent.angularSpeed = initialAngularSpeed;
        //Get the animator
        animator = gameObject.GetComponent<Animator>();
        //Spawn point is initial transform's position
        spawnPoint = transform.position;
        disableColliders = false;
        timeToDisableCollidersFor = timeStunned;
        justHitPlayer = false;
    }

    //Refactor if you wish, but I think raycasting is the easiest to
    //visualize at the moment and I personally don't think the performance
    //hit will be too great in our case
    //Gets the game object currently being targeted by a raycast
    //May return null if no game object is being targeted
    private bool CanSeePlayer()
    {
        //The direction of the ray (also I am aware the target is always the player)
        Vector3 rayDirection = target.position - transform.position;
        RaycastHit hit;

        //Detection within a certain radius
        if (Vector3.Angle(rayDirection, transform.forward) <= fovDegrees * 0.5f)
        {
            //Mask for all layers except ignore raycasts
            LayerMask ignoreRaycasts = LayerMask.GetMask("Default", "TransparentFX", "Water", "UI", "Player", "Ground", "Debris");
            //Determine if the player is within view
            //Ignore any items, such as ramps, that block most raycasts
            if (Physics.Raycast(transform.position, rayDirection, out hit, viewDistance, ignoreRaycasts))
            {
                Debug.Log("Where AI is looking at -> " + hit.transform.tag);
                //If yes, return true. Else, return false
                if (hit.transform.GetComponent<BasePlayer>() != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        //Debug.LogError("CanSeePlayer() at invalid state.");
        //Error out
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (tag == "Hunter")
        {
            Vector3 rayDirection = target.position - transform.position;
            rayDirection = new Vector3(rayDirection.x, rayDirection.y + 4.5f, rayDirection.z);
            Ray hunterView = new Ray(transform.position, rayDirection);
            Gizmos.DrawRay(hunterView.origin, hunterView.direction * 100.0f);
        }
    }

    private Transform FindPlayer()
    {
        //Find the ACTIVE player in the scene
        BasePlayer[] possiblePlayers = FindObjectsOfType<BasePlayer>();
        BasePlayer currPlayer = null;
        //The target will be the one of these items with
        //the base player script enabled
        foreach (BasePlayer player in possiblePlayers)
        {
            if (player.enabled)
            {
                Debug.Log("Player found -> " + player);
                currPlayer = player;
            }
        }
        return currPlayer.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(debug)
        {
            Debug.Log("Tag -> " + other.gameObject.tag);
        }
        //If in a conveyor, increase speed
        if (other.gameObject.tag == "Conveyor")
        {
            agent.speed = speedGoingOverConveyors;
        }
        //If object has an enabled base player script, this is the player
        //in which case, AI's state should be flashbanged momoentarily
        /*
        if (other.gameObject.GetComponent<BasePlayer>().enabled)
        {
            agent.Warp(spawnPoint);
            aiState = AiState.Flashbanged;
        }
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        //If object has an enabled base player script, this is the player
        //in which case, AI's state should be reset to patrolling so it goes away from the player
            //We can also warp if need be, and this only should occur with the hunter
        if (tag == "Hunter" && collision.gameObject.GetComponent<BasePlayer>() != null && collision.gameObject.GetComponent<BasePlayer>().enabled)
        {
            agent.Warp(spawnPoint);
            justHitPlayer = true;
            aiState = AiState.Stopped;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (debug)
            Debug.Log("Tag -> " + other.gameObject.tag);
        //If exiting a conveyor, set speed back to default
        if (other.gameObject.tag == "Conveyor")
        {
            agent.speed = speed;
        }
    }

    //Determine the robot's speed, acceleration, and angular speed
    private void DetermineAISpeedAccelAndAngSpeed()
    {
        //If in any sort of chasing state...
       
        if (aiState == AiState.ChasingBeforeSeeingPlayer ||
            aiState == AiState.ChasingAfterSeeingPlayer  ||
            aiState == AiState.ChasingAfterPlayerBreaksLOS)
        {
            animator.SetTrigger("StopStun");
            //Debug.Log("speedIncreaseAfterSeeingPlayer = " + speedIncreaseAfterSeeingPlayer);
            //Debug.Log("speed = " + speed);
            agent.speed = speedIncreaseAfterSeeingPlayer + (speed + (speedMultiplier * GameManager.angerLevel));
            agent.acceleration = agent.speed + accelerationIncreaseFactor;
            agent.angularSpeed = initialAngularSpeed + angularSpeedIncreaseFactor;
        }
        //If in any sort of patrolling state...
        //We want the agent to turn and stop fast, hence the high accel and angSpeed
        
        else if (aiState == AiState.Patrol ||
                 aiState == AiState.Wandering)
        {
            animator.SetTrigger("StopStun");
            agent.speed = patrolSpeed;
            agent.acceleration = initialAcceleration;
            agent.angularSpeed = 500.0f;
        }
        //If in a flashbanged or stopped state...
        else
        {
            //Acceleration is increased to slow down the agent immediately
            animator.SetTrigger("Stun");
            agent.speed = patrolSpeed;
            agent.acceleration = 500.0f;
            agent.angularSpeed = initialAngularSpeed;
        }
    }

    //Determine if ai should autobreak
    private void DetermineAIAutoBreaking()
    {
        //If in any sort of chasing state...
        if (aiState == AiState.ChasingBeforeSeeingPlayer ||
            aiState == AiState.ChasingAfterSeeingPlayer ||
            aiState == AiState.ChasingAfterPlayerBreaksLOS)
        {
            agent.autoBraking = false;
        }
        //Otherwise...
        else
        {
            agent.autoBraking = true;
        }
    }

    public void ChangeEyeColorToFlashbangEyes(NavMeshAgent agent)
    {
        //Change the materials array associated with the hunter's eyes
        //by making a whole new array, beacuse meshrenderer.materials returns
        //a copy of the materials array rather than the materials array itself (so because Unity is dumb)
        Transform eyes = agent.transform.Find("Eyes");
        Material[] originalMats = eyes.GetComponent<SkinnedMeshRenderer>().materials;
        Material[] newMats = new Material[originalMats.Length];

        for (int i = 0; i < originalMats.Length; i++)
        {
            if (i != 1)
            {
                newMats[i] = originalMats[i];
            }
            else
            {
                newMats[i] = flashbangEyes;
            }
        }

        //Replace old mats with new ones
        eyes.GetComponent<SkinnedMeshRenderer>().materials = newMats;
    }

    public void ChangeEyeColorToRegularEyes(NavMeshAgent agent)
    {
        //Change the materials array associated with the hunter' eyes
        //by making a whole new array, beacuse meshrenderer.materials returns
        //a copy of the materials array rather than the materials array itself (so because Unity is dumb)
        Transform eyes = agent.transform.Find("Eyes");
        Material[] originalMats = eyes.GetComponent<SkinnedMeshRenderer>().materials;
        Material[] newMats = new Material[originalMats.Length];

        for (int i = 0; i < originalMats.Length; i++)
        {
            if (i != 1)
            {
                newMats[i] = originalMats[i];
            }
            else
            {
                newMats[i] = regularEyes;
            }
        }

        //Replace old mats with new ones
        eyes.GetComponent<SkinnedMeshRenderer>().materials = newMats;
    }

    private void DisableAIColliders()
    {
        foreach (Collider collider in GetComponents<Collider>())
        {
            collider.enabled = false;
        }
    }

    private void EnableAIColliders()
    {
        foreach (Collider collider in GetComponents<Collider>())
        {
            collider.enabled = true;
        }
    }

    private IEnumerator DisableCollidersForSecs()
    {
        //We don't run this to run more than once
        disableColliders = false;
        DisableAIColliders();
        yield return new WaitForSeconds(timeToDisableCollidersFor);
        EnableAIColliders();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("speed" + agent.speed);
        Debug.Log("current pos " + currentPosition);
        Debug.Log("previous pos " + previousPosition);
        if (debug)
            Debug.Log(aiState);
        if (debug)
            Debug.Log(GameManager.angerLevel);
        //The target should be whatever gameobject has the player
        target = FindPlayer();
        // maintain speed at 0 if position of agent does not change
        currentPosition = agent.transform.position;
        if (previousPosition == currentPosition)
            animator.SetFloat("Speed", 0);
        //Modify AI speed and acceleration based on state
        else if (previousPosition != currentPosition) 
            animator.SetFloat("Speed", 1);
        DetermineAISpeedAccelAndAngSpeed();
        previousPosition = currentPosition;
        //Determine if AI should autobreak
        DetermineAIAutoBreaking();

        //Two different classes of AI:
        //1.) Hunter (needs FSM to function)
        //2.) Base robot (only wanders or patrols, does not change states)

        //Case 1: Hunter (needs FSM and state transitions)
        if (GetComponent<BaseEnemy>() != null)
        {
            //Disable colliders in certain circumstances
            if (disableColliders)
            {
                StartCoroutine(DisableCollidersForSecs());
            }
            switch (aiState)
            {
                case AiState.Wandering:
                    AIWandering.GetTarget(agent, patrolPoints, CanSeePlayer(), GameManager.IsFlashBanged(), this);
                    break;
                case AiState.Patrol:
                    AIPatrol.GetTarget(agent, patrolPoints, CanSeePlayer(), GameManager.IsFlashBanged(), this);
                    break;
                case AiState.ChasingBeforeSeeingPlayer:
                    AIChasingBeforeSeeingPlayer.GetTarget(agent, target, CanSeePlayer(), GameManager.IsFlashBanged(), this);
                    break;
                case AiState.ChasingAfterSeeingPlayer:
                    AIChasingAfterSeeingPlayer.GetTarget(agent, target, CanSeePlayer(), GameManager.IsFlashBanged(), this);
                    break;
                case AiState.ChasingAfterPlayerBreaksLOS:
                    AIChasingAfterPlayerBreaksLOS.GetTarget(agent, target, CanSeePlayer(), timeToKeepChasingFor, GameManager.IsFlashBanged(), this);
                    break;
                case AiState.Flashbanged:
                    AIFlashbanged.GetTarget(agent, target, timeStunned, CanSeePlayer(), this);
                    break;
                case AiState.Stopped:
                    AIStopped.GetTarget(agent, target, timeStunned, CanSeePlayer(), this);
                    break;
                default:
                    //Debug.LogError("Default state for Hunter should never be reached");
                    break;
            }
        }

        //Case 2: Base Robot (state will be preset, will not change for them)
        else
        {
            switch (aiState)
            {
                case AiState.Wandering:
                    AIWandering.GetTarget(agent, patrolPoints, CanSeePlayer(), GameManager.IsFlashBanged(), this);
                    break;
                case AiState.Patrol:
                    AIPatrol.GetTarget(agent, patrolPoints, CanSeePlayer(), GameManager.IsFlashBanged(), this);
                    break;
                default:
                    //Debug.LogError("Default state for Base Robot should never be reached");
                    break;
            }
        }
    }

}
