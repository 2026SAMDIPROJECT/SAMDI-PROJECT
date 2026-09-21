using UnityEngine;

public class InvDragController
{
    private readonly InventoryGrid grid;
    private readonly DragGhost ghost;
    private readonly RectTransform gridContainer;
    private readonly Camera uiCamera;
    private readonly Vector2 cellSize;
    private readonly Vector2 spacing;

    private InventoryGrid.PlacedItemInfo draggingItem;
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
    private bool CanPlaceIgnoringSelf(InventoryGrid.PlacedItemInfo item, Vector2Int targetCell)
    {
        //자기 자신 위치는 임시로 비웠다가 검사 후 복구함
        return grid.CanPlaceIgnoring(item.item, targetCell.x, targetCell.y, item.origin, item.item);
    }

    public void BeginDrag(InventoryGrid.PlacedItemInfo item)
    {
        draggingItem = item;
        ghost.show(item.item.itemIcon, CalcSize(item.item));
    }
    public void UpdateDrag(Vector2 screenPos)
    {
        if (draggingItem == null) return;

        ghost.UpdatePosition(screenPos);

        currentTargetcell = GridCoordinateConverter.WorldToCell(gridContainer, screenPos, uiCamera, cellSize, spacing);
        // 자기 자신이 있었던 칸은 비운 상태로 가정하고 검사해야하므로, 제자리 이동도 유효하게 판단되도록 처리
        currentTargetValid = CanPlaceIgnoringSelf(draggingItem, currentTargetcell);
        ghost.SetValid(currentTargetValid);
    }
    public void EndDrag()
    {
        if(draggingItem == null) return;

        if (currentTargetValid)
        {
            grid.RemoveItemAt(draggingItem.origin);
            grid.PlaceItem(draggingItem.item, currentTargetcell.x, currentTargetcell.y);
        }
        ghost.hide();
        draggingItem = null;
    }
}
