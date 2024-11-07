using UnityEngine;

public class TimeIncrementer : MonoBehaviour
{
    private float timePassed = 0f;  // 시간을 추적할 변수
    public int CountTime = 0;  // 증가시킬 변수

    void Update()
    {
        timePassed += Time.deltaTime;  // 프레임마다 흐른 시간 누적

        if (timePassed >= 10f)  // 10초가 지났을 때
        {
            CountTime++;  // CountTime x값을 1 증가
            timePassed = 0f;  // 시간을 리셋
        }
    }
}
