using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is for tooltip when hover the item in world
// to help player decide is it worth pickup
public class IngameHoverTooltipSystem : MonoBehaviour
{
    public static IngameHoverTooltipSystem instance;
    public GameObject tooltip;
    private IngameHoverTooltip tooltipScript;

    // Start is called before the first frame update
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            tooltipScript = tooltip.GetComponent<IngameHoverTooltip>();
            tooltip.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Show(ItemScript item)
    {
        tooltip.SetActive(true);
        tooltipScript.SetText(item);
    }

    public void Hide()
    {
        tooltip.SetActive(false);
    }
}