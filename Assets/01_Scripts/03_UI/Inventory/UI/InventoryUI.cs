using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private int totalSlot = 20;
    [SerializeField] private int columns = 5;

    private SlotPool slotPool;
    private InventoryGrid grid;
    private GridLayoutGroup gridLayoutGroup;


    private void Awake()
    {
        ConnectGridLayout();
        grid = InventoryGridBuilder.Build(totalSlot, columns);
        slotPool = new SlotPool(slotPrefab, gridContainer);

        GenerateSlot();
    }
    private void OnValidate()
    {
        ConnectGridLayout();
    }
    private void ConnectGridLayout()
    {
        if (gridContainer == null) return;

        var gridLayout = gridContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout == null) return;

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
    }

    public void ToggleInventory()
    {
        bool isActive = !inventoryCanvas.activeSelf;
        inventoryCanvas.SetActive(isActive);

        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isActive;
    }
    public void GenerateSlot()
    {
        slotPool.ReleaseAll();

        int totalCells = grid.gridWidth * grid.gridHeight;
        Debug.Log($"totalCells = {totalCells}");

        for (int i = 0; i < totalCells; i++)
        {
            InventorySlotUI slotUI = slotPool.Get();
            Vector2Int cell = new Vector2Int(i % columns, i / columns);
            bool isRealSlot = i < totalSlot;

            slotUI.Init(cell, grid, isRealSlot);
        }
    }
}
