using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("인벤토리 UI 연결")]
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Transform itemLayer;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private ItemUI itemPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private DragGhost dragGhost;
    private Image invenImg;
    private SlotPool slotPool;
    private InventoryGrid grid;
    private GridLayoutGroup gridLayoutGroup;
    private ItemLayerRenderer itemLayerRenderer;
    private InvDragController dragController;
    private readonly Dictionary<Vector2Int, InventorySlotUI> slotLookup = new Dictionary<Vector2Int, InventorySlotUI>();
    private SlotHighlighter highlighter;

    private void Awake()
    {
        // UI 초기화만 진행하도록 변경
        if (slotPool == null && slotPrefab != null && gridContainer != null)
        {
            slotPool = new SlotPool(slotPrefab, gridContainer);
        }
    }
    public void ConnectUIManager()
    {
        if (UIManager.instance != null)
        {
            invenImg = UIManager.instance.invImg;
        } 
        else
        {
            Debug.LogWarning("UIManager.Instance가 null임");
        }
    }
    private void OnDestroy() // 메모리 누수 방지
    {
        itemLayerRenderer?.Dispose();
    }
    private void ConnectGridLayout(int columnCount)
    {
        if (gridContainer == null) return;

        gridLayoutGroup = gridContainer.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup == null) return;

        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columnCount;
    }
    public void BindInventory(InventoryGrid targetGrid)
    {
        itemLayerRenderer?.Dispose();
        
        grid = targetGrid;

        if (slotPool == null)
        {
            slotPool = new SlotPool(slotPrefab, gridContainer);
        }
        ConnectGridLayout(grid.Width);

        GenerateSlot();
        highlighter = new SlotHighlighter(grid,slotLookup);
        
        var layoutGroup = gridContainer.GetComponent<GridLayoutGroup>();
        dragController = new InvDragController(
            grid, 
            dragGhost, 
            gridContainer.GetComponent<RectTransform>(),
            canvas.worldCamera,
            layoutGroup.cellSize,
            layoutGroup.spacing,
            highlighter
        );
        itemLayerRenderer = new ItemLayerRenderer(grid,itemLayer,itemPrefab,layoutGroup.cellSize,layoutGroup.spacing,dragController);

        // GenerateSlot();
    }
    public bool AddItem(ItemData item)
    {
        if (!grid.FindEmptySpace(item, out int x, out int y)) return false;
        grid.PlaceItem(item,x,y);
        return true;
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
        slotPool?.ReleaseAll();
        slotLookup.Clear();

        int totalCells = grid.Width * grid.Height;
        Debug.Log($"totalCells = {totalCells}");

        for (int i = 0; i < totalCells; i++)
        {
            InventorySlotUI slotUI = slotPool.Get();
            Vector2Int cell = new Vector2Int(i % grid.Width, i / grid.Width);
            bool isRealSlot = i < grid.TotalSlot;

            slotUI.Init(cell, grid, isRealSlot);
            slotLookup[cell] = slotUI;
        }
    }
}
