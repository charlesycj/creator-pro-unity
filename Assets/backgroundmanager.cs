using System.Collections;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] backgrounds; // 3���� ��� ������Ʈ�� �迭�� ����
    public float changeInterval = 5f; // ��� ���� �ֱ� (5��)

    private int currentState = 0; // ���� ��� ���� (0: ù ��°, 1: �� ��°, 2: �� ��°)

    void Start()
    {
        // �ʱ� ����: ù ��° Ȱ��ȭ, ������ ��Ȱ��ȭ
        UpdateBackgroundState(0);

        // ���� ���� ����
        StartCoroutine(ChangeBackgroundRoutine());
    }

    IEnumerator ChangeBackgroundRoutine()
    {
        while (currentState < 2) // �� ��° ��� Ȱ��ȭ ���� ���� ����
        {
            yield return new WaitForSeconds(changeInterval);

            currentState++;
            UpdateBackgroundState(currentState);
        }
    }

    void UpdateBackgroundState(int state)
    {
        // ��� ��� ��Ȱ��ȭ
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].SetActive(false);
        }

        // ���� ���¿� ���� Ȱ��ȭ�� ��� ����
        if (state >= 0 && state < backgrounds.Length)
        {
            backgrounds[state].SetActive(true);
        }
    }
}
