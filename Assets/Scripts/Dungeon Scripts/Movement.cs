using Unity.VisualScripting;
using UnityEngine;

public class Movement
{
    // Dependencies
    private Rigidbody2D rb;
    private Transform transform;
    private Animator animator;

    // Movement input
    private float moveX;
    private float facingDirection = 1f;

    // Running
    private float maxSpeed;
    private float acceleration;
    private float deceleration;
    public bool isRunning;

    // Dash
    private float dashSpeed;
    private float dashDuration;
    private float dashCooldown;

    public bool isDashing { get; private set; }
    private float dashTimeLeft;
    public float dashCooldownTimer { get; private set; }

    // Jumping
    private float jumpForce;
    private int maxJumpCount;
    private int jumpCount;

    private Transform groundCheck;
    private float groundCheckRadius;
    private LayerMask groundLayer;
    public bool isGrounded { get; private set; }
    
    // Constructor
    public Movement(
        Rigidbody2D rb,
        Transform transform,
        Animator animator,
        Transform groundCheck,
        LayerMask groundLayer,
        float groundCheckRadius,
        float maxSpeed,
        float acceleration,
        float deceleration,
        float dashSpeed,
        float dashDuration,
        float dashCooldown,
        float jumpForce,
        int maxJumpCount
    )
    {
        this.rb = rb;
        this.transform = transform;
        this.animator = animator;
        this.groundCheck = groundCheck;
        this.groundLayer = groundLayer;
        this.groundCheckRadius = groundCheckRadius;

        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.deceleration = deceleration;

        this.dashSpeed = dashSpeed;
        this.dashDuration = dashDuration;
        this.dashCooldown = dashCooldown;

        this.jumpForce = jumpForce;
        this.maxJumpCount = maxJumpCount;
    }

    // =======================
    // Public API for movement
    // =======================

    public void SetMoveInput(float x)
    {
        moveX = x;
        if (Mathf.Abs(x) > 0.01f)
            facingDirection = Mathf.Sign(x);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            jumpCount = 0;
            ApplyJump();
        }
        else if (jumpCount < maxJumpCount)
        {
            ApplyJump();
            jumpCount++;
        }
    }

    public void Dash()
    {
        if (isDashing || dashCooldownTimer > 0f)
            return;

        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        float dir = Mathf.Abs(moveX) > 0.01f ? Mathf.Sign(moveX) : facingDirection;
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);

        animator.SetBool("isDashing", true);
    }

    // =======================
    // Updating movement loops
    // =======================

    public void Tick() // Call in per frame update (normal Update())
    {
        UpdateFacing();
        UpdateAnimations();
    }

    public void FixedTick() // Call in physics loop (FixedUpdate())
    {
        CheckGrounded();
        HandleDash();
        HandleMovement();
    }

    // ==============
    // Movement Logic
    // ==============

    private void UpdateFacing()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(facingDirection) * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void UpdateAnimations()
    {
        Vector2 vel = rb.linearVelocity;

        isRunning = Mathf.Abs(vel.x) > 0.05f;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", vel.y > 0.05f);
        animator.SetBool("isFalling", vel.y < -0.05f);
        animator.SetBool("isIdle", Mathf.Abs(vel.x) < 0.05f && Mathf.Abs(vel.y) < 0.05f);
    }

    private void HandleDash()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.fixedDeltaTime;

        if (!isDashing)
            return;
        
        dashTimeLeft -= Time.fixedDeltaTime;
        if (dashTimeLeft <= 0f)
        {
            isDashing = false;
            animator.SetBool("isDashing", false);
        }
    }

    private void HandleMovement()
    {
        if (isDashing)
            return;

        if (Mathf.Abs(moveX) > 0.01f)
        {
            rb.AddForce(new Vector2(moveX * acceleration, 0), ForceMode2D.Force);

            if (Mathf.Abs(rb.linearVelocityX) > maxSpeed)
            {
                rb.linearVelocity = new Vector2(
                    Mathf.Sign(rb.linearVelocityX) * maxSpeed,
                    rb.linearVelocityY
                );
            }
        }
        else
        {
            rb.AddForce(new Vector2(rb.linearVelocityX * -deceleration, 0f), ForceMode2D.Force);
        }
    }

    private void ApplyJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }
}
