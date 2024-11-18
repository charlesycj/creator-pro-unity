using UnityEngine;
using System.Collections.Generic;

public class DualbuffSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject upwardbuffPrefab;    // 위로 올라가는 오브젝트 프리팹
    [SerializeField]
    public GameObject downwardbuffPrefab;  // 아래로 내려가는 오브젝트 프리팹
    public float spawnIntervalbuff = 15.0f;         // 오브젝트 생성 간격 (초)
    private bool gameStopped = false;        // 게임 상태 확인
    private float gravityScale = 10f;        // 중력 스케일 초기값

    void Start()
    {
        // 일정 간격으로 두 방향 오브젝트 생성
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

        // 무작위 X 좌표와 Y 좌표 설정 (아래에서 위로 올라가는 방향)
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, -425f, 0f);

        // 오브젝트 생성
        Spawnbuff(upwardbuffPrefab, spawnPosition, -gravityScale);
    }

    public void SpawnDownwardbuff()
    {
        if (gameStopped) return;

        // 무작위 X 좌표와 Y 좌표 설정 (위에서 아래로 내려오는 방향)
        float randomX = Random.Range(-850f, 850f);
        Vector3 spawnPosition = new Vector3(randomX, 425f, 0f);

        // 오브젝트 생성
        Spawnbuff(downwardbuffPrefab, spawnPosition, gravityScale);
    }

    private void Spawnbuff(GameObject prefab, Vector3 position, float gravity)
    {
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);

        // Rigidbody2D 추가 및 중력 설정
        Rigidbody2D rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // 공통 스크립트 추가 및 스포너 참조 전달
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
    public DualbuffSpawner spawnerScript; // DualbuffSpawner 참조

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
            Destroy(gameObject);
        }
    }
}
