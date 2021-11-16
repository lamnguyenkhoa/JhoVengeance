using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableScript : ItemScript
{
    public static int MAX_BONUS_VALUE = 10;
    [Range(0, 100)] [SerializeField] private int defaultHealValue;
    [Range(0, 100)] [SerializeField] private int healValue;

    private void Awake()
    {
        type = ItemType.Consumable;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public int GetHealValue()
    {
        return this.healValue;
    }

    public int GetDefaultHealValue()
    {
        return defaultHealValue;
    }

    public void SetHealValue(int healVal)
    {
        this.healValue = healVal;
    }

    public int Consume()
    {
        int retVal = this.healValue;
        return retVal;
    }
}