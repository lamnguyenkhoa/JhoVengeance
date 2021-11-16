using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourScript : ItemScript
{
    public static float MAX_PROTECTION = 75f;
    public static float DEFAULT_PROTECTION = 20f;
    public static float MIN_PROTECTION = 10f;
    [Range(1f, 75f)] [SerializeField] private float armourProtectionBonus;
    [Range(1f, 75f)] [SerializeField] private float defaultArmourProtectionBonus;

    private void Awake()
    {
        type = ItemType.Armour;
    }

    protected override void Start()
    {
        base.Start();
        material = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
    }

    protected override void Update()
    {
        base.Update();
    }

    public void SetProtectionBonus(float protectionBonus)
    {
        this.armourProtectionBonus = protectionBonus;
    }

    public float GetProtectionBonus()
    {
        return armourProtectionBonus;
    }

    public float GetDefaultArmourProtectionBonus()
    {
        return defaultArmourProtectionBonus;
    }
}