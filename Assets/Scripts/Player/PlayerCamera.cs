using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float xSensitivity = 400.0f;
    [SerializeField]
    private float ySensitivity = 400.0f;

    public Transform playerOrientation;

    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        //Lock cursor in middle of screen
        //Make it invisible for later body-swapping
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Get mouse input (x, y)
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity;

        //Change cam rotations
        yRotation = yRotation + mouseX;
        xRotation = xRotation - mouseY;

        //Clamp x rotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Rotation camera and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerOrientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }
}
