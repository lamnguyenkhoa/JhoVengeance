using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IEnemy : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public int damage;
    public int xp;
    public bool isDead = false;
    public float disappearSpeed = 3f;
    private GameObject DamagePopupPrefab;
    private IEnemyAI ai;
    private Rigidbody rb;
    private ParticleSystem ps;

    private Transform statusCanvas;
    protected GameObject healthBar;
    protected Slider healthSlider;
    public bool isStunned = false;
    public bool stunImmune = false;
    private Coroutine stunnedCoroutine = null;
    private GameObject bloodEffect;

    public float stunnedTime = 0.5f;

    protected virtual void Start()
    {
        // Material is actually part of MeshRenderer component
        rb = transform.GetComponent<Rigidbody>();
        ps = transform.GetComponent<ParticleSystem>();
        ai = transform.GetComponent<IEnemyAI>();
        DamagePopupPrefab = Resources.Load<GameObject>("Prefabs/DamagePopup");
        statusCanvas = transform.Find("EnemyStatusCanvas");
        if (statusCanvas)
        {
            healthBar = statusCanvas.GetChild(0).gameObject;
            healthSlider = healthBar.GetComponent<Slider>();
            healthBar.SetActive(false);
        }
        else
        {
            // Will be asigned in children class
            healthBar = null;
            healthSlider = null;
        }
        bloodEffect = Resources.Load<GameObject>("Prefabs/BloodEffect");
    }

    protected virtual void Update()
    {
        // Dead, start to disappear
        if (isDead)
        {
            ai.Death();
        }
    }

    /// <summary>
    /// This function is used when enemy got damaged. They will also be push away from
    /// the player a bit if they are not stun immune. Negative damage = healing.
    /// </summary>
    /// <param name="amount"> How much damage the enemy take. If it's negative, enemy
    /// will heal instead.</param>
    /// <param name="knockbackPower">How much they are pushed away.</param>
    /// <param name="playerPos">The player position to calculate the push direction.</param>
    public virtual void Damaged(int amount, float knockbackPower, Vector3 playerPos)
    {
        // If already dead, no more thing happen
        if (isDead) return;

        // If it is damaged, detect player
        if (amount > 0) ai.visionRange = 10000;

        // Take damage
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health == 0)
        {
            PlayerStatHUD.psh.xpCollected += xp;
            isDead = true;
        }

        if (health == maxHealth)
        {
            healthBar.SetActive(false);
        }
        else
        {
            healthBar.SetActive(true);
            healthSlider.value = (float)health / maxHealth;
        }

        // Damage popup
        ShowDamagePopup(amount);

        // Play particle effect (blood splatter)
        Instantiate(bloodEffect, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);

        // Stun and apply knockback
        if (!stunImmune)
        {
            if (stunnedCoroutine != null)
            {
                StopCoroutine(stunnedCoroutine);
            }
            stunnedCoroutine = StartCoroutine(Stunned(knockbackPower, playerPos));
        }
    }

    /// <summary>
    /// Create a damage popup above the enemy to indicate how much damage you deal
    /// </summary>
    /// <param name="amount"></param>
    public void ShowDamagePopup(int amount)
    {
        GameObject damagePopup = Instantiate(DamagePopupPrefab, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        damagePopup.GetComponent<DamagePopup>().Setup(amount);
    }

    /// <summary>
    /// This function set a timer keep in check how long an enemy would be stun.
    /// The enemies only are stunned and pushed if they are not stun immune
    /// </summary>
    /// <param name="knockbackPower"></param>
    /// <param name="playerPos"></param>
    /// <returns></returns>
    private IEnumerator Stunned(float knockbackPower, Vector3 playerPos)
    {
        // Push enemy away slightly
        Animator anim = transform.GetComponent<Animator>();
        isStunned = true;
        anim.SetBool("stunned", true);
        anim.Play("Stunned");
        rb.isKinematic = false;
        Vector3 forceDirection = (transform.position - playerPos).normalized;
        rb.AddForce(forceDirection * knockbackPower, ForceMode.Impulse);
        yield return new WaitForSeconds(stunnedTime);
        rb.isKinematic = true;
        isStunned = false;
        anim.SetBool("stunned", false);
    }
}