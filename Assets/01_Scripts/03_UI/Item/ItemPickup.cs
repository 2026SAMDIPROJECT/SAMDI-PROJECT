using Unity.Netcode;
using UnityEngine;

public class ItemPickup : InteractiveObject
{
    [SerializeField] private ItemData itemData;
    public override void Interact(PlayerInteract interactor)
    {
        if (!interactor.IsOwner) return;
        if (!IsSpawned) return;
        RequestPickUpServerRpc(interactor.OwnerClientId);
    }
    [Rpc(SendTo.Server)]
    private void RequestPickUpServerRpc(ulong requesterClientId)
    {
        // 서버에서 아이템 줍는 것을 검증함
        if(!NetworkManager.Singleton.ConnectedClients.TryGetValue(requesterClientId, out var client)) return;
        
        var playerObj = client.PlayerObject;
        if (!playerObj.TryGetComponent(out PlayerInventoryHolder inventoryHolder)) return;

        var grid = inventoryHolder.ServerInventory;
        if (grid == null) return;
        if (grid.FindEmptySpace(itemData, out int x, out int y))
        {
           bool added = grid.PlaceItem(itemData, x, y);
           if(added)
            {
                if (TryGetComponent(out NetworkObject netObj) && netObj.IsSpawned)
                {
                    DisableItemClientRpc();
                    netObj.Despawn(false);
                }
            }
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void DisableItemClientRpc()
    {
        gameObject.SetActive(false);
    }
}
