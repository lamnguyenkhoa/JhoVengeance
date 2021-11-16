using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatScript : MonoBehaviour
{
    private float originalY;

    private void Start()
    {
        originalY = transform.position.y;
    }

    private void Update()
    {
        // Floaty object up and down
        Vector3 newFloatPosition = transform.position;
        newFloatPosition.y = originalY + (Mathf.Sin(Time.time) * 0.2f);
        transform.position = newFloatPosition;
    }
}