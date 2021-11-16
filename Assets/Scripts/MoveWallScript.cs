using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWallScript : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveWall()
    {
        animator.SetTrigger("MoveWall");
    }
}
