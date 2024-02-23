using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using Photon.Pun;

public class scr_MinionAi : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform[] players;
    //public Transform target;
    public LayerMask whatIsGround, whatIsPlayer;
    private Animator animator;
    private HealthSystem healthSystem;

    //[Header("Stats")]
    //public float health = 65;

    // Idle
    [Header("Idle")]
    public float idleDuration = 5f;
    private float idleTimer = 0f;

    // Patrolling
    [Header("Patrol")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    [Header("Attack")]
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // Dead
    public bool isDead;

    // Range
    [Header("Range")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Audio")]
    public AudioSource attackAudioSource;
    public AudioSource chaseAudioSource;
    public AudioSource idleAudioSource;
    public AudioSource deathAudioSource;

    private PhotonView photonView;

    // States
    private enum MinionState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead,
    }

    private MinionState currentState = MinionState.Idle;

    private void Awake()
    {
        //target = GameObject.Find("Player").transform;
        //target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
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

        isDead = false;
        healthSystem = GetComponent<HealthSystem>();

        // Delayed initialization after a short delay (adjust delay time as needed)
        StartCoroutine(DelayedInitialization(10.0f));

      


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

    private void Update()
    {
        // Check if player in sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        CheckDamage();

        switch (currentState)
        {
            case MinionState.Idle:
                Idle();
                break;

            case MinionState.Patrol:
                if (playerInSightRange && !playerInAttackRange)
                {
                    currentState = MinionState.Chase;
                    ChasePlayer();
                }
                else if (playerInSightRange && playerInAttackRange)
                {
                    currentState = MinionState.Attack;
                    AttackPlayer();
                }
                else
                {
                    Patroling();
                }
                break;

            case MinionState.Chase:
                if (!playerInSightRange)
                {
                    currentState = MinionState.Patrol;
                    Patroling();
                }
                else if (playerInAttackRange)
                {
                    currentState = MinionState.Attack;
                    AttackPlayer();
                }
                else
                {
                    ChasePlayer();
                }
                break;

            case MinionState.Attack:
                if (!playerInSightRange)
                {
                    currentState = MinionState.Patrol;
                    Patroling();
                }
                else if (!playerInAttackRange)
                {
                    currentState = MinionState.Chase;
                    ChasePlayer();
                }
                else
                {
                    AttackPlayer();
                }
                break;

            case MinionState.Dead:
                Death();

                break;
        }

    }

    private void Idle()
    {
        //// Stop other audio sources
        //StopAllAudioExcept(idleAudioSource);

        // Play idle sound
        idleAudioSource.Play();

        animator.SetBool("isPatrolling", false);

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;
            currentState = MinionState.Patrol;
        }
    }

    private void Patroling()

    {
        if (idleAudioSource != null && !idleAudioSource.isPlaying)
        {
            idleAudioSource.Play();
        }

        animator.SetBool("isPatrolling", true);
        animator.SetBool("isChasing", false);
        
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        // Stop idle and attack sounds
        if (idleAudioSource != null)
            idleAudioSource.Stop();
        if (attackAudioSource != null)
            attackAudioSource.Stop();

        // Play chase sound
        if (chaseAudioSource != null && !chaseAudioSource.isPlaying)
            chaseAudioSource.Play();

        foreach (Transform player in players)
        {
            animator.SetBool("isChasing", true);
            agent.SetDestination(player.position);
            agent.speed = 5;
            agent.acceleration = 6; 
        }
    }

    private void AttackPlayer()
    { // Stop idle and chase sounds
        if (idleAudioSource != null)
            idleAudioSource.Stop();
        if (chaseAudioSource != null)
            chaseAudioSource.Stop();

        // Play attack sound
        if (attackAudioSource != null && !attackAudioSource.isPlaying)
            attackAudioSource.Play();

        // Make sure enemy does not move
        agent.SetDestination(transform.position);

        foreach (Transform player in players)
        {
            transform.LookAt(player); 
        }

        // Ensure rotation around x-axis is zero
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.x = 0;
        transform.rotation = Quaternion.Euler(newRotation);

        if (!alreadyAttacked)
        {
            /// Attack code here
            
            ///
            alreadyAttacked = true;
            animator.SetBool("isAttacking", true);
            StartCoroutine(ResetAttackAfterAnimation());
        }
    }

    private void StopAllAudioExcept(AudioSource exceptSource)
    {
        if (exceptSource != attackAudioSource)
            attackAudioSource.Stop();
        if (exceptSource != chaseAudioSource)
            chaseAudioSource.Stop();
        if (exceptSource != idleAudioSource)
            idleAudioSource.Stop();
        if (exceptSource != deathAudioSource)
            deathAudioSource.Stop();
    }

    private IEnumerator ResetAttackAfterAnimation()
    {
        // Wait for the duration of the attack animation
        yield return new WaitForSeconds(timeBetweenAttacks);

        // Reset attack state and animation
        ResetAttackState();
    }

    public void ResetAttackState()
    {
        alreadyAttacked = false;
        animator.SetBool("isAttacking", false);
    }

    public void CheckDamage()
    {
        if (healthSystem.currentHealth <= 0f && !isDead)
        {
            currentState = MinionState.Dead;
            Death();
        }
    }

    private void Death()
    {
        //// Stop other audio sources
        //StopAllAudioExcept(deathAudioSource);

        // Play death sound
        deathAudioSource.Play();

        isDead = true;
        agent.velocity = Vector3.zero;  
        StartCoroutine(DestroyEnemy());
    }

    IEnumerator DestroyEnemy()
    {
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
