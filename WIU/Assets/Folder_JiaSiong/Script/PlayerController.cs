using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 200f;
    public float jumpForce = 5f;
    private bool isGrounded;

    private Rigidbody rb;
    private InventoryManager inventoryManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inventoryManager = GetComponent<InventoryManager>();

        // Configure Rigidbody
        rb.freezeRotation = true;
        rb.useGravity = true; // We'll handle gravity manually
    }

    void Update()
    {
        // Handle player movement
        HandleRotation();
        HandleMovement();

        //// Check for pickup when player presses the "F" key
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    inventoryManager.TryPickupItem();
        //}

        //// Check for dropping when player presses the "G" key
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    inventoryManager.TryDropItem();
        //}

        // Check for jumping when player presses the space key
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void HandleRotation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float rotateAmount = horizontal * rotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotateAmount);
    }

    void HandleMovement()
    {
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = transform.forward * vertical * moveSpeed * Time.deltaTime;

        // Apply movement using Rigidbody
        rb.MovePosition(rb.position + movement);

        // Apply gravity manually only when not grounded
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * 10f); // Adjust the force as needed
        }
    }

    void Jump()
    {
        // Apply a vertical force to simulate jumping
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionStay(Collision collision)
    {
        // Check if the player is grounded based on collisions
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        // Set isGrounded to false when not in contact with the ground
        isGrounded = false;
    }
}
