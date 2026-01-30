using UnityEditor.Rendering.Analytics;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage;
    public float lifetime = 0.2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IsDamageable damageable = other.GetComponent<IsDamageable>();

        if (damageable != null)
        {
            Debug.Log("Dealing damage to enemy: " + damage);
            damageable.TakeDamage(damage);
        }
    }
}
