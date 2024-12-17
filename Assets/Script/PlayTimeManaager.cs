using UnityEngine;
using TMPro; // TextMeshPro ���ӽ����̽� �߰�

public class PlayTimeManager : MonoBehaviour
{
    // �÷��� Ÿ���� ǥ���� TextMeshPro �ؽ�Ʈ
    public TMP_Text playTimeText;

    // �÷��� Ÿ���� ����� ����
    private float playTime;

    void Update()
    {
        // �ð� ����
        playTime += Time.deltaTime;

        // �ð� ������ (��:��)
        string minutes = ((int)playTime / 60).ToString("00");
        string seconds = (playTime % 60).ToString("00");

        // TMP �ؽ�Ʈ�� �ð� ǥ��
        playTimeText.text = $"Time: {minutes}:{seconds}";
    }
}