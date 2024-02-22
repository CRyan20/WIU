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
        ATTACK
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
    public float patrolRange = 5f; // Range for random patrol movement
    private float nextTurnTime;
    private bool isRoaming = false;

    [Header("Chase State")]
    public float chaseRange = 2f;
    public float chaseSpeed = 3f;
    private bool isCooldownActive = false;
    private float cooldownDuration = 6.0f;
    private float originalChaseSpeed;

    [Header("Attack State")]
    public float attackRange = 1f;

    [Header("Animation")]
    public Animator animator;

    [Header("Field of View")]
    public float fovAngle = 150f;
    public float viewDistance = 7f;

    [Header("Audio")]
    public AudioSource idleAudioSource;
    public AudioSource chaseAudioSource;
    public AudioSource attackAudioSource;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

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
        Vector3 lookAtPosition = transform.position + transform.forward * lookAtOffset;
        lookAtPosition.y = originalLookAtYPos;
        lookAt.transform.position = lookAtPosition;
        lookAt.transform.rotation = transform.rotation;

        //start state is patrol
        currState = EnemyState.PATROL;

        StartCoroutine(DelayedInitialization(1.0f));

        // Start playing idle sound
        if (idleAudioSource != null)
            idleAudioSource.Play();
    }

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

        if (chaser.velocity.magnitude < 0.1f)
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Chasing", false);
        }
    }

    IEnumerator DelayedInitialization(float delay)
    {
        yield return new WaitForSeconds(delay);

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

        // Stop chase and attack sounds
        if (chaseAudioSource != null)
            chaseAudioSource.Stop();
        if (attackAudioSource != null)
            attackAudioSource.Stop();

        // Check if patrol sound is not playing and play it
        if (idleAudioSource != null && !idleAudioSource.isPlaying)
        {
            StartCoroutine(WaitForPatrolAudioCompletion(idleAudioSource));
            idleAudioSource.Play();
        }


        // Check for players within chase range
        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < chaseRange)
            {
                // If a player is within chase range, switch to chase state and play chase sound
                currState = EnemyState.CHASE;
                if (chaseAudioSource != null && !chaseAudioSource.isPlaying)
                    chaseAudioSource.Play();
                return;
            }
        }

        // Check if it's time to turn or the tank has reached its destination
        if (Time.time >= nextTurnTime || !chaser.pathPending && chaser.remainingDistance <= chaser.stoppingDistance)
        {
            // Set the tank to roam
            isRoaming = true;

            // Generate a new random destination within NavMesh
            Vector3 randomDestination = RandomNavSphere(transform.position, patrolRange, -1);

            // Set the new destination
            chaser.SetDestination(randomDestination);

            // Set the next turn time for a delay (adjust the delay time as needed)
            nextTurnTime = Time.time + Random.Range(5f, 15f); // Random delay between 5 to 15 seconds
        }

        animator.SetBool("Walking", isRoaming);
    }
    IEnumerator WaitForPatrolAudioCompletion(AudioSource audioSource)
    {
        // Wait until the patrol audio clip finishes playing
        yield return new WaitForSeconds(audioSource.clip.length);

        // Continue with the rest of the logic after the audio clip finishes playing
        // For example:
        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < chaseRange)
            {
                currState = EnemyState.CHASE;
                yield break; // Exit the coroutine if chasing
            }
        }

        // Check if it's time to turn or the tank has reached its destination
        if (Time.time >= nextTurnTime || !chaser.pathPending && chaser.remainingDistance <= chaser.stoppingDistance)
        {
            // Set the tank to roam
            isRoaming = true;

            // Generate a new random destination within NavMesh
            Vector3 randomDestination = RandomNavSphere(transform.position, patrolRange, -1);

            // Set the new destination
            chaser.SetDestination(randomDestination);

            // Set the next turn time for a delay (adjust the delay time as needed)
            nextTurnTime = Time.time + Random.Range(5f, 15f); // Random delay between 5 to 15 seconds
        }

        animator.SetBool("Walking", isRoaming);
    }

    // Helper function to generate a random destination within NavMesh
    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.collider.name);

        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player collision detected!");

            chaser.velocity = Vector3.zero;
            currState = EnemyState.ATTACK;
            animator.SetBool("Attack", true);
            animator.SetBool("Chasing", false);
            animator.SetBool("Walking", false);
        }
    }

    void Chase()
    {

        chaser.speed = Mathf.Lerp(chaser.speed, chaseSpeed, Time.deltaTime);

        bool shouldChase = false;

        // Stop idle and attack sounds
        if (idleAudioSource != null)
            idleAudioSource.Stop();
        if (attackAudioSource != null)
            attackAudioSource.Stop();

        // Play chase sound
        if (chaseAudioSource != null && !chaseAudioSource.isPlaying)
            chaseAudioSource.Play();

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
                    chaser.SetDestination(player.position);
                    animator.SetBool("Chasing", true);
                    animator.SetBool("Walking", false);

                    if (distanceToPlayer <= chaser.stoppingDistance + attackRange)
                    {
                        currState = EnemyState.ATTACK;
                        animator.SetBool("Attack", true);
                        animator.SetBool("Chasing", false);
                        animator.SetBool("Walking", false);
                        return;
                    }
                    else
                    {
                        shouldChase = true;
                    }
                }
                else
                {
                    chaser.velocity = Vector3.zero;

                    animator.SetBool("Chasing", false);
                    animator.SetBool("Walking", true);

                    if (!IsPlayerInFOV(player.position))
                    {
                        currState = EnemyState.PATROL;
                        animator.SetBool("Chasing", false);
                        animator.SetBool("Walking", false);
                        isCooldownActive = false;
                        return;
                    }
                }
            }
        }

        if (!shouldChase)
        {
            if (chaser.speed <= 0.1f)
            {
                currState = EnemyState.IDLE;
                chaser.speed = 0f;
            }
            else
            {
                currState = EnemyState.PATROL;
                animator.SetBool("Chasing", false);
                animator.SetBool("Walking", true);
                isCooldownActive = false;
                chaser.speed = originalChaseSpeed;
            }
        }
    }

    IEnumerator SlowDownCooldown()
    {
        isCooldownActive = true;
        chaser.speed *= 0.5f;
        yield return new WaitForSeconds(3.0f);
        chaser.speed = originalChaseSpeed;
        yield return new WaitForSeconds(cooldownDuration - 3.0f);
        isCooldownActive = false;
    }

    void Idle()
    {
        chaser.speed = 0f;

        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < chaseRange)
            {
                currState = EnemyState.CHASE;
                chaser.speed = chaseSpeed;
                return;
            }
        }
    }

    void Attack()
    {
        // Stop idle and chase sounds
        if (idleAudioSource != null && idleAudioSource.isPlaying)
            idleAudioSource.Stop();
        if (chaseAudioSource != null && chaseAudioSource.isPlaying)
            chaseAudioSource.Stop();

        // Play attack sound
        if (attackAudioSource != null && !attackAudioSource.isPlaying)
            attackAudioSource.Play();

        //health decrease, attack anim here etc
        animator.SetBool("Walking", true);
        animator.SetBool("Attack", true);

        foreach (Transform player in players)
        {
            if (player == null)
                continue;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= chaser.stoppingDistance)
            {
                chaser.speed = 0f;
                animator.SetBool("Attack", true);
                animator.SetBool("Walking", false);

                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                currState = EnemyState.CHASE;
                chaser.speed = chaseSpeed;
                animator.SetBool("Attack", false);
                animator.SetBool("Chasing", true);
                animator.SetBool("Walking", false);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, fovAngle * 0.5f, 0) * transform.forward * viewDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -fovAngle * 0.5f, 0) * transform.forward * viewDistance);
    }
}
