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


    public void startMinigame(DifficultyData diffData, FishData fishData)
    {
        selectDiffData = diffData;
        selectFishData = fishData;

        logic = new FishingMinigameLogic(selectDiffData, selectFishData);
        logic.OnFinished += HandleFinished;
        canvas.SetActive(true);
        ScheduledQte();
    }
    private void Update()
    {
        if (logic == null) return;

        if (qte != null)
        {
            UpdateQte();
            return;
        }
        
        bool holding = Mouse.current != null && Mouse.current.leftButton.isPressed;
        logic.Tick(Time.deltaTime, holding);

        if(logic == null) return;
        view.Render(logic);
        nextQteTimer -= Time.deltaTime;
        if(nextQteTimer <= 0f) StartQte();
    }
    private static Key? ReadPressedKey(Key[] keys)
    {
        var kb = Keyboard.current;
        if (kb == null) return null;

        foreach(var key in keys)
            if (kb[key].wasPressedThisFrame) return key;
        return null;
    }
    private void StartQte()
    {
        Key key = selectDiffData.qteKeys[UnityEngine.Random.Range(0,selectDiffData.qteKeys.Length)];
        qte = new FishingQteLogic(key, selectDiffData.qteTimeLimit);
        qte.OnFinished += HandleQteFinished;
        qteView.Show(key);
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
        logic.AddProgress(success ? 0f : -selectDiffData.qteFailPenalty);
    }

    private void HandleFinished(bool success)
    {
        logic.OnFinished -= HandleFinished;
        logic = null;
        canvas.SetActive(false);
        OnResult?.Invoke(success);
    }
}
