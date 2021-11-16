using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private float moveYSpeed = 0.5f;
    private float lifetime = 1.5f;
    private float disappearSpeed = 10f;

    public enum PopupType { normal, critical, heal }

    private PopupType popupType;
    private TextMeshPro textMesh;
    private Color textColor;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        textColor = textMesh.color;
        transform.localPosition += new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.2f, -0.2f),
            Random.Range(-0.1f, 0.1f));
    }

    public void Setup(int amount, PopupType type = PopupType.normal)
    {
        this.popupType = type;
        textMesh.SetText(amount.ToString());
    }

    private void Update()
    {
        // Move the damage popup, floating upward
        transform.position += new Vector3(0, moveYSpeed, 0) * Time.deltaTime;

        // Disappear after some time
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            // Start disappear
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}