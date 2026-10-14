using UnityEngine;
// TODO: 나중에 미니게임 프레임워크 제작시 삭제
public class FishingMinigameTester : MonoBehaviour
{
    [SerializeField] private FishingMinigameController controller;
    [SerializeField] private DifficultFishingData data;

    private void Start()
    {
        controller.startMinigame(data);
    }
}
