using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangeSoldierAI : IEnemyAI
{
    public ProjectileController BulletPrefab;
    public float shootForce;
    public float timeBetweenShooting;
    [SerializeField] private float timer;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        // If stunned, then do nothing
        if (transform.GetComponent<IEnemy>().isStunned)
        {
            isAttacking = false;
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
            return;
        }

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
        Vector3 lookVector = (player.position - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 900 * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer > timeBetweenShooting)
        {
            timer = 0;
            isAttacking = true;
            anim.Play("Attack");
        }
    }

    /// <summary>
    /// This function used in AnimationEvent.
    /// </summary>
    public void ShootPlayer()
    {
        ProjectileController projectile = Instantiate(BulletPrefab, transform.position + transform.forward + new Vector3(0, 1, 0), Quaternion.identity);
        projectile.damage = stat.damage;
        projectile.owner = this.gameObject;
    }

    /// <summary>
    /// This function used in AnimationEvent.
    /// </summary>
    public void FinishedAttack()
    {
        isAttacking = false;
    }
}