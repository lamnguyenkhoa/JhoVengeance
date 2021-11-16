using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorScript : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    public void OpenTheDoor()
    {
        Debug.Log("Open");
        anim.SetTrigger("open");
    }
}