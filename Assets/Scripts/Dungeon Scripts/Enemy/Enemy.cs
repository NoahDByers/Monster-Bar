using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Space]
    [Header("AI Settings")]
    [SerializeField] private float triggerDistance = 5f;
    
    [Space]
    [Header("Movement Settings")] 
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 20f;

    [Space]
    [Header("Dash")] 
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Space]
    [Header("Jump")] 
    [SerializeField] private float jumpForce = 12f; 
    [SerializeField] private int maxJumpCount = 1;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Movement movement;

    void Awake()
    {
        movement = new Movement(
            rb,
            transform,
            animator,
            groundCheck,
            groundLayer,
            groundCheckRadius,
            maxSpeed,
            acceleration,
            deceleration,
            dashSpeed,
            dashDuration,
            dashCooldown,
            jumpForce,
            maxJumpCount
        );
    }

    void Update()
    {
        movement.Tick();
    }

    void FixedUpdate()
    {
        if (InRange())
        {
            MoveTowardPlayer();
        }
        else
        {
            movement.SetMoveInput(0f); // stand still
        }

        movement.FixedTick();
    }

    void MoveTowardPlayer()
    {
        float deltaX = player.transform.position.x - transform.position.x;

        if (Math.Abs(deltaX) < 0.1f)
        {
            movement.SetMoveInput(0f);
            return;
        }

        movement.SetMoveInput(Mathf.Sign(deltaX));
    }

    bool InRange()
    {
        float sqrDistance = (transform.position - player.transform.position).sqrMagnitude;
        float sqrTrigger = triggerDistance * triggerDistance;

        return sqrDistance < sqrTrigger;
    }
}
