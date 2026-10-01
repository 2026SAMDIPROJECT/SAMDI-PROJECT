using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOpenInv : MonoBehaviour
{
    [SerializeField] private InventoryUI inventory;

    public void OnInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            inventory.ToggleInventory();
        }
    }
}
