using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCam : MonoBehaviour
{
    public float sensitivity;
    public float slowSpeed;
    public float normalSpeed;
    public float sprintSpeed;
    float currentSpeed;

    public GameObject player;

    void Update()
    {
        if (player == null)
        {
            // Enable control for the spectator camera
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Movement();
            Rotation();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(player);
        }
    }

    public void Rotation()
    {
        Vector3 mouseInput = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        transform.Rotate(mouseInput * sensitivity);
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
    }

    public void Movement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            currentSpeed = slowSpeed;
        }
        else
        {
            currentSpeed = normalSpeed;
        }
        transform.Translate(input * currentSpeed * Time.deltaTime);
    }
}
