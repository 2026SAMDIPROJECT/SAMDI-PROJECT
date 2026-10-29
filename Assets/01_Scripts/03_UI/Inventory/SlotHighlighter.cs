using System.Collections.Generic;
using UnityEngine;

public class SlotHighlighter
{
    private readonly InventoryGrid grid;
    private readonly Dictionary<Vector2Int, InventorySlotUI> slotLookup;
    private readonly List<Vector2Int> currentHighlight = new List<Vector2Int>();

    public SlotHighlighter(InventoryGrid grid, Dictionary<Vector2Int, InventorySlotUI> slotLookup)
    {
        this.grid = grid;
        this.slotLookup = slotLookup;
    }
    public void UpdateHighlight(ItemData itemData, Vector2Int targetCell)
    {
        ClearHighlight();

        for (int y = targetCell.y ; y < targetCell.y + itemData.height ; y++)
        {
            for (int x = targetCell.x ; x < targetCell.x + itemData.width ; x++)
            {
                if (!grid.IsValidCell(x,y)) continue;

                var cell = new Vector2Int(x,y);
                if (slotLookup.TryGetValue(cell, out var slotUI))
                {
                    slotUI.SetHighlight(true);
                    currentHighlight.Add(cell);
                }
            }
        }
    }
    public void ClearHighlight()
    {
        foreach(var cell in currentHighlight)
        {
            if (slotLookup.TryGetValue(cell, out var slotUI))
                slotUI.SetHighlight(false);
        }
        currentHighlight.Clear();
    }

}
