using UnityEngine;
// 그리드 계산 및 잠금 로직 전달
public class InventoryGridBuilder
{
    public static InventoryGrid Build(int totalSlot, int columns)
    {
        int rows = Mathf.CeilToInt((float)totalSlot / columns);
        var grid = new InventoryGrid(columns,rows);

        int totalCells = columns * rows;
        for (int i = totalSlot; i < totalCells; i++)
        {
            grid.LockCell(i % columns, i / columns);
        }
        return grid;
    }
}
