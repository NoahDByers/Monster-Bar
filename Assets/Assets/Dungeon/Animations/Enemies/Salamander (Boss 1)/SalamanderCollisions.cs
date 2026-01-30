using UnityEngine;

public class SalamanderCollisions : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] private BoxCollider2D collisions;

    [Space]
    [Header("Shapes")]
    [SerializeField] private ColliderShape idleShape;
    [SerializeField] private ColliderShape attackShape;
    [SerializeField] private ColliderShape hurtShape;
    [SerializeField] private ColliderShape deathShape;
    [SerializeField] private ColliderShape runShape;

    public void OnIdle()
    {
        ApplyShape(idleShape);
    }

    public void OnAttack()
    {
        ApplyShape(attackShape);
    }

    public void OnRun()
    {
        ApplyShape(runShape);
    }

    public void OnHurt()
    {
        ApplyShape(hurtShape);
    }

    public void OnDeath()
    {
        ApplyShape(deathShape);
    }

    private void ApplyShape(ColliderShape shape)
    {
        collisions.size = shape.size;
        collisions.offset = shape.offset;
    }
}
