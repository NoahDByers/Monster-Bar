using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Item item;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Inventory inventory = other.GetComponent<Inventory>();
        if (inventory != null)
        {
            inventory.AddItem(item, amount);
            Destroy(gameObject);
        }
    }
}
