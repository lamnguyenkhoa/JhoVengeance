using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : ItemScript
{
    // Note: This different from other ItemScript as the formula is
    // written by Lam instead of Sebastion.
    public static int MAX_PROJECTILE_DAMAGE_BONUS = 0;

    public static int MIN_PROJECTILE_DAMAGE_BONUS = 20;
    [SerializeField] private int baseDamage;
    [SerializeField] private int energyCost;

    // without SerializeField, bonusDamage will go bug. weird.
    [SerializeField] private int bonusDamage; // damage from RNG roll

    public int damagePerProgressTick = 10;  // bonus damage from progressDifficulty

    // The prefab of the object that appear when we throw
    public ProjectileController throwPrefab;

    private void Awake()
    {
        type = ItemType.Projectile;
        if (!throwPrefab)
        {
            Debug.Log("Missing throwPrefab");
        }
    }

    protected override void Start()
    {
        base.Start();
        material = gameObject.GetComponent<MeshRenderer>().material;
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Projectile base damage / default damage is get from the throwPrefab.
    /// </summary>
    /// <returns></returns>
    public float GetDefaultDamage()
    {
        return baseDamage;
    }

    public void SetProjectileBonusDamage(int dam)
    {
        this.bonusDamage = dam;
    }

    public int GetFinalDamage()
    {
        return baseDamage + bonusDamage;
    }

    public int GetEnergyCost()
    {
        return energyCost;
    }
}