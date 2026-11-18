using TMPro;
using UnityEngine;

public class FlowFreeScore : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreTxt;
    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private TMP_Text[] scoreText;
    private int lastSec = -1;
    
    public void SetScore(int score)
    {
        scoreTxt.text = $"Score\n{score}";
    }

    public void SetTime(float time)
    {

        int minute = (int)(time / 60f);
        int second = (int)(time % 60f);

        if(second != lastSec)
        {
            timeTxt.text = string.Format("{0:D2}:{1:D2}", minute, second);
            lastSec = second;
        }
    }

    public void ScoreBoard(int[] results)
    {
        for(int i = 0; i < results.Length; i++)
        {
            scoreText[i].text = $"Score : {results[i]}";
        }
    }
}