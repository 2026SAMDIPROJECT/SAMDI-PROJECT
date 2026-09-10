using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    public int SlotIndex {get; private set;}

    public void Init(int index)
    {
        SlotIndex = index;
        ClearSlot();
    }
    
    public void UpdateSlot(Sprite icon, int count)
    {
        if (icon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.enabled = true;
            quantityText.text = count > 1 ? count.ToString() : "";
            quantityText.enabled = count > 1;
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
        quantityText.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO: 클릭시 아이템 정보 표시 또는 사용
    }
    public void OnDrop(PointerEventData eventData)
    {
        // TODO: 드래그 앤 드롭으로 슬롯 위치 교환 처리
    }
}
