using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private InventoryController inventory;
    private Transform itemParent;
    // Start is called before the first frame update

    private void Awake()
    {
    }

    private void Start()
    {
        itemParent = transform.Find("Inventory").GetChild(0);
        inventory = InventoryController.inventoryController;
        itemParent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!itemParent)
        {
            Debug.Log("Not found itemParent");
        }
    }

    public void ShowUI(bool showing)
    {
        UpdateUI();
        if (showing)
        {
            itemParent.gameObject.SetActive(true);
        }
        else
        {
            TooltipScript.instance.HideTooltip();
            itemParent.gameObject.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        if (!inventory)
            Debug.Log("inventory null");
        if (itemParent == null)
            Debug.Log("itemParent null");
        //Onhand
        itemParent = transform.Find("Inventory").GetChild(0);
        if (inventory.onHand)
        {
            itemParent.Find("OnhandSlot").GetComponent<InventorySlotScript>().AddItemToSlot(inventory.onHand);
        }
        else
        {
            itemParent.Find("OnhandSlot").GetComponent<InventorySlotScript>().ClearItemFromSlot();
        }

        //Offhand
        if (inventory.offHand)
        {
            itemParent.Find("OffhandSlot").GetComponent<InventorySlotScript>().AddItemToSlot(inventory.offHand);
        }
        else
        {
            itemParent.Find("OffhandSlot").GetComponent<InventorySlotScript>().ClearItemFromSlot();
        }

        //Armour
        if (inventory.armour)
        {
            itemParent.Find("ArmourSlot").GetComponent<InventorySlotScript>().AddItemToSlot(inventory.armour);
        }
        else
        {
            itemParent.Find("ArmourSlot").GetComponent<InventorySlotScript>().ClearItemFromSlot();
        }

        //Projectile
        if (inventory.projectile)
        {
            itemParent.Find("ProjectileSlot").GetComponent<InventorySlotScript>().AddItemToSlot(inventory.projectile);
        }
        else
        {
            itemParent.Find("ProjectileSlot").GetComponent<InventorySlotScript>().ClearItemFromSlot();
        }

        //Consumable
        if (inventory.consumable)
        {
            itemParent.Find("ConsumableSlot").GetComponent<InventorySlotScript>().AddItemToSlot(inventory.consumable);
        }
        else
        {
            itemParent.Find("ConsumableSlot").GetComponent<InventorySlotScript>().ClearItemFromSlot();
        }
    }
}