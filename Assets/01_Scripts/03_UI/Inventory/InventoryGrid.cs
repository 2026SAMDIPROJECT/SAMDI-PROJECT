using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    public int gridWidth {get; private set;}
    public int gridHeight {get; private set;}

    private bool[,] gridSlotOccupid; // 2차원 배열 선언

    public InventoryGrid(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
        gridSlotOccupid = new bool[width,height];
    }
    public bool CanPlaceItem(ItemData item, int startX, int startY)
    {
        if (startX < 0 || startY < 0 || startX + item.width > gridWidth || startY + item.height > gridHeight)
            return false; // 그리드 범위 벗어났을때
        for (int x = startX ; x < startX + item.width ; x++)
            for (int y = startY ; y < startY + item.height; y++)
                if (gridSlotOccupid[x,y]) return false;
        return true;
    }
    // 아이템 배치
    public void PlaceItem(ItemData item, int startX, int startY)
    {
        for (int x = startX ; x < startX + item.width ; x++)
            for (int y = startY ; y < startY + item.height; y++)
                gridSlotOccupid[x,y] = true;
    }
    public bool FindEmetySpace(ItemData item, out int foundX, out int foundY)
    {
        for (int y = 0; y <= gridHeight - item.height ; y++)
            for (int x = 0; x <= gridWidth - item.width; x++)
                if (CanPlaceItem(item, x, y))
                {
                    foundX = x;
                    foundY = y;
                    return true;
                }
        foundX = -1;
        foundY = -1;
        return false;
    }
}
