using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeSoldierAI : IEnemyAI
{
    public bool inCharge = false;
    public float extraDistance;

    public float timeBetweenCharges, recoveryRate;
    private float chaseTimer, timer;

    private Vector3 chargePosition;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        // If stunned, then do nothing
        if (transform.GetComponent<IEnemy>().isStunned)
        {
            agent.enabled = false;
            return;
        }
        else if (!agent.enabled)
        {
            agent.enabled = true;
        }
        agent.isStopped = false;

        if (agent.transform.position == chargePosition)
        {
            agent.isStopped = true;
            inCharge = false;
        }
        // Check for sight and attack range
        inVisionRange = Physics.CheckSphere(transform.position, visionRange, playerMask);
        inAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        // Stop the charge
        if (inCharge)
        {
            timer += Time.deltaTime;
            if (timer > recoveryRate)
            {
                inCharge = false;
                anim.SetBool("inCharge", false);
                agent.speed = 3.5f;
                agent.acceleration = 8;
                timer = 0;
            }
            if ((transform.position - chargePosition).magnitude < 0.2)
            {
                inCharge = false;
                anim.SetBool("inCharge", false);
                agent.speed = 3.5f;
                agent.acceleration = 8;
                timer = 0;
            }
        }
        else
        {
            if (inVisionRange && inAttackRange)
            {
                AttackMode();
            }
            else if (inVisionRange && !inAttackRange)
            {
                isAttacking = false;
                timer = 0;
                ChasePlayer(); // Chase player nonstop
            }
            else if (!inVisionRange && !inAttackRange)
            {
                isAttacking = false;
                timer = 0;
                Wander(); // Wander near their spawn point
            }
            AnimationState();
            anim.SetInteger("state", (int)state);
        }
    }

    protected override void AnimationState()
    {
        if (isAttacking)
        {
            state = State.attack;
        }
        else if (inAttackRange)
        {
            state = State.chasing;
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

    protected override void ChasePlayer()
    {
        // After first time spot the player, the enemy will keep chasing player forever
        // The enemy will stop to attempt to attack player when the player is within its attack range
        visionRange = 10000;
        agent.SetDestination(player.position);
    }

    private void AttackMode()
    {
        visionRange = 10000;
        // Look at the player
        Vector3 lookVector = (player.position - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 900 * Time.deltaTime);
        chargePosition = player.position + (lookVector * extraDistance);

        // Precharging
        isAttacking = true;
        agent.isStopped = true;

        timer += Time.deltaTime;
        if (timer > timeBetweenCharges)
        {
            ChargePlayer();
            timer = 0;
        }
    }

    private void ChargePlayer()
    {
        inCharge = true;
        anim.SetBool("inCharge", true);
        agent.SetDestination(chargePosition);
        agent.speed = 30;
        agent.acceleration = 30;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (inCharge)
        {
            PlayerController player = col.transform.GetComponent<PlayerController>();
            // Check if componenent is not null first or it will error for isGuarding
            if (player)
            {
                player.Damaged(stat.damage, transform.position);
            }
        }
    }
}