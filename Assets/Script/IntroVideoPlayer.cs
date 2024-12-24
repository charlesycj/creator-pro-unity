using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoPlayer : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    public string nextSceneName = "0_Loby"; // ���� �� �̸� ����

    void Start()
    {
        // VideoPlayer ������Ʈ ��������
        videoPlayer = GetComponent<VideoPlayer>();

        // ���� ���� �� �̺�Ʈ ����
        videoPlayer.loopPointReached += OnVideoEnd;

        // ���� ���
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // ������ ���� �� ���� ������ �̵�
        SceneManager.LoadScene(nextSceneName);
    }

    void Update()
    {
        // ESC Ű �Է����� ��ŵ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            videoPlayer.Stop(); // ���� �ߴ�
            SceneManager.LoadScene(nextSceneName); // ���� ������ �̵�
        }
    }

}