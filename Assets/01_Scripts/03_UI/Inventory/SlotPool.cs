using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool; // 오브젝트 풀링을 쉽게 사용 가능한 모듈 추가
// 오브젝트 폴링 전달
public class SlotPool
{
    private readonly IObjectPool<InventorySlotUI> pool;
    // private readonly List<InventorySlotUI> active = new List<InventorySlotUI>();
    private readonly InventorySlotUI prefab;
    private readonly Transform container;

    private readonly List<InventorySlotUI> activeList = new List<InventorySlotUI>();

    public SlotPool(InventorySlotUI prefab, Transform container, int defaultCapacity = 0, int maxSize = 100)
    {
        this.prefab = prefab;
        this.container = container;

        pool = new ObjectPool<InventorySlotUI>(
            createFunc: () => Object.Instantiate(this.prefab, this.container),
            actionOnGet: slot =>
            {
                slot.gameObject.SetActive(true);
                if (!activeList.Contains(slot))
                    activeList.Add(slot);
            },
            actionOnRelease: slot => slot.gameObject.SetActive(false),
            actionOnDestroy: slot => Object.Destroy(slot.gameObject),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }
    public InventorySlotUI Get() => pool.Get();
    public void ReleaseAll()
    {
        for (int i = 0; i < activeList.Count; i++)
            pool.Release(activeList[i]);
        activeList.Clear();
    }
}
