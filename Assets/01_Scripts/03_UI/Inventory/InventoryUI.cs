using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Transform gridContainer;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private int totalSlot = 20;

    private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();

    private void Awake()
    {
        GenerateSlot(); // TODO: 나중에 키 지정해서 슬롯 불러올 예정
    }
    private void GenerateSlot()
    {
        foreach(Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        slotUIList.Clear();

        for (int i = 0; i < totalSlot ; i++)
        {
            InventorySlotUI slotUI = Instantiate(slotPrefab, gridContainer);
            slotUI.Init(i);
            slotUIList.Add(slotUI);
        }
    }
    
    // 빈 슬롯중 가장 앞에 있는 곳 인덱스 반환 (자동배치)
    public int GetEmptySlotIndex(bool[] fillSlot)
    {
        for (int i = 0; i < fillSlot.Length; i++)
        {
            if (!fillSlot[i]) return i; 
        }
        return -1; //인벤토리 가득참
    }
}
