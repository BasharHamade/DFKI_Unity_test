using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstpersoncontroller : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float mouseSensitivity = 2.0f;

    private float verticalRotation = 0;
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        characterController.SimpleMove(moveDirection.normalized * walkSpeed);

        // Player rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        // Unlock the cursor with the Escape key (for testing)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}


