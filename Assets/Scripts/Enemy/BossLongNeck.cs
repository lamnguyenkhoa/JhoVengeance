using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLongNeck : IEnemy
{
    private Text hpText;
    private float BAR_WIDTH;
    private Transform damageBar;

    protected override void Start()
    {
        base.Start();
        healthBar = GameObject.Find("BossHPBar").transform.Find("HealthBar").gameObject;
        healthSlider = healthBar.GetComponent<Slider>();
        healthSlider.value = 1;
        BAR_WIDTH = healthBar.GetComponent<RectTransform>().rect.width;
        damageBar = healthBar.transform.Find("DamageBar");
        hpText = healthBar.transform.Find("HPText").GetComponent<Text>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Damaged(int amount, float knockbackPower, Vector3 playerPos)
    {
        base.Damaged(amount, knockbackPower, playerPos);
        hpText.text = health + " / " + maxHealth;
        float hpPercent = (float)health / maxHealth;

        // Damaged bar effect
        float lostPercent = (float)amount / maxHealth;
        Transform newDamagedBar = Instantiate(damageBar, healthBar.transform);
        newDamagedBar.gameObject.SetActive(true);
        newDamagedBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(hpPercent * BAR_WIDTH, damageBar.GetComponent<RectTransform>().anchoredPosition.y);
        newDamagedBar.GetComponent<Slider>().value = lostPercent;

        hpText.transform.SetAsLastSibling();
    }
}