using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButtonManager : MonoBehaviour
{
    private bool isPaused = false; // ������ �Ͻ����� �������� ���θ� ����

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
        }
        else
        {
            // ���� �Ͻ�����
            Time.timeScale = 0f; // ���� �ӵ��� 0���� ����
            isPaused = true;
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