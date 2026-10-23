using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlowFreeInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public event Action<Vector2> Down;
    public event Action<Vector2> Drag;
    public event Action Up;
    [SerializeField] private RectTransform rect;

    public void OnPointerDown(PointerEventData data)
    {
        Debug.Log("down_input");
        if(data.button != PointerEventData.InputButton.Left || !LocalPos(data, out var local))
            return;

        Debug.Log("down_left");
        Down?.Invoke(local);
    }

    public void OnDrag(PointerEventData data)
    {
        if(!LocalPos(data, out var local)) return;
        Drag?.Invoke(local);
    }

    public void OnPointerUp(PointerEventData data)
    {
        Up?.Invoke();
    }

    private bool LocalPos(PointerEventData data, out Vector2 local)
    => RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, data.position, data.pressEventCamera, out local);
}