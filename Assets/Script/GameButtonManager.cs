using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButtonManager : MonoBehaviour
{
    private bool isPaused = false; // ������ �Ͻ����� �������� ���θ� ����

    public GameObject pauseButton;  // Pause ��ư
    public GameObject resumeButton; // Resume ��ư

    // EXIT ��ư 
    public void ExitGame()
    {
        // ������ �޴��� �̵�
        SceneManager.LoadScene("0_Loby");
    }

    // PAUSE ��ư
    public void PauseGame()
    {
        if (isPaused)
        {
            // ���� �簳
            Time.timeScale = 1f; // ���� �ӵ��� �������� ����
            isPaused = false;

            // ��ư ���� ����
            resumeButton.SetActive(false); // Resume ��ư ��Ȱ��ȭ
            pauseButton.SetActive(true);  // Pause ��ư Ȱ��ȭ
        }
        else
        {
            // ���� �Ͻ�����
            Time.timeScale = 0f; // ���� �ӵ��� 0���� ����
            isPaused = true;

            // ��ư ���� ����
            resumeButton.SetActive(true); // Resume ��ư Ȱ��ȭ
            pauseButton.SetActive(false); // Pause ��ư ��Ȱ��ȭ
        }
    }

    // RETRY ��ư
    public void RetryGame()
    {
        // ���� �� �ٽ� �ε�
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f; // ���� �ӵ��� �ٽ� �������� ����
    }

    public void QuitGame()
    {
        // ������ �޴��� �̵�
        SceneManager.LoadScene("0_Loby");
    }
}