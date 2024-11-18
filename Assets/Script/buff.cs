using UnityEngine;
using System.Collections.Generic;

public class DualbuffSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject upwardbuffPrefab;    // ���� �ö󰡴� ������Ʈ ������
    [SerializeField]
    public GameObject downwardbuffPrefab;  // �Ʒ��� �������� ������Ʈ ������
    public float spawnIntervalbuff = 15.0f;         // ������Ʈ ���� ���� (��)
    private bool gameStopped = false;        // ���� ���� Ȯ��
    private float gravityScale = 10f;        // �߷� ������ �ʱⰪ

    void Start()
    {
        // ���� �������� �� ���� ������Ʈ ����
        InvokeRepeating("SpawnUpwardbuff", 0f, spawnIntervalbuff);
        InvokeRepeating("SpawnDownwardbuff", 0f, spawnIntervalbuff);
    }

    void Update()
    {
        if (gameStopped) return;

    }

    public void SpawnUpwardbuff()
    {
        if (gameStopped) return;

        // ������ X ��ǥ�� Y ��ǥ ���� (�Ʒ����� ���� �ö󰡴� ����)
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, -425f, 0f);

        // ������Ʈ ����
        Spawnbuff(upwardbuffPrefab, spawnPosition, -gravityScale);
    }

    public void SpawnDownwardbuff()
    {
        if (gameStopped) return;

        // ������ X ��ǥ�� Y ��ǥ ���� (������ �Ʒ��� �������� ����)
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, 425f, 0f);

        // ������Ʈ ����
        Spawnbuff(downwardbuffPrefab, spawnPosition, gravityScale);
    }

    private void Spawnbuff(GameObject prefab, Vector3 position, float gravity)
    {
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);

        // Rigidbody2D �߰� �� �߷� ����
        Rigidbody2D rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // ���� ��ũ��Ʈ �߰� �� ������ ���� ����
        buffMover moverScript = newObject.AddComponent<buffMover>();
        moverScript.spawnerScript = this;
    }

    public void StopGame()
    {
        gameStopped = true;
        CancelInvoke("SpawnUpwardbuff");
        CancelInvoke("SpawnDownwardbuff");
    }
}

public class buffMover : MonoBehaviour
{
    public DualbuffSpawner spawnerScript; // DualbuffSpawner ����

    void OnCollisionEnter2D(Collision2D collision)
    {
        // �ٴ� �浹 �� ������Ʈ �ı�
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        // �÷��̾� �浹 �� ���� ����
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
