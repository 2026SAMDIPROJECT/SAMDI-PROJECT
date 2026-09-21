using System.Collections.Generic;
using UnityEngine;
// 오브젝트 폴링 전달
public class SlotPool
{
    private readonly List<InventorySlotUI> pool = new List<InventorySlotUI>();
    private readonly List<InventorySlotUI> active = new List<InventorySlotUI>();
    private readonly InventorySlotUI prefab;
    private readonly Transform container;

    public SlotPool(InventorySlotUI prefab, Transform container)
    {
        this.prefab = prefab;
        this.container = container;
    }

    public InventorySlotUI Get()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                pool[i].gameObject.SetActive(true);
                active.Add(pool[i]);
                return pool[i];
            }
        }
        InventorySlotUI newSlot = Object.Instantiate(prefab, container);
        pool.Add(newSlot);
        active.Add(newSlot);
        return newSlot;
    }
    public void ReleaseAll()
    {
        foreach (var slot in active) 
            slot.gameObject.SetActive(false);
        active.Clear();
    }
}
