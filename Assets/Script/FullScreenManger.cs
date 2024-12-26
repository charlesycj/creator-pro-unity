using UnityEngine;

public class FullScreenManager : MonoBehaviour
{
    void Start()
    {
        // 현재 화면 해상도 가져오기
        Resolution currentResolution = Screen.currentResolution;

        // 해상도와 풀스크린 모드 설정
        Screen.SetResolution(currentResolution.width, currentResolution.height, true);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
}