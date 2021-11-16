using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    /* Binds */
    private static KeyCode pickUpBind = KeyCode.E;
    private static KeyCode dropBind = KeyCode.G;
    private static KeyCode inventoryBind = KeyCode.Tab;
    private static KeyCode eatBind = KeyCode.C;
    private static KeyCode itemSwitchBind = KeyCode.Q;
    private static KeyCode attackBind = KeyCode.Mouse0;
    private static KeyCode guardBind = KeyCode.Mouse1;
    private static KeyCode dashBind = KeyCode.Space;
    private const KeyCode projectileThrowBind = KeyCode.F;

    /* Components variables */
    private CharacterController controller;
    public Animator anim;
    private TrailRenderer dashTrail;
    public PlayerAudio playerAudio;
    public LayerMask groundLayer;
    public LayerMask hittableLayers;
    public LayerMask parryLayer;
    public ParticleSystem swordTrailPS;
    public ParticleSystem.EmissionModule swordTrail;
    public ProjectileController projectilePrefab;
    private GameObject sparklePrefab;
    private GameObject bloodPrefab;
    private CameraFollow cameraFollow;

    // Finite State Machine
    private enum State { idle, running, attack, guard, stunned, dead }

    [SerializeField] private State state = State.idle;

    /* Movement */
    private Vector3 moveDirection;
    public Vector3 velocity;
    private bool isGrounded;
    private bool isAttacking = false;
    public bool isGuarding = false;
    private float stunTime = 0.3f;
    private float iFrameTime = 0.5f;
    private bool isInIFrame = false;

    /* Others */
    private bool justThrowing = false;
    private bool comboPossible = false;
    private bool canCancelAnimation = false;
    public static bool isInInventory = false;
    [SerializeField] private int comboStep = 0;
    public float attackRange = 1.5f;
    public float attackPositionMul = 1.5f;
    public float knockbackPower = 3f;
    private float dashTimer;
    private float timeBetweenDash = 1f;
    private float projectileTimer;
    private float timeBetweenProjectile = 1.1f;

    // Inventory
    public InventoryController inventory;

    private InventoryUI inventoryUI;

    public static float maxPickupDistanceSqr = 14.0f;

    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        swordTrailPS = transform.Find("Weapon").GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
        if (swordTrailPS)
        {
            swordTrail = swordTrailPS.emission;
            swordTrail.enabled = false;
        }
        sparklePrefab = Resources.Load<GameObject>("Prefabs/Sparkle");
        bloodPrefab = Resources.Load<GameObject>("Prefabs/BloodEffect");
        dashTrail = transform.Find("DinoBody").GetComponent<TrailRenderer>();
        dashTrail.emitting = false;
        dashTimer = Time.time;
        projectileTimer = Time.time;
        playerAudio = transform.GetComponent<PlayerAudio>();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inventory = InventoryController.inventoryController;
        inventoryUI = GameObject.Find("InventoryCanvas").GetComponent<InventoryUI>();
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        // Load equipment from inventory
        LevelEvents.levelEvents.OnHandWeaponChangeTriggerEnter(inventory.onHand, true);
        LevelEvents.levelEvents.OffHandWeaponChangeTriggerEnter(inventory.offHand, true);
        LevelEvents.levelEvents.InventoryChangeTriggerEnter();
        if (inventory.projectile)
        {
            projectilePrefab = inventory.projectile.throwPrefab;
        }
        else
        {
            projectilePrefab = null;
        }

        anim.SetFloat("atkSpeedMul", PlayerStatHUD.psh.attackSpeed);
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == State.stunned || state == State.dead ||
            PauseMenu.isPaused || PlayerStatHUD.psh.isVictory) return;

        // Another way to close inventory
        if (Input.GetKeyDown(KeyCode.Escape) && isInInventory)
        {
            isInInventory = false;
            inventoryUI.ShowUI(false);
            Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(inventoryBind))
        {
            if (isInInventory)
            {
                isInInventory = false;
                inventoryUI.ShowUI(false);
                Time.timeScale = 1f;
            }
            else if (!PauseMenu.isPaused)
            {
                isInInventory = true;
                inventoryUI.ShowUI(true);
                Time.timeScale = 0f;
            }
        }

        if (isAttacking)
        {
            // Combo attack
            if (Input.GetKeyDown(attackBind))
            {
                if (comboPossible)
                {
                    comboPossible = false;
                    comboStep += 1;
                }
            }

            // Cancel to guard/parry
            if (Input.GetKeyDown(guardBind) && canCancelAnimation)
            {
                LookAtMouse();
                CheckParry();
                FinishedAttack();
            }

            // Cancel to dash
            if (Input.GetKeyDown(dashBind) && canCancelAnimation)
            {
                moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
                Dash();
                FinishedAttack();
            }
            return;
        }

        moveDirection = Vector3.zero;
        if (!isGuarding)
        {
            // Moving player
            moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            controller.Move(moveDirection * PlayerStatHUD.psh.moveSpeed * Time.deltaTime);
            // Rotate player direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, PlayerStatHUD.psh.rotateSpeed * Time.deltaTime);
            }
        }

        // Gravity
        // Reset y velocity to 0 when you on the ground
        isGrounded = Physics.CheckSphere(transform.position + new Vector3(0, 0.05f, 0), 0.1f, groundLayer, QueryTriggerInteraction.Ignore);
        if (isGrounded)
            velocity.y = 0f;
        // Increase velocity as you falling down
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Dash
        if (Input.GetKeyDown(dashBind))
        {
            Dash();
        }

        // Guard n Parry
        if (Input.GetKeyDown(guardBind))
        {
            LookAtMouse();
            CheckParry();
        }
        if (Input.GetKey(guardBind))
        {
            isGuarding = true;
        }
        else
        {
            isGuarding = false;
        }

        // Attack
        if (Input.GetKeyDown(attackBind) && !isGuarding)
        {
            LookAtMouse();
            isAttacking = true;
            anim.Play("PlayerDino_Combo1");
            comboStep = 1;
            if (swordTrailPS)
                swordTrail.enabled = true;
            // The actual deal damage function will be called in animation for better precision
        }

        // Throw projectiles
        if (Input.GetKeyDown(projectileThrowBind) && !isGuarding && projectilePrefab)
        {
            LookAtMouse();
            if (Time.time > projectileTimer + timeBetweenProjectile)
            {
                projectileTimer = Time.time;
                // This considered as an attack, and allow to chain combo from here
                isAttacking = true;
                justThrowing = true;
                comboStep = 1;
                anim.Play("PlayerDino_Throw");
                if (swordTrailPS)
                    swordTrail.enabled = true;
            }
        }

        // Pick up by click
        if (Input.GetKeyDown(pickUpBind))
        {
            GameObject objectHit = GetObjectClicked();
            if (objectHit != null && objectHit != gameObject && objectHit.CompareTag("Pickupable"))
            {
                Vector3 difference = transform.position - objectHit.transform.position;
                if (difference.sqrMagnitude <= maxPickupDistanceSqr)
                {
                    PickUp(objectHit, true);
                }
            }
        }

        //Eat consumeable
        if (Input.GetKeyDown(eatBind))
        {
            if (!(PlayerStatHUD.psh.GetHealth() == PlayerStatHUD.psh.GetMaxHealth()))
            {
                int healVal = inventory.ConsumeConsumable();
                PlayerStatHUD.psh.UpdateCurrentHealth(healVal);
            }
        }

        //Switch items
        if (Input.GetKeyDown(itemSwitchBind))
        {
            inventory.SwitchOnhand();
            inventoryUI.UpdateUI();
            if (inventory.onHand == null)
            {
                PlayerStatHUD.psh.UpdateWeapon(0, 1f);
            }
            else
            {
                PlayerStatHUD.psh.UpdateWeapon(inventory.onHand.GetDamageBonus(), inventory.onHand.attackSpeed);
            }
        }

        //Drop onhand item
        if (Input.GetKeyDown(dropBind))
        {
            if (inventory.onHand)
            {
                Drop(inventory.DropOnhand());
                PlayerStatHUD.psh.UpdateWeapon(0, 1f);
            }
        }

        //Update PlayerStatHUD if there was an equipemnt change

        // Update movement animation
        AnimationState();
        anim.SetInteger("state", (int)state);
        //anim.SetFloat("runSpeedMul", PlayerStatHUD.psh.moveSpeed / 7f);
    }

    /// <summary>
    /// A funciton used in AnimationEvent.
    /// </summary>
    private void ThrowProjectile()
    {
        //  playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.throwing);
        ProjectileController projectile = Instantiate(projectilePrefab, transform.position + transform.forward + new Vector3(0, 1, 0), Quaternion.identity);
        projectile.damage = PlayerStatHUD.psh.projectileDamage;
        projectile.transform.rotation = transform.rotation;
        projectile.transform.Rotate(0, 90, 0);
        projectile.owner = this.gameObject;
    }

    /// <summary>
    /// A function used in AnimationEvent. It's used in the frame where the weapon hit enemies and deal damage
    /// </summary>
    private void AttackDealDamage(int currentComboStep)
    {
        // Calculate damage
        int finalDamage = PlayerStatHUD.psh.damage;
        if (currentComboStep == 2)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * 1.1f);
        }
        if (currentComboStep == 3)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * 1.25f);
        }
        if (currentComboStep == 4)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * 1.75f);
        }

        // Move the player slightly ahead to keep track with enemies being knocked back
        float extraMove = 0.4f;
        if (currentComboStep >= 3) extraMove += 0.4f;

        // Shake the screen on strong attack (the 4th attack)
        if (currentComboStep > 3)
        {
            StartCoroutine(cameraFollow.Shake(0.2f, 0.1f));
        }

        if (justThrowing)
        {
            extraMove += 2f;
            justThrowing = false;
        }

        bool hitEnemies = false;
        // Detect enemies and breakable objects in range of attack
        Vector3 attackPoint = transform.position + new Vector3(0, 0.8f, 0) + transform.forward * attackPositionMul;
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint, attackRange, hittableLayers);

        // Damage them
        // In addition, calculate stuff to improve combo accuracy
        Vector3 enemyDirection = Vector3.zero;
        float enemyAngle;
        float minEnemyAngle = float.MaxValue;
        float minEnemyDistance = float.MaxValue;

        foreach (Collider hitObject in hitObjects)
        {
            // Check enemy
            IEnemy enemy = hitObject.GetComponent<IEnemy>();
            if (enemy)
            {
                hitEnemies = true;
                enemy.Damaged(finalDamage, knockbackPower, transform.position);
                PlayerStatHUD.psh.damageDealt += finalDamage;
                enemyAngle = Vector3.Angle(enemy.transform.position - transform.position, transform.forward);
                if (enemyAngle < minEnemyAngle)
                {
                    minEnemyAngle = enemyAngle;
                    enemyDirection = enemy.transform.position - transform.position;
                    minEnemyDistance = enemyDirection.magnitude;
                }
            }
        }
        // Look at closest enemy (if available) to improve combo accuracy
        if (hitEnemies)
        {
            playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.swordHitBody);
            Quaternion toRotation = Quaternion.LookRotation(enemyDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360);
            StartCoroutine(HitFeedback());
        }
        else
        {
            playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.swordSwing);
        }

        // If enemy is kinda far away, extra move toward them
        if (minEnemyDistance > attackRange * 2)
            controller.Move(transform.forward * extraMove);
        canCancelAnimation = true;
    }

    /// <summary>
    /// A function used in AnimationEvent. Start this frame, the player can queue
    /// input for the next combo attack.
    /// </summary>
    private void ComboPossible()
    {
        comboPossible = true;
    }

    /// <summary>
    /// A function used in AnimationEvent. The player will perform the combo attack if
    /// input are received.
    /// </summary>
    private void ComboAttack()
    {
        // Since these animation uncheck HasExitTime, it wont be repeated so if the comboStep
        // hasn't increased (or decreased if there is a bug), player won't attack. Thus, no need
        // for a bool to check if player clicked button during the last attack animation.
        canCancelAnimation = false;
        if (comboStep == 2)
            anim.Play("PlayerDino_Combo2");
        if (comboStep == 3)
            anim.Play("PlayerDino_Combo3");
        if (comboStep == 4)
            anim.Play("PlayerDino_Combo4");
    }

    /// <summary>
    /// A function used in AnimationEvent. The player stopped his combo. A new combo will start if
    /// the player attack again.
    /// </summary>
    private void FinishedAttack()
    {
        if (swordTrailPS)
            swordTrail.enabled = false;
        isAttacking = false;
        justThrowing = false;
        comboPossible = false;
        comboStep = 0;
        canCancelAnimation = false;
    }

    private void Dash()
    {
        if (Time.time > dashTimer + timeBetweenDash)
        {
            // Use throwing sound for dash sound too, temporary fix
            playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.throwing);
            dashTimer = Time.time;

            if (moveDirection != Vector3.zero)
                StartCoroutine(Dash(moveDirection));
            else
                StartCoroutine(Dash(transform.forward));
        }
    }

    private void CheckParry()
    {
        Vector3 attackPoint = transform.position + new Vector3(0, 0.8f, 0) + transform.forward * attackPositionMul;
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint, attackRange * 0.8f, parryLayer);
        // Parry something
        foreach (Collider hitObject in hitObjects)
        {
            ProjectileController projectile = hitObject.GetComponent<ProjectileController>();
            if (projectile)
            {
                playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.parry);
                Instantiate(sparklePrefab, projectile.transform.position, Quaternion.identity);
                anim.Play("PlayerDino_Parry");
                StartCoroutine(PlayerStatHUD.psh.SlowDownTime(0.05f, 0.2f));
                projectile.owner = this.gameObject;
                projectile.damage *= projectile.parryMultiplier;
                projectile.velocity = Vector3.zero;
                projectile.GetComponent<Rigidbody>().AddForce(transform.forward * 60, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator Dash(Vector3 moveDirection)
    {
        dashTrail.emitting = true;

        float startTime = Time.time;

        while (Time.time < startTime + 0.10f)
        {
            controller.Move(moveDirection * 40 * Time.deltaTime);
            yield return null;
        }
        dashTrail.emitting = false;
    }

    /// <summary>
    /// Make the player attack has more weight and slow the weapon after hitting enemies.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HitFeedback()
    {
        float tmp = anim.speed;
        anim.speed = 0.1f;
        yield return new WaitForSeconds(0.15f);
        anim.speed = tmp;
    }

    public IEnumerator InvincibleFrame()
    {
        isInIFrame = true;
        yield return new WaitForSeconds(iFrameTime);
        isInIFrame = false;
    }

    public IEnumerator Stunned()
    {
        FinishedAttack();
        isGuarding = false;
        state = State.stunned;
        anim.SetInteger("state", (int)state);
        anim.Play("PlayerDino_Hurt");
        yield return new WaitForSeconds(stunTime);
        state = State.idle;
        anim.SetInteger("state", (int)state);
    }

    /// <summary>
    /// Make the player rotate and look at the mouse position
    /// </summary>
    private void LookAtMouse()
    {
        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 direction = target - transform.position;
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    private void PickUp(GameObject _object, bool isClick)
    {
        ItemScript gameItem = _object.GetComponent<ItemScript>();
        bool slotFull = inventory.IsFullSlot(gameItem);
        if (!isClick && slotFull)
        {
            return;
        }
        else if (!isClick && !slotFull)
        {
            inventory.AddItem(gameItem);
            playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.equip);
            gameItem.HideTooltip();
            UpdatePlayerStatHUD(gameItem);
        }
        else if (isClick && !slotFull)
        {
            inventory.AddItem(gameItem);
            playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.equip);
            gameItem.HideTooltip();
            UpdatePlayerStatHUD(gameItem);
        }
        else
        {
            ItemScript itemToDrop = inventory.AddItem(gameItem);
            gameItem.HideTooltip();
            UpdatePlayerStatHUD(gameItem);
            Drop(itemToDrop);
        }
        // Make item not DestroyOnSceneLoad and carry across scene
        _object.transform.parent = InventoryController.inventoryController.transform;
        _object.SetActive(false);
    }

    private void Drop(ItemScript itemToDrop)
    {
        playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.equip);
        Vector3 dropBias = new Vector3(0, 0.8f, 0);
        itemToDrop.gameObject.transform.position = transform.position + transform.forward + dropBias;
        itemToDrop.gameObject.SetActive(true);
        // Make item DestroyOnSceneLoad again
        itemToDrop.UpdatePlayerTransform(this.transform);
        itemToDrop.transform.parent = null;
        SceneManager.MoveGameObjectToScene(itemToDrop.gameObject, SceneManager.GetActiveScene());
    }

    /// <summary>
    /// Control the state of the Animator
    /// </summary>
    private void AnimationState()
    {
        // stunned and dead state are activate by different code
        if (isGuarding)
        {
            state = State.guard;
        }
        else if (isAttacking)
        {
            state = State.attack;
        }
        else if (moveDirection != Vector3.zero)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
    }

    /// <summary>
    /// The player got damaged. Negative = heal instead.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="damageOrigin">Position where damage come from</param>
    public void Damaged(int amount, Vector3 damageOrigin)
    {
        // if still in i-frame or death then nothing
        if (isInIFrame || state == State.dead) return;

        Vector3 vectorPlayerToDamage = damageOrigin - transform.position;
        Vector3 pushedVector = -vectorPlayerToDamage.normalized * 0.25f;
        pushedVector = new Vector3(pushedVector.x, 0, pushedVector.z);  // Prevent player pushed upward or downward
        controller.Move(pushedVector);
        float guardAngle = Vector3.Angle(transform.forward, vectorPlayerToDamage);
        if (isGuarding && guardAngle <= 60f)
        {
            Instantiate(sparklePrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
            playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.swordBlock);
        }
        else
        {
            Instantiate(bloodPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
            playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.hit);
            PlayerStatHUD.psh.UpdateCurrentHealth(-amount);
            if (PlayerStatHUD.psh.CheckIfDeath())
            {
                Death();
            }
            else
            {
                StartCoroutine(InvincibleFrame());
                StartCoroutine(Stunned());
            }
        }
    }

    public void Death()
    {
        state = State.dead;
        anim.SetInteger("state", (int)state);
        anim.Play("PlayerDino_Death");
    }

    // Draw a sphere to visualize the player attack range
    private void OnDrawGizmosSelected()
    {
        Vector3 attackPoint = transform.position + new Vector3(0, 0.8f, 0) + transform.forward * attackPositionMul;
        Gizmos.DrawWireSphere(attackPoint, attackRange);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.05f, 0), 0.1f);
    }

    private GameObject GetObjectClicked()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out raycastHit, 100f);
        if (raycastHit.transform != null) // If something is clicked
        {
            return raycastHit.transform.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Update stats/variable on PlayerStatHUD end
    /// </summary>
    /// <param name="item"></param>
    private void UpdatePlayerStatHUD(ItemScript item)
    {
        if (item.type == ItemType.Consumable)
        {
            return;
        }
        if (item.type == ItemType.Weapon)
        {
            WeaponScript weaponScript = (WeaponScript)item;
            PlayerStatHUD.psh.UpdateWeapon(weaponScript.GetDamageBonus(), weaponScript.attackSpeed);
        }
        else if (item.type == ItemType.Armour)
        {
            ArmourScript armourScript = (ArmourScript)item;
            PlayerStatHUD.psh.UpdateProtection(armourScript.GetProtectionBonus());
        }
        else if (item.type == ItemType.Projectile)
        {
            // Formula by Lam, different from Sebastian's formula
            ProjectileScript projectileScript = (ProjectileScript)item;
            PlayerStatHUD.psh.UpdateProjectile(projectileScript.GetFinalDamage());
            projectilePrefab = projectileScript.throwPrefab;
        }
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    var item = collision.collider.GetComponent<ItemScript>();
    //    if (item)
    //    {
    //        PickUp(collision.collider.gameObject, false);
    //    }
    //}

    private void OnApplicationQuit()
    {
        if (inventory)
            inventory.ClearInventory();
        else
            Debug.Log("Forgot to set player inventory variable. Can be ignored if in testing environment.");
    }
}