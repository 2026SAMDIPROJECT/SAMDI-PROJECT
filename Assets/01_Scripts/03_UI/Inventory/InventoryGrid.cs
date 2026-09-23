using System;
using UnityEngine;

public class InventoryGrid
{
    public int Width { get; }
    public int Height { get; }
    public int TotalSlot { get; private set;}

    // Dictionary 대신 1차원 Flat 배열로 관리를 더 쉽게함
    private readonly PlacedItemInfo[] gridArray;
    private readonly bool[] lockedCells;

    public event Action<PlacedItemInfo> OnItemPlaced;
    public event Action<PlacedItemInfo> OnItemRemoved;

    public void SetTotalSlot(int total)
    {
        TotalSlot += total;
    }

    public InventoryGrid(int width, int height)
    {
        Width = width;
        Height = height;
        gridArray = new PlacedItemInfo[width * height];
        lockedCells = new bool[width * height];
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private int ToIndex(int x, int y) => y * Width + x;

    public bool IsValidCell(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public bool CanPlaceItem(ItemData item, int startX, int startY, PlacedItemInfo ignoreInfo = null)
    {
        if (item == null) return false;
        if (startX < 0 || startY < 0 || startX + item.width > Width || startY + item.height > Height)
            return false;

        for (int y = startY; y < startY + item.height; y++)
        {
            for (int x = startX; x < startX + item.width; x++)
            {
                int idx = ToIndex(x, y);
                if (lockedCells[idx]) return false;

                var currentInfo = gridArray[idx];
                if (currentInfo != null && currentInfo != ignoreInfo)
                    return false;
            }
        }
        return true;
    }

    public bool PlaceItem(ItemData item, int startX, int startY)
    {
        if (!CanPlaceItem(item, startX, startY)) return false;

        var info = new PlacedItemInfo(item, new Vector2Int(startX, startY));
        for (int y = startY; y < startY + item.height; y++)
        {
            for (int x = startX; x < startX + item.width; x++)
            {
                gridArray[ToIndex(x, y)] = info;
            }
        }

        OnItemPlaced?.Invoke(info);
        return true;
    }

    public bool RemoveItem(PlacedItemInfo info)
    {
        if (info == null) return false;

        var origin = info.origin;
        var item = info.item;

        for (int y = origin.y; y < origin.y + item.height; y++)
        {
            for (int x = origin.x; x < origin.x + item.width; x++)
            {
                int idx = ToIndex(x, y);
                if (gridArray[idx] == info)
                {
                    gridArray[idx] = null;
                }
            }
        }

        OnItemRemoved?.Invoke(info);
        return true;
    }

    public PlacedItemInfo GetItemAt(int x, int y)
    {
        if (!IsValidCell(x, y)) return null;
        return gridArray[ToIndex(x, y)];
    }

    public bool FindEmptySpace(ItemData item, out int foundX, out int foundY)
    {
        for (int y = 0; y <= Height - item.height; y++)
        {
            for (int x = 0; x <= Width - item.width; x++)
            {
                if (CanPlaceItem(item, x, y))
                {
                    foundX = x;
                    foundY = y;
                    return true;
                }
            }
        }
        foundX = -1;
        foundY = -1;
        return false;
    }

    public void LockCell(int x, int y)
    {
        if (IsValidCell(x, y))
            lockedCells[ToIndex(x, y)] = true;
    }
}