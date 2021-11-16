using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovTexture : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.1f;
    private Renderer rend;

    // Start is called before the first frame update
    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        rend.material.SetTextureOffset("_MainTex", new Vector2(Time.time * scrollSpeedX, Time.time * scrollSpeedY));
    }
}