using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ItemLayerRenderer
{
    private readonly InventoryGrid grid;
    private readonly Transform itemLayer;
    private readonly ItemUI itemPrefab;
    private readonly Vector2 cellSize;
    private readonly Vector2 spacing;
    private readonly InvDragController dragController;

    private readonly Dictionary<PlacedItemInfo, ItemUI> spawndIcon 
        = new Dictionary<PlacedItemInfo, ItemUI>();
    private readonly IObjectPool<ItemUI> itemUiPool;
    
    public ItemLayerRenderer(InventoryGrid grid, Transform itemLayer, ItemUI itemPrefab, Vector2 cellSize, Vector2 spacing, InvDragController dragController)
    {
        this.grid = grid;
        this.itemLayer = itemLayer;
        this.itemPrefab = itemPrefab;
        this.cellSize = cellSize;
        this.spacing = spacing;
        this.dragController = dragController;

        itemUiPool = new ObjectPool<ItemUI> (
            createFunc: () => Object.Instantiate(this.itemPrefab, this.itemLayer),
            actionOnGet: itemUi => itemUi.gameObject.SetActive(true),
            actionOnRelease: itemUi => itemUi.gameObject.SetActive(false),
            actionOnDestroy: itemUi => Object.Destroy(itemUi.gameObject),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 50
        );

        grid.OnItemPlaced += HandleItemPlaced;
        grid.OnItemRemoved += HandleItemRemoved;
    }
    public void Dispose() // 이벤트 해제 (InventoryUI의 OnDestroy에서 호출)
    {
        grid.OnItemPlaced -= HandleItemPlaced;
        grid.OnItemRemoved -= HandleItemRemoved;
    }
    private void HandleItemPlaced(PlacedItemInfo info)
    {
        ItemUI icon = itemUiPool.Get();
        icon.Setup(info, cellSize, spacing, dragController);
        spawndIcon[info] = icon;
    }
    private void HandleItemRemoved(PlacedItemInfo info)
    {
        if (spawndIcon.TryGetValue(info, out ItemUI icon))
        {
            itemUiPool.Release(icon);
            spawndIcon.Remove(info);
        }
    }
}
