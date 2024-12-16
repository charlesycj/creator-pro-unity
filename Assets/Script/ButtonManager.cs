using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI 관련 네임스페이스 추가

public class ButtonManager : MonoBehaviour
{
    // 가이드 이미지를 표시할 Image UI 객체
    public Image guideImage;

    // 게임 시작 버튼
    public void StartGame()
    {
        // 1_GameScene으로 이동
        SceneManager.LoadScene("1_GameScene");
    }

    // 가이드 버튼 클릭 시 호출
    public void OpenGuide()
    {
        if (guideImage != null)
        {
            // 가이드 이미지를 활성화
            guideImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Guide Image가 연결되지 않았습니다!");
        }
    }

    // 가이드 닫기 버튼 클릭 시 호출
    public void CloseGuide()
    {
        if (guideImage != null)
        {
            // 가이드 이미지를 비활성화
            guideImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Guide Image가 연결되지 않았습니다!");
        }
    }

    // ESC 키 입력 처리
    private void Update()
    {
        // ESC 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 가이드 패널이 활성화 상태라면 비활성화
            if (guideImage != null && guideImage.gameObject.activeSelf)
            {
                CloseGuide();
            }
        }
    }
}
