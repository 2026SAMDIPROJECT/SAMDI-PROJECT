using UnityEngine;
public enum ItemType // 아이템 타입
{
    Consumable,
    Equipment,
    Material
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public int itemID;
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    [TextArea] public string description;

    [Header("드롭")]
    public GameObject dropObject;
}
