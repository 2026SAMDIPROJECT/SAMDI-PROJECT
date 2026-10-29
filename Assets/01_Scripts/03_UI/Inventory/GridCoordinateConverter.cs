using UnityEngine;

public static class GridCoordinateConverter
{
    public static Vector2Int WorldToCell(RectTransform gridContainer, Vector2 screenPos, Camera cam, Vector2 cellSize, Vector2 spacing)
    {
        // 그리드 컨테이너 기준으로 로컬 좌표를 셀 좌표로 변환함
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridContainer, screenPos, cam, out Vector2 localPoint
        );

        Rect rect = gridContainer.rect; // 피벗 (0,1)로 로컬 포인트 좌표계 정규화
        float localX = localPoint.x - rect.xMin;
        float localY = rect.yMax - localPoint.y;

        int x = Mathf.FloorToInt(localX / (cellSize.x + spacing.x));
        int y = Mathf.FloorToInt(localY / (cellSize.y + spacing.y));
 
        return new Vector2Int(x,y);
    }
}
