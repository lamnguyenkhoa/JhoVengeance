using UnityEngine;
using System;

public class LevelEvents : MonoBehaviour
{

    // A basic script to hold different event triggers within the project
    public static LevelEvents levelEvents;

    private void Awake()
    {
        if (!levelEvents)
        {
            levelEvents = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public event Action<Vector3> onEnemyDeathTriggerEnter;

    public void EnemyDeathTriggerEnter(Vector3 position)
    {
        if (onEnemyDeathTriggerEnter != null)
        {
            onEnemyDeathTriggerEnter(position);
        }
    }

    public event Action<WeaponScript, bool> onOnhandWeaponChangeTriggerEnter;

    public void OnHandWeaponChangeTriggerEnter(WeaponScript newOnHand, bool prevWasNull)
    {
        if (onOnhandWeaponChangeTriggerEnter != null)
        {
            onOnhandWeaponChangeTriggerEnter(newOnHand, prevWasNull);
        }
    }

    public event Action<WeaponScript, bool> onOffhandWeaponChangeTriggerEnter;

    public void OffHandWeaponChangeTriggerEnter(WeaponScript newOffhand, bool prevWasNull)
    {
        if (onOffhandWeaponChangeTriggerEnter != null)
        {
            onOffhandWeaponChangeTriggerEnter(newOffhand, prevWasNull);
        }
    }

    public event Action onInventoryChangeTriggerEnter;

    public void InventoryChangeTriggerEnter()
    {
        if (onInventoryChangeTriggerEnter != null)
        {
            onInventoryChangeTriggerEnter();
        }
    }

    public event Action<ItemScript> onOnGroundItemHoverTriggerEnter;
    public void OnGroundItemHoverTriggerEnter(ItemScript itemScript)
    {
        if(onOnGroundItemHoverTriggerEnter != null)
        {
            onOnGroundItemHoverTriggerEnter(itemScript);
        }
    }
}