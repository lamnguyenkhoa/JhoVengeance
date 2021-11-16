using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* https://www.youtube.com/watch?v=HXFoUGw7eKk&ab_channel=GameDevGuide */

[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{
    private TextMeshProUGUI headerField;
    private TextMeshProUGUI statField;
    private TextMeshProUGUI descriptionField;
    public LayoutElement layoutElement;
    public int characterLimit;
    public RectTransform rectTransform;
    private Vector2 bias = new Vector2(50f, -25f);

    private void Awake()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        headerField = transform.Find("Header").GetComponent<TextMeshProUGUI>();
        statField = transform.Find("Stat").GetComponent<TextMeshProUGUI>();
        descriptionField = transform.Find("Description").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        // Resize tooltip based on text
        int headerLength = headerField.text.Length;
        int statLength = statField.text.Length;
        int descriptionLength = descriptionField.text.Length;

        if (headerLength > characterLimit || statLength > characterLimit || descriptionLength > characterLimit)
            layoutElement.enabled = true;
        else
            layoutElement.enabled = false;

        // Set tooltip location and pivot
        Vector2 mousePos = Input.mousePosition;
        float pivotX = mousePos.x / Screen.width;
        float pivotY = mousePos.y / Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = mousePos;
    }
}