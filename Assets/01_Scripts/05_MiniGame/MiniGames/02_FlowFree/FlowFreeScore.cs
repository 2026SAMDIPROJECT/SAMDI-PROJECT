using TMPro;
using UnityEngine;

public class FlowFreeScore : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreTxt;
    
    public void SetScore(int score)
    {
        scoreTxt.text = $"Score\n{score}";
    }
}