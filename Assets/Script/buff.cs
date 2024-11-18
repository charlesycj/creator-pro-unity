using UnityEngine;
using System.Collections.Generic;

public class ObjectBuff : MonoBehaviour
{
    [SerializeField]
    private GameObject upwardBuffPrefab;    // 위로 올라가는 버프 프리팹
    [SerializeField]
    private GameObject downwardBuffPrefab; // 아래로 내려가는 버프 프리팹
    private float timeElapsed = 0f;        // 경과 시간
    private float buffCycleTime = 15f;     // 버프 주기 (15초)
    private float nextUpwardBuffTime = 0f; // 다음 위로 올라가는 버프 생성 시간
    private float nextDownwardBuffTime = 0f; // 다음 아래로 내려가는 버프 생성 시간
    private bool gameStopped = false;      // 게임 상태 확인
    private float gravityScale = 10f;      // 중력 스케일 초기값

    private List<GameObject> activeBuffs = new List<GameObject>(); // 활성화된 버프 리스트

    void Start()
    {
        // 초기 버프 생성 시간 설정
        SetNextUpwardBuffTime();
        SetNextDownwardBuffTime();
    }

    void Update()
    {
        // 게임이 중단된 경우 업데이트 동작 정지
        if (gameStopped) return;

        // 버프 아이템 관련 타이머 업데이트
        timeElapsed += Time.deltaTime;

        // 위로 올라가는 버프 생성 시간 확인
        if (timeElapsed >= nextUpwardBuffTime)
        {
            SpawnBuff(upwardBuffPrefab, true);
            SetNextUpwardBuffTime(); // 다음 위로 올라가는 버프 시간 설정
        }

        // 아래로 내려가는 버프 생성 시간 확인
        if (timeElapsed >= nextDownwardBuffTime)
        {
            SpawnBuff(downwardBuffPrefab, false);
            SetNextDownwardBuffTime(); // 다음 아래로 내려가는 버프 시간 설정
        }
    }

    public void SpawnBuff(GameObject buffPrefab, bool isUpwardBuff)
    {
        if (gameStopped) return; // 게임이 멈췄다면 버프 생성 중지

        // 화면의 무작위 X 좌표에서 버프를 생성
        float randomX = Random.Range(-850f, 850f);
        float spawnY = isUpwardBuff ? -425f : 425f; // 위로 가는 경우는 아래에서, 내려가는 경우는 위에서 시작
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        // 버프 생성
        GameObject buff = Instantiate(buffPrefab, spawnPosition, Quaternion.identity);
        activeBuffs.Add(buff); // 리스트에 추가

        // Rigidbody2D 컴포넌트 추가 및 중력 설정
        Rigidbody2D rb = buff.AddComponent<Rigidbody2D>();
        rb.gravityScale = isUpwardBuff ? -gravityScale : gravityScale; // 위로 가는 경우 중력 반전
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Collider2D가 없다면 추가 (물리 충돌을 위해)
        if (buff.GetComponent<Collider2D>() == null)
        {
            buff.AddComponent<BoxCollider2D>();
        }
    }

    private void SetNextUpwardBuffTime()
    {
        // 현재 시간을 기준으로 무작위 시간 설정
        nextUpwardBuffTime = timeElapsed + Random.Range(0f, buffCycleTime);
    }

    private void SetNextDownwardBuffTime()
    {
        // 현재 시간을 기준으로 무작위 시간 설정
        nextDownwardBuffTime = timeElapsed + Random.Range(0f, buffCycleTime);
    }

    public void StopGame()
    {
        gameStopped = true; // 게임 멈춤 상태 설정

        // 활성화된 모든 버프 제거
        foreach (var buff in activeBuffs)
        {
            if (buff != null)
            {
                Destroy(buff);
            }
        }
        activeBuffs.Clear(); // 리스트 초기화
    }
}
