using UnityEngine;

public class DualObjectAndBuffSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] stage1UpwardPrefabs;
    [SerializeField] private GameObject[] stage1DownwardPrefabs;
    [SerializeField] private GameObject[] stage2UpwardPrefabs;
    [SerializeField] private GameObject[] stage2DownwardPrefabs;
    [SerializeField] private GameObject[] stage3UpwardPrefabs;
    [SerializeField] private GameObject[] stage3DownwardPrefabs; 

    [SerializeField] private GameObject SpeedBuffPrefab;
    [SerializeField] private GameObject ShieldBuffPrefab;
    [SerializeField] private GameObject HideBuffPrefab;

   
    public float objectSpawnInterval = 1f; // ������Ʈ ���� ����
    public float buffSpawnInterval = 1f;   // ���� ���� ���� (������� ����)
    private float timeElapsed = 0f;
    private bool gameStopped = false;
    private float gravityScale = 10f;
    public int currentStage = 1;

    void Start()
    {
        // ������Ʈ ���� ����
        InvokeRepeating(nameof(SpawnUpwardObject), 0f, objectSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardObject), 0f, objectSpawnInterval);

        // ���� ���� ���� (���������� ���� ���� ����)
        InvokeRepeating(nameof(SpawnUpwardBuff), 0f, buffSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardBuff), buffSpawnInterval / 2, buffSpawnInterval);
    }

    void Update()
    {
        if (gameStopped) return;

        timeElapsed += Time.deltaTime;

        // 3�и��� �������� ��ȯ
        if (timeElapsed >= 180.0f)
        {
            gravityScale += 5f;
            timeElapsed = 0f;
            currentStage = currentStage == 3 ? 1 : currentStage + 1;

            // ������Ʈ ���� ���� ���� (�ּҰ� ����)
            if (objectSpawnInterval > 0.1f) objectSpawnInterval -= 0.1f;

            // ������Ʈ ���� ȣ�� �缳��
            ResetObjectSpawns();
        }
    }

    private void ResetObjectSpawns()
    {
        CancelInvoke(nameof(SpawnUpwardObject));
        CancelInvoke(nameof(SpawnDownwardObject));
        InvokeRepeating(nameof(SpawnUpwardObject), 0f, objectSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardObject), 0f, objectSpawnInterval);
    }

    // ������Ʈ ���� ���� �Լ�
    public void SpawnUpwardObject()
    {
        if (gameStopped) return;
        SpawnObject(GetStagePrefab(currentStage, true), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale);
        
    }

    public void SpawnDownwardObject()
    {
        if (gameStopped) return;
        SpawnObject(GetStagePrefab(currentStage, false), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);
        
    }

    private GameObject GetStagePrefab(int stage, bool isUpward)
    {
        GameObject[] prefabArray = stage switch
        {
            1 => isUpward ? stage1UpwardPrefabs : stage1DownwardPrefabs,
            2 => isUpward ? stage2UpwardPrefabs : stage2DownwardPrefabs,
            3 => isUpward ? stage3UpwardPrefabs : stage3DownwardPrefabs,
            _ => isUpward ? stage1UpwardPrefabs : stage1DownwardPrefabs
        };

        return prefabArray[Random.Range(0, prefabArray.Length)];
    }

    // ���� ���� ���� �Լ�
    public void SpawnUpwardBuff()
    {
        if (gameStopped) return;
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale);
    }

    public void SpawnDownwardBuff()
    {
        if (gameStopped) return;
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);
    }

    private GameObject GetRandomBuffPrefab()
    {
        int buffType = Random.Range(1, 4);
        return buffType switch
        {
            1 => SpeedBuffPrefab,
            2 => ShieldBuffPrefab,
            3 => HideBuffPrefab,
            _ => null
        };
    }

    // ���� ���� �Լ�
    private void SpawnObject(GameObject prefab, Vector3 position, float gravity)
    {
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        var rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (newObject.GetComponent<Collider2D>() == null)
            newObject.AddComponent<BoxCollider2D>();

        var moverScript = newObject.AddComponent<ObjectMover>();
        moverScript.spawnerScript = this;
    }

    private void SpawnBuff(GameObject prefab, Vector3 position, float gravity)
    {
        if (prefab == null) return;

        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        var rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var moverScript = newObject.AddComponent<BuffMover>();
        moverScript.spawnerScript = this;
    }

    public void StopGame()
    {
        gameStopped = true;
        CancelInvoke(nameof(SpawnUpwardObject));
        CancelInvoke(nameof(SpawnDownwardObject));
        CancelInvoke(nameof(SpawnUpwardBuff));
        CancelInvoke(nameof(SpawnDownwardBuff));
    }
}

public class ObjectMover : MonoBehaviour
{
    public DualObjectAndBuffSpawner spawnerScript;

    void OnCollisionEnter2D(Collision2D collision)
    {
   

        if (collision.gameObject.CompareTag("Player"))
        {
            // PlayerManager ������Ʈ�� ã��
            GameObject PlayerManager = GameObject.Find("PlayerManager"); // PlayerManager ������Ʈ�� ã�´�.

            if (PlayerManager != null)
            {
                // PlayerManager���� PlayerMove ��ũ��Ʈ�� ��������
                PlayerController PlayerMoveScript = PlayerManager.GetComponent<PlayerController>();

                if (PlayerMoveScript != null)
                {
                    // PlayerMove ��ũ��Ʈ�� ������ playerA�� playerB���� ���� TriggerGameOver ȣ��
                    PlayerController playerAController = PlayerMoveScript.playerA.GetComponent<PlayerController>();
                    PlayerController playerBController = PlayerMoveScript.playerB.GetComponent<PlayerController>();

                    if (playerAController != null)
                    {
                        playerAController.TriggerGameOver();
                        Debug.Log("Player A�� ���ӿ��� ȣ���");
                    }

                    if (playerBController != null)
                    {
                        playerBController.TriggerGameOver();
                        Debug.Log("Player B�� ���ӿ��� ȣ���");
                    }

                    spawnerScript.StopGame(); // ���� ����
                    Debug.Log("�÷��̾�� �浹�Ͽ� ������ ������ϴ�.");
                }
                else
                {
                    Debug.LogWarning("PlayerMove ��ũ��Ʈ�� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogWarning("PlayerManager�� ã�� �� �����ϴ�.");
            }

            Destroy(gameObject); // �浹�� ������Ʈ ����
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // Ground�� �浹 �� ����
        }
    }



}



public class BuffMover : MonoBehaviour
{
    public DualObjectAndBuffSpawner spawnerScript;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
