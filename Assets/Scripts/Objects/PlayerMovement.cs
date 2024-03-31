using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Regular movement */
public class PlayerMovement : MonoBehaviour
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

    private void Start()
    {
        originalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        //If we are scamper, check for sprinting
        if (gameObject.CompareTag("Scamper"))
        {
            Debug.Log("Inside scamper");
            //Sprint check
            if (Input.GetKey(KeyCode.R))
            {
                Debug.Log("R pressed, speed should increase");
                speed = originalSpeed * speedMultiplier;
            }
            else
            {
                Debug.Log("Speed should not increase");
                speed = originalSpeed;
            }
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
                velocity.y = Mathf.Sqrt(jump * -15f * gravity);
            }
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        if (direction.magnitude >= 0.1f) {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }

        velocity.y += gravity * 8 * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
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
