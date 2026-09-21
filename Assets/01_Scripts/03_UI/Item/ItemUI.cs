using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image itemIcon;
    private RectTransform rectTransform;

    public InventoryGrid.PlacedItemInfo ItemInfo {get; private set;}
    private InvDragController dragController;


    private void Awake()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

        rectTransform.pivot = new Vector2(0,1);
        rectTransform.anchorMin = new Vector2(0,1);
        rectTransform.anchorMax = new Vector2(0,1);
    }
    
    public void Setup(InventoryGrid.PlacedItemInfo info, Vector2 cellsize, Vector2 spacing, InvDragController dragController)
    {
        ItemInfo = info;
        this.dragController = dragController;
        itemIcon.sprite = info.item.itemIcon;
        itemIcon.enabled = info.item.itemIcon != null;

        float width = (cellsize.x * info.item.width) + (spacing.x * (info.item.width - 1));
        float height = (cellsize.y * info.item.height) + (spacing.y * (info.item.height - 1));
        rectTransform.sizeDelta = new Vector2(width,height);

        float posX = info.origin.x * (cellsize.x + spacing.x);
        float posY = -info.origin.y * (cellsize.y + spacing.y);
        rectTransform.anchoredPosition = new Vector2(posX, posY);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        itemIcon.enabled = false;
        dragController.BeginDrag(ItemInfo);
    }
    public void OnDrag(PointerEventData evnetData)
    {
        dragController.UpdateDrag(evnetData.position);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        dragController.EndDrag();
    }
}
