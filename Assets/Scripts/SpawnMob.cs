using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMob : MonoBehaviour
{
    public int maxmimumNumberOfEnemies = 1;
    public bool spawnCharger = true;
    public bool spawnMelee = true;
    public bool spawnRanged = true;
    private int totalSpawns;
    private int mobToSpawn;
    private GameObject meleePrefab;
    private GameObject chargerPrefab;
    private GameObject rangedPrefab;
    private double randomLocationBuffer;
    private double randomLocationBuffer2;
    private Vector3 bufferlocation;
    private int amountSpawned;
    private static System.Random random = new System.Random();
    public bool notRunAsStart = false;

    // Start is called before the first frame update
    private void Awake()
    {
        PreventHanging();

        meleePrefab = Resources.Load<GameObject>("Prefabs/Enemy/MeleeSoldier");
        chargerPrefab = Resources.Load<GameObject>("Prefabs/Enemy/ChargeSoldier");
        rangedPrefab = Resources.Load<GameObject>("Prefabs/Enemy/RangedSoldier");
        if (notRunAsStart) return;

        totalSpawns = random.Next(1, maxmimumNumberOfEnemies);
        while (amountSpawned < totalSpawns)
        {
            mobToSpawn = random.Next(1, 4);
            randomLocationBuffer = -3 + random.NextDouble() * 6;
            randomLocationBuffer2 = -3 + random.NextDouble() * 6;
            bufferlocation = new Vector3((float)randomLocationBuffer, 1, (float)randomLocationBuffer2);
            if (mobToSpawn == 1 && spawnMelee)
            {
                GameObject mob = Instantiate(meleePrefab, transform.position + bufferlocation, Quaternion.identity);
                amountSpawned++;
            }
            else if (mobToSpawn == 2 && spawnCharger)
            {
                GameObject mob = Instantiate(chargerPrefab, transform.position + bufferlocation, Quaternion.identity);
                amountSpawned++;
            }
            else if (mobToSpawn == 3 && spawnRanged)
            {
                GameObject mob = Instantiate(rangedPrefab, transform.position + bufferlocation, Quaternion.identity);
                amountSpawned++;
            }
        }
    }

    /// <summary>
    /// Make sure the game doesn't lag when developers forget to set the spawn.
    /// </summary>
    private void PreventHanging()
    {
        if (!spawnCharger && !spawnMelee && !spawnRanged)
        {
            Debug.Log("YO you forgot to set the spawns");
            spawnMelee = true;
        }
    }

    /// <summary>
    /// Used for boss skills. Boss can summon soldiers for help.
    /// </summary>
    /// <param name="amount"></param>
    public void Spawn(int amount)
    {
        while (amountSpawned < amount)
        {
            mobToSpawn = random.Next(1, 4);
            randomLocationBuffer = -3 + random.NextDouble() * 6;
            randomLocationBuffer2 = -3 + random.NextDouble() * 6;
            bufferlocation = new Vector3((float)randomLocationBuffer, 1, (float)randomLocationBuffer2);
            if (mobToSpawn == 1 && spawnMelee)
            {
                GameObject mob = Instantiate(meleePrefab, transform.position + bufferlocation, Quaternion.identity);
                mob.GetComponent<IEnemyAI>().visionRange = 1000;
                amountSpawned++;
            }
            else if (mobToSpawn == 2 && spawnCharger)
            {
                GameObject mob = Instantiate(chargerPrefab, transform.position + bufferlocation, Quaternion.identity);
                mob.GetComponent<IEnemyAI>().visionRange = 1000;
                amountSpawned++;
            }
            else if (mobToSpawn == 3 && spawnRanged)
            {
                GameObject mob = Instantiate(rangedPrefab, transform.position + bufferlocation, Quaternion.identity);
                mob.GetComponent<IEnemyAI>().visionRange = 1000;
                amountSpawned++;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}