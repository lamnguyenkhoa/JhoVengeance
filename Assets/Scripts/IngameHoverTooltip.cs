using UnityEngine;
using UnityEngine.UI;
using TMPro;

// https://www.youtube.com/watch?v=HXFoUGw7eKk
// This script is for tooltip when hover the item in world
// to help player decide is it worth pickup
public class IngameHoverTooltip : MonoBehaviour
{
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI statField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    public Image backgroundImage;

    public RectTransform rectTransform;

    [SerializeField] private Vector2 bias;
    // Start is called before the first frame update

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            int nameLength = nameField.text.Length;
            int contentLength = statField.text.Length;

            if (nameLength > characterWrapLimit || contentLength > characterWrapLimit)
            {
                layoutElement.enabled = false;
            }
            else
            {
                layoutElement.enabled = true;
            }
        }
        Vector2 pos = Input.mousePosition;
        pos += bias;
        float pivotX = pos.x / Screen.width;
        float pivoty = pos.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivoty);
        transform.position = pos;
    }

    public void SetText(ItemScript item)
    {
        string newStat;
        if (item.type == ItemType.Consumable)
        {
            ConsumableScript cS = (ConsumableScript)item;
            newStat = "Heal: " + cS.GetHealValue().ToString();
        }
        else if (item.type == ItemType.Armour)
        {
            ArmourScript aS = (ArmourScript)item;
            newStat = "Armour: " + aS.GetProtectionBonus().ToString("F2") + "%";
        }
        else if (item.type == ItemType.Projectile)
        {
            ProjectileScript aS = (ProjectileScript)item;
            newStat = "Damage: " + aS.GetFinalDamage().ToString();
        }
        else
        {
            WeaponScript wS = (WeaponScript)item;
            newStat = "Damage: " + wS.GetDamageBonus().ToString();
            newStat += "\nAttack speed: " + wS.attackSpeed.ToString("F2");
        }
        nameField.text = item.itemName;
        statField.text = newStat;

        int nameLength = nameField.text.Length;
        int contentLength = statField.text.Length;

        if (nameLength > characterWrapLimit || contentLength > characterWrapLimit)
        {
            layoutElement.enabled = false;
        }
        else
        {
            layoutElement.enabled = true;
        }
    }

    public void HideText()
    {
        nameField.text = "0";
        statField.text = "0";
    }
}