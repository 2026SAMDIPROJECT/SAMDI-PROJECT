using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class FlowFreeMiniGame : MiniGameManager
{
    [SerializeField] private Transform gridBoard;
    [SerializeField] private GameObject gridCell;
    [SerializeField] private GameObject[] dots;
    [SerializeField] private GameObject[] lines;
    [SerializeField] private Canvas canvas;
    private IObjectPool<GameObject> gridCellPool;
    private IObjectPool<GameObject>[] dotPools;
    private IObjectPool<GameObject>[] linePools;
    private int[,] grid;
    private bool[,] occupied;
    private int score;
    private float fillRatio = 0.6f;
    private Vector2Int[] starts;
    private Vector2Int[] ends;
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

        dotPools = new IObjectPool<GameObject>[dots.Length];
        for(int i = 0; i < dots.Length; i++)
        {
            int index = i; // index 따로 쓰는건 캡처 억까 방지
            dotPools[index] = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(dots[index], Vector3.zero, Quaternion.identity, gridBoard),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: 20,
            maxSize: 100
            );
        }

        linePools = new IObjectPool<GameObject>[lines.Length];
        for(int i = 0; i < lines.Length; i++)
        {
            int index = i;
            linePools[index] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(lines[index], Vector3.zero, Quaternion.identity, gridBoard),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                defaultCapacity: 100,
                maxSize: 200
            );
        }
        base.OnNetworkSpawn();
    }

    public override void OnStartGame()
    {
        gridBoard.gameObject.SetActive(true);

        Generate(3);
    }

   private void Generate(int size)
    {
        grid = new int[size, size];
        occupied = new bool[size, size];

        int maxCount = rng.Next(2, Mathf.FloorToInt(size * size / 2));
        float avg = size * size * fillRatio / maxCount;

        int easyCount = 0;
        int easyMax = size - 2;               // easy = 2개 바로 붙어있는거(공간 점유용 칸)
        var startList = new List<Vector2Int>();
        var endList = new List<Vector2Int>();

        for(int i = 0; i < maxCount; i++)
        {
            Vector2Int startPos = new Vector2Int(rng.Next(0, size), rng.Next(0, size)); // 시작 점은 0부터 크기까지의 좌표를 랜덤으로 설정
            if(!occupied[startPos.x, startPos.y]) // 그 지점이 점유되어 있지 않을 때만 실행, 이로 실패 시 반복 기회 차감
            {
                int minLen = easyCount < easyMax ? 2 : 3; //  // 길이가 2일 땐 바로 붙어있는거, 따라서 easy가 최대치 넘어가면 최소를 높힘
                int maxLen = Mathf.Max(minLen, Mathf.RoundToInt(avg * 1.5f)); // 최대 크기는 총 칸의 60%를 총 색의 개수를 나눈 평균으로 가져옴

                int length = rng.Next(minLen, maxLen + 1); // 랜덤들은 맥스를 제외함(int 기준)

                // 걷는 동안 지나간 칸을 occupied에 바로 표시
                // (자기 경로와 겹치는 것도 이 표시로 막힘)
                var path = new List<Vector2Int> { startPos }; // 지나온 길 리스트
                occupied[startPos.x, startPos.y] = true; // 시작점 점유

                for(int j = 1; j < length; j++)
                {
                    Vector2Int cur = path[path.Count - 1]; // 지나온 길 가장 위에 있는 거(바로 직전에 놓은 지점)

                    var candidates = new List<Vector2Int>(4); // 후보는 상하좌우 4개
                    foreach(var d in dirs)
                    {
                        Vector2Int n = cur + d; // 현재 + 방향 -> 다음 칸
                        if(n.x < 0 || n.y < 0 || n.x >= size || n.y >= size) continue; // 예외 :: 공간 넘어감
                        if(occupied[n.x, n.y]) continue; // 예외 :: 점유됨
                        candidates.Add(n); // 점유되어있지 않고, 공간이 넘어가지 않음
                    }
                    if(candidates.Count == 0) break;   // 예외 :: 막다른 길

                    Vector2Int next = candidates[rng.Next(candidates.Count)]; // 가능한 무작위 칸
                    path.Add(next); // 지나온 길에 추가
                    occupied[next.x, next.y] = true; // 점유시킴
                }

                Vector2Int endPos = path[path.Count - 1]; // 마지막 위치
                if(startPos == endPos) // 같은 점에 시작, 끝이 동시에 있을 때
                {
                    occupied[endPos.x, endPos.y] = false; // 점유 취소
                    continue;
                }

                if(IsAdjacent(startPos, endPos))
                {
                    easyCount++;
                }

                startList.Add(startPos);
                endList.Add(endPos);
            }
        }

        starts = startList.ToArray();
        ends = endList.ToArray();
        for(int i = 0; i < starts.Length; i++)
        {
            grid[starts[i].x, starts[i].y] = i + 1;   // 색 번호는 1부터, 0은 점 없음
            grid[ends[i].x, ends[i].y] = i + 1;
        }

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                GameObject cell = gridCellPool.Get();
                cell.transform.position = new Vector2(i * 50, j * 50);
            }
        }

    }

    private bool IsAdjacent(Vector2Int start, Vector2Int end) => Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y) == 1;
}