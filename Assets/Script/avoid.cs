using UnityEngine;

public class DualObjectSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject upwardObjectPrefab;    // ���� �ö󰡴� ������Ʈ ������
    [SerializeField]
    public GameObject downwardObjectPrefab;  // �Ʒ��� �������� ������Ʈ ������
    public float spawnInterval = 1f;         // ������Ʈ ���� ���� (��)
    private float timeElapsed = 0f;          // ��� �ð�
    private bool gameStopped = false;        // ���� ���� Ȯ��
    private float gravityScale = 10f;        // �߷� ������ �ʱⰪ

    void Start()
    {
        // ���� �������� �� ���� ������Ʈ ����
        InvokeRepeating("SpawnUpwardObject", 0f, spawnInterval);
        InvokeRepeating("SpawnDownwardObject", 0f, spawnInterval);
    }

    void Update()
    {
        if (gameStopped) return;

        // 3�и��� �߷°� ���� ���� ����
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 180.0f) // 3�� ��� ��
        {
            gravityScale += 5f; // �߷� ����
            timeElapsed = 0f;   // ��� �ð� �ʱ�ȭ

            // spawnInterval ����, �ּҰ� ����
            if (spawnInterval > 0.05f)
            {
                spawnInterval -= 0.05f;
            }

            // ���� �ݺ� ȣ�� ��� �� ���ο� �������� �缳��
            CancelInvoke("SpawnUpwardObject");
            CancelInvoke("SpawnDownwardObject");
            InvokeRepeating("SpawnUpwardObject", 0f, spawnInterval);
            InvokeRepeating("SpawnDownwardObject", 0f, spawnInterval);
        }
    }

    public void SpawnUpwardObject()
    {
        if (gameStopped) return;

        // ������ X ��ǥ�� Y ��ǥ ���� (�Ʒ����� ���� �ö󰡴� ����)
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, -425f, 0f);

        // ������Ʈ ����
        SpawnObject(upwardObjectPrefab, spawnPosition, -gravityScale);
    }

    public void SpawnDownwardObject()
    {
        if (gameStopped) return;

        // ������ X ��ǥ�� Y ��ǥ ���� (������ �Ʒ��� �������� ����)
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, 425f, 0f);

        // ������Ʈ ����
        SpawnObject(downwardObjectPrefab, spawnPosition, gravityScale);
    }

    private void SpawnObject(GameObject prefab, Vector3 position, float gravity)
    {
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);

        // Rigidbody2D �߰� �� �߷� ����
        Rigidbody2D rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Collider2D �߰� (���� ���)
        if (newObject.GetComponent<Collider2D>() == null)
        {
            newObject.AddComponent<BoxCollider2D>();
        }

        // ���� ��ũ��Ʈ �߰� �� ������ ���� ����
        ObjectMover moverScript = newObject.AddComponent<ObjectMover>();
        moverScript.spawnerScript = this;
    }

    public void StopGame()
    {
        gameStopped = true;
        CancelInvoke("SpawnUpwardObject");
        CancelInvoke("SpawnDownwardObject");
    }
}

public class ObjectMover : MonoBehaviour
{
    public DualObjectSpawner spawnerScript; // DualObjectSpawner ����

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
            spawnerScript.StopGame();
            Destroy(gameObject);
        }
    }
}
