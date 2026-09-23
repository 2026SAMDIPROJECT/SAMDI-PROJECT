using UnityEngine;

public class InvDragController
{
    private readonly InventoryGrid grid;
    private readonly DragGhost ghost;
    private readonly RectTransform gridContainer;
    private readonly Camera uiCamera;
    private readonly Vector2 cellSize;
    private readonly Vector2 spacing;

    private PlacedItemInfo draggingItem;
    private Vector2Int currentTargetcell;
    private bool currentTargetValid;

    public InvDragController(InventoryGrid grid, DragGhost ghost, RectTransform gridContainer, Camera uiCamera, Vector2 cellSize, Vector2 spacing)
    {
        this.grid = grid;
        this.ghost = ghost;
        this.gridContainer = gridContainer;
        this.uiCamera = uiCamera;
        this.cellSize = cellSize;
        this.spacing = spacing;
    }
    private Vector2 CalcSize(ItemData item)
    {
        float w = cellSize.x * item.width + spacing.x * (item.width - 1);
        float h = cellSize.y * item.height + spacing.y * (item.height - 1);
        return new Vector2(w,h);
    }

    public void BeginDrag(PlacedItemInfo item)
    {
        draggingItem = item;
        ghost.show(item.item.itemIcon, CalcSize(item.item));
    }
    public void UpdateDrag(Vector2 screenPos)
    {
        if (draggingItem == null) return;

        ghost.UpdatePosition(screenPos);
        // 마우스 커서 위치 셀 좌표로 변환 및 origin(피벗)을 드래그 기준점으로 잡음
        Vector2Int mouseCell = GridCoordinateConverter.WorldToCell(gridContainer, screenPos, uiCamera, cellSize, spacing);
        currentTargetcell = mouseCell;
        
        // 자기 자신이 있었던 칸은 비운 상태로 가정하고 검사해야하므로, 제자리 이동도 유효하게 판단되도록 처리
        currentTargetValid = grid.CanPlaceItem(draggingItem.item, currentTargetcell.x, currentTargetcell.y, draggingItem);
        ghost.SetValid(currentTargetValid);
    }
    public void EndDrag()
    {
        if(draggingItem == null) return;

        if (currentTargetValid)
        {
            grid.RemoveItem(draggingItem);
            grid.PlaceItem(draggingItem.item, currentTargetcell.x, currentTargetcell.y);
        }
        ghost.hide();
        draggingItem = null;
    }
}
