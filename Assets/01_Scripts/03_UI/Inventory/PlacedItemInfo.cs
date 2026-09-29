using UnityEngine;

public class PlacedItemInfo // 사용하는 곳이 많아 별도의 클래스로 분리
{
    public ItemData item { get; }
    public Vector2Int origin { get; set; }

    public PlacedItemInfo(ItemData item, Vector2Int origin)
    {
        this.item = item;
        this.origin = origin;
    }
}
