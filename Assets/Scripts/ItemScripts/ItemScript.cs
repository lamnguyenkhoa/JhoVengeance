using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Projectile,
    Armour,
    Weapon,
    Consumable
}

public enum ItemRarity
{
    Normal,
    Rare,
    Epic
}

public class ItemScript : MonoBehaviour
{
    public ItemType type;
    public Sprite inventorySprite;
    public ItemRarity itemRarity;
    public string itemName;
    public string itemDescription;
    private float originalY;
    private Vector3 originalScale;

    /* Hover hightlight */
    protected Transform player;
    public Shader hoverShader;
    public Shader defaultShader;
    protected Material material;

    private void Awake()
    {
        // Should not set item tag in awake()
        // https://docs.unity3d.com/ScriptReference/GameObject-tag.html
    }

    protected virtual void Start()
    {
        player = GameObject.Find("PlayerDino").transform;
        material = null; // Get from children class
        originalY = transform.position.y;
        originalScale = transform.localScale;
    }

    protected virtual void OnEnable()
    {
        player = GameObject.Find("PlayerDino").transform;
        originalY = transform.position.y;
    }

    protected void OnMouseEnter()
    {
        if ((player.position - transform.position).sqrMagnitude <= PlayerController.maxPickupDistanceSqr)
        {
            transform.localScale = originalScale * 1.2f;
            ShowTooltip();
            if (type != ItemType.Consumable)
            {
                material.shader = hoverShader;
            }
        }
        else
        {
            transform.localScale = originalScale;
            HideTooltip();
            if (type != ItemType.Consumable)
            {
                material.shader = defaultShader;
            }
        }
    }

    protected void OnMouseExit()
    {
        transform.localScale = originalScale;
        HideTooltip();
        if (type != ItemType.Consumable)
        {
            material.shader = defaultShader;
        }
    }

    protected virtual void Update()
    {
        // Floaty object up and down
        Vector3 newFloatPosition = transform.position;
        float positiveSin = (Mathf.Sin(Time.time) + 1) / 2; // result in 0 and 1
        newFloatPosition.y = originalY + positiveSin * 0.4f;
        transform.position = newFloatPosition;

        // Rotate object
        transform.Rotate(Vector3.up, 0.5f);
    }

    public void SetDefaultShader()
    {
        if (material.shader != defaultShader)
        {
            material.shader = defaultShader;
        }
    }

    /// <summary>
    ///  Use this when throw out item in new scenes(scenes that are different
    ///  from where the item is found)
    /// </summary>
    /// <param name="newPlayer"></param>
    public void UpdatePlayerTransform(Transform newPlayer)
    {
        player = newPlayer;
    }

    public void ShowTooltip()
    {
        if (IngameHoverTooltipSystem.instance)
        {
            IngameHoverTooltipSystem.instance.Show(this);
        }
        else
        {
            Debug.Log("Missing IngameHoverTooltipSystem");
        }
    }

    public void HideTooltip()
    {
        if (IngameHoverTooltipSystem.instance)
        {
            IngameHoverTooltipSystem.instance.Hide();
        }
        else
        {
            Debug.Log("Missing IngameHoverTooltipSystem");
        }
    }
}