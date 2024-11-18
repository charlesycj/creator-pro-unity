using UnityEngine;
using System.Collections.Generic;

public class ObjectBuff : MonoBehaviour
{
    [SerializeField]
    private GameObject upwardBuffPrefab;    // ���� �ö󰡴� ���� ������
    [SerializeField]
    private GameObject downwardBuffPrefab; // �Ʒ��� �������� ���� ������
    private float timeElapsed = 0f;        // ��� �ð�
    private float buffCycleTime = 15f;     // ���� �ֱ� (15��)
    private float nextUpwardBuffTime = 0f; // ���� ���� �ö󰡴� ���� ���� �ð�
    private float nextDownwardBuffTime = 0f; // ���� �Ʒ��� �������� ���� ���� �ð�
    private bool gameStopped = false;      // ���� ���� Ȯ��
    private float gravityScale = 10f;      // �߷� ������ �ʱⰪ

    private List<GameObject> activeBuffs = new List<GameObject>(); // Ȱ��ȭ�� ���� ����Ʈ

    void Start()
    {
        // �ʱ� ���� ���� �ð� ����
        SetNextUpwardBuffTime();
        SetNextDownwardBuffTime();
    }

    void Update()
    {
        // ������ �ߴܵ� ��� ������Ʈ ���� ����
        if (gameStopped) return;

        // ���� ������ ���� Ÿ�̸� ������Ʈ
        timeElapsed += Time.deltaTime;

        // ���� �ö󰡴� ���� ���� �ð� Ȯ��
        if (timeElapsed >= nextUpwardBuffTime)
        {
            SpawnBuff(upwardBuffPrefab, true);
            SetNextUpwardBuffTime(); // ���� ���� �ö󰡴� ���� �ð� ����
        }

        // �Ʒ��� �������� ���� ���� �ð� Ȯ��
        if (timeElapsed >= nextDownwardBuffTime)
        {
            SpawnBuff(downwardBuffPrefab, false);
            SetNextDownwardBuffTime(); // ���� �Ʒ��� �������� ���� �ð� ����
        }
    }

    public void SpawnBuff(GameObject buffPrefab, bool isUpwardBuff)
    {
        if (gameStopped) return; // ������ ����ٸ� ���� ���� ����

        // ȭ���� ������ X ��ǥ���� ������ ����
        float randomX = Random.Range(-850f, 850f);
        float spawnY = isUpwardBuff ? -425f : 425f; // ���� ���� ���� �Ʒ�����, �������� ���� ������ ����
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        // ���� ����
        GameObject buff = Instantiate(buffPrefab, spawnPosition, Quaternion.identity);
        activeBuffs.Add(buff); // ����Ʈ�� �߰�

        // Rigidbody2D ������Ʈ �߰� �� �߷� ����
        Rigidbody2D rb = buff.AddComponent<Rigidbody2D>();
        rb.gravityScale = isUpwardBuff ? -gravityScale : gravityScale; // ���� ���� ��� �߷� ����
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Collider2D�� ���ٸ� �߰� (���� �浹�� ����)
        if (buff.GetComponent<Collider2D>() == null)
        {
            buff.AddComponent<BoxCollider2D>();
        }
    }

    private void SetNextUpwardBuffTime()
    {
        // ���� �ð��� �������� ������ �ð� ����
        nextUpwardBuffTime = timeElapsed + Random.Range(0f, buffCycleTime);
    }

    private void SetNextDownwardBuffTime()
    {
        // ���� �ð��� �������� ������ �ð� ����
        nextDownwardBuffTime = timeElapsed + Random.Range(0f, buffCycleTime);
    }

    public void StopGame()
    {
        gameStopped = true; // ���� ���� ���� ����

        // Ȱ��ȭ�� ��� ���� ����
        foreach (var buff in activeBuffs)
        {
            if (buff != null)
            {
                Destroy(buff);
            }
        }
        activeBuffs.Clear(); // ����Ʈ �ʱ�ȭ
    }
}
