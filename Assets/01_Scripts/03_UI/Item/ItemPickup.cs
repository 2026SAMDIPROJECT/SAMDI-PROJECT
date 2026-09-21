using Unity.Netcode;
using UnityEngine;

public class ItemPickup : InteractiveObject
{
    [SerializeField] private ItemData itemData;
    private NetworkObject netObj;

    private void Awake()
    {
        netObj = GetComponent<NetworkObject>();
    }
    public override void Interact(PlayerInteract interactor)
    {
        RequestPickUpServerRpc(interactor.OwnerClientId);
    }
    [Rpc(SendTo.Server)]
    private void RequestPickUpServerRpc(ulong requesterClientId)
    {
        // 서버에서 아이템 줍는 것을 검증함
        var requesterObject = NetworkManager.Singleton.ConnectedClients[requesterClientId].PlayerObject;
        
        if (!requesterObject.TryGetComponent(out PlayerInteract playerInteract)) return;
        if (playerInteract.inventoryUI == null) return;

        bool added = playerInteract.inventoryUI.AddItem(itemData);
        if (added) GetComponent<NetworkObject>().Despawn();
    }
}
