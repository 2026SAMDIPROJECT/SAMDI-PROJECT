using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private CanvasGroup canvasGroup;
    public Vector2Int Cell {get; private set;}
    private InventoryGrid grid;

    public void Init(Vector2Int cell, InventoryGrid grid, bool isRealSlot)
    {
        Cell = cell;
        this.grid = grid;
        ClearSlot();
        SetInteractable(isRealSlot);
    }
    public void SetInteractable(bool interactable)
    {
        if (canvasGroup == null) return;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
        canvasGroup.alpha = interactable ? 1f : 0f;
    }
    public void UpdateSlot(Sprite icon)
    {
        if (icon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.enabled = true;
        }
        else
        {
            ClearSlot();
        }
    }
    public void ClearSlot()
    {
        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (grid.TryGetItemAt(Cell, out InventoryGrid.PlacedItemInfo info))
            Debug.Log($"클릭한 아이템 : {info.item.name}");
    }
    public void OnDrop(PointerEventData eventData)
    {
        // TODO: 드래그 앤 드롭으로 슬롯 위치 교환 처리
    }
}
