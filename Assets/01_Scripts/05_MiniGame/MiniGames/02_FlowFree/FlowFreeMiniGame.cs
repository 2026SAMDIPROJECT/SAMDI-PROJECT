using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class FlowFreeMiniGame : MiniGameManager
{
    private class FlowPath
    {
        public readonly List<Vector2Int> cells = new List<Vector2Int>();
        public List<FlowFreeUI> activeLines = new List<FlowFreeUI>();
        public Vector2Int start;
        public bool complete = false;
        public Color color;

        public void Reset()
        {
            cells.Clear();
            complete = false;
        }
    }
    [SerializeField] private Transform gridBoard;
    [SerializeField] private GameObject gridCell;
    [SerializeField] private FlowFreeUI uiPrefab;
    [SerializeField] private FlowFreeInput input;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float boardSize = 600f;
    private float cellSize;
    private float numSize;
    private int size;
    private IObjectPool<GameObject> gridCellPool;
    private IObjectPool<FlowFreeUI> uiPool;
    private int[,] grid;
    private int[,] lineGrid;
    private bool[,] occupied;
    private FlowFreeUI[,] dotUI;
    private int score;
    private float fillRatio = 0.6f;
    private List<Vector2Int> starts = new List<Vector2Int>();
    private List<Vector2Int> ends = new List<Vector2Int>();
    private List<FlowPath> paths = new List<FlowPath>();
    private List<GameObject> activeCells = new List<GameObject>();
    private List<FlowFreeUI> activeDots = new List<FlowFreeUI>();
    private HashSet<Vector3> usedColor = new HashSet<Vector3>();
    private int selectedNum;
    private Vector2Int lastCell = Vector2Int.zero;
    private bool dragging;

    private System.Random rng = new System.Random();

    private static readonly Vector2Int[] dirs =
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    public override void OnNetworkSpawn()
    {
        canvas.gameObject.SetActive(true);
        if(!IsOwner) return;
        gridCellPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(gridCell, Vector3.zero, Quaternion.identity, gridBoard),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 64,
            maxSize: 1024
        );

        uiPool = new ObjectPool<FlowFreeUI>(
            createFunc: () => Instantiate(uiPrefab, Vector3.zero, Quaternion.identity, gridBoard),
            actionOnGet: (obj) => obj.gameObject.SetActive(true),
            actionOnRelease: (obj) => obj.gameObject.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 200,
            maxSize: 1000
        );

        input.Down += OnDown;
        input.Drag += OnDrag;
        input.Up += OnUp;

        Cursor.lockState = CursorLockMode.None;

        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        input.Down -= OnDown;
        input.Drag -= OnDrag;
        input.Up -= OnUp;

        ClearBoard();
    }

    private int DirToBit(Vector2Int dir)
    {
        if(dir == Vector2Int.up) return 1;
        if(dir == Vector2Int.right) return 2;
        if(dir == Vector2Int.down) return 4;
        return 8; // left
    }

    private LineState BitToLine(int bit) => bit switch
    {
        1 => LineState.UP, // 0b_0001
        2 => LineState.RIGHT, // 0b_0010
        4 => LineState.DOWN, // 0b_0100
        8 => LineState.LEFT, // 0b_1000
        5 => LineState.UP_DOWN, // 0b_0101, UP | DOWN
        10 => LineState.LEFT_RIGHT, // 0b_1010, RIGHT | LEFT
        3 => LineState.UP_RIGHT, // 0b_0011, UP | RIGHT
        9 => LineState.UP_LEFT, // 0b_1001, UP | LEFT
        6 => LineState.DOWN_RIGHT, // 0b_0110, DOWN | RIGHT
        12 => LineState.DOWN_LEFT, // 0b_1100, DOWN | LEFT
        _ => LineState.NON
    };

    private DotState BitToDot(int bit) => bit switch
    {
        1 => DotState.UP, // 0b_0001
        2 => DotState.RIGHT, // 0b_0010
        4 => DotState.DOWN, // 0b_0100
        8 => DotState.LEFT, // 0b_1000
        _ => DotState.NON
    };

    private int DirBit(FlowPath path, int i)
    {
        int bit = 0;
        if(i > 0) bit |= DirToBit(path.cells[i - 1] - path.cells[i]); // (이전 칸 - 현재 칸)으로 방향 구해서 파라미터로 줌
        if(i < path.cells.Count - 1) bit |= DirToBit(path.cells[i + 1] - path.cells[i]);
        return bit;
    }

    private void OnDown(Vector2 pos)
    {
        if(TryGetCell(pos, out var cellPos)) BeginDrag(cellPos);
    }

    private void OnDrag(Vector2 pos)
    {
        if(TryGetCell(pos, out var cellPos)) UpdateDrag(cellPos);
    }

    private void OnUp() => EndDrag();

    private bool TryGetCell(Vector2 mousePos, out Vector2Int cellPos)
    {
        int cellX = Mathf.FloorToInt(mousePos.x / cellSize + size * 0.5f);
        int cellY = Mathf.FloorToInt(mousePos.y / cellSize + size * 0.5f);
        cellPos = new Vector2Int(cellX, cellY);

        return cellX >= 0 && cellY >= 0 && cellX < size && cellY < size;
    }

    private void BeginDrag(Vector2Int cellPos)
    {
        if(grid == null) return;
        dragging = true;
        int cellNum = grid[cellPos.x, cellPos.y];

        if(cellNum != 0)
        {
            CutPath(cellNum, 0);
            paths[cellNum - 1].cells.Add(cellPos);
            paths[cellNum - 1].start = cellPos;
            paths[cellNum - 1].activeLines.Add(null);
            lineGrid[cellPos.x, cellPos.y] = cellNum;
        }
        else
        {
            cellNum = lineGrid[cellPos.x, cellPos.y];
            if(cellNum == 0) return;
            CutPath(cellNum, paths[cellNum - 1].cells.IndexOf(cellPos) + 1);
        }

        selectedNum = cellNum;
        lastCell = cellPos;
    }

    private void UpdateDrag(Vector2Int cellPos)
    {
        while(selectedNum != 0 && lastCell != cellPos) // 반복문은 포인터 점프 때문
        {
            var dis = cellPos - lastCell;
            var step = Mathf.Abs(dis.x) >= Mathf.Abs(dis.y)
            ? new Vector2Int((int)Mathf.Sign(dis.x), 0)
            : new Vector2Int(0, (int)Mathf.Sign(dis.y));

            var next = lastCell + step;
            if(!TryEnter(next)) break; // 실패 시 멈춤
            lastCell = next;
        }
    }

    private void RefreshShape(FlowPath path, int i)
    {
        if(i < 0 || i >= path.cells.Count) return;
        var ui = path.activeLines[i];
        if(ui == null) 
        {
            RefreshDot(path, i);
            return;                                   // 점 칸은 조각이 없음
        }

        ui.SetLine(BitToLine(DirBit(path, i)));
    }

    private void RefreshDot(FlowPath path, int i)
    {
        var dotPos = path.cells[i];
        var dot = dotUI[dotPos.x, dotPos.y];

        if(dot == null) return;
        dot.SetDot(BitToDot(DirBit(path, i)));
    }

    private void ResetDot(Vector2Int pos)
    {
        var ui = dotUI[pos.x, pos.y];
        if(ui != null) ui.SetDot(DotState.NON);
    }

    private void EndDrag()
    {
        if(!dragging) return;
        dragging = false;
        selectedNum = 0;
        foreach(var path in paths)
            if(!path.complete) return;
        
        Debug.Log("Complete puzzle!");
    }

    private void CutPath(int selected, int keep)
    {
        var path = paths[selected - 1];

        for(int i = path.cells.Count - 1; i >= keep; i--) // count - 1인건 인덱스 때문
        {
            var cell = path.cells[i];
            lineGrid[cell.x, cell.y] = 0;

            if(path.activeLines[i] != null)
                uiPool.Release(path.activeLines[i]);
            else
                ResetDot(cell);

            path.activeLines.RemoveAt(i);
            path.cells.RemoveAt(i);
        }

        path.complete = false; // 끊었으니 실패로 처리
        if(path.cells.Count > 0)
            RefreshShape(path, path.cells.Count - 1);
    }

    private bool TryEnter(Vector2Int cell)
    {
        var path = paths[selectedNum - 1];

        int index = path.cells.IndexOf(cell);
        if(index >= 0)
        {
            CutPath(selectedNum, index + 1);
            return true; // 선 자르고 거기서부터 다시 선 잇기 시작이기에 true로 반환함.
        }

        int diffNum = grid[cell.x, cell.y];
        if(diffNum != 0 && diffNum != selectedNum) return false; // 다른 색 점에 드래그 -> 실패

        int diffLine = lineGrid[cell.x, cell.y];
        if(diffLine != 0 && diffLine != selectedNum) return false; // 다른 색 선에 드래그 -> 실패

        FlowFreeUI line = null;
        if(diffNum == 0)
        {
            line = uiPool.Get();
            if(line.transform is RectTransform rt)
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = CellPosition(cell.x, cell.y);
                rt.sizeDelta = new Vector2(cellSize, cellSize);
            }
            (line.transform as RectTransform).anchoredPosition = CellPosition(cell.x, cell.y);
            line.SetColor(path.color);

        }
        path.cells.Add(cell);
        path.activeLines.Add(line);
        lineGrid[cell.x, cell.y] = selectedNum;

        int num = path.cells.Count;
        RefreshShape(path, num - 1);
        RefreshShape(path, num - 2);

        if(diffNum == selectedNum && path.start != cell)
        {
            path.complete = true;
            selectedNum = 0;
        }

        return true; // 기본 성공
    }

    public override void OnStartGame()
    {
        gridBoard.gameObject.SetActive(true);

        if(gridCellPool == null)
        {
            Debug.LogError("격자 칸 풀 없음");
            return;
        }

        Generate(5);
    }


    private bool SetupSize(int newSize)
    {
        if(gridBoard is RectTransform gridRect)
        {
            size = newSize;

            gridRect.anchorMin = gridRect.anchorMax = gridRect.pivot = new Vector2(0.5f, 0.5f);
            gridRect.anchoredPosition = Vector2.zero;
            gridRect.sizeDelta = new Vector2(boardSize, boardSize); 

            cellSize = boardSize / size;
            numSize = cellSize / 2;
            return true;
        }
        else
        {
            Debug.LogError("Error :: gridBoard가 RectTransform이 아님");
            return false;
        }
    }

    private Vector2 CellPosition(int x, int y)
    {
        float offset = (size - 1) * cellSize * 0.5f;
        return new Vector2(x * cellSize - offset, y * cellSize - offset);
    }

   private void Generate(int size)
    {
        ClearBoard();
        if(!SetupSize(size)) return;

        grid = new int[size, size];
        occupied = new bool[size, size];
        lineGrid = new int[size, size];
        dotUI = new FlowFreeUI[size, size];

        int maxCount = size * size * 2;                         // 5x5면 색 5개 정도
        int longCount = Mathf.Max(1, maxCount / 3);
        int total = size * size;

        int easyCount = 0;
        int easyMax = size - 2;

        var solutions = new List<List<Vector2Int>>();     // 반복문 밖에서 한 번만

        for(int attempt = 0; attempt < maxCount * 3 && solutions.Count < maxCount; attempt++)
        {
            Vector2Int startPos = new Vector2Int(rng.Next(0, size), rng.Next(0, size));
            if(occupied[startPos.x, startPos.y]) continue;

            int minLen = easyCount < easyMax ? 2 : 3;

            bool isLong = solutions.Count < longCount;    // 건너뛴 시도가 긴 선 자리를 먹지 않게
            int length = isLong
                ? rng.Next(total / 4, total / 2 + 1)
                : rng.Next(minLen, minLen + 2);

            var path = new List<Vector2Int> { startPos };
            occupied[startPos.x, startPos.y] = true;

            for(int j = 1; j < length; j++)
            {
                Vector2Int cur = path[path.Count - 1];

                var candidates = new List<Vector2Int>(4);
                foreach(var d in dirs)
                {
                    Vector2Int n = cur + d;
                    if(n.x < 0 || n.y < 0 || n.x >= size || n.y >= size) continue;
                    if(occupied[n.x, n.y]) continue;
                    candidates.Add(n);
                }
                if(candidates.Count == 0) break;           // 막다른 길: 여기서 끝

                Vector2Int next;
                if(rng.NextDouble() < 0.7)                 // 벽 따라가기
                {
                    int best = int.MaxValue;
                    next = candidates[0];
                    foreach(var candidate in candidates)
                    {
                        int free = FreeNeighbors(candidate);
                        if(free < best || (free == best && rng.Next(2) == 0))
                        {
                            best = free;
                            next = candidate;
                        }
                    }
                }
                else
                {
                    next = candidates[rng.Next(candidates.Count)];
                }
                path.Add(next);
                occupied[next.x, next.y] = true;
            }

            Vector2Int endPos = path[path.Count - 1];
            if(startPos == endPos)
            {
                occupied[endPos.x, endPos.y] = false;
                continue;
            }

            if(IsAdjacent(startPos, endPos)) easyCount++;
            solutions.Add(path);                           // 확정된 경로 보관
        }

        // 남은 빈 칸을 선 끝으로 흡수 (반복문이 끝난 뒤 한 번)
        bool grew = true;
        while(grew)
        {
            grew = false;
            foreach(var solution in solutions)
            {
                var tail = solution[solution.Count - 1];
                foreach(var dir in dirs)
                {
                    var n = tail + dir;
                    if(n.x < 0 || n.y < 0 || n.x >= size || n.y >= size) continue;
                    if(occupied[n.x, n.y]) continue;
                    solution.Add(n);
                    occupied[n.x, n.y] = true;
                    tail = n;
                    grew = true;
                }
            }
        }

        // 연장된 경로에서 점 좌표를 다시 뽑는다
        starts.Clear();
        ends.Clear();
        foreach(var solution in solutions)
        {
            starts.Add(solution[0]);
            ends.Add(solution[solution.Count - 1]);
        }

        for(int i = 0; i < starts.Count; i++)
        {
            grid[starts[i].x, starts[i].y] = i + 1;   // 색 번호 내부 지정
            grid[ends[i].x, ends[i].y] = i + 1;
        }

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                GameObject cell = gridCellPool.Get();
                if(cell.transform is RectTransform rt)
                {
                    rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f); // 앵커를 부모 중앙으로
                    rt.sizeDelta = new Vector2(cellSize, cellSize); // 칸 크기와 간격을 일치시킴
                    rt.anchoredPosition = CellPosition(i, j);
                    activeCells.Add(cell);
                }
                else
                {
                    Debug.LogError("셀의 Transform이 Rect가 아님, 재시작 필요");
                }
            }
        }

        Color selectedCol;

        for(int i = 0; i < starts.Count; i++)
        {
            FlowFreeUI start = uiPool.Get();
            selectedCol = RandomColor();
            Vector2Int startDot = starts[i];

            if(start.transform is RectTransform startRt)
            {
                startRt.anchorMin = startRt.anchorMax = startRt.pivot = new Vector2(0.5f, 0.5f);
                startRt.sizeDelta = new Vector2(cellSize, cellSize);
                startRt.anchoredPosition = CellPosition(startDot.x, startDot.y);

                start.isDot = true;
                start.SetDot(DotState.NON);
                start.SetColor(selectedCol, i + 1);
                start.SetNumSize(numSize);

                activeDots.Add(start);
                dotUI[startDot.x, startDot.y] = start;
            }

            FlowFreeUI end = uiPool.Get();
            Vector2Int endDot = ends[i];

            if(end.transform is RectTransform endRt)
            {
                endRt.anchorMin = endRt.anchorMax = endRt.pivot = new Vector2(0.5f, 0.5f);
                endRt.sizeDelta = new Vector2(cellSize, cellSize);
                endRt.anchoredPosition = CellPosition(endDot.x, endDot.y);

                end.isDot = true;
                end.SetDot(DotState.NON);
                end.SetColor(selectedCol, i + 1);
                end.SetNumSize(numSize);

                activeDots.Add(end);
                dotUI[endDot.x, endDot.y] = end;
            }

            paths.Add(new FlowPath{ color = selectedCol });
        }
    }

    private int FreeNeighbors(Vector2Int cell) // 4방향 비어있는 칸 계산    
    {
        int n = 0;
        foreach(var d in dirs)
        {
            var pos = cell + d;
            if(pos.x < 0 || pos.y < 0 || pos.x >= size || pos.y >= size) continue;
            if(!occupied[pos.x, pos.y]) n++;
        }
        return n;
    }

    private Color RandomColor()
    {
        float randH = 0f;
        float randS = 0f;
        float randV = 0f;
        bool isUnique = false;
        float minH = 0.2f;
        float minS = 0.1f;
        float minV = 0.1f;

        for(int att = 0; !isUnique && att < 50; att++)
        {
            randH = Random.Range(0f, 1f);
            randS = Random.Range(0.5f, 1f);
            randV = Random.Range(0.6f, 1f);
            isUnique = true;

            foreach(Vector3 item in usedColor)
            {
                if(Mathf.Abs(randH - item.x) < minH && Mathf.Abs(randS - item.y) < minS && Mathf.Abs(randV - item.z) < minV)
                {
                    isUnique = false;
                    break;
                }
            }
        }

        usedColor.Add(new Vector3(randH, randS, randV));

        return Color.HSVToRGB(randH, randS, randV);
    }

    private void ClearBoard()
    {
        foreach(GameObject cell in activeCells)
        {
            if(cell != null)
                gridCellPool.Release(cell);
        }
        activeCells.Clear();

        foreach(FlowFreeUI dot in activeDots)
        {
            if(dot != null)
                uiPool.Release(dot);
        }
        activeDots.Clear();

        foreach(var path in paths)
        {
            foreach(var line in path.activeLines)
            {
                if(line != null)
                {
                    uiPool.Release(line);
                }
            }
        }
        paths.Clear();
        usedColor.Clear();
        starts.Clear();
        ends.Clear();
    }

    private bool IsAdjacent(Vector2Int start, Vector2Int end) => Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y) == 1;
}