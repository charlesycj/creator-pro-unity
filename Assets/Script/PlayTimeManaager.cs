using UnityEngine;
using TMPro; // TextMeshPro ���ӽ����̽� �߰�

public class PlayTimeManager : MonoBehaviour
{
    // �÷��� Ÿ���� ǥ���� TextMeshPro �ؽ�Ʈ (���� �� ������ �� �ֵ��� �迭�� ����)
    public TMP_Text[] playTimeTexts;

    public GameManager gameManager;

    // �÷��� Ÿ���� ����� ����
    private float playTime;

    void Update()
    {
        // ������ ������ �����̸� �ð��� ������Ʈ���� ����
        if (gameManager != null && gameManager.gameStopped)
        {
            return;
        }

        // �ð� ����
        playTime += Time.deltaTime;

        // �ð� ������ (��:��)
        string minutes = ((int)playTime / 60).ToString("00");
        string seconds = (playTime % 60).ToString("00");

        // TMP �ؽ�Ʈ�鿡 �ð� ǥ��
        foreach (TMP_Text playTimeText in playTimeTexts)
        {
            playTimeText.text = $"Time: {minutes}:{seconds}";
        }
    }
}
