using System.Collections.Generic;
using UnityEngine;

public class ItemLayerRenderer
{
    private readonly InventoryGrid grid;
    private readonly Transform itemLayer;
    private readonly ItemUI itemPrefab;
    private readonly Vector2 cellSize;
    private readonly Vector2 spacing;
    private readonly InvDragController dragController;

    private readonly Dictionary<InventoryGrid.PlacedItemInfo, ItemUI> spawndIcon 
        = new Dictionary<InventoryGrid.PlacedItemInfo, ItemUI>();
    
    public ItemLayerRenderer(InventoryGrid grid, Transform itemLayer, ItemUI itemPrefab, Vector2 cellSize, Vector2 spacing, InvDragController dragController)
    {
        this.grid = grid;
        this.itemLayer = itemLayer;
        this.itemPrefab = itemPrefab;
        this.cellSize = cellSize;
        this.spacing = spacing;
        this.dragController = dragController;

        grid.OnItemPlaced += HandleItemPlaced;
        grid.OnItemRemoved += HandleItemRemoved;
    }
    public void Dispose() // 이벤트 해제 (InventoryUI의 OnDestroy에서 호출)
    {
        grid.OnItemPlaced -= HandleItemPlaced;
        grid.OnItemRemoved -= HandleItemRemoved;
    }
    private void HandleItemPlaced(InventoryGrid.PlacedItemInfo info)
    {
        ItemUI icon = Object.Instantiate(itemPrefab, itemLayer);
        icon.Setup(info, cellSize, spacing, dragController);
        spawndIcon[info] = icon;
    }
    private void HandleItemRemoved(InventoryGrid.PlacedItemInfo info)
    {
        if (spawndIcon.TryGetValue(info, out ItemUI icon))
        {
            Object.Destroy(icon.gameObject);
            spawndIcon.Remove(info);
        }
    }
}
