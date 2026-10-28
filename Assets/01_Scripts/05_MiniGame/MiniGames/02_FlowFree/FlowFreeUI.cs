using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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
    private Vector3 originScale = new Vector3(1f, 1f, 1f);
    private Vector3 highLightScale => originScale * 1.3f;

    private void OnEnable()
    {
        numText.gameObject.SetActive(false);
    }

    public void SetColor(Color color, int num)
    {
        Debug.Log(color.a);
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
        if(isDot && dotState != DotState.NON && dotState == state) return;
        dotState = state;
        image.sprite = dotSprite[(int)state];
    }

    public void SetLine(LineState state)
    {
        if(!isDot && lineState != LineState.NON && lineState == state) return;
        lineState = state;
        image.sprite = lineSprite[(int)state];
    }

    public void HighLight()
    {
        transform.DOKill();
        transform.DOScale(highLightScale, 0.25f)
        .SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            transform.DOScale(originScale, 0.25f)
            .SetEase(Ease.OutBack);
        });
    }
}
