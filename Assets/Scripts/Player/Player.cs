using UnityEngine;
using UnityEngine.InputSystem;

public partial class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 move;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Vector2 velocity;

    void Update()
    {
        velocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY);

        // Flip sprite
        if (Mathf.Abs(velocity.x) > 0.1f)
            spriteRenderer.flipX = velocity.x < 0;

        // Animation states
        bool isRunning = Mathf.Abs(velocity.x) > 0.1f;
        bool isJumping = velocity.y > 0.1f;
        bool isFalling = velocity.y < -0.1f;
        bool isIdle = !isRunning && !isJumping && !isFalling;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isIdle", isIdle);
    }

    void FixedUpdate()
    {
        rb.linearVelocityX = move.x * speed;
        rb.linearVelocityY = move.y * speed - rb.gravityScale;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
}
