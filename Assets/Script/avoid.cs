using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DualObjectSpawner : MonoBehaviour
{
    [Header("Stage Prefabs")]
    [SerializeField] private GameObject[] stage1UpwardPrefabs;    // �������� 1�� ���� �ö󰡴� ������Ʈ �����յ�
    [SerializeField] private GameObject[] stage1DownwardPrefabs;  // �������� 1�� �Ʒ��� �������� ������Ʈ �����յ�
    [SerializeField] private GameObject[] stage2UpwardPrefabs;    // �������� 2�� ���� �ö󰡴� ������Ʈ �����յ�
    [SerializeField] private GameObject[] stage2DownwardPrefabs;  // �������� 2�� �Ʒ��� �������� ������Ʈ �����յ�
    [SerializeField] private GameObject[] stage3UpwardPrefabs;    // �������� 3�� ���� �ö󰡴� ������Ʈ �����յ�
    [SerializeField] private GameObject[] stage3DownwardPrefabs;  // �������� 3�� �Ʒ��� �������� ������Ʈ �����յ�

    public float spawnInterval = 1f;      // ������Ʈ ���� ���� (��)
    private float timeElapsed = 0f;       // ��� �ð�
    private bool gameStopped = false;     // ���� ���� Ȯ��
    private float gravityScale = 10f;     // �߷� ������ �ʱⰪ
    public int currentStage = 1;         // ���� ��������

    void Start()
    {
        // �ʱ�ȭ
        InvokeRepeating("SpawnUpwardObject", 0f, spawnInterval);
        InvokeRepeating("SpawnDownwardObject", 0f, spawnInterval);
    }

    void Update()
    {
        if (gameStopped) return;

        // ��� �ð� ������Ʈ
        timeElapsed += Time.deltaTime;

        // 3�и��� �������� ��ȯ
        if (timeElapsed >= 180.0f) // 3�� ��� ��
        {
            gravityScale += 5f; // �߷� ����
            timeElapsed = 0f;   // ��� �ð� �ʱ�ȭ

            // �������� ����

            switch (currentStage)
            {
                case 1:
                   currentStage +=1; break;
                case 2:
                    currentStage += 1; break;
                case 3:
                    currentStage =1; break;
            }


            // spawnInterval ����, �ּҰ� ����
            if (spawnInterval > 0.1f)
            {
                spawnInterval -= 0.1f;
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

        // ������ X ��ǥ ����
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, -425f, 0f);

        // ���� ���������� ���� ������ ����
        GameObject prefab = GetStagePrefab(currentStage, true);

        // ������Ʈ ����
        SpawnObject(prefab, spawnPosition, -gravityScale);
    }

    public void SpawnDownwardObject()
    {
        if (gameStopped) return;

        // ������ X ��ǥ ����
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, 425f, 0f);

        // ���� ���������� ���� ������ ����
        GameObject prefab = GetStagePrefab(currentStage, false);

        // ������Ʈ ����
        SpawnObject(prefab, spawnPosition, gravityScale);
    }

    private GameObject GetStagePrefab(int stage, bool isUpward)
    {
        GameObject[] prefabArray;

        // ���������� ���� ������ �迭 ����
        switch (stage)
        {
            case 1:
                prefabArray = isUpward ? stage1UpwardPrefabs : stage1DownwardPrefabs;
                break;
            case 2:
                prefabArray = isUpward ? stage2UpwardPrefabs : stage2DownwardPrefabs;
                break;
            case 3:
                prefabArray = isUpward ? stage3UpwardPrefabs : stage3DownwardPrefabs;
                break;
            default:
                prefabArray = isUpward ? stage1UpwardPrefabs : stage1DownwardPrefabs;
                break;
        }

        // �迭���� ������ ������ ����
        return prefabArray[Random.Range(0, prefabArray.Length)];
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
