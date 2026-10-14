
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingFlowController : MonoBehaviour
{
    private enum State{Idle, Waiting, Fishing};

    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private FishingMinigameController minigame;
    [SerializeField] private DifficultFishingData difficult;
    [SerializeField] private Vector2 biteDelayRange = new Vector2(2f, 5f);

    private readonly FishingScore score = new FishingScore();
    private State state = State.Idle;

    private void OnStartPress()
    {
        if (state != State.Idle) return;
        StartCoroutine(CastRoutine());
    }
    private IEnumerator CastRoutine()
    {
        state = State.Waiting;
        startButton.interactable = false;
        statusText.text = "Fishing...";

        yield return new WaitForSeconds(Random.Range(biteDelayRange.x, biteDelayRange.y));

        state = State.Fishing;
        statusText.text = "Cast!";
        minigame.startMinigame(difficult);  
    }
    
    private void HandleResult(bool success)
    {
        score.ScoreIncrease(success ? 1 : 0);
        statusText.text = success ? "Success! +1" : "Fail... +0";
        EnterIdle();
    }

    private void EnterIdle()
    {
        state = State.Idle;
        startButton.interactable = true;
    }

    private void UpdateScoreText(int total) => scoreText.text = $"Score : {total}";

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartPress);
        minigame.OnResult += HandleResult;
        score.OnChanged += UpdateScoreText;

        UpdateScoreText(score.totalScore);  
        EnterIdle();
    }
}
