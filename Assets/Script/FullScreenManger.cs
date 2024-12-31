using UnityEngine;

public class FullScreenManager : MonoBehaviour
{
    void Start()
    {

        // 해상도를 1920x1080으로 고정
        int targetWidth = 1920;
        int targetHeight = 1080;

        // 해상도와 풀스크린 모드 설정
        Screen.SetResolution(targetWidth, targetHeight, true);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        // 위의 코드나 아래 코드중 하나만 활성화해주세요

        
       /*// 현재 화면 해상도 가져오기
        Resolution currentResolution = Screen.currentResolution;

        // 해상도와 풀스크린 모드 설정
        Screen.SetResolution(currentResolution.width, currentResolution.height, true);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;*/
    }
}