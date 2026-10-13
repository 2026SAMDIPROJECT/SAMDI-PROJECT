using UnityEngine;
using UnityEngine.UI;

public class FishingMinigameView : MonoBehaviour
{
    [SerializeField] private RectTransform track;
    [SerializeField] private RectTransform greenBar;
    [SerializeField] private RectTransform fishIcon;
    [SerializeField] private Slider progressSlider;

    public void Render(FishingMinigameLogic logic)
    {
        float h = track.rect.height;

        greenBar.sizeDelta = new Vector2(greenBar.sizeDelta.x, greenBar.sizeDelta.y);
        greenBar.anchoredPosition = new Vector2(0f, logic.barBottom * h);
        fishIcon.anchoredPosition = new Vector2(0f, logic.fishPos * h);
        progressSlider.value = logic.progress;
    }
}
