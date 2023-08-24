using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float normalMovementSpeed = 5.0f;
    public float boostedMovementSpeed = 15.0f;
    public float mouseSensitivity = 2.0f;

    private void Update()
    {
        // Camera movement
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        float verticalCameraMovement = 0;

        if (Input.GetKey(KeyCode.Space))
        {
            verticalCameraMovement = 1;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            verticalCameraMovement = -1;
        }

        float currentMovementSpeed = Input.GetKey(KeyCode.LeftControl) ? boostedMovementSpeed : normalMovementSpeed;

        Vector3 movementDirection = new Vector3(horizontalMovement, verticalCameraMovement, verticalMovement);
        movementDirection = transform.TransformDirection(movementDirection);
        transform.position += movementDirection * currentMovementSpeed * Time.deltaTime;

        // Camera rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 rotation = transform.localEulerAngles;
        rotation.y += mouseX * mouseSensitivity;
        rotation.x -= mouseY * mouseSensitivity;
        // rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);

        transform.localEulerAngles = rotation;
    }
}
