using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* https://www.youtube.com/watch?v=cR8jP8OGbhM&list=PLMIiQsI9KWh5rq7USnIxrb5gqjhf99LsN&index=46&t=9s */

public class HealthBarCutFallDown : MonoBehaviour
{
    private RectTransform rectTransform;
    private float fallDownTimer;
    private float fadeTimer;
    private Image image;
    private Color color;
    private float fallSpeed = 50f;
    private float alphaFadeSpeed = 5f;

    private void Awake()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        image = transform.GetChild(0).GetComponent<Image>();
        color = image.color;
        fallDownTimer = 1f;
        fadeTimer = 0.5f;
    }

    private void Update()
    {
        fallDownTimer -= Time.deltaTime;
        if (fallDownTimer < 0)
        {
            rectTransform.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

            fadeTimer -= Time.deltaTime;
            if (fadeTimer < 0)
            {
                color.a -= alphaFadeSpeed * Time.deltaTime;
                image.color = color;

                if (color.a <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}