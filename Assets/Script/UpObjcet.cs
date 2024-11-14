using UnityEngine;

public class ObjectUp : MonoBehaviour
{
    public GameObject objectPrefab;      // ������ ������Ʈ ������
    public float spawnInterval = 1f;     // ������Ʈ ���� ���� (��)
    private float timeElapsed = 0f;      // ��� �ð�
    private bool gameStopped = false;    // ���� ���� Ȯ��
    private float gravityScale = 10f;    // �߷� ������ �ʱⰪ
    private bool gravityReversed = true; // �߷¹���

    void Start()
    {
        // 1�ʿ� �� ���� ������Ʈ�� �����ϴ� �޼��� ȣ��
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    void Update()
    {
        // ������ �ߴܵ� ��� ������Ʈ ���� ����
        if (gameStopped) return;

        // �ð� 3���� ����� ������ spawnInterval�� 0.05�� ����
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 180.0f) // 3�� ��� ��
        {
            // �߷°� ����
            gravityScale += 5f; // ���÷� 5�� ����, �ʿ信 ���� ����

            // ��� �ð� �ʱ�ȭ
            timeElapsed = 0f;

            // spawnInterval�� 0.05�� ����
            if (spawnInterval > 0.05f)
            {
                spawnInterval -= 0.05f;
            }

            // ���� InvokeRepeating ���
            CancelInvoke("SpawnObject");
            // ���ο� spawnInterval�� �ݺ� ȣ�� ����
            InvokeRepeating("SpawnObject", 0f, spawnInterval);
        }

      
      
    }

    public void SpawnObject()
    {
        if (gameStopped) return; // ������ ����ٸ� ������Ʈ ���� ����

        // ȭ���� ������ X ��ǥ���� ������Ʈ�� ����
        float randomX = Random.Range(-850f, 850f); // ȭ�� ������ �°� X ���� ����
        Vector3 spawnPosition = new Vector3(randomX, -425f, 0f); // Y�� -425���� ���� (�Ʒ����� ���� )

        // ������Ʈ ����
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        // Rigidbody2D ������Ʈ �߰� �� �߷� ����
        Rigidbody2D rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravityReversed ? -gravityScale : gravityScale; // �߷°� ����

        // Collider2D�� ���ٸ� �߰� (���� �浹�� ����)
        if (newObject.GetComponent<Collider2D>() == null)
        {
            newObject.AddComponent<BoxCollider2D>();
        }

        // Up ��ũ��Ʈ �߰�
        Up UpScript = newObject.AddComponent<Up>();
        UpScript.objectUpScript = this; // ObjectUp ���� ����
    }

    public void StopGame()
    {
        gameStopped = true; // ���� ���� ���� ����
        CancelInvoke("SpawnObject"); // ������Ʈ ���� ����
    }
}

public class Up : MonoBehaviour
{
    public ObjectUp objectUpScript;  // ObjectUp ����

    void OnCollisionEnter2D(Collision2D collision)
    {
        // �ٴڰ� �浹���� �� ������Ʈ �ı�
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // ������Ʈ �ı�
        }

        // �÷��̾�� �浹���� �� ���� ����
        if (collision.gameObject.CompareTag("Player"))
        {
            objectUpScript.StopGame();
            Destroy(gameObject);
        }
    }
}
