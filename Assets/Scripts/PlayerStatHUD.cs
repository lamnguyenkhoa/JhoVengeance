using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStatHUD : MonoBehaviour
{
    /* Defaults, can be improved with stat upgrade */
    public static int DEFAULT_MAX_HEALTH = 100;
    public static float DEFAULT_MS = 4f;
    public static int DEFAULT_DAMAGE = 10;
    public static int DEFAULT_PROJECTILE_DAMAGE = 0;
    public static float DEFAULT_PROTECTION = 0f;
    public static float DEFAULT_ATTACK_SPEED = 1f;

    /* Stat var, can be improved with equipment */
    [SerializeField] private int health = DEFAULT_MAX_HEALTH;
    [SerializeField] private int maxHealth = DEFAULT_MAX_HEALTH;
    public float moveSpeed = DEFAULT_MS;
    public int damage = DEFAULT_DAMAGE;
    public int projectileDamage = DEFAULT_PROJECTILE_DAMAGE;
    public float protection = DEFAULT_PROTECTION;
    public float attackSpeed = DEFAULT_ATTACK_SPEED;

    /* HUD var */
    public Text hpText;
    private Image flashEffect;
    private Image lowHealthEffect;
    private GameObject deathScreen;
    private GameObject victoryScreen;
    private GameObject healthBar;
    private float BAR_WIDTH;
    private Slider heathSlider;
    private Transform damageBar;

    /* Other */
    private GameObject sparkleLight;
    public GameObject player;
    public PlayerController playerController;
    public int xpCollected = 0;
    public int damageDealt = 0;
    public int damageTaken = 0;
    public float rotateSpeed = 900f;
    public bool justDied = false;
    public bool isVictory = false;
    public bool disablePause = false;
    public float startTime;

    // Singleton
    public static PlayerStatHUD psh;

    private void Awake()
    {
        if (!psh)
        {
            psh = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sparkleLight = Resources.Load<GameObject>("Prefabs/SparkleLight");
        flashEffect = transform.GetChild(0).Find("FlashEffect").GetComponent<Image>();
        flashEffect.color = new Color(1, 1, 1, 0);
        lowHealthEffect = transform.GetChild(0).Find("LowHealthEffect").GetComponent<Image>();
        deathScreen = transform.GetChild(0).Find("DeathScreen").gameObject;
        deathScreen.SetActive(false);
        victoryScreen = transform.GetChild(0).Find("VictoryScreen").gameObject;
        victoryScreen.SetActive(false);
        healthBar = transform.GetChild(0).Find("HealthBar").gameObject;
        BAR_WIDTH = healthBar.GetComponent<RectTransform>().rect.width;
        damageBar = healthBar.transform.Find("DamageBar");
        heathSlider = healthBar.GetComponent<Slider>();
        UpdateAllStatForHUD();
        if (SceneManager.GetActiveScene().name == "ZenGarden")
        {
            healthBar.SetActive(false);
        }
        startTime = Time.time;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.Find("PlayerDino");
        if (player)
            playerController = player.GetComponent<PlayerController>();
    }

    /// <summary>
    /// Change the player health. A positive amount mean heal, negative mean damaged.
    /// </summary>
    /// <param name="amount"></param>
    public void UpdateCurrentHealth(int amount)
    {
        //If damage
        if (amount < 0)
        {
            // If it damage, then reduce it by armor
            // Ex: protection = 12 mean you only take 100-12=88% of damage
            Mathf.Clamp(protection, 0, 100);
            amount = (int)(amount * (1 - protection / 100));
            // Since damage is negative number, we have to -= amount instead of +=
            damageTaken -= amount;
        }
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        // Update health bar
        hpText.text = "HP: " + health + "/" + maxHealth;
        float hpPercent = (float)health / maxHealth;
        heathSlider.value = hpPercent;

        // Damaged bar effect
        if (amount < 0)
        {
            float lostPercent = (float)-amount / maxHealth;
            Transform newDamagedBar = Instantiate(damageBar, healthBar.transform);
            newDamagedBar.gameObject.SetActive(true);
            newDamagedBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(hpPercent * BAR_WIDTH, damageBar.GetComponent<RectTransform>().anchoredPosition.y);
            newDamagedBar.GetComponent<Slider>().value = lostPercent;
        }

        hpText.transform.SetAsLastSibling();

        // Red screen effect
        if (hpPercent <= 0.3f)
        {
            lowHealthEffect.color = new Color(1f, 0.5f, 0.5f, 0.2f - hpPercent); ;
        }
        else
        {
            lowHealthEffect.color = new Color(0, 0, 0, 0);
        }

        if (health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        disablePause = true;
        deathScreen.transform.Find("Exp").GetComponent<Text>().text = "Exp: " + xpCollected;
        deathScreen.transform.Find("StageClear").GetComponent<Text>().text = "Stage cleared: " + LevelSystem.levelSystem.stageCleared;
        deathScreen.transform.Find("EnemyKill").GetComponent<Text>().text = "Enemies killed: " + LevelSystem.levelSystem.enemiesKilled;
        deathScreen.transform.Find("DamageDealt").GetComponent<Text>().text = "Damage dealt: " + damageDealt;
        deathScreen.transform.Find("DamageTaken").GetComponent<Text>().text = "Damage taken: " + damageTaken;
        float timeTaken = Time.time - startTime;
        int minutes = Mathf.FloorToInt(timeTaken / 60F);
        int seconds = Mathf.FloorToInt(timeTaken - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        deathScreen.transform.Find("Time").GetComponent<Text>().text = "Time:  " + niceTime;
        deathScreen.SetActive(true);
    }

    public void Victory()
    {
        disablePause = true;
        isVictory = true;
        MusicManager.musicManager.PlayVictoryTheme();
        victoryScreen.transform.Find("Exp").GetComponent<Text>().text = "Exp: " + xpCollected;
        victoryScreen.transform.Find("StageClear").GetComponent<Text>().text = "Stage cleared: " + (LevelSystem.levelSystem.stageCleared + 1);
        victoryScreen.transform.Find("EnemyKill").GetComponent<Text>().text = "Enemies killed: " + LevelSystem.levelSystem.enemiesKilled;
        victoryScreen.transform.Find("DamageDealt").GetComponent<Text>().text = "Damage dealt: " + damageDealt;
        victoryScreen.transform.Find("DamageTaken").GetComponent<Text>().text = "Damage taken: " + damageTaken;
        float timeTaken = Time.time - startTime;
        int minutes = Mathf.FloorToInt(timeTaken / 60F);
        int seconds = Mathf.FloorToInt(timeTaken - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        victoryScreen.transform.Find("Time").GetComponent<Text>().text = "Time:  " + niceTime;
        victoryScreen.SetActive(true);
    }

    public bool CheckIfDeath()
    {
        if (health <= 0) return true;
        else return false;
    }

    /// <summary>
    /// Update all stat for hud display correct value.
    /// </summary>
    public void UpdateAllStatForHUD()
    {
        UpdateCurrentHealth(0);
    }

    public void UpdateWeapon(int newBonusDamage, float newBonusAttackSpeed)
    {
        this.damage = DEFAULT_DAMAGE + newBonusDamage;
        this.attackSpeed = DEFAULT_ATTACK_SPEED * newBonusAttackSpeed;
        playerController.anim.SetFloat("atkSpeedMul", attackSpeed);
    }

    public void UpdateProtection(float newBonusProtection)
    {
        this.protection = DEFAULT_PROTECTION + newBonusProtection;
    }

    public void UpdateProjectile(int newBonusDamage)
    {
        this.projectileDamage = DEFAULT_PROJECTILE_DAMAGE + newBonusDamage;
    }

    public IEnumerator SlowDownTime(float scale, float duration)
    {
        //TODO: The sparkle effect should not be here. It's just temporary quick fix
        GameObject tmp = Instantiate(sparkleLight, player.transform.position + new Vector3(0, 1, 0) + player.transform.forward, Quaternion.identity);
        Time.timeScale = scale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        Destroy(tmp);
    }

    public void TurnOnFlash()
    {
        flashEffect.color = new Color(1, 1, 1, 0.18f);
    }

    public void TurnOffFlash()
    {
        flashEffect.color = new Color(1, 1, 1, 0);
    }

    /* Getter, Setter */

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// This function is run when the Respawn button is clicked. It will reset
    /// some stat before send player to ZenGarden. Xp are kept.
    /// </summary>
    public void ResetToGoZen()
    {
        disablePause = false;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(LevelEvents.levelEvents.gameObject, currentScene);
        SceneManager.MoveGameObjectToScene(LevelSystem.levelSystem.gameObject, currentScene);
        SceneManager.MoveGameObjectToScene(InventoryController.inventoryController.gameObject, currentScene);
        maxHealth = DEFAULT_MAX_HEALTH;
        health = maxHealth;
        damage = DEFAULT_DAMAGE;
        moveSpeed = DEFAULT_MS;
        projectileDamage = DEFAULT_PROJECTILE_DAMAGE;
        protection = DEFAULT_PROTECTION;
        UpdateCurrentHealth(0);
        victoryScreen.SetActive(false);
        deathScreen.SetActive(false);
        healthBar.SetActive(false);
        TurnOffFlash();
        justDied = true;
        damageDealt = 0;
        damageTaken = 0;
    }

    public void ResetToGoDungeon()
    {
        isVictory = false;
        justDied = false;
        healthBar.SetActive(true);
        startTime = Time.time;
    }

    /// <summary>
    /// Use this if you want to set health without take into account
    /// protection or damage dealt.
    /// </summary>
    /// <param name="amount"></param>
    public void SetHealth(int amount)
    {
        health = amount;
        UpdateCurrentHealth(0);
    }
}