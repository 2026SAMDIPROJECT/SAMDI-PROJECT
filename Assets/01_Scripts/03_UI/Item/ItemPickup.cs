using Unity.Netcode;
using UnityEngine;

public class ItemPickup : InteractiveObject
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private float maxDis;
    public override void Interact(NetworkBehaviourReference inventoryHolderRef)
    {
        if (!IsSpawned) return;
        PickupItemRpc(inventoryHolderRef);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void PickupItemRpc(NetworkBehaviourReference inventoryHolderRef, RpcParams rpcParam = default)
    {
        if(!IsSpawned) return;
        if(!inventoryHolderRef.TryGet(out PlayerInventoryHolder inventoryHolder)) return;

        if(inventoryHolder.OwnerClientId != rpcParam.Receive.SenderClientId) return;

        var grid = inventoryHolder.ServerInventory;
        if(grid == null) return;

        if(!grid.FindEmptySpace(itemData, out int x, out int y)) return;

        if(!grid.PlaceItem(itemData, x, y)) return;

        NetworkObject.Despawn(true); // 위에서 spawned를 체크함 -> 여기선 체크 안 함
    }
}
