using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingQteKeySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private float currentScale = 1.1f;
    [SerializeField] private float nextScale = 0.8f;
    [SerializeField] private CanvasGroup group;
    public void SetUp(Key key) => keyText.text = key.ToString();

    public void SetCurrent() // 현재 키 프리뷰
    {
        group.alpha = 1f;
        transform.localScale = Vector3.one * currentScale;
    }
    public void SetPreview(float alpha) // 다음 키의 프리뷰
    {
        group.alpha = alpha;
        transform.localScale = Vector3.one * nextScale;
    }
}
