
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingQteView : MonoBehaviour
{
    [SerializeField] private GameObject root; // 아마 메인 캔버스 또는 그 자식에 설정할 듯 하니까 그 자식의 자식으로 둬야겠지
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private Image timerFill; // 일단 타이머 같은거 줄어들게 할거면 ImageType = filled

    private void Awake() => root.SetActive(false);
    public void Show(Key key)
    {
        keyText.text = key.ToString();
        timerFill.fillAmount = 1f;
        root.SetActive(true);
    }
    
    public void UpdateTimer(float ratio) => timerFill.fillAmount = ratio;
    public void Hide() => root.SetActive(false);
}
