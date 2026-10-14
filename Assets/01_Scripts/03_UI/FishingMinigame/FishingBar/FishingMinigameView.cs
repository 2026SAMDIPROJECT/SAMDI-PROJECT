using UnityEngine;
using UnityEngine.UI;

public class FishingMinigameView : MonoBehaviour
{
    [SerializeField] private RectTransform track;
    [SerializeField] private RectTransform greenBar;
    [SerializeField] private RectTransform fishIcon;
    [SerializeField] private Slider progressSlider;

    public float barSizeRatio {get; private set;}
    public void Render(FishingMinigameLogic logic)
    {
        float h = track.rect.height; // 트랙 높이
        float barH = logic.barSize * h; // 바 UI 사이즈
        float bottom = Mathf.Clamp(logic.barBottom * h, 0f, h - barH); // Clamp로 위치 제한 (최솟값 0, 최댓값 h - barH)

        greenBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, barH); // 현재 앵커 기준으로 세로 크기가 되도록 조정
        greenBar.anchoredPosition = new Vector2(0f, bottom);
        fishIcon.anchoredPosition = new Vector2(0f, logic.fishPos * h);
        progressSlider.value = logic.progress;
    }
}
