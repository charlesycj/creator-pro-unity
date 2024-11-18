using System.Collections;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] backgrounds; // 3개의 배경 오브젝트를 배열로 관리
    public float changeInterval = 5f; // 배경 변경 주기 (5초)

    private int currentState = 0; // 현재 배경 상태 (0: 첫 번째, 1: 두 번째, 2: 세 번째)

    void Start()
    {
        // 초기 상태: 첫 번째 활성화, 나머지 비활성화
        UpdateBackgroundState(0);

        // 변경 루프 시작
        StartCoroutine(ChangeBackgroundRoutine());
    }

    IEnumerator ChangeBackgroundRoutine()
    {
        while (currentState < 2) // 세 번째 배경 활성화 이후 루프 종료
        {
            yield return new WaitForSeconds(changeInterval);

            currentState++;
            UpdateBackgroundState(currentState);
        }
    }

    void UpdateBackgroundState(int state)
    {
        // 모든 배경 비활성화
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].SetActive(false);
        }

        // 현재 상태에 따라 활성화할 배경 설정
        if (state >= 0 && state < backgrounds.Length)
        {
            backgrounds[state].SetActive(true);
        }
    }
}
