using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingMinigameController : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private FishingMinigameView view;

    public event Action<bool> OnResult;

    private FishingMinigameLogic logic;

    public void startMinigame(DifficultFishingData data)
    {
        logic = new FishingMinigameLogic(data);
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
