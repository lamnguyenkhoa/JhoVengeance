using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BetterButton : MonoBehaviour, IPointerEnterHandler
{
    public AudioSource audioSource;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.Play();
    }
}