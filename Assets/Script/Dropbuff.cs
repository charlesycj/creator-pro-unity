using UnityEngine;

public class ObjectDownbuff : MonoBehaviour
{
    [SerializeField]
    private GameObject buffPrefabDown;        // 버프 아이템 프리팹
    private float timeElapsed = 0f;      // 경과 시간
    private float buffCycleTime = 15f;   // 버프 주기 (15초)
    private float nextBuffTime = 0f;     // 다음 버프 아이템 생성 시간
    private bool gameStopped = false;    // 게임 상태 확인
    private float gravityScale = 10f;    // 중력 스케일 초기값

    void Start()
    {
        // 초기 버프 생성 시간 설정
        SetNextBuffTime();
    }

    void Update()
    {
        // 게임이 중단된 경우 업데이트 동작 정지
        if (gameStopped) return;

        // 버프 아이템 관련 타이머 업데이트
        timeElapsed += Time.deltaTime;

        // 버프 아이템 생성 시간 확인
        if (timeElapsed >= nextBuffTime)
        {
            SpawnBuff();
            SetNextBuffTime(); // 다음 버프 시간 설정
        }
    }

    public void SpawnBuff()
    {
        if (gameStopped) return; // 게임이 멈췄다면 버프 생성 중지

        // 화면의 무작위 X 좌표에서 버프를 생성
        float randomX = Random.Range(-850f, 850f); // 화면 범위에 맞게 X 범위 설정
        Vector3 spawnPosition = new Vector3(randomX, 425f, 0f); // Y는 425으로 설정 (위에서 내려오기 시작)

        // 버프 생성
        GameObject buff = Instantiate(buffPrefabDown, spawnPosition, Quaternion.identity);

        // Rigidbody2D 컴포넌트 추가 및 중력 설정
        Rigidbody2D rb = buff.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale; // 중력값 적용

        // Collider2D가 없다면 추가 (물리 충돌을 위해)
        if (buff.GetComponent<Collider2D>() == null)
        {
            buff.AddComponent<BoxCollider2D>();
        }
    }

    private void SetNextBuffTime()
    {
        // 현재 시간을 기준으로 15초 주기 안에서 무작위 시간 설정
        nextBuffTime = timeElapsed + Random.Range(0f, buffCycleTime);
    }

    public void StopGame()
    {
        gameStopped = true; // 게임 멈춤 상태 설정
    }
}

public class Downbuff : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥과 충돌했을 때 버프 파괴
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // 버프 파괴
        }

        // 플레이어와 충돌했을 때 버프 습득
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // 버프 습득 후 파괴
        }
    }
}
