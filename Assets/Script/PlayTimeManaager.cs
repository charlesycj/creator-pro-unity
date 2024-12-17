using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가

public class PlayTimeManager : MonoBehaviour
{
    // 플레이 타임을 표시할 TextMeshPro 텍스트
    public TMP_Text playTimeText;

    // 플레이 타임을 기록할 변수
    private float playTime;

    void Update()
    {
        // 시간 누적
        playTime += Time.deltaTime;

        // 시간 포맷팅 (분:초)
        string minutes = ((int)playTime / 60).ToString("00");
        string seconds = (playTime % 60).ToString("00");

        // TMP 텍스트에 시간 표시
        playTimeText.text = $"Time: {minutes}:{seconds}";
    }
}