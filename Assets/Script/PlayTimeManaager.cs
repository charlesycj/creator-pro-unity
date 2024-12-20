using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가

public class PlayTimeManager : MonoBehaviour
{
    // 플레이 타임을 표시할 TextMeshPro 텍스트 (여러 개 연결할 수 있도록 배열로 변경)
    public TMP_Text[] playTimeTexts;

    public GameManager gameManager;

    // 플레이 타임을 기록할 변수
    private float playTime;

    void Update()
    {
        // 게임이 정지된 상태이면 시간을 업데이트하지 않음
        if (gameManager != null && gameManager.gameStopped)
        {
            return;
        }

        // 시간 누적
        playTime += Time.deltaTime;

        // 시간 포맷팅 (분:초)
        string minutes = ((int)playTime / 60).ToString("00");
        string seconds = (playTime % 60).ToString("00");

        // TMP 텍스트들에 시간 표시
        foreach (TMP_Text playTimeText in playTimeTexts)
        {
            playTimeText.text = $"Time: {minutes}:{seconds}";
        }
    }
}
