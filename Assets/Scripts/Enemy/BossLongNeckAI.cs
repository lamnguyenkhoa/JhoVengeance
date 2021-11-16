using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLongNeckAI : IEnemyAI
{
    public float actionIimer = 0f;
    public float timeBetweenAction = 30f; // Special action
    public float attackTimer = 0f;
    public float timeBetweenAttack = 4f;

    private int phase = 1;

    public enum BossAction { tailswipe, createBoulder, summonMinion }

    public BossAction currentAction;
    public bool finishedCurrentAction = true;
    public bool playedAnimation = true;
    public bool finishedAttack = true;

    public SpawnMob[] spawnSoldierSpots; // there should be 6 of them. Drag n drop.
    private AudioSource[] audioSources; // should be 2
    private AudioClip roarSound;
    private AudioClip crashSound;
    private GameObject boulder;

    protected override void Start()
    {
        base.Start();
        audioSources = GetComponents<AudioSource>();
        roarSound = Resources.Load<AudioClip>("Audio/longneck_roar");
        crashSound = Resources.Load<AudioClip>("Audio/crash");
        audioSources[0].clip = roarSound;
        audioSources[0].volume = 0.8f;
        audioSources[1].clip = crashSound;
        boulder = Resources.Load<GameObject>("Prefabs/Enemy/FallingBoulder");
    }

    private void Update()
    {
        CheckPhase();

        PerformAction();

        // only count time when previous action is finished
        if (finishedCurrentAction)
        {
            actionIimer += Time.deltaTime;
            attackTimer += Time.deltaTime;

            inVisionRange = Physics.CheckSphere(transform.position, visionRange, playerMask);
            inAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
            if (inVisionRange && inAttackRange)
            {
                state = State.idle;
                AttackMode();
            }
            else if (inVisionRange && !inAttackRange)
            {
                state = State.walking;
                ChasePlayer();
            }
            anim.SetInteger("state", (int)state);
        }

        if (actionIimer > timeBetweenAction && finishedAttack)
        {
            actionIimer = 0;
            finishedCurrentAction = false;
            playedAnimation = false;
            // pick a random action to perform this "turn"
            currentAction = (BossAction)Random.Range(0, 3);
        }
    }

    /// <summary>
    /// If Boss has less than 50% hp, and
    /// it not phase 2 yet, move to phase 2
    /// </summary>
    private void CheckPhase()
    {
        if (phase > 1) return;
        if (stat.health < stat.maxHealth / 2)
        {
            phase = 2;
            timeBetweenAction = 12f;
        }
    }

    private void AttackMode()
    {
        Vector3 lookVector = player.transform.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(lookVector, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 55 * Time.deltaTime);

        if (attackTimer > timeBetweenAttack && finishedCurrentAction)
        {
            finishedAttack = false;
            anim.SetTrigger("attack");
            attackTimer = 0;
        }
    }

    public override void Death()
    {
        Quaternion bloodDirection = Quaternion.LookRotation(transform.position - player.position, Vector3.up);
        Instantiate(DeathEffectPrefab, transform.position, bloodDirection);
        LevelEvents.levelEvents.EnemyDeathTriggerEnter(this.transform.position);
        PlayerStatHUD.psh.Victory();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // Destroy every enemies
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    /// <summary>
    /// For animation event.
    /// </summary>
    private void AttackDealDamage()
    {
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        StartCoroutine(cam.Shake(0.1f, 0.6f));
        Vector3 attackPoint = transform.position + new Vector3(0, 0.8f, 0) + transform.forward * 4.5f;
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint, attackRange * 0.35f, playerMask);
        audioSources[1].Play();
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

    private void TailSwipeDealDamage()
    {
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        Vector3 attackPoint = transform.position + new Vector3(0, 0.8f, 0) + transform.forward * 3f;
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint, attackRange * 0.45f, playerMask);
        foreach (Collider hitObject in hitObjects)
        {
            // Check enemy
            PlayerController player = hitObject.GetComponent<PlayerController>();
            if (player)
            {
                StartCoroutine(cam.Shake(0.1f, 0.6f));
                player.Damaged((int)(stat.damage * 0.5f), transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 attackPoint = transform.position + new Vector3(0, 0.8f, 0) + transform.forward * 4.5f;
        Gizmos.DrawWireSphere(attackPoint, attackRange * 0.35f);
        Vector3 attackPoint2 = transform.position + new Vector3(0, 0.8f, 0) + transform.forward * 3f;
        Gizmos.DrawWireSphere(attackPoint2, attackRange * 0.45f);
    }

    /// <summary>
    /// Boss using his special skill
    /// </summary>
    private void PerformAction()
    {
        if (playedAnimation || finishedCurrentAction || !finishedAttack) return;

        state = State.idle;
        anim.SetInteger("state", (int)state);

        if (currentAction == BossAction.tailswipe)
        {
            anim.SetTrigger("tailswipe");
        }
        if (currentAction == BossAction.createBoulder)
        {
            // Shake screen and stun player
            CreateBoulder();
            anim.SetTrigger("roar");
        }
        if (currentAction == BossAction.summonMinion)
        {
            // Not stun player, summon soldiers instead
            Summon();
            anim.SetTrigger("roar");
        }
        playedAnimation = true;
    }

    private void Summon()
    {
        CameraFollow cam = Camera.main.transform.GetComponent<CameraFollow>();
        StartCoroutine(cam.Shake(0.1f, 1f));
        audioSources[0].Play();
        int ranNum = Random.Range(0, spawnSoldierSpots.Length);
        spawnSoldierSpots[ranNum].Spawn(1);
    }

    private void CreateBoulder()
    {
        // Stun part
        CameraFollow cam = Camera.main.transform.GetComponent<CameraFollow>();
        StartCoroutine(cam.Shake(0.15f, 1f));
        audioSources[0].Play();
        PlayerController pc = player.GetComponent<PlayerController>();
        StartCoroutine(pc.Stunned());

        // Boulder part

        Instantiate(boulder, player.position + new Vector3(Random.Range(-10, 10), 35, Random.Range(-10, 10)), Quaternion.identity);
        Instantiate(boulder, player.position + new Vector3(Random.Range(-10, 10), 25, Random.Range(-10, 10)), Quaternion.identity);
        Instantiate(boulder, player.position + new Vector3(0, 15, 0), Quaternion.identity);
    }

    /// <summary>
    /// For animation event.
    /// </summary>
    public void FinishedAction()
    {
        finishedCurrentAction = true;
    }

    /// <summary>
    /// For animation event.
    /// </summary>
    public void FinishedAttack()
    {
        finishedAttack = true;
    }
}