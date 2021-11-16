using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem levelSystem;
    [HideInInspector] public int enemiesKilled = 0;
    [HideInInspector] public int stageCleared = -1;
    private Vector3 spawnBias = new Vector3(0f, 0.5f, 0f);
    public int enemiesToKill;
    public float bonusRNG = 0f;
    public List<GameObject> allWeapons;
    public List<GameObject> allConsumables;
    public List<GameObject> allArmours;
    public List<GameObject> allProjectiles;
    private AudioSource lootDropSound;
    [Range(1, 20)] [SerializeField] private int invDropFactor;

    /* As we travel to higher levels, this variable is increased
     * and thus the loot drops are better
     */
    private float progressDifficulty = 0;

    private void Awake()
    {
        if (!levelSystem)
        {
            levelSystem = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        stageCleared += 1;
        //Do not increase progress if loading the peaceful scene
        if (scene.name != "ZenGarden")
        {
            //Progress should go up by 1/#levels-1 as one level is the zen garden
            //progressDifficulty += 0.1f;
            progressDifficulty += 1f / (SceneManager.sceneCountInBuildSettings - 1);
        }

        enemiesToKill = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemiesToKill == 0)
        {
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
            foreach (GameObject door in doors)
            {
                door.GetComponent<OpenDoorScript>().OpenTheDoor();
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        LevelEvents.levelEvents.onEnemyDeathTriggerEnter += OnEnemyDeath;
        lootDropSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnEnemyDeath(Vector3 enemyPos)
    {
        enemiesKilled += 1;
        // If killed all enemies on stage, open the door
        enemiesToKill -= 1;
        if (enemiesToKill <= 0)
        {
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
            foreach (GameObject door in doors)
            {
                door.GetComponent<OpenDoorScript>().OpenTheDoor();
            }
        }
        float random;
        //If on normal levels of health
        if (PlayerStatHUD.psh.GetHealth() > 30)
        {
            random = UnityEngine.Random.Range(0f, invDropFactor);
            Debug.Log("Roll " + random);
            // drop rate = 1/invDropFactor
            if ((random + bonusRNG) > (invDropFactor - 1))
            {
                ItemType itemGeneratedType = ItemType.Consumable;
                random = UnityEngine.Random.Range(0f, 10);
                // 4 in ten chance of weapon
                if (random < 4)
                {
                    bonusRNG = 0;
                    itemGeneratedType = ItemType.Weapon;
                }
                // 2 in ten chance of armour
                else if (random < 6)
                {
                    itemGeneratedType = ItemType.Armour;
                }
                // 3 in ten chance of projectile
                else if (random < 9)
                {
                    itemGeneratedType = ItemType.Projectile;
                }
                // 1 in ten chance of consumable
                GenerateItem(itemGeneratedType, enemyPos);
            }
            else
            {
                // If fail RNG, make it more likely next time
                bonusRNG += 0.1f;
            }
        }
        else
        {
            random = UnityEngine.Random.Range(0f, invDropFactor);
            /* You are 33% more likely to get an item drop if low, but the chance of dropping
             * armour or weaponry is much lower*/
            if ((random + bonusRNG) > (invDropFactor - (invDropFactor / 3)))
            {
                bonusRNG = 0;
                ItemType itemGeneratedType = ItemType.Consumable;
                random = UnityEngine.Random.Range(0f, 10);

                if (random < 1)
                {
                    itemGeneratedType = ItemType.Armour;
                }
                else if (random < 2)
                {
                    itemGeneratedType = ItemType.Weapon;
                }
                else if (random < 3)
                {
                    itemGeneratedType = ItemType.Projectile;
                }
                GenerateItem(itemGeneratedType, enemyPos);
            }
            else
            {
                bonusRNG += 0.1f;
            }
        }
    }

    /// <summary>
    /// Generates an item of itemtype type at the desired spawn position
    /// </summary>
    /// <param name="type"></param>
    /// <param name="spawnPosition"></param>
    private void GenerateItem(ItemType type, Vector3 spawnPosition)
    {
        lootDropSound.Play();
        if (type.Equals(ItemType.Weapon))
        {
            Debug.Log("spawning weapon");
            if (allWeapons.Count == 0) Debug.Log("Cant spawn. allWeapons count is 0");
            int randomInt = UnityEngine.Random.Range(0, allWeapons.Count);
            GameObject gameObject = allWeapons[randomInt];
            WeaponScript wS = gameObject.GetComponent<WeaponScript>();
            SetWeaponStats(wS);
            Instantiate(gameObject, spawnPosition + spawnBias, Quaternion.identity);
        }
        else if (type.Equals(ItemType.Armour))
        {
            Debug.Log("spawning armour");
            if (allArmours.Count == 0) Debug.Log("Cant spawn. allArmours count is 0");
            int randomInt = UnityEngine.Random.Range(0, allArmours.Count);
            GameObject gameObject = allArmours[randomInt];
            ArmourScript aS = gameObject.GetComponent<ArmourScript>();
            SetArmourStats(aS);
            Instantiate(gameObject, spawnPosition + spawnBias, Quaternion.identity);
        }
        else if (type.Equals(ItemType.Projectile))
        {
            Debug.Log("spawning projectile");
            if (allProjectiles.Count == 0) Debug.Log("Cant spawn. allProjectiles count is 0");
            int randomInt = UnityEngine.Random.Range(0, allProjectiles.Count);
            GameObject gameObject = allProjectiles[randomInt];
            ProjectileScript pS = gameObject.GetComponent<ProjectileScript>();
            SetProjectileStats(pS);
            Instantiate(gameObject, spawnPosition + spawnBias, Quaternion.identity);
        }
        else
        {
            Debug.Log("spawning consumeable");
            if (allConsumables.Count == 0) Debug.Log("Cant spawn. allConsumables count is 0");
            int randomInt = UnityEngine.Random.Range(0, allConsumables.Count);
            GameObject gameObject = allConsumables[randomInt];
            ConsumableScript cS = gameObject.GetComponent<ConsumableScript>();
            if (cS == null)
            {
                Debug.Log("ConsumScript is null");
            }
            else
            {
                SetConsumableStats(cS);
            }
            Instantiate(gameObject, spawnPosition + spawnBias, Quaternion.identity);
        }
    }

    private void SetWeaponStats(WeaponScript wS)
    {
        int defaultWeaponBonus = wS.GetBaseDamage();
        int maximumAdditon = (int)((wS.GetMaxWeaponDamage() - defaultWeaponBonus) * progressDifficulty);
        int addition = UnityEngine.Random.Range(WeaponScript.MIN_WEAPON_DAMAGE_BONUS, maximumAdditon + defaultWeaponBonus);
        wS.SetDamageBonus(addition);
        wS.RandomizeAttackSpeed();
    }

    private void SetArmourStats(ArmourScript aS)
    {
        float defaultProt = aS.GetDefaultArmourProtectionBonus();
        float maximumAddition = (ArmourScript.MAX_PROTECTION - defaultProt) * progressDifficulty;
        float addition = UnityEngine.Random.Range(ArmourScript.MIN_PROTECTION - defaultProt, maximumAddition);
        aS.SetProtectionBonus(defaultProt + addition);
    }

    private void SetConsumableStats(ConsumableScript cS)
    {
        int defaultHeal = cS.GetDefaultHealValue();
        int addition = UnityEngine.Random.Range(0, ConsumableScript.MAX_BONUS_VALUE);
        cS.SetHealValue(defaultHeal + addition);
    }

    // This formula is written by Lam, which is why it different from above formula
    /// <summary>
    /// Calculate a random bonus damage. Then calculate a determined damage from progress
    /// difficulty. These 2 will be added to base damage and the result will be final
    /// damage, which is used for calculating damage.
    /// </summary>
    /// <param name="pS"></param>
    private void SetProjectileStats(ProjectileScript pS)
    {
        int bonusRandomDam = Random.Range(ProjectileScript.MIN_PROJECTILE_DAMAGE_BONUS, ProjectileScript.MAX_PROJECTILE_DAMAGE_BONUS);
        int progressDam = (int)progressDifficulty * pS.damagePerProgressTick;
        pS.SetProjectileBonusDamage(bonusRandomDam + progressDam);
    }
}