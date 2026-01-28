using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();
    public int maxSlots = 20;

    public void AddItem(Item item, int amount = 1)
    {
        // Try to stack if possible
        if (item.stackable)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item)
                {
                    slot.amount += amount;
                    return;
                }
            }
        }

        // If not stackable (or no existing stack) add a new slott
        if (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot(item, amount));
        }
        else
        {
            Debug.Log("Inventory Full!");
        }
    }
}
