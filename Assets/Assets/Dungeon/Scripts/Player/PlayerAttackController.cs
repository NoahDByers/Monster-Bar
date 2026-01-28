using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerAttackController : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject hitboxPrefab;
    public Transform hitboxSpawnPoint;
    public float damage = 10f;
    public float windupTime = 0.3f;
    public float activeTime = 0.2f;
    public float cooldownTime = 2f;
    private float cooldownClock;

    private bool isOnCooldown = false;
    private bool isAttacking = false;

    private Attack attack;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        attack = new Attack(hitboxPrefab, hitboxSpawnPoint, damage, activeTime);   
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return; 
        if (isAttacking) return;
        
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (cooldownClock > 0f)
            cooldownClock -= Time.deltaTime;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // Windup
        animator.SetTrigger("Light Attack");
        yield return new WaitForSeconds(windupTime);

        // Active 
        attack.PerformAttack();

        // Cooldown
        yield return new WaitForSeconds(cooldownTime);

        isAttacking = false;
    }
}
