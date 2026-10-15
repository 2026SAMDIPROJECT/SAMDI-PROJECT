using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingMinigameController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private FishingMinigameView view;
    private DifficultyData selectDiffData;
    private FishData selectFishData;

    public event Action<bool> OnResult;
    private FishingMinigameLogic logic;

    public void startMinigame(DifficultyData diffData, FishData fishData)
    {
        selectDiffData = diffData;
        selectFishData = fishData;

        logic = new FishingMinigameLogic(selectDiffData, selectFishData);
        
        logic.OnFinished += HandleFinished;
        canvas.SetActive(true);
    }
    private void Update()
    {
        if (logic == null) return;
        
        bool holding = Mouse.current != null && Mouse.current.leftButton.isPressed;
        logic.Tick(Time.deltaTime, holding);

        if(logic == null) return;
        view.Render(logic);
    }
    private void HandleFinished(bool success)
    {
        logic.OnFinished -= HandleFinished;
        logic = null;
        canvas.SetActive(false);
        OnResult?.Invoke(success);
    }
}
