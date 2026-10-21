using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingMinigameController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private FishingMinigameView view;
    [SerializeField] private FishingQteView qteView;
    private DifficultyData selectDiffData;
    private FishData selectFishData;

    public event Action<bool> OnResult;
    private FishingMinigameLogic logic;
    private FishingQteLogic qte;
    private float nextQteTimer;

    public void startMinigame(DifficultyData diffData, FishData fishData) // 게임 시작 
    {
        selectDiffData = diffData;
        selectFishData = fishData;

        logic = new FishingMinigameLogic(selectDiffData, selectFishData);
        logic.OnFinished += HandleFinished;
        canvas.SetActive(true);
        ScheduledQte(); // QTE 스케줄해서 다음 QTE 타이머 돌림 
    }
    private void Update()
    {
        if (logic == null) return;

        if (qte != null)
        {
            UpdateQte(); // QTE 키 배열을 받아서 
            return;
        }
        
        bool holding = Mouse.current != null && Mouse.current.leftButton.isPressed;
        logic.Tick(Time.deltaTime, holding); // 낚시 미니게임 시작

        if(logic == null) return;
        view.Render(logic); // 낚시 미니게임 상태 
        nextQteTimer -= Time.deltaTime; 
        if(nextQteTimer <= 0f) StartQte();
    }
    private static Key? ReadPressedKey(Key[] keys) 
    {
        var kb = Keyboard.current;
        if (kb == null) return null;

        foreach(var key in keys)
            if (kb[key].wasPressedThisFrame) return key; // 여기서 누른 키 입력 받아 반환
            
        return null; // 누른키만 반응하게 할거면 마지막에 return null; 해도 되지 않을까
    }
    private void StartQte()
    {
        int count = UnityEngine.Random.Range(selectDiffData.qteCount.x, selectDiffData.qteCount.y); // qte갯수

        Key[] keys = new Key[count]; 
        for(int i = 0; i < count; i++)
            keys[i] = selectDiffData.qteKeys[UnityEngine.Random.Range(0, selectDiffData.qteKeys.Length)]; // qteKeys 안의 요소 랜덤으로 추출

        qte = new FishingQteLogic(keys, selectDiffData.qteTimeLimit);
        qte.OnFinished += HandleQteFinished;
        qte.OnKeyCleared += HandleQteKeyCleared;
        qteView.Show(keys, 1);
    }
    private void UpdateQte()
    {
        qte.Tick(Time.deltaTime, ReadPressedKey(selectDiffData.qteKeys));
        if (qte != null) qteView.UpdateTimer(qte.timeLeft / qte.timeLimit);
    }
    private void ScheduledQte() => nextQteTimer = UnityEngine.Random.Range(selectDiffData.qteInterval.x, selectDiffData.qteInterval.y);
    private void CancleQte()
    {
        if (qte != null)
        {
            qte.OnFinished -= HandleQteFinished;
            qte = null;
        }
        qteView.Hide();
    }
    private void HandleQteFinished(bool success)
    {
        CancleQte();
        ScheduledQte();
        logic.AddProgress(success ? 0f : -selectDiffData.qteFailPenalty); // qte 성공, 실패
    }
    private void HandleQteKeyCleared(int clearedCount) => qteView.Refresh(clearedCount);

    private void HandleFinished(bool success)
    {
        logic.OnFinished -= HandleFinished;
        logic = null;
        canvas.SetActive(false);
        OnResult?.Invoke(success);
    }
}
