using NUnit.Framework;
using UnityEngine;

public class Attack
{
    private GameObject hitboxPrefab;
    private Transform spawnPoint;
    private float damage;
    private float active;

    public bool IsActive;
    
    public Attack(GameObject hitboxPrefab, Transform spawnPoint, float damage, float active)
    {
        this.hitboxPrefab = hitboxPrefab;
        this.spawnPoint = spawnPoint;
        this.damage = damage;
        this.active = active;
    }

    public void PerformAttack()
    {
        if (IsActive) return;

        IsActive = true;

        GameObject hitbox = Object.Instantiate(
            hitboxPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            spawnPoint
        );

        Hitbox hitboxScript = hitbox.GetComponent<Hitbox>();
        hitboxScript.damage = damage;
        hitboxScript.lifetime = active;

        Debug.Log("Attack spawned");
        IsActive = false;
    }
}
