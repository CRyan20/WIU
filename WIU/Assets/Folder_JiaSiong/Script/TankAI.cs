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
    public HealthSystem healthSystem;
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
        healthSystem = GetComponent<HealthSystem>();

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
                if (distanceToPlayer > chaser.stoppingDistance)
                {
                    chaser.SetDestination(player.position);
                }
                else
                {
                    chaser.velocity = Vector3.zero;

                    // Trigger the Idle function every 6 seconds
                    StartCoroutine(IdleAfterDelay(6.0f));

                    // Check if player is still in range
                    if (!IsPlayerInFOV(player.position))
                    {
                        // Player is out of range, transition to PATROL state
                        currState = EnemyState.PATROL;
                    }
                }

                if (IsPlayerInFOV(player.position))
                {
                    return;
                }
            }
            else
            {
                //if not in range make it patrol state
                currState = EnemyState.PATROL;
            }
        }
    } 

    IEnumerator IdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Check if player is still in range
        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < chaseRange)
            {
                // Player is still in range, transition to CHASE state
                currState = EnemyState.CHASE;
            }
            else
            {
                // Player is out of range, transition to PATROL state
                currState = EnemyState.PATROL;
            }
        }
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
