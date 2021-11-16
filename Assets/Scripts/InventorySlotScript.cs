using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;

    private static Color seeThroughWhite = new Color(1f, 1f, 1f, 0f);
    private ItemScript item;

    public void AddItemToSlot(ItemScript newItem)
    {
        item = newItem;
        icon.sprite = item.inventorySprite;
        icon.color = Color.white;
        icon.enabled = true;
    }

    public void ClearItemFromSlot()
    {
        item = null;
        if (icon)
        {
            icon.sprite = null;
            icon.color = seeThroughWhite;
            icon.enabled = false;
        }
        else
        {
            Debug.Log("Icon = null" + icon == null);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (!item)
        {
            return;
        }
        TooltipScript.instance.ShowTooltipPrep(item);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (!item)
        {
            return;
        }
        TooltipScript.instance.HideTooltip();
    }
}