using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerZen : MonoBehaviour
{
    /* Components variables */
    private CharacterController controller;
    private Animator anim;
    public LayerMask groundLayer;

    // Finite State Machine
    private enum State { idle, running, attack, guard }

    [SerializeField] private State state = State.idle;

    /* Movement */
    private Vector3 moveDirection;
    public Vector3 velocity;
    private bool isGrounded;
    public float zenMoveSpeed = 2.5f;
    public float zenRotateSpeed = 900f;
    public bool isInAnimation = false;

    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (PlayerStatHUD.psh.justDied)
        {
            isInAnimation = true;
            anim.Play("PlayerDino_Revive");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (isInAnimation) return;

        moveDirection = Vector3.zero;
        // Moving player
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        controller.Move(moveDirection * zenMoveSpeed * Time.deltaTime);
        // Rotate player direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, zenRotateSpeed * Time.deltaTime);
        }

        // Gravity
        // Reset y velocity to 0 when you on the ground
        isGrounded = Physics.CheckSphere(transform.position + new Vector3(0, 0.05f, 0), 0.1f, groundLayer, QueryTriggerInteraction.Ignore);
        if (isGrounded)
            velocity.y = 0f;
        // Increase velocity as you falling down
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Update movement animation
        AnimationState();
        anim.SetInteger("state", (int)state);
    }

    /// <summary>
    /// Control the state of the Animator
    /// </summary>
    private void AnimationState()
    {
        if (moveDirection != Vector3.zero)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
    }

    public void FinishRevive()
    {
        PlayerStatHUD.psh.justDied = false;
        isInAnimation = false;
    }
}