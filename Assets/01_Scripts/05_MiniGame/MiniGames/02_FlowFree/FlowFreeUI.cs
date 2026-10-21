using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Sprite[] dotSprite;
    [SerializeField] private Sprite[] lineSprite;
    public bool isDot;
    public DotState dotState;
    public LineState lineState;
    

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void SetDot(DotState state)
    {
        dotState = state;
        image.sprite = dotSprite[(int)state];
    }

    public void SetLIne(LineState state)
    {
        lineState = state;
        image.sprite = lineSprite[(int)state];
    }
}
