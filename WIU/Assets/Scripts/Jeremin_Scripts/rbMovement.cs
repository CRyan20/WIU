using UnityEngine;

public class rbMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Animator animator;
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 100f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float crouchHeight = 0.5f;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;

    bool isGrounded;
    bool isCrouching = false;
    bool isSprinting = false;
    float standingHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        standingHeight = transform.localScale.y; // Store the standing height
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, -2f, rb.velocity.z);
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = isCrouching ? walkSpeed / 2f : (isSprinting ? sprintSpeed : walkSpeed);
        rb.MovePosition(rb.position + move * currentSpeed * Time.deltaTime);

        bool isMoving = (x != 0 || z != 0);
        animator.SetBool("isMoving", z > 0);


        animator.SetBool("isSprinting", isSprinting);


        animator.SetBool("isWalkingBackwards", z < 0);

        animator.SetBool("isSprintingBackwards", isSprinting && z < 0);

        animator.SetBool("isStrafing", x > 0);

        animator.SetBool("isStrafingL", x < 0);

        animator.SetBool("isStrafeRunning", isSprinting && x > 0);

        animator.SetBool("isStrafingRunningL", isSprinting && x < 0);

        if (Input.GetKeyDown(crouchKey) && isGrounded)
        {
            isCrouching = !isCrouching;
            animator.SetBool("isCrouching", isCrouching);
            if (isCrouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, standingHeight, transform.localScale.z);
            }
        }

        if (Input.GetKeyDown(sprintKey) && !isCrouching)
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(sprintKey) || isCrouching)
        {
            isSprinting = false;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * gravity), ForceMode.VelocityChange);
            animator.SetTrigger("jumpTrigger");
        }

        rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }
}
