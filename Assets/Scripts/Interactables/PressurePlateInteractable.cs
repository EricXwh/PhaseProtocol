using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateInteractable : Interactable
{
    //The pressure plate either destroys walls
    //or moves blocks down depending on the use case
    public enum PressurePlateFunction
    {
        DestroyObject,
        MoveBlock,
        StopConveyors
    }

    public PressurePlateFunction pressurePlateFunction = PressurePlateFunction.DestroyObject;

    private bool touched = false;

    [SerializeField] GameObject PressureIndicator; //Colored box of pressureplate

    //Object with a "Reaction" script To be called when plate is pushed (Required)
    [SerializeField] GameObject[] reactors;

    //object to be moved by pressure plate
    [SerializeField] GameObject movingBlock;

    //conveyor to be stopped, if this is a pressure plate used to stop a conveyor
    [SerializeField] Conveyor[] conveyorsToStop;

    //The material of the stopped conveyor
    [SerializeField] Material stoppedConveyorMat;

    [SerializeField] AudioSource pressedSound;

    [SerializeField] AudioSource movingBlockSound;

    public float blockMoveSpeed = 0.1f;

    private Vector3 endPoint;

    private void Start()
    {
        if (pressurePlateFunction == PressurePlateFunction.MoveBlock && movingBlock.transform.Find("MovePoint") == null)
        {
            Debug.LogError("You must have a 'MovePoint' object attached to the MovingBlock object, given this pressure plate moves a block at all");
        }
        if (pressurePlateFunction == PressurePlateFunction.StopConveyors && conveyorsToStop == null)
        {
            Debug.LogError("You must have a conveyor list attached to the PressurePlate instance to set the state to ConveyorToStop");
        }
        endPoint = movingBlock.transform.Find("MovePoint").transform.position;
    }

    void OnTriggerEnter(Collider collide)
    {
        Debug.Log("Touchy");
        if (collide.gameObject.GetComponent<BaseRobot>() != null || collide.gameObject.GetComponent<BasePlayer>() != null)
        {
            //Only destroy an object if this pressure plate should do so
            if (pressurePlateFunction == PressurePlateFunction.DestroyObject)
            {
                foreach (GameObject reactor in reactors)
                {
                    reactor.GetComponent<Reaction>().objectReaction();
                }
            }
            //Only stop the conveyor if this pressure plate should do so
            if (pressurePlateFunction == PressurePlateFunction.StopConveyors)
            {
                foreach (Conveyor conveyor in conveyorsToStop)
                {
                    conveyor.GetComponent<Renderer>().material = stoppedConveyorMat;
                    conveyor.speed = 0.0f;
                }
            }
            if (pressurePlateFunction == PressurePlateFunction.MoveBlock)
            {
                movingBlockSound.Play();
            }
            touched = true;
            PressureIndicator.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.cyan); //WILL HAVE TO SET AS _BaseColor id we go URP (DID)
            pressedSound.Play();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //None of this logic applies unless this pressure plate should move an object
        if (pressurePlateFunction == PressurePlateFunction.MoveBlock)
        {
            BasePlayer player = other.gameObject.GetComponent<BasePlayer>();
            if (player != null && movingBlock != null)
            {
                if (touched)
                {
                    //Move block down to the endpoint
                    moveBlockDown();
                }
            }
        }
    }

    void OnTriggerExit(Collider collide)
    {
        robotOffPlate();
    }

    //In case of robot leaving plate
    private void robotOffPlate() {
        if (touched == true) { //Only true in case of robot
            touched = false;
            foreach (GameObject reactor in reactors)
            {
                reactor.GetComponent<Reaction>().objectReactionLeave();
            }
            PressureIndicator.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red); //WILL HAVE TO SET AS _BaseColor id we go URP (DID)
        }
    }

    // move block down 
    private void moveBlockDown() {
        //Move the block towards the end point overtime
        //Factor in the block's move speed as well
        movingBlock.transform.position = Vector3.Lerp(
            movingBlock.transform.position,
            endPoint,
            Time.deltaTime * blockMoveSpeed
        );
    }

}
