using UnityEngine;

public class FullScreenManager : MonoBehaviour
{
    void Start()
    {

        // �ػ󵵸� 1920x1080���� ����
        int targetWidth = 1920;
        int targetHeight = 1080;

        // �ػ󵵿� Ǯ��ũ�� ��� ����
        Screen.SetResolution(targetWidth, targetHeight, true);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        // ���� �ڵ峪 �Ʒ� �ڵ��� �ϳ��� Ȱ��ȭ���ּ���

        
       /*// ���� ȭ�� �ػ� ��������
        Resolution currentResolution = Screen.currentResolution;

        // �ػ󵵿� Ǯ��ũ�� ��� ����
        Screen.SetResolution(currentResolution.width, currentResolution.height, true);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;*/
    }
}