using Unity.Netcode;
using UnityEngine;

public class PlayerInventoryHolder : NetworkBehaviour
{
    public InventoryGrid ServerInventory {get; private set;} // 서버/클라이언트 공용 인벤토리 데이터 모델

    [Header("인벤토리 원본 데이터 설정")] // 이 스크립트 파일에서 인벤토리 데이터를 설정함
    [SerializeField] private int totalSlot = 30;
    [SerializeField] private int columns = 5;
    [Header("UI 연결")]
    [SerializeField] private InventoryUI inventoryUI;

    private void Awake()
    {
        ServerInventory = InventoryGridBuilder.Build(totalSlot,columns);
    }
    public override void OnNetworkSpawn()
    {
        if(IsOwner && inventoryUI != null)
        {
            inventoryUI.BindInventory(ServerInventory);
        }
    }
}
