using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IEnemyAI : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Animator anim;
    protected Transform player;
    public LayerMask groundMask, playerMask;
    protected IEnemy stat;

    public Vector3 spawnPoint;
    public float wanderRange;
    protected Vector3 wanderSpot;
    public bool foundNewWanderSpot = false;
    protected GameObject DeathEffectPrefab;

    public float visionRange, attackRange;
    protected bool inVisionRange, inAttackRange;
    protected bool isAttacking = false;

    /* Finite State Machine */
    public double stayTime;

    protected enum State { idle, walking, chasing, attack }

    [SerializeField] protected State state = State.idle;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        stat = GetComponent<IEnemy>();
        player = GameObject.Find("PlayerDino").transform;
        DeathEffectPrefab = Resources.Load<GameObject>("Prefabs/DeathEffectPrefab");

        if (spawnPoint == Vector3.zero) spawnPoint = transform.position;
    }

    protected virtual void AnimationState()
    {
        if (isAttacking)
        {
            state = State.attack;
        }
        else if (inAttackRange)
        {
            state = State.idle;
        }
        else if (inVisionRange)
        {
            state = State.chasing;
        }
        else if (!inVisionRange)
        {
            state = State.walking;
        }
        else
        {
            // Not used yet
            state = State.idle;
        }
    }

    protected virtual void Wander()
    {
        if (!foundNewWanderSpot)
        {
            SearchNewWanderSpot();
            agent.SetDestination(wanderSpot);
            stayTime = 0;
        }

        stayTime += Time.deltaTime;
        if (stayTime > 5)
        {
            SearchNewWanderSpot();
            agent.SetDestination(wanderSpot);
            stayTime = 0;
        }

        Vector3 distanceToWanderSpot = transform.position - wanderSpot;
        // Arrived at the wanderSpot, now need to search for newer one
        if (distanceToWanderSpot.magnitude < 1f)
            foundNewWanderSpot = false;
    }

    protected virtual void SearchNewWanderSpot()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-wanderRange, wanderRange);
        float randomX = Random.Range(-wanderRange, wanderRange);
        wanderSpot = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Make sure that the newly calculated spot is possible to move there
        if (Physics.Raycast(wanderSpot, -transform.up, 2f, groundMask) && NavMesh.SamplePosition(wanderSpot, out _, 1f, NavMesh.AllAreas))
            foundNewWanderSpot = true;
    }

    protected virtual void ChasePlayer()
    {
        // After first time spot the player, the enemy will keep chasing player forever
        visionRange = 10000;
        // The enemy will stop to attempt to attack player when the player is within its attack range
        agent.stoppingDistance = attackRange;
        agent.SetDestination(player.position);
    }

    public virtual void Death()
    {
        Quaternion bloodDirection = Quaternion.LookRotation(transform.position - player.position, Vector3.up);
        Instantiate(DeathEffectPrefab, transform.position, bloodDirection);
        Destroy(this.gameObject);
        LevelEvents.levelEvents.EnemyDeathTriggerEnter(this.transform.position);
    }
}