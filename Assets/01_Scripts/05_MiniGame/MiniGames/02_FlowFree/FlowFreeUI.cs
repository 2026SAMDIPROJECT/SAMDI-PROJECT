using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    DOWN_RIGHT
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
    

    public void SetColor(Color color, int num)
    {
        image.color = color;
        numText.text = num.ToString();
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
