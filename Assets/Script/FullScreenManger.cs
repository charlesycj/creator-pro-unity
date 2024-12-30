using UnityEngine;

public class FullScreenManager : MonoBehaviour
{
    void Start()
    {
        // ���� ȭ�� �ػ� ��������
        Resolution currentResolution = Screen.currentResolution;

        // �ػ󵵿� Ǯ��ũ�� ��� ����
        Screen.SetResolution(currentResolution.width, currentResolution.height, true);
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
}