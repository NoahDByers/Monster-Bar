using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour
{
    [Header("Running")]
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _deceleration = 20f;
    [SerializeField] private Rigidbody2D rigidBody;
    private float moveX;

    [Header("Dash")]
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashCooldown = 1f;
    private bool isDashing = false;
    private float dashTimeLeft; 
    private float dashCooldownTimer;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int maxJumpCount = 1;
    private int jumpCount = 0;
    private bool isGrounded;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    private float facingDirection = 1f; // 1 = right -1 = left

    private Vector2 velocity;

    void Update()
    {
        velocity = rigidBody.linearVelocity;

        // Flip sprite
        spriteRenderer.flipX = facingDirection < 0;

        // Animation states
        bool isRunning = Mathf.Abs(velocity.x) > 0.05f;
        bool isJumping = velocity.y > 0.05f;
        bool isFalling = velocity.y < -0.05f;
        bool isIdle = !isRunning && !isJumping && !isFalling;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isIdle", isIdle);
    }

    void FixedUpdate()
    {
        //Checking if the player can jump
        CheckGrounded();

        //Dash logic
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.fixedDeltaTime;

        if (isDashing)
        {
            dashTimeLeft -= Time.fixedDeltaTime;

            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
                animator.SetBool("isDashing", false);
            }

            return;
        }

        //Normal movement logic
        if (Mathf.Abs(moveX) > 0.01f)
        {
            rigidBody.AddForce(new Vector2(moveX * _acceleration, 0), ForceMode2D.Force);

            if (Mathf.Abs(rigidBody.linearVelocityX) > _maxSpeed)
            {
                rigidBody.linearVelocity = new Vector2(
                    Mathf.Sign(rigidBody.linearVelocityX) * _maxSpeed,
                    rigidBody.linearVelocityY
                );
            }
        }
        else
        {
            rigidBody.AddForce(new Vector2(rigidBody.linearVelocityX * -_deceleration, 0f), ForceMode2D.Force);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveX = context.ReadValue<float>();

        if (Mathf.Abs(moveX) > 0.01f)
            facingDirection = Mathf.Sign(moveX);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        
        if (isDashing || dashCooldownTimer > 0f)
            return;

        StartDash();
    }

    void StartDash()
    {
        isDashing = true;
        animator.SetBool("isDashing", true);
        dashTimeLeft = _dashDuration;
        dashCooldownTimer = _dashCooldown;

        float dashDir = Mathf.Abs(moveX) > 0.01f ? Mathf.Sign(moveX) : facingDirection;

        rigidBody.linearVelocity = new Vector2(dashDir * _dashSpeed, 0f);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (isGrounded)
        {
            jumpCount = 0;
            applyJumpForce();
        }
        else if (jumpCount < maxJumpCount)
        {
            applyJumpForce();
            jumpCount++;
        }
    }
    void applyJumpForce()
    {
        rigidBody.linearVelocity = new Vector2(
                rigidBody.linearVelocityX,
                0f
            );

            rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            animator.SetTrigger("Light Attack");
    }
}
