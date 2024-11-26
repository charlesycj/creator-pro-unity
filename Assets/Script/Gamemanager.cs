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

   
    public float objectSpawnInterval = 1f; // 오브젝트 생성 간격
    public float buffSpawnInterval = 1f;   // 버프 생성 간격 (변경되지 않음)
    private float timeElapsed = 0f;
    private bool gameStopped = false;
    private float gravityScale = 10f;
    public int currentStage = 1;

    void Start()
    {
        // 오브젝트 생성 시작
        InvokeRepeating(nameof(SpawnUpwardObject), 0f, objectSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardObject), 0f, objectSpawnInterval);

        // 버프 생성 시작 (스테이지에 따라 변경 없음)
        InvokeRepeating(nameof(SpawnUpwardBuff), 0f, buffSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardBuff), buffSpawnInterval / 2, buffSpawnInterval);
    }

    void Update()
    {
        if (gameStopped) return;

        timeElapsed += Time.deltaTime;

        // 3분마다 스테이지 전환
        if (timeElapsed >= 180.0f)
        {
            gravityScale += 5f;
            timeElapsed = 0f;
            currentStage = currentStage == 3 ? 1 : currentStage + 1;

            // 오브젝트 생성 간격 감소 (최소값 제한)
            if (objectSpawnInterval > 0.1f) objectSpawnInterval -= 0.1f;

            // 오브젝트 생성 호출 재설정
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

    // 오브젝트 스폰 관련 함수
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

    // 버프 스폰 관련 함수
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

    // 공통 스폰 함수
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
            // PlayerManager 오브젝트를 찾기
            GameObject PlayerManager = GameObject.Find("PlayerManager"); // PlayerManager 오브젝트를 찾는다.

            if (PlayerManager != null)
            {
                // PlayerManager에서 PlayerMove 스크립트를 가져오기
                PlayerController PlayerMoveScript = PlayerManager.GetComponent<PlayerController>();

                if (PlayerMoveScript != null)
                {
                    // PlayerMove 스크립트가 있으면 playerA와 playerB에서 각각 TriggerGameOver 호출
                    PlayerController playerAController = PlayerMoveScript.playerA.GetComponent<PlayerController>();
                    PlayerController playerBController = PlayerMoveScript.playerB.GetComponent<PlayerController>();

                    if (playerAController != null)
                    {
                        playerAController.TriggerGameOver();
                        Debug.Log("Player A의 게임오버 호출됨");
                    }

                    if (playerBController != null)
                    {
                        playerBController.TriggerGameOver();
                        Debug.Log("Player B의 게임오버 호출됨");
                    }

                    spawnerScript.StopGame(); // 게임 중지
                    Debug.Log("플레이어와 충돌하여 게임이 멈췄습니다.");
                }
                else
                {
                    Debug.LogWarning("PlayerMove 스크립트를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("PlayerManager를 찾을 수 없습니다.");
            }

            Destroy(gameObject); // 충돌한 오브젝트 제거
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // Ground와 충돌 시 제거
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
