using System;
using System.Collections.Generic;
using UnityEngine;
public class InventoryGrid
{
    public int gridWidth { get; private set; }
    public int gridHeight { get; private set; }
    
    // true: 이미 아이템이 차지함, false: 빈 공간
    private bool[,] gridSlotOccupied;
    // 칸에서 아이템으로 역추적을 위한 코드
    private Dictionary<Vector2Int, PlacedItemInfo> placedItem = new Dictionary<Vector2Int, PlacedItemInfo>();

    // UI가 구독할 이벤트
    public event Action<PlacedItemInfo> OnItemPlaced;
    // public event Action<PlacedItemInfo> OnItemRemoved;

    public InventoryGrid(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
        gridSlotOccupied = new bool[width, height];
    }

    // (startX, startY) 위치에 itemData(w, h)를 놓을 수 있는지 검사
    public bool CanPlaceItem(ItemData item, int startX, int startY)
    {
        if (startX < 0 || startY < 0 || startX + item.width > gridWidth || startY + item.height > gridHeight)
            return false; // 그리드 범위를 벗어남

        for (int x = startX; x < startX + item.width; x++)
            for (int y = startY; y < startY + item.height; y++)
                if (gridSlotOccupied[x, y]) return false; // 이미 다른 아이템이 있음
        return true;
    }

    // 아이템 배치 (점유 처리)
    public void PlaceItem(ItemData item, int startX, int startY)
    {
        var info = new PlacedItemInfo(item, new Vector2Int(startX, startY));
        for (int x = startX; x < startX + item.width; x++)
            for (int y = startY; y < startY + item.height; y++)
            {
                gridSlotOccupied[x, y] = true;
                placedItem[new Vector2Int(x,y)] = info; // 역추적
            }
        OnItemPlaced?.Invoke(info);
    }
    // 아이템 제거 기능
    public bool RemoveItemAt(Vector2Int cell)
    {
        if (!placedItem.TryGetValue(cell, out var info)) return false;

        var item = info.item;
        var origin = info.origin;

        for(int x = origin.x; x < origin.x + item.width ; x++) 
            for (int y = origin.y; y < origin.y + item.height ; y++)
            {
                gridSlotOccupied[x,y] = false;
                placedItem.Remove(new Vector2Int(x,y));
            }
        OnItemPlaced?.Invoke(info);
        return true;
    }
    // 자동 획득용: 빈 공간(X, Y) 찾기
    public bool FindEmptySpace(ItemData item, out int foundX, out int foundY)
    {
        for (int y = 0; y <= gridHeight - item.height; y++)
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
    public bool TryGetItemAt(Vector2Int cell, out PlacedItemInfo info)
    {
        return placedItem.TryGetValue(cell, out info);
    }
    // 불규칙한 수 (예: 줄 한개가 5개인데 23칸의 슬롯이 있으면 나머지 2개는 막음)
    public void LockCell (int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
            gridSlotOccupied[x,y] = true;
    }
    public class PlacedItemInfo
    {
        public ItemData item {get;}
        public Vector2Int origin {get;}

        public PlacedItemInfo(ItemData data, Vector2Int Origin)
        {
            item = data;
            origin = Origin;
        }
    }
}