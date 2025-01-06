using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using JetBrains.Annotations;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //떨어지는 장애물 관리
    [SerializeField] private GameObject[] stage1UpwardPrefabs;
    [SerializeField] private GameObject[] stage1DownwardPrefabs;
    [SerializeField] private GameObject[] stage2UpwardPrefabs;
    [SerializeField] private GameObject[] stage2DownwardPrefabs;
    [SerializeField] private GameObject[] stage3UpwardPrefabs;
    [SerializeField] private GameObject[] stage3DownwardPrefabs;

    //떨어지는 버프 관리
    [SerializeField] private GameObject SpeedBuffPrefab; //스피드
    [SerializeField] private GameObject ShieldBuffPrefab; //보호막
    [SerializeField] private GameObject HideBuffPrefab; // 3초무적

    [SerializeField] public GameObject scorePrefab; // Score 프리팹 참조
    [SerializeField] private TextMeshProUGUI scoreTextMesh1; //현재 점수 텍스트
    [SerializeField] private TextMeshProUGUI scoreTextMesh2; //현재 점수 텍스트
    [SerializeField] private TextMeshProUGUI bestScoreTextMesh; // 최고 점수 텍스트

    [SerializeField] private Animator playerAnimatorA; // Animator 참조
    [SerializeField] private Animator playerAnimatorB; // Animator 참조

    [SerializeField] public float objectSpawnInterval=4f; // 오브젝트 생성 간격
    [SerializeField] public float minobjectSpawnInterval=0.4f; // 최소 오브젝트 생성 간격
    [SerializeField] public float spawnIntervalStep=0.1f; // 장애물 스폰주기 감소값

    [SerializeField] public float buffSpawnInterval = 6f;   // 버프 생성 간격 
    [SerializeField] public float minbuffSpawnInterval=3f; // 최소 버프 생성 간격
    [SerializeField] public float buffspawnIntervalStep=0.05f; //버프 스폰주기 감소값

    [SerializeField] private float gravityScale = 20f; //중력값

    private float timeElapsed = 0f;
    private float timeElapsedobject = 0f;
    public bool gameStopped = false;

    public int currentStage = 1;
    private GameObject scoreObject;


    public int Score = 0; //현재 점수
    public int BestScore; //최고점수 
    public int AccumulatedScore = 0; //누적점수

    public GameObject gameOverCanvas; // Canvas-GameOver 오브젝트를 참조할 변수
    public GameObject DeadPlayerA; // Canvas-GameOver 오브젝트를 참조할 변수
    public GameObject DeadPlayerB; // Canvas-GameOver 오브젝트를 참조할 변수

    public Action<int> onStageEntered;
    public Action<int> onScoreReached;
    public Action<int> onAccumulatedScoreReached;
    public Action<int> onBestScoreAchieved;

    public BuffMover buffMover;

    void Start()
    {
        onStageEntered?.Invoke(currentStage);
        // 최고 점수 불러오기
        BestScore = PlayerPrefs.GetInt("BestScore", 0); // 기본값은 0
        BestScore = 0; // 테스트용 최고점수 초기화
        AccumulatedScore = PlayerPrefs.GetInt("AccumulatedScore", 0); // 누적점수 불러오기
        UpdateScoreText(); // 초기 점수 갱신
        UpdateBestScoreText(); // 최고 점수 텍스트 초기화

        //스코어 텍스트 설정
        if (scorePrefab != null)
        {
            // scorePrefab이 씬에 존재한다고 가정하고, 그 안에 있는 TextMeshProUGUI를 찾음
            scoreTextMesh1 = scorePrefab.GetComponentInChildren<TextMeshProUGUI>();

            if (scoreTextMesh1 != null)
            {
                UpdateScoreText(); // 초기 점수 표시
            }
            if (scoreTextMesh2 != null)
            {
                scoreTextMesh2.text = $"Score: {Score:D5}"; // 두 번째 점수 UI 초기화
            }
        }

        // SoundManager 인스턴스 가져오기
        SoundManager soundManager = FindObjectOfType<SoundManager>();
        if (soundManager != null)
        {
            soundManager.PlayStageBGM(1); // 초기 스테이지 1 BGM 재생
        }
        else
        {
            Debug.LogError("SoundManager를 찾을 수 없습니다!");
        }
    }


    public void IncreaseScore(int amount)
    {
        Score += amount;
        onScoreReached?.Invoke(Score);

        AccumulatedScore += amount;
        PlayerPrefs.SetInt("AccumulatedScore", AccumulatedScore);
        onAccumulatedScoreReached?.Invoke(AccumulatedScore);

        if (Score > BestScore)
        {
            BestScore = Score;
            onBestScoreAchieved?.Invoke(BestScore);
            //Debug.Log($"최고 점수가 갱신되었습니다: {BestScore}");

            // 최고 점수 저장
            PlayerPrefs.SetInt("BestScore", BestScore);
        }
        PlayerPrefs.Save(); // 변경 사항을 저장
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        // 첫 번째 텍스트 UI에 점수 갱신
        if (scoreTextMesh1 != null)
        {
            scoreTextMesh1.text = $"Score: {Score:D5}"; // 5자리로 점수 표시
        }

        // 두 번째 텍스트 UI에도 점수 갱신
        if (scoreTextMesh2 != null)
        {
            scoreTextMesh2.text = $"Score: {Score:D5}"; // 5자리로 점수 표시
        }
    }
    private void UpdateBestScoreText()
    {
        if (bestScoreTextMesh != null)
        {
            bestScoreTextMesh.text = $"Best Score: {BestScore:D5}"; // 최고 점수를 5자리로 표시
        }
    }
    void Update()
    {
        // 게임 정지 후 플레이어 좌우반전 방지
        if (gameStopped)
        {
            if (playerAnimatorA != null)
                FixPlayerFlip(playerAnimatorA.gameObject);

            if (playerAnimatorB != null)
                FixPlayerFlip(playerAnimatorB.gameObject);
        }

        if (gameStopped) return;

        timeElapsed += Time.deltaTime;
        timeElapsedobject += Time.deltaTime;

        // 1분마다 스테이지 전환
        if (timeElapsed >= 60f)
        {
            SoundManager soundManager = FindObjectOfType<SoundManager>(); // SoundManager 찾기
            soundManager.playLevelTransition(); //사운드 재생
            gravityScale += 5f;
            timeElapsed = 0f;
            currentStage = currentStage == 3 ? 1 : currentStage + 1;

            // 스테이지에 맞는 BGM 재생
            soundManager.PlayStageBGM(currentStage);

            // 오브젝트 생성 호출 재설정
            ResetObjectSpawns();
        }

        //1초마다 오브젝트 생성 간격 변경
        if (timeElapsedobject >= 5f)
        {
            timeElapsedobject = 0f;
            // 오브젝트 생성 간격 감소
            if (objectSpawnInterval > minobjectSpawnInterval) //최소값 제한
            {
                objectSpawnInterval -= spawnIntervalStep;
                objectSpawnInterval = Mathf.Round(objectSpawnInterval * 1000f) / 1000f; // 소수점 셋째 자리까지 반올림
                // 오브젝트 생성 호출 재설정
                ResetObjectSpawns();
            }
            //버프 오브젝트 생성 간격 감소 
            if (buffSpawnInterval > minbuffSpawnInterval)//최소값 제한
            {
                buffSpawnInterval -= buffspawnIntervalStep;
                buffSpawnInterval = Mathf.Round(buffSpawnInterval * 1000f) / 1000f; // 소수점 셋째 자리까지 반올림

                // 오브젝트 생성 호출 재설정
                ResetbuffSpawns();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.F1)) //테스트용 최고점수 초기화 코드
        {
            BestScore = 0; // F1을 누르면 BestScore를 초기화
            Debug.Log("최고점수가 초기화 되었습니다");
            bestScoreTextMesh.text = $"Best Score: {BestScore:D5}";
        }
    }

    private void ResetObjectSpawns()
    {
            Debug.Log($"장애물 생성 간격  {objectSpawnInterval}");
            CancelInvoke(nameof(SpawnUpwardObject));
            CancelInvoke(nameof(SpawnDownwardObject));
        if (objectSpawnInterval > minobjectSpawnInterval)
        {
            InvokeRepeating(nameof(SpawnUpwardObject), 0f, objectSpawnInterval);
            InvokeRepeating(nameof(SpawnDownwardObject), 0f, objectSpawnInterval);
            Debug.Log("장애물 생성 재시작");
        }
        else
        {
            Debug.LogWarning("objectSpawnInterval이 최소값 이하입니다. " +
                "InvokeRepeating 실행되지 않음.");
        }

    }
    private void ResetbuffSpawns()
    {
            Debug.Log($"버프아이템 생성 간격  {buffSpawnInterval}");
            CancelInvoke(nameof(SpawnUpwardBuff));
            CancelInvoke(nameof(SpawnDownwardBuff));

        if (buffSpawnInterval > minbuffSpawnInterval)
        {
            InvokeRepeating(nameof(SpawnUpwardBuff), 0f, buffSpawnInterval);
            InvokeRepeating(nameof(SpawnDownwardBuff), 0f, buffSpawnInterval);
        }
        else
        {
            Debug.LogWarning("buffSpawnInterval이 최소값 이하입니다." +
                " InvokeRepeating 실행되지 않음.");
        }
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

    // 버프 스폰 관련 함수
    public void SpawnUpwardBuff() //올라 는 버프 무작위좌표에 생성
    {
        if (gameStopped) return;
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale);
    }

    public void SpawnDownwardBuff() //내려가는 버프 무작위좌표에 생성
    {
        if (gameStopped) return;
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);
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

 

    private GameObject GetRandomBuffPrefab() //버프아이템 무작위결정
    {
        int buffType = Random.Range(1, 4);
        return buffType switch
        {
            1 => SpeedBuffPrefab, //1은 속도증가 버프
            2 => ShieldBuffPrefab,//2는 1회용보호막
            3 => HideBuffPrefab, //3은 5초무적
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
        buffMover = moverScript;
    }

    public void StopGame() //게임정지시 오브젝트 생성 정지
    {

        SoundManager soundManager = FindObjectOfType<SoundManager>(); // SoundManager 찾기
        soundManager.PlayGameOverSound(); // 속도 버프 사운드 재생
        gameStopped = true; // 게임 정지 상태 플래그 설정
        CancelInvoke(nameof(SpawnUpwardObject));
        CancelInvoke(nameof(SpawnDownwardObject));
        CancelInvoke(nameof(SpawnUpwardBuff));
        CancelInvoke(nameof(SpawnDownwardBuff));

        // Player 사망시 애니메이션 실행
        if (playerAnimatorA != null || playerAnimatorB != null)
        {
            playerAnimatorA.SetTrigger("dead");
            playerAnimatorB.SetTrigger("dead");
        }

        // 현재 점수가 최고 점수보다 높으면 갱신
        if (Score > BestScore)
        {
            BestScore = Score;
            Debug.Log($"최고 점수가 갱신되었습니다: {BestScore}");

            // 최고 점수 저장
            PlayerPrefs.SetInt("Score", BestScore);
            PlayerPrefs.Save(); // 변경 사항을 저장
        }

        // 화면에 있는 모든 장애물 및 버프 제거
        ClearAllObstaclesAndBuffs();
        // 최고 점수 텍스트 갱신
        UpdateBestScoreText();


        // Canvas-GameOver 활성화
        if (gameOverCanvas != null)
        {
            GameObject playerManagerObject = GameObject.Find("PlayerManager");
            if (playerManagerObject != null)
            {
                PlayerManager playerManager = playerManagerObject.GetComponent<PlayerManager>();

                gameOverCanvas.SetActive(true);
                if (playerManager != null)
                    //DeadPlayer 값 확인
                    if (playerManager.DeadPlayer == "Player A")
                    {
                        // Player A 전용 Canvas 활성화
                        DeadPlayerA.SetActive(true);

                    }
                    else if (playerManager.DeadPlayer == "Player B")
                    {
                        // Player B 전용 Canvas 활성화                                       
                        DeadPlayerB.SetActive(true);
                    }
            }
        }
    }

    // 플레이어 사망시 좌우반전 방지 함수
    private void FixPlayerFlip(GameObject player)
    {
        if (player != null)
        {
            var spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.flipX = false;

            // localScale.x를 양수로 강제
            Vector3 fixedScale = player.transform.localScale;
            fixedScale.x = Mathf.Abs(fixedScale.x);
            player.transform.localScale = fixedScale;
        }
    }

    // 장애물 및 버프 제거 함수
    private void ClearAllObstaclesAndBuffs()
    {

        // 모든 태그를 배열로 정의
        string[] buffTags = { "SpeedBuff", "ShieldBuff", "HideBuff" };
        // 장애물 제거
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Avoid");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        foreach (string tag in buffTags)
        {
            GameObject[] buffs = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject buff in buffs)
            {
                Destroy(buff);
            }
        }
        Debug.Log("화면의 모든 장애물과 버프가 제거되었습니다.");

    }
}

public class ObjectMover : MonoBehaviour
{
    public GameManager spawnerScript;
    private bool isExploding = false; // explosion 상태 플래그

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Player와 충돌 처리
        if (collision.gameObject.CompareTag("Player"))
        {
            // explosion 상태라면 충돌 무시
            if (isExploding)
            {
                return;
            }

            GameObject playerManager = GameObject.Find("PlayerManager");
            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

            // 은신 상태일 때 충돌 무시
            if (playerController.IsPlayerHide(isPlayerA))
            {
                Debug.Log("은신 상태로 장애물 무시");
                Destroy(gameObject); // 장애물 제거
                return;
            }

            // 실드가 활성화된 경우 장애물만 방어
            if (playerController.IsShieldActive(isPlayerA))
            {
                Debug.Log("실드가 활성화되어 장애물 방어");
                playerController.RemoveShieldBuff(isPlayerA); // 실드 버프 해제
                Destroy(gameObject); // 장애물 제거
                return;
            }

            // 게임 오버 처리
            if (collision.gameObject == playerController.playerA)
            {
                Debug.Log("Player A가 죽었습니다. 게임 오버!");
                playerController.DeadPlayer = "Player A";
            }
            else if (collision.gameObject == playerController.playerB)
            {
                Debug.Log("Player B가 죽었습니다. 게임 오버!");
                playerController.DeadPlayer = "Player B";
            }
            spawnerScript.StopGame();
            Destroy(gameObject);
            Debug.Log("충돌로 게임 오버!");
            playerController.playerASpeed = 0;
            playerController.playerBSpeed = 0;

        }

        // Ground와 충돌 처리
        if (collision.gameObject.CompareTag("Ground"))
        {
            spawnerScript.IncreaseScore(1); // 점수 1 증가

            Animator enemyAnimator = gameObject.GetComponent<Animator>();
            if (enemyAnimator != null)
            {
                enemyAnimator.SetTrigger("explosion"); // 애니메이션 트리거 호출
                isExploding = true; // explosion 상태로 설정
            }

            // explosion 상태에서 충돌을 트리거로 전환 : explosion상태에서 enemy와 충돌 방지
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            // Rigidbody2D 중력 비활성화 및 속도 초기화 : enemy explosion상태에서 collider가 없어서 내려가는 문제 방지
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;       // 속도 초기화
                rb.gravityScale = 0;             // 중력 비활성화
            }


            // 애니메이션이 끝난 후에 게임 오브젝트를 삭제하도록 Coroutine 호출
            StartCoroutine(DestroyAfterAnimation(enemyAnimator));
        }
    }

    IEnumerator DestroyAfterAnimation(Animator animator)
    {
        // 애니메이션 길이만큼 대기
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // 대기 후 게임 오브젝트 삭제
        Destroy(gameObject);
    }
}




public class BuffMover : MonoBehaviour
{
    public GameManager spawnerScript;

    public static Action onItemAcquired;

    // 충돌 처리
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject playerManager = GameObject.Find("PlayerManager");
            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

            SoundManager soundManager = FindObjectOfType<SoundManager>(); // SoundManager 찾기

            // 은신 상태와 충돌 처리
            if (playerController.IsPlayerHide(isPlayerA) && !CompareTag("SpeedBuff") && !CompareTag("ShieldBuff") && !CompareTag("HideBuff"))
            {
                Debug.Log("은신 상태로 장애물 무시");
                Destroy(gameObject); // 장애물 제거
                return;
            }

            if (CompareTag("SpeedBuff"))
            {
                // 스피드 버프 코루틴 관리
                playerController.StartManagedCoroutine(
                    playerController.activeSpeedBuffCoroutines, isPlayerA,
                    ApplySpeedBuff(playerController, isPlayerA)
                );

                soundManager.PlaySpeedBuffSound(); // 속도 버프 사운드 재생
                Debug.Log("스피드업 버프 획득");
            }

            else if (CompareTag("HideBuff"))
            {
                float hideBuffDuration = 3f; // 은신 버프 지속 시간 (초)
                // 은신 버프 코루틴 관리
                playerController.StartManagedCoroutine(
                    playerController.activeHideBuffCoroutines, isPlayerA,
                    ApplyHideBuff(hideBuffDuration, isPlayerA)
                );

                soundManager.PlayHideBuffSound(); // 은신 버프 사운드 재생
                Debug.Log("은신 버프 획득");
            }

            else if (CompareTag("ShieldBuff"))
            {
                if (!playerController.HasShield(isPlayerA)) // 이미 실드가 없는 경우에만 적용
                {
                    playerController.ApplyShieldBuff(isPlayerA);
                    Debug.Log("실드 버프 획득");
                }
                else
                {
                    Debug.Log("실드가 이미 활성화되어 있습니다.");
                }
                soundManager.PlayShieldBuffSound(); // 실드 버프 사운드 재생
            }
            onItemAcquired?.Invoke();

            Destroy(gameObject);
        }


        if (collision.gameObject.CompareTag("Ground"))
        {
                   Destroy(gameObject); //바닥에 충돌하면 제거
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject playerManager = GameObject.Find("PlayerManager");
            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

            // 은신 상태일 때 충돌을 계속 무시
            if (playerController.IsPlayerHide(isPlayerA) && !CompareTag("SpeedBuff") && !CompareTag("ShieldBuff") && !CompareTag("HideBuff"))
            {
                GameObject player = isPlayerA ? playerController.playerA : playerController.playerB;
                Collider2D playerCollider = player.GetComponent<Collider2D>();

                foreach (GameObject Avoid in GameObject.FindGameObjectsWithTag("Avoid"))
                {
                    Collider2D obstacleCollider = Avoid.GetComponent<Collider2D>();
                    if (obstacleCollider != null)
                    {
                        Physics2D.IgnoreCollision(playerCollider, obstacleCollider, true);
                    }
                }
            }
        }
    }

    // 스피드업 버프 적용
    private IEnumerator ApplySpeedBuff(PlayerManager playerController, bool isPlayerA)
    {
        // Animator 가져오기
        GameObject player = isPlayerA ? playerController.playerA : playerController.playerB;
        Animator playerAnimator = player.GetComponent<Animator>();

        if (playerAnimator == null)
        {
            Debug.LogError("Animator를 찾을 수 없습니다. 플레이어에 Animator가 추가되어 있는지 확인하세요.");
            yield break;
        }
        // 애니메이션 속도 2배로 증가
        playerAnimator.speed = 2.5f;


        // 버프 적용
        if (isPlayerA)
            playerController.playerASpeed += 200;
        else
            playerController.playerBSpeed += 200;

        yield return new WaitForSeconds(2f); // 첫 2초 동안 고정된 증가 유지

        // 남은 2초 동안 0.5초 간격으로 속도를 줄임
        int decrementAmount = 50; // 0.5초마다 줄어드는 양
        int steps = 4; // 2초 동안 0.5초 간격 == 속도가 감소하는 횟수
        for (int i = 0; i < steps; i++)
        {
            if (isPlayerA)
            {
                playerController.playerASpeed -= decrementAmount;
                Debug.Log("A의 속도 50감소");
            }

            else
            {
                playerController.playerBSpeed -= decrementAmount;
                Debug.Log("B의 속도 50감소");
            }

            yield return new WaitForSeconds(0.5f);
        }

        // 애니메이션 속도 복원
        playerAnimator.speed = 1f;

        Debug.Log("스피드업 버프 해제");
    }

    // 은신 버프 적용
    IEnumerator ApplyHideBuff(float duration, bool isPlayerA)
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        PlayerManager playerController = playerManager.GetComponent<PlayerManager>();

        // 은신 상태 활성화
        playerController.SetPlayerHide(true, isPlayerA, duration);


        float remainingTime = duration;

        //남은 은신시간 측정s
        while (remainingTime > 0)
        {
            if (isPlayerA)
            {
                Debug.Log($"A캐릭터의 은신 효과 남은 시간: {remainingTime:F2}초");
                yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 업데이트
                remainingTime -= 0.5f;
            }
            else if (!isPlayerA)
            {
                Debug.Log($"B캐릭터의 은신 효과 남은 시간: {remainingTime:F2}초");
                yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 업데이트
                remainingTime -= 0.5f;
            }
        }

        // 은신 상태 비활성화
        playerController.SetPlayerHide(false, isPlayerA, duration);
    }
}