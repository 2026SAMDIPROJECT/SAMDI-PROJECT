using UnityEngine;

public class Timer
{
    public bool during = false; // 타이머가 돌고 있는가
    private float maxTime; // 셀 시간
    private float passedTime; // 지나간 시간

    public float remainTime => (maxTime - passedTime) > 0? (maxTime - passedTime) : 0; // 남은 시간
    public float progress => maxTime > 0f? passedTime / maxTime : 0; // 남은 진행도

    public void StartTimer(float time) // 타이머 시작. 생성자가 아닌 이유는 한 번 생성하고 돌려쓸 것이기 때문.
    {
        during = true;
        maxTime = time;
        passedTime = 0;
    }

    public void RunTimer() // 타이머 실행. Update에 직접 넣지 않은 이유는 시간을 멈췄을 때를 대비함. 나중에 기믹 만들기 편하고 어차피 Time.deltaTime은 이전 프레임에서부터 걸린 시간을 가져옴.
    {
        if(during)
        {
            passedTime += Time.deltaTime;
            if(passedTime >= maxTime)
            {
                during = false;
                passedTime = 0;
            }
        }
    }

    public void EndTimer()
    {
        during = false;
        passedTime = 0;
    }
}
