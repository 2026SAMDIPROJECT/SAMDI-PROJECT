using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingFlowController : NetworkBehaviour
{
    private enum State{Idle, Waiting, Fishing};

    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private FishingMinigameController minigame;
    [SerializeField] private FishTableData fishTable;
    [SerializeField] private Vector2 biteDelayRange = new Vector2(2f, 5f);

    private readonly FishingScore score = new FishingScore();
    private State state = State.Idle;
    private FishDropEntry selectedFish;

    private void Update()
    {
        Debug.Log($"클릭 감지 / IsOwner={IsOwner} / IsSpawned={IsSpawned} / state={state}");
        if(state != State.Idle) return;
        if(Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
        if(!IsOwner) return;
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Debug.Log("CastRoutine 시작");
        StartCoroutine(CastRoutine());
    }
    private IEnumerator CastRoutine()
    {
        state = State.Waiting;
        statusText.text = "Fishing...";

        yield return new WaitForSeconds(Random.Range(biteDelayRange.x, biteDelayRange.y));

        selectedFish = fishTable.GetRandomEntry();

        state = State.Fishing;
        statusText.text = $"Cast! quality: {selectedFish.fishData.quality}";
        minigame.startMinigame(
            selectedFish.difficulty,
            selectedFish.fishData
        );  
    }
    private void HandleResult(bool success)
    {
        score.ScoreIncrease(success ? selectedFish.fishData.fishScore : 0);
        statusText.text = success ? $"Success! +{selectedFish.fishData.fishScore}" : "Fail... +0";
        EnterIdle();
    }

    private void EnterIdle()
    {
        state = State.Idle;
        statusText.text = "Click To Cast";
    }

    private void UpdateScoreText(int total) => scoreText.text = $"Score : {total}";

    private void OnEnable()
    {
        minigame.OnResult += HandleResult;
        score.OnChanged += UpdateScoreText;

        UpdateScoreText(score.totalScore);  
        EnterIdle();
    }
    private void OnDisable()
    {
        minigame.OnResult -= HandleResult;
        score.OnChanged -= UpdateScoreText;
    }
}
