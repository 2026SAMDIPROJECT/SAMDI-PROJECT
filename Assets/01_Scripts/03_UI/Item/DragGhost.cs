using UnityEngine;
using UnityEngine.UI;

public class DragGhost : MonoBehaviour
{
    [SerializeField] private Image ghostImage;
    [SerializeField] private Color validColor = new Color(1f,1f,1f,0.6f);
    [SerializeField] private Color invalidColor = new Color(1f,0.3f,0.3f,0.6f);

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    public void show(Sprite icon, Vector2 size)
    {
        ghostImage.sprite = icon;
        rectTransform.sizeDelta = size;
        gameObject.SetActive(true);
    }
    public void UpdatePosition(Vector2 screenPos)
    {
        rectTransform.position = screenPos;
    }
    public void SetValid(bool isValid)
    {
        ghostImage.color = isValid ? validColor : invalidColor;
    }
    public void hide()
    {
        gameObject.SetActive(false);
    }
}
