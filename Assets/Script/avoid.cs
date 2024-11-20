using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DualObjectSpawner : MonoBehaviour
{
    [Header("Stage Prefabs")]
    [SerializeField] private GameObject[] stage1UpwardPrefabs;    // 스테이지 1의 위로 올라가는 오브젝트 프리팹들
    [SerializeField] private GameObject[] stage1DownwardPrefabs;  // 스테이지 1의 아래로 내려가는 오브젝트 프리팹들
    [SerializeField] private GameObject[] stage2UpwardPrefabs;    // 스테이지 2의 위로 올라가는 오브젝트 프리팹들
    [SerializeField] private GameObject[] stage2DownwardPrefabs;  // 스테이지 2의 아래로 내려가는 오브젝트 프리팹들
    [SerializeField] private GameObject[] stage3UpwardPrefabs;    // 스테이지 3의 위로 올라가는 오브젝트 프리팹들
    [SerializeField] private GameObject[] stage3DownwardPrefabs;  // 스테이지 3의 아래로 내려가는 오브젝트 프리팹들

    public float spawnInterval = 1f;      // 오브젝트 생성 간격 (초)
    private float timeElapsed = 0f;       // 경과 시간
    private bool gameStopped = false;     // 게임 상태 확인
    private float gravityScale = 10f;     // 중력 스케일 초기값
    public int currentStage = 1;         // 현재 스테이지

    void Start()
    {
        // 초기화
        InvokeRepeating("SpawnUpwardObject", 0f, spawnInterval);
        InvokeRepeating("SpawnDownwardObject", 0f, spawnInterval);
    }

    void Update()
    {
        if (gameStopped) return;

        // 경과 시간 업데이트
        timeElapsed += Time.deltaTime;

        // 3분마다 스테이지 전환
        if (timeElapsed >= 180.0f) // 3분 경과 시
        {
            gravityScale += 5f; // 중력 증가
            timeElapsed = 0f;   // 경과 시간 초기화

            // 스테이지 변경

            switch (currentStage)
            {
                case 1:
                   currentStage +=1; break;
                case 2:
                    currentStage += 1; break;
                case 3:
                    currentStage =1; break;
            }


            // spawnInterval 감소, 최소값 제한
            if (spawnInterval > 0.1f)
            {
                spawnInterval -= 0.1f;
            }

            // 기존 반복 호출 취소 및 새로운 간격으로 재설정
            CancelInvoke("SpawnUpwardObject");
            CancelInvoke("SpawnDownwardObject");
            InvokeRepeating("SpawnUpwardObject", 0f, spawnInterval);
            InvokeRepeating("SpawnDownwardObject", 0f, spawnInterval);
        }
    }

    public void SpawnUpwardObject()
    {
        if (gameStopped) return;

        // 무작위 X 좌표 설정
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, -425f, 0f);

        // 현재 스테이지에 따라 프리팹 선택
        GameObject prefab = GetStagePrefab(currentStage, true);

        // 오브젝트 생성
        SpawnObject(prefab, spawnPosition, -gravityScale);
    }

    public void SpawnDownwardObject()
    {
        if (gameStopped) return;

        // 무작위 X 좌표 설정
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, 425f, 0f);

        // 현재 스테이지에 따라 프리팹 선택
        GameObject prefab = GetStagePrefab(currentStage, false);

        // 오브젝트 생성
        SpawnObject(prefab, spawnPosition, gravityScale);
    }

    private GameObject GetStagePrefab(int stage, bool isUpward)
    {
        GameObject[] prefabArray;

        // 스테이지에 따라 프리팹 배열 선택
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

        // 배열에서 무작위 프리팹 선택
        return prefabArray[Random.Range(0, prefabArray.Length)];
    }

    private void SpawnObject(GameObject prefab, Vector3 position, float gravity)
    {
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);

        // Rigidbody2D 추가 및 중력 설정
        Rigidbody2D rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Collider2D 추가 (없을 경우)
        if (newObject.GetComponent<Collider2D>() == null)
        {
            newObject.AddComponent<BoxCollider2D>();
        }

        // 공통 스크립트 추가 및 스포너 참조 전달
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
    public DualObjectSpawner spawnerScript; // DualObjectSpawner 참조

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥 충돌 시 오브젝트 파괴
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        // 플레이어 충돌 시 게임 중지
        if (collision.gameObject.CompareTag("Player"))
        {
            spawnerScript.StopGame();
            Destroy(gameObject);
        }
    }
}
