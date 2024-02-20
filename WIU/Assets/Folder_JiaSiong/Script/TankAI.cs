using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class TankAI : MonoBehaviour
{
    [Header("AI States")]
    public EnemyState currState = EnemyState.IDLE;
    public enum EnemyState
    {
        PATROL,
        IDLE,
        CHASE,
        ATTACK,
        DEAD
    }
    [Header("Enemy Data")]
    public NavMeshAgent chaser;
    public Transform[] players;
    private float rotationSpeed = 1f;
    public GameObject lookAt;
    public float lookAtOffset = 0.5f;
    private float originalLookAtYPos;
    PhotonView photonView;

    [Header("Patrol State")]
    public float patrolSpeed = 1f;
    public Transform[] waypoints;
    public float waypointRange = 3f; //range when enemy swaps to next waypoint
    private int currWaypointIndex = 1;
    private bool hasReachedWaypoint = false;

    [Header("Chase State")]
    public float chaseRange = 15f;
    public float chaseSpeed = 5f;
    private bool isCooldownActive = false;
    private float cooldownDuration = 6.0f;
    // Add a variable to store the original chase speed
    private float originalChaseSpeed;

    [Header("Animation")]
    public Animator animator;

    [Header("Field of View")]
    public float fovAngle = 150f; // Field of view angle
    public float viewDistance = 7f; // Maximum distance the AI can see

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find all with player tag
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }
        if (players == null || players.Length == 0)
        {
            Debug.LogError("Player not found");
        }

        animator = GetComponent<Animator>();

        //original y pos
        originalLookAtYPos = lookAt.transform.position.y;
        // Position the eyes relative to the enemy's position and forward direction
        Vector3 lookAtPosition = transform.position + transform.forward * lookAtOffset;
        lookAtPosition.y = originalLookAtYPos;
        lookAt.transform.position = lookAtPosition;
        lookAt.transform.rotation = transform.rotation;

        //start state is patrol
        currState = EnemyState.PATROL;

        // Delayed initialization after a short delay (adjust delay time as needed)
        StartCoroutine(DelayedInitialization(1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case EnemyState.IDLE:
                Idle();
                break;
            case EnemyState.PATROL:
                Patrol();
                break;
            case EnemyState.CHASE:
                Chase();
                break;
            case EnemyState.ATTACK:
                Attack();
                break;
        }

    }
    IEnumerator DelayedInitialization(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Find all with player tag
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new Transform[playerObjects.Length];
        for (int i = 0; i < playerObjects.Length; i++)
        {
            players[i] = playerObjects[i].transform;
        }

        if (players == null || players.Length == 0)
        {
            Debug.LogError("Player not found");
        }
    }

    void Patrol()
    {
        chaser.speed = patrolSpeed;
        // Iterate through each player in the players array
        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            if (Vector3.Distance(transform.position, player.position) < chaseRange)
            {
                //switch to chase state
                currState = EnemyState.CHASE;
                return;
            }
        }

        //if it hasnt reached waypoint
        if (!hasReachedWaypoint)
        {
            // Check the distance to the current waypoint
            float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currWaypointIndex].position);

            // Check if the enemy is close enough to the current waypoint
            if (distanceToWaypoint <= waypointRange)
            {
                // Move to the next waypoint
                currWaypointIndex = (currWaypointIndex + 1) % waypoints.Length;
                hasReachedWaypoint = true;
            }
        }
        else
        {
            hasReachedWaypoint = false;
        }

        // Set destination to the current waypoint
        chaser.SetDestination(waypoints[currWaypointIndex].position);
        animator.SetBool("Walking", true);
    }

    void Chase()
    {
        chaser.speed = chaseSpeed;

        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < chaseRange)
            {
                if (!isCooldownActive)
                {
                    StartCoroutine(SlowDownCooldown());
                }

                if (distanceToPlayer > chaser.stoppingDistance)
                {
                    // Set destination and start chasing animation
                    chaser.SetDestination(player.position);
                    animator.SetBool("Chasing", true);
                    animator.SetBool("Walking", false);
                }
                else
                {
                    chaser.velocity = Vector3.zero;

                    animator.SetBool("Chasing", false);
                    animator.SetBool("Walking", false);

                    // Check if player is still in range
                    if (!IsPlayerInFOV(player.position))
                    {
                        // Player is out of range, transition to PATROL state
                        currState = EnemyState.PATROL;
                        animator.SetBool("Chasing", false);
                        animator.SetBool("Walking", true);
                    }
                }

                return;  // Early return to avoid executing the rest of the code for other players
            }
        }

        // If not in range for any player, make it patrol state
        currState = EnemyState.PATROL;
        animator.SetBool("Chasing", false);
        animator.SetBool("Walking", true);
    }

    IEnumerator SlowDownCooldown()
    {
        isCooldownActive = true;

        // Slow down the tank for 3 seconds
        chaser.speed = originalChaseSpeed * 0.5f; // You can adjust the multiplier as needed

        yield return new WaitForSeconds(3.0f);

        // Restore the original speed after 3 seconds
        chaser.speed = originalChaseSpeed;

        // Wait for the full cooldown duration before allowing another slowdown
        yield return new WaitForSeconds(cooldownDuration - 3.0f);

        isCooldownActive = false;
    }
    void Idle()
    {
        // Implement the behavior during the IDLE state here
        // For example, you can rotate the tank or play an idle animation

        // Check if any player is in range to transition to CHASE state
        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < chaseRange)
            {
                currState = EnemyState.CHASE;
                return;
            }
        }
    }
    void Attack()
    {
        //health decrease, attack anim here etc
        animator.SetBool("Walking", true);
        animator.SetBool("Attack", true);

        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            //not in range
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > chaseRange)
            {
                animator.SetBool("Attack", false);
                currState = EnemyState.PATROL;
            }
        }
    }
    bool IsPlayerInFOV(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - transform.position;
        float angle = Vector3.Angle(directionToTarget, transform.forward);

        if (angle <= fovAngle * 0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        // Draw the field of view
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, fovAngle * 0.5f, 0) * transform.forward * viewDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -fovAngle * 0.5f, 0) * transform.forward * viewDistance);
    }
}
