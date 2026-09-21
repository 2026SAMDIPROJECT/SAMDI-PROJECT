using UnityEngine;

public static class GridCoordinateConverter
{
    public static Vector2Int WorldToCell(RectTransform gridContainer, Vector2 screenPos, Camera cam, Vector2 cellSize, Vector2 spacing)
    {
        // 그리드 컨테이너 기준으로 로컬 좌표를 셀 좌표로 변환함
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridContainer, screenPos, cam, out Vector2 localPoint
        );
        // gridContainer 피벗을 몇으로 할지 잘 몰라서 일단 (0,1) 좌상단 기준으로 맞춤
        int x = Mathf.FloorToInt(localPoint.x/ (cellSize.x + spacing.x));
        int y = Mathf.FloorToInt(-localPoint.y/ (cellSize.y + spacing.y));

        return new Vector2Int(x,y);
    }
}
