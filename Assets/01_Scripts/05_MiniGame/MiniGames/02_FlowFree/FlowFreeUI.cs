using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum DotState
{
    NON,
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public enum LineState
{
    UP_DOWN,
    LEFT_RIGHT,
    UP,
    DOWN,
    LEFT,
    RIGHT,
    UP_LEFT,
    UP_RIGHT,
    DOWN_LEFT,
    DOWN_RIGHT,
    NON
}
public class FlowFreeUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text numText;
    [SerializeField] private Sprite[] dotSprite;
    [SerializeField] private Sprite[] lineSprite;
    public bool isDot;
    public DotState dotState;
    public LineState lineState;

    private void OnEnable()
    {
        numText.gameObject.SetActive(false);
    }

    public void SetColor(Color color, int num)
    {
        image.color = color;
        numText.gameObject.SetActive(true);
        numText.text = num.ToString();
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void SetNumSize(float newSize)
    {
        numText.fontSize = newSize;
    }

    public void SetDot(DotState state)
    {
        dotState = state;
        image.sprite = dotSprite[(int)state];
    }

    public void SetLine(LineState state)
    {
        lineState = state;
        image.sprite = lineSprite[(int)state];
    }
}
