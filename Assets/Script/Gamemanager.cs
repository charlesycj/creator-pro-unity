using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using JetBrains.Annotations;

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
    [SerializeField] private GameObject SpeedBuffPrefab;
    [SerializeField] private GameObject ShieldBuffPrefab;
    [SerializeField] private GameObject HideBuffPrefab;

    [SerializeField] public GameObject scorePrefab; // Score 프리팹 참조
    [SerializeField] private TextMeshProUGUI scoreTextMesh; //현재 점수 텍스트
    [SerializeField] private TextMeshProUGUI bestScoreTextMesh; // 최고 점수 텍스트

    [SerializeField] private Animator playerAnimatorA; // Animator 참조
    [SerializeField] private Animator playerAnimatorB; // Animator 참조

    public float objectSpawnInterval = 1f; // 오브젝트 생성 간격
    public float buffSpawnInterval = 15f;   // 버프 생성 간격 (변경되지 않음)
    private float timeElapsed = 0f;
    private bool gameStopped = false;
    private float gravityScale = 20f; //중력값
    public int currentStage = 1;
    private GameObject scoreObject;


    public int Score = 0; //현재 점수
    public int MaxScore; //최고점수 

    public GameObject gameOverCanvas; // Canvas-GameOver 오브젝트를 참조할 변수


    void Start()
    {
        // 최고 점수 불러오기
        MaxScore = PlayerPrefs.GetInt("MaxScore", 0); // 기본값은 0
        UpdateScoreText(); // 초기 점수 갱신
        UpdateBestScoreText(); // 최고 점수 텍스트 초기화

        //스코어 텍스트 설정
        if (scorePrefab != null)
        {
            // scorePrefab이 씬에 존재한다고 가정하고, 그 안에 있는 TextMeshProUGUI를 찾음
            scoreTextMesh = scorePrefab.GetComponentInChildren<TextMeshProUGUI>();

            if (scoreTextMesh != null)
            {
                UpdateScoreText(); // 초기 점수 표시
            }
        }

        // 오브젝트 생성 시작
        InvokeRepeating(nameof(SpawnUpwardObject), 0f, objectSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardObject), 0f, objectSpawnInterval);

        // 버프 생성 시작 (스테이지에 따라 변경 없음)
        InvokeRepeating(nameof(SpawnUpwardBuff), 0f, buffSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardBuff), buffSpawnInterval / 2, buffSpawnInterval);
    }

    public void IncreaseScore(int amount)
    {
        Score += amount;

        if (Score > MaxScore)
        {
            MaxScore = Score;
            Debug.Log($"최고 점수가 갱신되었습니다: {MaxScore}");

            // 최고 점수 저장
            PlayerPrefs.SetInt("MaxScore", MaxScore);
            PlayerPrefs.Save(); // 변경 사항을 저장
        }
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreTextMesh != null)
        {
            scoreTextMesh.text = $"Score:{Score:D5}"; // 5자리로 점수 표시
        }
    }
    private void UpdateBestScoreText()
    {
        if (bestScoreTextMesh != null)
        {
            bestScoreTextMesh.text = $"Best Score: {MaxScore:D5}"; // 최고 점수를 5자리로 표시
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

        // 3분마다 스테이지 전환
        if (timeElapsed >= 5f)
        {
            gravityScale += 5f;
            timeElapsed = 0f;
            currentStage = currentStage == 3 ? 1 : currentStage + 1;

            // 오브젝트 생성 간격 감소 (최소값 제한)
            if (objectSpawnInterval > 0.1f) objectSpawnInterval -= 0.1f;

            // 오브젝트 생성 호출 재설정
            ResetObjectSpawns();
        }
        if (Input.GetKeyDown(KeyCode.R)) //테스트용 최고점수 초기화 코드
        {
            MaxScore = 0; // R을 누르면 MAXScore를 초기화
            Debug.Log("최고점수가 초기화 되었습니다");
            bestScoreTextMesh.text = $"Best Score: {MaxScore:D5}";
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
    }

    public void StopGame() //게임정지시 오브젝트 생성 정지
    {
        
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
        if (Score > MaxScore)
        {
            MaxScore = Score;
            Debug.Log($"최고 점수가 갱신되었습니다: {MaxScore}");

            // 최고 점수 저장
            PlayerPrefs.SetInt("MaxScore", MaxScore);
            PlayerPrefs.Save(); // 변경 사항을 저장
        }

        // 화면에 있는 모든 장애물 및 버프 제거
        ClearAllObstaclesAndBuffs();
        // 최고 점수 텍스트 갱신
        UpdateBestScoreText();

        // Canvas-GameOver 활성화
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
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

    // 충돌 처리
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject playerManager = GameObject.Find("PlayerManager");
            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

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
                Debug.Log("스피드업 버프 획득");
            }
            else if (CompareTag("HideBuff"))
            {
                float hideBuffDuration = 5f; // 은신 버프 지속 시간 (초)
                // 은신 버프 코루틴 관리
                playerController.StartManagedCoroutine(
                    playerController.activeHideBuffCoroutines, isPlayerA,
                    ApplyHideBuff(hideBuffDuration, isPlayerA)
                );
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
            }
            Destroy(gameObject);
        }


        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("버프아이템 제거");
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
        // 버프 적용
        if (isPlayerA)
            playerController.playerASpeed += 200;
        else
            playerController.playerBSpeed += 200;

        yield return new WaitForSeconds(10f); // 10초 지속

        // 버프 해제
        if (isPlayerA)
            playerController.playerASpeed -= 200;
        else
            playerController.playerBSpeed -= 200;

        Debug.Log("스피드업 버프 해제");
    }

    // 은신 버프 적용
    IEnumerator ApplyHideBuff(float duration, bool isPlayerA)
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        PlayerManager playerController = playerManager.GetComponent<PlayerManager>();

        // 은신 상태 활성화
        playerController.SetPlayerHide(true, isPlayerA, duration);

        yield return new WaitForSeconds(duration);

        // 은신 상태 비활성화
        playerController.SetPlayerHide(false, isPlayerA, duration);
    }
}