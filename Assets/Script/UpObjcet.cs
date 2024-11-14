using UnityEngine;

public class ObjectUp : MonoBehaviour
{
    public GameObject objectPrefab;      // 내려갈 오브젝트 프리팹
    public float spawnInterval = 1f;     // 오브젝트 생성 간격 (초)
    private float timeElapsed = 0f;      // 경과 시간
    private bool gameStopped = false;    // 게임 상태 확인
    private float gravityScale = 10f;    // 중력 스케일 초기값
    private bool gravityReversed = true; // 중력반전

    void Start()
    {
        // 1초에 한 번씩 오브젝트를 생성하는 메서드 호출
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    void Update()
    {
        // 게임이 중단된 경우 업데이트 동작 정지
        if (gameStopped) return;

        // 시간 3분이 경과할 때마다 spawnInterval을 0.05씩 감소
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 180.0f) // 3분 경과 시
        {
            // 중력값 증가
            gravityScale += 5f; // 예시로 5씩 증가, 필요에 따라 조정

            // 경과 시간 초기화
            timeElapsed = 0f;

            // spawnInterval을 0.05씩 감소
            if (spawnInterval > 0.05f)
            {
                spawnInterval -= 0.05f;
            }

            // 기존 InvokeRepeating 취소
            CancelInvoke("SpawnObject");
            // 새로운 spawnInterval로 반복 호출 시작
            InvokeRepeating("SpawnObject", 0f, spawnInterval);
        }

      
      
    }

    public void SpawnObject()
    {
        if (gameStopped) return; // 게임이 멈췄다면 오브젝트 생성 중지

        // 화면의 무작위 X 좌표에서 오브젝트를 생성
        float randomX = Random.Range(-850f, 850f); // 화면 범위에 맞게 X 범위 설정
        Vector3 spawnPosition = new Vector3(randomX, -425f, 0f); // Y는 -425으로 설정 (아래에서 위로 )

        // 오브젝트 생성
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        // Rigidbody2D 컴포넌트 추가 및 중력 설정
        Rigidbody2D rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravityReversed ? -gravityScale : gravityScale; // 중력값 적용

        // Collider2D가 없다면 추가 (물리 충돌을 위해)
        if (newObject.GetComponent<Collider2D>() == null)
        {
            newObject.AddComponent<BoxCollider2D>();
        }

        // Up 스크립트 추가
        Up UpScript = newObject.AddComponent<Up>();
        UpScript.objectUpScript = this; // ObjectUp 참조 전달
    }

    public void StopGame()
    {
        gameStopped = true; // 게임 멈춤 상태 설정
        CancelInvoke("SpawnObject"); // 오브젝트 생성 중지
    }
}

public class Up : MonoBehaviour
{
    public ObjectUp objectUpScript;  // ObjectUp 참조

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥과 충돌했을 때 오브젝트 파괴
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // 오브젝트 파괴
        }

        // 플레이어와 충돌했을 때 게임 중지
        if (collision.gameObject.CompareTag("Player"))
        {
            objectUpScript.StopGame();
            Destroy(gameObject);
        }
    }
}
