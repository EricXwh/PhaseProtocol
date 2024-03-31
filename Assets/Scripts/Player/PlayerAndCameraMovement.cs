using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/** Regular movement */
public class PlayerAndCameraMovement : MonoBehaviour
{
    public CharacterController controller;

    [SerializeField]
    private float speed = 6.0f;
    private float originalSpeed;
    [SerializeField]
    private int speedMultiplier = 10;
    [SerializeField]
    private float turnSmoothTime = 0.1f;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float groundDistance = 0.2f;
    [SerializeField]
    private float turnSmoothVelocity;
    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private float jump = 20f;
    [SerializeField]
    private AudioSource jumpSound;
    [SerializeField]
    private AudioSource dashSound;
    

    [SerializeField]
    private float sensitivity = 2f;
    public CinemachineVirtualCamera virtualCam;
    bool increased = false;
    public float scamperDashCooldown = 5.0f;
    private float elapsedTime = 0.0f;
    bool coolingdown = false;

    private void Start()
    {
        originalSpeed = speed;
        virtualCam.Priority = 1;
        //Cursor should be invisible
        Cursor.visible = false;
        //Cursor should also be locked
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        sensitivity = GameManager.sensitivity;

        //If we are scamper, check for sprinting
        if (gameObject.CompareTag("Scamper"))
        {
            Debug.Log("Inside scamper");
            //Sprint check
            Debug.Log("cooling: "+coolingdown);
            if(coolingdown){
                elapsedTime += Time.deltaTime;
                GameManager.scamperDashOnCD = true;
                if (elapsedTime >= scamperDashCooldown) {
                    elapsedTime = 0.0f;
                    coolingdown = false;
                    GameManager.scamperDashOnCD = false;
                }
            }
            else
            {
                speedup();
            }
            // if (Input.GetKey(KeyCode.R))
            // {
            //     Debug.Log("R pressed, speed should increase");
            //     speed = originalSpeed * speedMultiplier;
            // }
            // else
            // {
            //     Debug.Log("Speed should not increase");
            //     speed = originalSpeed;
            // }
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //If we are leaper, check for jumping
        if (this.gameObject.CompareTag("Leaper"))
        {
            Debug.Log("Inside leaper");
            if (Input.GetKeyDown(KeyCode.R) && isGrounded)
            {
                jumpSound.Play();
                velocity.y = Mathf.Sqrt(jump * -15f * gravity);
            }
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }


        //Austin edites
        //This rotates camera and player model

        transform.rotation = Quaternion.Euler(new Vector3(0, cam.eulerAngles.y + Input.GetAxis("Mouse X") * sensitivity, 0));

        float xRotation = cam.eulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity;

        if (xRotation > 180)
        {
            xRotation -= 360;
        }

        xRotation = Mathf.Clamp(xRotation, -80f, 80);
        //This rotates our camera independant of the model
        cam.rotation = Quaternion.Euler(new Vector3(xRotation, transform.rotation.eulerAngles.y, 0));

        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");
            if (virtualCam.Priority == 1)
            {
                virtualCam2.Priority = 1;
                virtualCam.Priority = 0;
            }
            else
            {
                virtualCam2.Priority = 0;
                virtualCam.Priority = 1;
            }
        }
        */

        velocity.y += gravity * 8 * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }


    void speedup(){
        if(Input.GetKeyDown(KeyCode.R)) {
            if(!increased && !coolingdown){
                increased = true;
                coolingdown = true;
                Debug.Log("R pressed, speed should increase");
                speed = originalSpeed * speedMultiplier;
                StartCoroutine("speedIncrease");
            }
            // else{
            //     Debug.Log("Speed should not increase");
            //     speed = originalSpeed;
            // }
        }      
    }

    IEnumerator speedIncrease (){
        dashSound.Play();
        //print("speedup() running");          
        yield return new WaitForSeconds(1);
        speed = originalSpeed;
        increased = false;
        //print("moveSpeed reverted to original"); 
    }
    /*
    public float moveSpeed;

    public Transform orientation;

    float hoizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    CharacterController player;

    private void Update()
    {
        hoizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    //Moves the player
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * hoizontalInput;
    }
    */

}
