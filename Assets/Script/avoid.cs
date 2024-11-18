using UnityEngine;

public class DualObjectSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject upwardObjectPrefab;    // 위로 올라가는 오브젝트 프리팹
    [SerializeField]
    public GameObject downwardObjectPrefab;  // 아래로 내려가는 오브젝트 프리팹
    public float spawnInterval = 1f;         // 오브젝트 생성 간격 (초)
    private float timeElapsed = 0f;          // 경과 시간
    private bool gameStopped = false;        // 게임 상태 확인
    private float gravityScale = 10f;        // 중력 스케일 초기값

    void Start()
    {
        // 일정 간격으로 두 방향 오브젝트 생성
        InvokeRepeating("SpawnUpwardObject", 0f, spawnInterval);
        InvokeRepeating("SpawnDownwardObject", 0f, spawnInterval);
    }

    void Update()
    {
        if (gameStopped) return;

        // 3분마다 중력과 생성 간격 조정
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 180.0f) // 3분 경과 시
        {
            gravityScale += 5f; // 중력 증가
            timeElapsed = 0f;   // 경과 시간 초기화

            // spawnInterval 감소, 최소값 제한
            if (spawnInterval > 0.05f)
            {
                spawnInterval -= 0.05f;
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

        // 무작위 X 좌표와 Y 좌표 설정 (아래에서 위로 올라가는 방향)
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, -425f, 0f);

        // 오브젝트 생성
        SpawnObject(upwardObjectPrefab, spawnPosition, -gravityScale);
    }

    public void SpawnDownwardObject()
    {
        if (gameStopped) return;

        // 무작위 X 좌표와 Y 좌표 설정 (위에서 아래로 내려오는 방향)
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, 425f, 0f);

        // 오브젝트 생성
        SpawnObject(downwardObjectPrefab, spawnPosition, gravityScale);
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
