using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private Canvas inventoryCanvas;

    public Inventory inventory;
    public Transform slotParent;
    public GameObject slotPrefab;
    private bool isOpen = false;

    public void InventoryAction(InputAction.CallbackContext context)
    {
        Debug.Log("Inventory action triggered");

        if (!context.performed)
            return;
        
        isOpen = !isOpen;
        inventoryCanvas.gameObject.SetActive(isOpen);

        if (isOpen)
        {
            RefreshInventory();
        }
        
    }

    void RefreshInventory()
        {
            foreach (Transform child in slotParent)
                Destroy(child.gameObject);

            foreach (var slot in inventory.slots)
            {
                GameObject uiSlot = Instantiate(slotPrefab, slotParent);
                uiSlot.transform.Find("Icon").GetComponent<Image>().sprite = slot.item.icon;
                uiSlot.transform.Find("Amount").GetComponent<TMP_Text>().text =
                    slot.item.stackable ? slot.amount.ToString() : "";
            }
        }
}