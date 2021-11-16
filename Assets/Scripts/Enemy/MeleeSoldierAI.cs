using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeSoldierAI : IEnemyAI
{
    public float timeBetweenAttack = 2f;
    private float timer;

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

        // If attacking, dont do anything else
        if (isAttacking)
        {
            Vector3 lookVector = player.transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(lookVector, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 60 * Time.deltaTime);
            return;
        }

        // Timer for attack
        timer += Time.deltaTime;

        // Check for sight and attack range
        inVisionRange = Physics.CheckSphere(transform.position, visionRange, playerMask);
        inAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if (inVisionRange && inAttackRange)
            AttackMode();
        else if (inVisionRange && !inAttackRange)
            ChasePlayer(); // Chase player nonstop
        else if (!inVisionRange && !inAttackRange)
            Wander(); // Wander near their spawn point

        AnimationState();
        anim.SetInteger("state", (int)state);
    }

    private void AttackMode()
    {
        // Look at the player
        Vector3 lookVector = player.transform.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(lookVector, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 900 * Time.deltaTime);

        if (timer > timeBetweenAttack)
        {
            AttackPlayer();
            timer = 0;
        }
    }

    private void AttackPlayer()
    {
        isAttacking = true;
        anim.Play("Attack");
    }

    public void AttackDealDamage()
    {
        Vector3 attackPoint = transform.position + new Vector3(0, 1.6f, 0) + transform.forward;
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint, attackRange * 0.5f, playerMask);
        foreach (Collider hitObject in hitObjects)
        {
            // Check enemy
            PlayerController player = hitObject.GetComponent<PlayerController>();
            if (player)
            {
                player.Damaged(stat.damage, transform.position);
            }
        }
    }

    public void FinishedAttack()
    {
        isAttacking = false;
    }
}