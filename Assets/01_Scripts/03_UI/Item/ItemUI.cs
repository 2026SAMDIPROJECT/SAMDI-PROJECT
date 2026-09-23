using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    public PlacedItemInfo ItemInfo {get; private set;}
    private InvDragController dragController;


    private void Awake()
    {

        rectTransform.pivot = new Vector2(0,1);
        rectTransform.anchorMin = new Vector2(0,1);
        rectTransform.anchorMax = new Vector2(0,1);
    }
    
    public void Setup(PlacedItemInfo info, Vector2 cellsize, Vector2 spacing, InvDragController dragController)
    {
        ItemInfo = info;
        this.dragController = dragController;
        
        bool hasIcon = info.item != null && info.item.itemIcon != null;
        itemIcon.sprite = hasIcon? info.item.itemIcon : null;
        itemIcon.enabled = hasIcon;

        //오브젝트 풀링 대응
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        float width = (cellsize.x * info.item.width) + (spacing.x * (info.item.width - 1));
        float height = (cellsize.y * info.item.height) + (spacing.y * (info.item.height - 1));
        Vector2 targetSize = new Vector2(width,height);

        if (rectTransform.sizeDelta != targetSize)
            rectTransform.sizeDelta = targetSize;

        float posX = info.origin.x * (cellsize.x + spacing.x);
        float posY = -info.origin.y * (cellsize.y + spacing.y);
        Vector2 targetPos = new Vector2 (posX, posY);
        
        if (rectTransform.anchoredPosition != targetPos)
            rectTransform.anchoredPosition = targetPos;
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
    private void OnDisable()
    {
        // 풀링 회수시 상태 초기화
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
