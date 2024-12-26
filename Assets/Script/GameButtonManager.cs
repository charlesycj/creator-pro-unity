using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButtonManager : MonoBehaviour
{
    private bool isPaused = false; // 게임이 일시정지 상태인지 여부를 저장

    public GameObject pauseButton;  // Pause 버튼
    public GameObject resumeButton; // Resume 버튼

    // EXIT 버튼 
    public void ExitGame()
    {
        // 누르면 메뉴로 이동
        SceneManager.LoadScene("0_Loby");
    }

    // PAUSE 버튼
    public void PauseGame()
    {
        if (isPaused)
        {
            // 게임 재개
            Time.timeScale = 1f; // 게임 속도를 정상으로 설정
            isPaused = false;

            // 버튼 상태 변경
            resumeButton.SetActive(false); // Resume 버튼 비활성화
            pauseButton.SetActive(true);  // Pause 버튼 활성화
        }
        else
        {
            // 게임 일시정지
            Time.timeScale = 0f; // 게임 속도를 0으로 설정
            isPaused = true;

            // 버튼 상태 변경
            resumeButton.SetActive(true); // Resume 버튼 활성화
            pauseButton.SetActive(false); // Pause 버튼 비활성화
        }
    }

    // RETRY 버튼
    public void RetryGame()
    {
        // 현재 씬 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f; // 게임 속도를 다시 정상으로 설정
    }

    public void QuitGame()
    {
        // 누르면 메뉴로 이동
        SceneManager.LoadScene("0_Loby");
    }
}