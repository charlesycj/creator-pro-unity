using UnityEngine;

public class TimeIncrementer : MonoBehaviour
{
    private float timePassed = 0f;  // �ð��� ������ ����
    public int CountTime = 0;  // ������ų ����

    void Update()
    {
        timePassed += Time.deltaTime;  // �����Ӹ��� �帥 �ð� ����

        if (timePassed >= 10f)  // 10�ʰ� ������ ��
        {
            CountTime++;  // CountTime x���� 1 ����
            timePassed = 0f;  // �ð��� ����
        }
    }
}
