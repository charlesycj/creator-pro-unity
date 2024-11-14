using UnityEngine;

public class ObjectDownbuff : MonoBehaviour
{
    [SerializeField]
    private GameObject buffPrefabDown;        // ���� ������ ������
    private float timeElapsed = 0f;      // ��� �ð�
    private float buffCycleTime = 15f;   // ���� �ֱ� (15��)
    private float nextBuffTime = 0f;     // ���� ���� ������ ���� �ð�
    private bool gameStopped = false;    // ���� ���� Ȯ��
    private float gravityScale = 10f;    // �߷� ������ �ʱⰪ

    void Start()
    {
        // �ʱ� ���� ���� �ð� ����
        SetNextBuffTime();
    }

    void Update()
    {
        // ������ �ߴܵ� ��� ������Ʈ ���� ����
        if (gameStopped) return;

        // ���� ������ ���� Ÿ�̸� ������Ʈ
        timeElapsed += Time.deltaTime;

        // ���� ������ ���� �ð� Ȯ��
        if (timeElapsed >= nextBuffTime)
        {
            SpawnBuff();
            SetNextBuffTime(); // ���� ���� �ð� ����
        }
    }

    public void SpawnBuff()
    {
        if (gameStopped) return; // ������ ����ٸ� ���� ���� ����

        // ȭ���� ������ X ��ǥ���� ������ ����
        float randomX = Random.Range(-850f, 850f); // ȭ�� ������ �°� X ���� ����
        Vector3 spawnPosition = new Vector3(randomX, 425f, 0f); // Y�� 425���� ���� (������ �������� ����)

        // ���� ����
        GameObject buff = Instantiate(buffPrefabDown, spawnPosition, Quaternion.identity);

        // Rigidbody2D ������Ʈ �߰� �� �߷� ����
        Rigidbody2D rb = buff.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale; // �߷°� ����

        // Collider2D�� ���ٸ� �߰� (���� �浹�� ����)
        if (buff.GetComponent<Collider2D>() == null)
        {
            buff.AddComponent<BoxCollider2D>();
        }
    }

    private void SetNextBuffTime()
    {
        // ���� �ð��� �������� 15�� �ֱ� �ȿ��� ������ �ð� ����
        nextBuffTime = timeElapsed + Random.Range(0f, buffCycleTime);
    }

    public void StopGame()
    {
        gameStopped = true; // ���� ���� ���� ����
    }
}

public class Downbuff : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // �ٴڰ� �浹���� �� ���� �ı�
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // ���� �ı�
        }

        // �÷��̾�� �浹���� �� ���� ����
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // ���� ���� �� �ı�
        }
    }
}
