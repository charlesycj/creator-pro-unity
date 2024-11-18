using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonManager : MonoBehaviour
{
    public void StartGame()
    {
        // 1_GameScene으로 이동
        SceneManager.LoadScene("1_GameScene");
    }
}
