using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController inventoryController;
    public ArmourScript armour;
    public ConsumableScript consumable;
    public WeaponScript onHand;
    public WeaponScript offHand;
    public ProjectileScript projectile;

    public void Awake()
    {
        if (!inventoryController)
        {
            inventoryController = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
    }

    /// <summary>
    /// Adds item to inventory. If adding item due to pickup bind being pressed, always
    /// pick up that item and drop what item may have been in the corresponding slot.
    /// If adding item due to collision (isClick=false), only add to inventory if
    /// there is space in specific slot.
    /// </summary>
    /// <param name="item">item to be picked up</param>
    /// <param name="isClick">bool representing if pickup was pressed</param>
    /// <returns>returns item to be dropped, null if none</returns>
    public ItemScript AddItem(ItemScript item)
    {
        ItemType type = item.type;
        ItemScript dropItem = null;
        if (item.type == ItemType.Weapon)
        {
            bool prevNull = (onHand == null);
            dropItem = onHand;
            onHand = (WeaponScript)item;
            onHand.SetDefaultShader();
            LevelEvents.levelEvents.OnHandWeaponChangeTriggerEnter(onHand, prevNull);
        }
        else if (item.type == ItemType.Armour)
        {
            dropItem = armour;
            armour = (ArmourScript)item;
        }
        else if (item.type == ItemType.Consumable)
        {
            dropItem = consumable;
            consumable = (ConsumableScript)item;
        }
        else if (item.type == ItemType.Projectile)
        {
            dropItem = projectile;
            projectile = (ProjectileScript)item;
        }
        LevelEvents.levelEvents.InventoryChangeTriggerEnter();
        return dropItem;
    }

    public ItemScript DropOnhand()
    {
        LevelEvents.levelEvents.OnHandWeaponChangeTriggerEnter(null, false);
        LevelEvents.levelEvents.InventoryChangeTriggerEnter();
        ItemScript dropItem = onHand;
        onHand = null;
        return dropItem;
    }

    public void ClearInventory()
    {
        /*
        Destroy(onHand);
        Destroy(offHand);
        Destroy(consumable);
        Destroy(armour);
        Destroy(projectile);*/

        onHand = null;
        consumable = null;
        offHand = null;
        projectile = null;
        armour = null;
        LevelEvents.levelEvents.InventoryChangeTriggerEnter();
    }

    /// <summary>
    /// Asks if there is space for the given item
    /// </summary>
    /// <param name="item"></param>
    /// <returns>boolean value representing whether there is space for the item</returns>
    public bool IsFullSlot(ItemScript item)
    {
        bool retVal = false;
        ItemType pickupType = item.type;
        if (onHand && item.type == ItemType.Weapon)
        {
            retVal = true;
        }
        else if (armour && item.type == ItemType.Armour)
        {
            retVal = true;
        }
        else if (consumable && item.type == ItemType.Consumable)
        {
            retVal = true;
        }
        else if (projectile && item.type == ItemType.Projectile)
        {
            retVal = true;
        }

        return retVal;
    }

    public int ConsumeConsumable()
    {
        if (consumable)
        {
            LevelEvents.levelEvents.InventoryChangeTriggerEnter();
            int healVal = consumable.Consume();
            Destroy(consumable.gameObject);
            consumable = null;
            return healVal;
        }
        return 0;
    }

    public int GetDamageBonus()
    {
        if (!onHand)
        {
            return 0;
        }
        return onHand.GetDamageBonus();
    }

    public float GetProtectionBonus()
    {
        if (!armour)
        {
            return 0f;
        }
        return armour.GetProtectionBonus();
    }

    public void SwitchOnhand()
    {
        bool prevNull = (onHand == null);
        bool prevNullOff = (offHand == null);
        WeaponScript temp = onHand;
        onHand = offHand;
        offHand = temp;
        LevelEvents.levelEvents.OnHandWeaponChangeTriggerEnter(onHand, prevNull);
        LevelEvents.levelEvents.OffHandWeaponChangeTriggerEnter(offHand, prevNullOff);
        LevelEvents.levelEvents.InventoryChangeTriggerEnter();
    }
}