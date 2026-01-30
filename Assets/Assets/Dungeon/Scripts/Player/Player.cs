
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour, IsDamageable
{   
    [Header("Player Settings")]
    [SerializeField] float hp;

    [Header("Required Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

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

    [Space]
    public bool isGrounded = false;
    public bool isRunning = false;

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
        RefreshStates();
    }

    void FixedUpdate()
    {
        movement.FixedTick();

        if (hp <= 0)
        {
            Die();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement.SetMoveInput(context.ReadValue<float>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            movement.Jump();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            movement.Dash();
    }

    void RefreshStates()
    {
        isGrounded = movement.isGrounded;
        isRunning = movement.isRunning;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
    }

    public void Die()
    {
        animator.SetTrigger("Death");
        animator.SetBool("isDead", true);
        movement.SetMoveInput(0);
        rb.linearVelocityX = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    
    }
}
