using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingBoard;
    public int loadingCount;

    public void Show()
    {
        loadingCount++;
        loadingBoard.SetActive(true);
    }

    public void Hide()
    {
        loadingCount = Mathf.Max(0, --loadingCount);
        if(loadingCount <= 0)
            loadingBoard.SetActive(false);
    }
}