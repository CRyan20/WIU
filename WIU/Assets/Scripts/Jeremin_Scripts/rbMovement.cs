using UnityEngine;
using Photon.Pun;

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

    public AudioSource Sprint;
    public AudioSource Walk;
    public AudioSource Jump;

    private InventoryManager inventoryManager;

    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        inventoryManager = GetComponent<InventoryManager>();
        standingHeight = transform.localScale.y; // Store the standing height

        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
    }

    void Update()
    {
        if (!photonView)
        {
            return;
        }

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

        if (isMoving && !isCrouching && !isSprinting && isGrounded)
        {
            if (!Walk.isPlaying)
            {
                Walk.Play();
            }
        }
        else
        {
            Walk.Stop();
        }

   
        if (isSprinting && isGrounded)
        {
            if (!Sprint.isPlaying)
            {
                Sprint.Play();
            }
        }
        else
        {
            Sprint.Stop();
        }

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
            Sprint.Play();
            isSprinting = true;

           
        }
        if (Input.GetKeyUp(sprintKey) || isCrouching)
        {
            Sprint.Stop();
            isSprinting = false;
            
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * gravity), ForceMode.VelocityChange);
            animator.SetTrigger("jumpTrigger");
            Jump.Play();
        }

        // Check for pickup when player presses the "F" key
        if (Input.GetKeyDown(KeyCode.F))
        {
            inventoryManager.TryPickupItem();
        }

        // Check for dropping when player presses the "G" key
        if (Input.GetKeyDown(KeyCode.G))
        {
            inventoryManager.TryDropItem();
        }

        rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }
}
