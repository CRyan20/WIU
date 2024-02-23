using UnityEngine;
using Photon.Pun;
using TMPro;

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

    public TextMeshProUGUI doorText;
    public DoorInteraction doorInteraction;

    public AudioSource Sprint;
    public AudioSource Walk;
    public AudioSource Jump;

    private StaminaBar staminaBar;
    private InventoryManager inventoryManager;

    private PhotonView photonView;

    public HealthSystem healthSystem;

    public float sprintDepletionRate = 1f;

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
        healthSystem = GetComponent<HealthSystem>();
        staminaBar = GetComponentInChildren<StaminaBar>();
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        doorInteraction = GetComponentInChildren<DoorInteraction>();

        staminaBar = GetComponentInChildren<StaminaBar>();

        if (staminaBar != null)
        {
            staminaBar.SetMaxStamina((int)staminaBar.maxStamina);
        }
      
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (healthSystem.currentHealth <= 0)
        {
            healthSystem.currentHealth = 0;
            //display Gameover canvas.
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (PauseMenu.Instance.isPaused == false)
        {
            if (isGrounded && rb.velocity.y < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, -2f, rb.velocity.z);
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            float currentSpeed = isCrouching ? walkSpeed / 2f : (isSprinting && staminaBar.currentStamina > 0 ? sprintSpeed : walkSpeed);

            // Use rb.velocity to apply movement
            rb.velocity = new Vector3(move.x * currentSpeed, rb.velocity.y, move.z * currentSpeed);

            bool isMoving = (x != 0 || z != 0);

            // Set animation states based on player's movement
            if (isMoving && !isCrouching && !isSprinting && isGrounded)
            {
                if (!Walk.isPlaying)
                {
                    Walk.Play();
                }

                // Check if stamina is at 0 and switch to walking animation
                if (staminaBar.currentStamina <= 0)
                {
                    isSprinting = false;
                    animator.SetBool("isSprinting", false);
                    animator.SetBool("isMoving", true);
                }
            }
            else
            {
                Walk.Stop();

                // If not moving and not crouching, switch to idle animation
                if (!isCrouching)
                {
                    animator.SetBool("isMoving", false);
                }
            }

            if (isSprinting && isGrounded && isMoving && staminaBar.currentStamina > 0)
            {
                Debug.Log("Sprinting...");
                animator.SetBool("isSprinting", true);
                animator.SetBool("isMoving", true);
                Debug.Log(staminaBar.currentStamina);
                if (!Sprint.isPlaying)
                {
                    Sprint.Play();
                    staminaBar.DecreaseStamina(staminaBar.depletionSpeed * Time.deltaTime);
                    Debug.Log(staminaBar.currentStamina);
                }
            }
            else if (!isSprinting && isGrounded &&  isMoving)
            {
                // Play the idle or walking animation based on the player's state
                animator.SetBool("isSprinting", false);
                animator.SetBool("isMoving", true);
                if (staminaBar.currentStamina > 0)
                {
                    Walk.Play();
                }
                else
                {
                    Walk.Stop();
                }
            }

            // Update the stamina bar
            if (staminaBar != null)
            {
                staminaBar.UpdateStaminaSprite((int)staminaBar.currentStamina);
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

            if (Input.GetKeyUp(sprintKey) || isCrouching || staminaBar.currentStamina <= 0)
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

            // If stamina is at 0, switch to walking animation
            if (staminaBar.currentStamina <= 0)
            {
                isSprinting = false;
                animator.SetBool("isSprinting", false);
            }

            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            DoorInteraction doorInteraction = other.GetComponentInChildren<DoorInteraction>();
            if (doorInteraction != null)
            {
                doorInteraction.isPlayerNearby = true;
                if (!doorInteraction.isDoorOpened)
                {
                    Debug.Log("Press 'F' to open the door.");
                    doorText.gameObject.SetActive(true);
                    doorText.text = "Press 'F' to Open";
                }
                else
                {
                    Debug.Log("Press 'F' to close the door.");
                    doorText.gameObject.SetActive(true);
                    doorText.text = "Press 'F' to Close";
                    // You may want to hide the text if the door is already open.
                    // doorText.gameObject.SetActive(false);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            DoorInteraction doorInteraction = other.GetComponentInChildren<DoorInteraction>();
            if (doorInteraction != null)
            {
                doorInteraction.isPlayerNearby = false;
                doorText.gameObject.SetActive(false); // Hide the text when the player moves away from the door
            }
        }
    }
}
