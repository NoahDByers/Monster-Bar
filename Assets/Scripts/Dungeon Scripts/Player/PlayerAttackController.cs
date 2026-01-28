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
    public float activeTime = 0.2f;

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

        animator.SetTrigger("Light Attack");
        attack.PerformAttack();
    }
}
