
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingQteView : MonoBehaviour
{
    [SerializeField] private GameObject root; // 아마 메인 캔버스 또는 그 자식에 설정할 듯 하니까 그 자식의 자식으로 둬야겠지
    [SerializeField] private Transform slotContainer;
    [SerializeField] private FishingQteKeySlot keySlot;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private float[] previewColor = {0.55f, 0.3f, 0.15f};
    [SerializeField] private Image timerFill; // 일단 타이머 같은거 줄어들게 할거면 ImageType = filled

    private readonly List<FishingQteKeySlot> slots = new List<FishingQteKeySlot>();
    private IReadOnlyList<Key> keys;
    private int previewCount;

    private void Awake() => root.SetActive(false);
    public void Show(IReadOnlyList<Key> sequence, int preview) // QTE 키 보여주는 로직 (readonly 배열에서 들고옴)
    {
        keys = sequence;
        previewCount = Mathf.Clamp(preview, 0 , previewColor.Length);

        int needed = 1 + previewCount;
        while (slots.Count < needed)
            slots.Add(Instantiate(keySlot, slotContainer)); 

        Refresh(0);
        timerFill.fillAmount = 1f;
        root.SetActive(true);
    }
    public void Refresh(int clearedCount) // 남은 키 갯수 시각화 및 QTE 키 시각화
    {
        for (int i = 0; i < slots.Count ; i++)
        {
            int index = clearedCount + i;
            bool visible = i <= previewCount && index < keys.Count;

            slots[i].gameObject.SetActive(visible);
            if (!visible) continue;

            slots[i].SetUp(keys[index]);
            if (i == 0) slots[i].SetCurrent();
            else slots[i].SetPreview(previewColor[i - 1]);
        }

        if (keyText != null)
            keyText.text = $"{Mathf.Min(clearedCount + 1, keys.Count)} / {keys.Count}";
    }
    
    public void UpdateTimer(float ratio) => timerFill.fillAmount = ratio;
    public void Hide() => root.SetActive(false);
}
