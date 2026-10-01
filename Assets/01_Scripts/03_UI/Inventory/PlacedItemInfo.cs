using UnityEngine;

public struct PlacedItemInfo // 사용하는 곳이 많아 별도의 클래스로 분리
{
    public readonly ItemData item;
    public Vector2Int origin;

    public PlacedItemInfo(ItemData item, Vector2Int origin)
    {
        this.item = item;
        this.origin = origin;
    }
}
