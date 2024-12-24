using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoPlayer : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    public string nextSceneName = "0_Loby"; // 다음 씬 이름 설정

    void Start()
    {
        // VideoPlayer 컴포넌트 가져오기
        videoPlayer = GetComponent<VideoPlayer>();

        // 영상 종료 시 이벤트 연결
        videoPlayer.loopPointReached += OnVideoEnd;

        // 영상 재생
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // 영상이 끝난 후 다음 씬으로 이동
        SceneManager.LoadScene(nextSceneName);
    }

    void Update()
    {
        // ESC 키 입력으로 스킵
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            videoPlayer.Stop(); // 영상 중단
            SceneManager.LoadScene(nextSceneName); // 다음 씬으로 이동
        }
    }

}