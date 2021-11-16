using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponScript : ItemScript
{
    [SerializeField] private int maxWeaponDamageBonus;
    public static int MIN_WEAPON_DAMAGE_BONUS = 1;
    [SerializeField] [Range(1, 50)] private int baseDamage; // base damage is used in calculate weaponDamageBonus
    [SerializeField] [Range(1, 75)] private int weaponDamageBonus; // the final damage, what will be used
    public static int DEFAULT_DAMAGE = 5;
    public float attackSpeed = 1f;

    private void Awake()
    {
        type = ItemType.Weapon;
        if (weaponDamageBonus == 0)
        {
            weaponDamageBonus = baseDamage;
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

    public void SetDamageBonus(int damageBonus)
    {
        this.weaponDamageBonus = damageBonus;
    }

    // Final damage
    public int GetDamageBonus()
    {
        return weaponDamageBonus;
    }

    public int GetBaseDamage()
    {
        return baseDamage;
    }

    public int GetMaxWeaponDamage()
    {
        return maxWeaponDamageBonus;
    }

    public void RandomizeAttackSpeed()
    {
        attackSpeed = Random.Range(0.5f, 2f);
        // Compensation for atk speed
        if (attackSpeed <= 0.6f)
        {
            weaponDamageBonus = (int)(weaponDamageBonus * 1.75f);
        }
        if (0.6f < attackSpeed && attackSpeed <= 0.8f)
        {
            weaponDamageBonus = (int)(weaponDamageBonus * 1.25f);
        }
        if (1.25f <= attackSpeed && attackSpeed < 1.75f)
        {
            weaponDamageBonus = (int)(weaponDamageBonus * 0.9f);
        }
        if (attackSpeed >= 1.75f)
        {
            weaponDamageBonus = (int)(weaponDamageBonus * 0.75f);
        }
    }
}