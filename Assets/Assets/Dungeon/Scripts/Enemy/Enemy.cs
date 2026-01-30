using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class Enemy : MonoBehaviour, IsDamageable
{
    [Header("Required Components")]
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Space]
    [Header("Enemy Settings")]
    public float hp = 20f;
    [SerializeField] private float deathTime = 1;

    [Space]
    [Header("AI Settings")]
    [SerializeField] private float triggerDistance = 5f;
    [SerializeField] private float attackDistance = 5f;

    [Space]
    [Header("Attack Settings")]
    [SerializeField] private float attackOneDamage = 10f;
    private Attack attack1;
    public GameObject attackOnePrefab;
    public Transform attackOneSpawnPoint;
    public float windupTime = 0.3f;
    public float attackOneTime = 0.2f;
    public float cooldownTime = 2f;
    private float cooldownClock = 0f;
    private Attack attack2;
    public GameObject attackTwoPrefab;
    public Transform attackTwoSpawnPoint;
    public float attackTwoDamage = 15f;
    public float attackTwoTime = 0.3f;

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
    private bool isDead = false;

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

        attack1 = new Attack(attackOnePrefab, attackOneSpawnPoint, attackOneDamage, attackOneTime);
        attack2 = new Attack(attackTwoPrefab, attackTwoSpawnPoint, attackTwoDamage, attackTwoTime);
    }

    void Update()
    {
        movement.Tick();
    }

    void FixedUpdate()
    {
        if (cooldownClock > 0f)
            cooldownClock -= Time.deltaTime;

        if (InRange(attackDistance) && cooldownClock <= 0f)
        {
            cooldownClock = cooldownTime;

            animator.SetTrigger("Attack");
        }

        if (InRange(triggerDistance))
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

        if (Mathf.Abs(deltaX) < 0.1f)
        {
            movement.SetMoveInput(0f);
            return;
        }

        movement.SetMoveInput(Mathf.Sign(deltaX));
    }

    bool InRange(float range)
    {
        float sqrDistance = (transform.position - player.transform.position).sqrMagnitude;
        float sqrTrigger = range * range;

        return sqrDistance < sqrTrigger;
    }

    private void AttackOne()
    {
        attack1.PerformAttack();
    }

    private void AttackTwo()
    {
        attack2.PerformAttack();
    }

    private void LockMovement()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
    }

    private void UnlockMovement()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        hp -= damage;
        animator.SetTrigger("Hurt");

        if (hp <= 0f) {
            isDead = true;
            Die();
        }
    }

    public void Die()
    {
        animator.SetTrigger("Death");
        animator.SetBool("isDead", true);
        movement.SetMoveInput(0);
        rb.linearVelocityX = 0;
        LockMovement();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
