using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasGroup))]
public class FishingQteKeySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text keyText;
    // [SerializeField] private Image background;
    // [SerializeField] private Color currentColor = Color.yellow;
    // [SerializeField] private Color previewColor = Color.white;
    [SerializeField] private float currentScale = 1.1f;
    [SerializeField] private float nextScale = 0.8f;
    [SerializeField] private CanvasGroup group;

    // private void Awake() => group = GetComponent<CanvasGroup>();

    public void SetUp(Key key) => keyText.text = key.ToString();

    public void SetCurrent()
    {
        group.alpha = 1f;
        // background.color = currentColor;
        transform.localScale = Vector3.one * currentScale;
    }
    public void SetPreview(float alpha)
    {
        group.alpha = alpha;
        // background.color = previewColor;
        transform.localScale = Vector3.one * nextScale;
    }
}
