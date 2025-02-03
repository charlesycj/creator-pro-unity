using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using JetBrains.Annotations;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    //떨어지는 장애물 관리
    [SerializeField] private GameObject[] stage1Prefabs;
    [SerializeField] private GameObject[] stage2Prefabs;
    [SerializeField] private GameObject[] stage3Prefabs;

    //떨어지는 버프 관리
    [SerializeField] private GameObject SpeedBuffPrefab; //스피드
    [SerializeField] private GameObject ShieldBuffPrefab; //보호막
    [SerializeField] private GameObject HideBuffPrefab; // 3초무적

    [SerializeField] private GameObject SpeedDebuffPrefab; //스피드감소
    [SerializeField] private GameObject reverseDebuffPrefab; //스피드감소
    
    [SerializeField] public GameObject scorePrefab; // Score 프리팹 참조
    [SerializeField] private TextMeshProUGUI scoreTextMesh1; //현재 점수 텍스트
    [SerializeField] private TextMeshProUGUI scoreTextMesh2; //현재 점수 텍스트
    [SerializeField] private TextMeshProUGUI bestScoreTextMesh; // 최고 점수 텍스트

    [SerializeField] private Animator playerAnimatorA; // Animator 참조
    [SerializeField] private Animator playerAnimatorB; // Animator 참조

    [SerializeField] public float objectSpawnInterval = 4f; // 오브젝트 생성 간격
    [SerializeField] public float minobjectSpawnInterval = 0.4f; // 최소 오브젝트 생성 간격
    [SerializeField] public float spawnIntervalStep = 0.1f; // 장애물 스폰주기 감소값

    [SerializeField] public float buffSpawnInterval = 6f;   // 버프 생성 간격 
    [SerializeField] public float minbuffSpawnInterval = 3f; // 최소 버프 생성 간격
    [SerializeField] public float buffspawnIntervalStep = 0.05f; //버프 스폰주기 감소값

    [SerializeField] public float DebuffSpawnInterval = 6f;   // 버프 생성 간격 
    [SerializeField] public float minDebuffSpawnInterval = 3f; // 최소 버프 생성 간격
    [SerializeField] public float DebuffspawnIntervalStep = 0.05f; //버프 스폰주기 감소값

    [SerializeField] private float gravityScale = 20f; //중력값

    public SoundManager soundManager;
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
    public DeBuffMover debuffMover;


    void Start()
    {
        ResetObjectSpawns();
        ResetbuffSpawns();
        ResetDebuffSpawns();
        onStageEntered?.Invoke(currentStage);

        // 최고 점수 불러오기
        BestScore = PlayerPrefs.GetInt("BestScore", 0); // 기본값은 0

        // 누적 점수 불러오기
        AccumulatedScore = PlayerPrefs.GetInt("AccumulatedScore", 0);

        // 초기 점수 갱신
        UpdateScoreText();

        // 스코어 텍스트 설정
        if (scorePrefab != null)
        {
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
    }

    //점수 증가
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
            // 최고 점수 저장
            PlayerPrefs.SetInt("BestScore", BestScore);
        }
        PlayerPrefs.Save(); // 변경 사항을 저장
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        // 5자리로 점수 표시
        scoreTextMesh1.text = $"Score: {Score:D5}"; 
        scoreTextMesh2.text = $"Score: {Score:D5}";

        // 최고 점수를 5자리로 표시
        bestScoreTextMesh.text = $"Best Score: {BestScore:D5}";
    }

    void Update()
    {
        if (gameStopped) return;

        timeElapsed += Time.deltaTime;
        timeElapsedobject += Time.deltaTime;

        // 1분마다 스테이지 전환
        if (timeElapsed >= 60f)
        {
            gravityScale += 5f;
            timeElapsed = 0f; 
            currentStage = currentStage == 3 ? 1 : currentStage + 1;
            soundManager.playLevelTransition();
            soundManager.PlayStageBGM(currentStage);

            // 오브젝트 생성 재설정
            ResetObjectSpawns();
        }

        // 5초마다 오브젝트 생성 간격 변경
        if (timeElapsedobject >= 5f)
        {
            timeElapsedobject = 0f;

            // 오브젝트 생성 간격 감소
            if (objectSpawnInterval > minobjectSpawnInterval)
            {
                objectSpawnInterval -= spawnIntervalStep;
                objectSpawnInterval = Mathf.Round(objectSpawnInterval * 1000f) / 1000f; // 소수점 셋째 자리까지 반올림
                ResetObjectSpawns();
            }

            // 버프 생성 간격 감소
            if (buffSpawnInterval > minbuffSpawnInterval)
            {
                buffSpawnInterval -= buffspawnIntervalStep;
                buffSpawnInterval = Mathf.Round(buffSpawnInterval * 1000f) / 1000f; // 소수점 셋째 자리까지 반올림
                ResetbuffSpawns();
            }
            // 디버프 생성 간격 감소
            if (DebuffSpawnInterval > minDebuffSpawnInterval)
            {
                DebuffSpawnInterval -= DebuffspawnIntervalStep;
                DebuffSpawnInterval = Mathf.Round(DebuffSpawnInterval * 1000f) / 1000f; // 소수점 셋째 자리까지 반올림
                ResetDebuffSpawns();
            }
        }

        if (Input.GetKeyDown(KeyCode.F1)) // 테스트용 최고점수 초기화 코드
        {
            BestScore = 0; // F1을 누르면 BestScore를 초기화
            Debug.Log("최고점수가 초기화 되었습니다");
            bestScoreTextMesh.text = $"Best Score: {BestScore:D5}";
        }
    }


    private void ResetObjectSpawns()
    {
       
        CancelInvoke(nameof(RunSpawnsObject));
      
        // 최소값 이하로 떨어지더라도 최소값으로 설정하여 InvokeRepeating 실행
        if (objectSpawnInterval < minobjectSpawnInterval)
        {
            objectSpawnInterval = minobjectSpawnInterval;
        }

        InvokeRepeating(nameof(RunSpawnsObject), 0f, objectSpawnInterval);
        
    }

    private void ResetbuffSpawns()
    {
       
        CancelInvoke(nameof(RunSpawnsBuff));

        // 최소값 이하로 떨어지더라도 최소값으로 설정하여 InvokeRepeating 실행
        if (buffSpawnInterval < minbuffSpawnInterval)
        {
            buffSpawnInterval = minbuffSpawnInterval;
        }

        InvokeRepeating(nameof(RunSpawnsBuff), 0f, buffSpawnInterval);
       
    }
    private void ResetDebuffSpawns()
    {
       
        CancelInvoke(nameof(RunSpawnsDeBuff));

        // 최소값 이하로 떨어지더라도 최소값으로 설정하여 InvokeRepeating 실행
        if (DebuffSpawnInterval < minDebuffSpawnInterval)
        {
            DebuffSpawnInterval = minDebuffSpawnInterval;
        }

        InvokeRepeating(nameof(RunSpawnsDeBuff), 0f, DebuffSpawnInterval);
    }

    public void RunSpawnsObject()
    {
        if (gameStopped) return;

        // 위쪽과 아래쪽 위치에서 각각 프리팹 생성
        SpawnObject(GetStagePrefab(currentStage), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale); // 위쪽
        SpawnObject(GetStagePrefab(currentStage), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);   // 아래쪽
    }


    public void RunSpawnsBuff() //버프아이템 무작위좌표에 생성
    {
        if (gameStopped) return;
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale);
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);
    }

    public void RunSpawnsDeBuff() //너프아이템 무작위좌표에 생성
    {
        if (gameStopped) return;
        SpawnDeBuff(GetRandomDebuffPrefab(), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale);
        SpawnDeBuff(GetRandomDebuffPrefab(), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);
    }

    //스테이지 결정
    private GameObject GetStagePrefab(int stage)
    {
        // 스테이지 프리팹 배열 선택
        GameObject[] prefabArray = stage switch
        {
            1 => stage1Prefabs,
            2 => stage2Prefabs,
            3 => stage3Prefabs,
            _ => stage1Prefabs // 기본값: 스테이지 1 프리팹 배열
        };

        if (prefabArray == null || prefabArray.Length == 0)
            return null; // 프리팹 배열이 비어있으면 null 반환

        // 배열에서 무작위로 프리팹 선택
        return prefabArray[Random.Range(0, prefabArray.Length)];
    }

    private GameObject GetRandomBuffPrefab() //버프아이템 무작위결정
    {
        int buffType = Random.Range(1, 4);
        return buffType switch
        {
            1 => SpeedBuffPrefab, //1은 속도증가 버프
            2 => ShieldBuffPrefab,//2는 1회용보호막
            3 => HideBuffPrefab, //3은 3초무적
            _ => null
        };
    }
    private GameObject GetRandomDebuffPrefab() //버프아이템 무작위결정
    {
        int DebuffType = Random.Range(1, 3);
        return DebuffType switch
        {
            1 => SpeedDebuffPrefab, //1은 속도감소 디버프
            2 => reverseDebuffPrefab, //2는 키보드 반전 디버프
            _ => null
        };

    }

    //공통스폰 함수
    private T Spawn<T>(GameObject prefab, Vector3 position, float gravity) where T : MonoBehaviour
    {
        if (prefab == null) return null;

        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
        var rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (newObject.GetComponent<Collider2D>() == null)
            newObject.AddComponent<BoxCollider2D>();

        var moverScript = newObject.AddComponent<T>();
        if (moverScript is ObjectMover objectMover)
        {
            objectMover.spawnerScript = this;
        }

        return moverScript;
    }
        private void SpawnObject(GameObject prefab, Vector3 position, float gravity)
        {
                 Spawn<ObjectMover>(prefab, position, gravity);
        }

         private void SpawnBuff(GameObject prefab, Vector3 position, float gravity)
         {
              Spawn<BuffMover>(prefab, position, gravity);
         }
         private void SpawnDeBuff(GameObject prefab, Vector3 position, float gravity)
         {
               Spawn<DeBuffMover>(prefab, position, gravity);
         }

    public void StopGame() //게임정지시 오브젝트 생성 정지
    {
        soundManager.PlayGameOverSound(); // 게임오버 사운드 재생
        gameStopped = true; // 게임 정지 상태 플래그 설정
        CancelInvoke(nameof(RunSpawnsObject));
        CancelInvoke(nameof(RunSpawnsBuff));
        CancelInvoke(nameof(RunSpawnsDeBuff));

        // Player 사망시 애니메이션 실행
        if (playerAnimatorA != null || playerAnimatorB != null)
        {
            playerAnimatorA.SetTrigger("dead");
            playerAnimatorB.SetTrigger("dead");
        }

        // 화면에 있는 모든 장애물 및 버프 제거
        ClearAllObstaclesAndBuffs();


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

    // 장애물 및 버프 제거 함수
    private void ClearAllObstaclesAndBuffs()
    {
       
        // 장애물 제거
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Avoid");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        // 버프를 배열로 정의
        string[] buffTags = { "SpeedBuff", "ShieldBuff", "HideBuff","SpeedDeBuff","ReverseDeBuff" };
        //버프 제거
        foreach (string tag in buffTags)
        {
            GameObject[] buffs = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject buff in buffs)
            {
                Destroy(buff);
            }
        }
    }
}

public class ObjectMover : MonoBehaviour  //장애물 충돌관리
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

public class BuffMover : MonoBehaviour //버프 충돌 관리
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

            if (CompareTag("SpeedBuff"))
            {
                // 속도 버프 코루틴만 중지 후 시작
                playerController.StartSpeedCoroutine(
                    playerController.ApplySpeedBuff(playerController, isPlayerA),
                    isPlayerA
                );

                soundManager.PlaySpeedBuffSound(); // 속도 버프 사운드 재생
                Debug.Log("스피드업 버프 획득");
            }

            else if (CompareTag("HideBuff"))
            {
                float hideBuffDuration = 3f; // 은신 버프 지속 시간 (초)

                // 은신 버프는 속도 버프와 독립적으로 관리
                playerController.StartHideCoroutine(
                    playerController.ApplyHideBuff(hideBuffDuration, isPlayerA),
                    isPlayerA
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
}
public class DeBuffMover : MonoBehaviour //디버프 충돌 관리
{
    public GameManager spawnerScript;

    // 충돌 처리
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
       
            GameObject playerManager = GameObject.Find("PlayerManager");

            if (playerManager == null)
            {
                Debug.LogError("PlayerManager 오브젝트를 찾을 수 없습니다.");
                return;
            }

            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

            SoundManager soundManager = FindObjectOfType<SoundManager>(); // SoundManager 찾기
            if (soundManager == null)
            {
                Debug.LogError("SoundManager를 찾을 수 없습니다.");
            }

            if (CompareTag("SpeedDeBuff"))
            {


                // 속도 디버프 코루틴만 중지 후 시작
                playerController.StartSpeedCoroutine(
                    playerController.ApplySpeedDeBuff(playerController, isPlayerA),
                    isPlayerA
                );

                soundManager.PlaySpeedDeBuffSound(); // 속도 버프 사운드 재생
                Debug.Log("스피드다운 디버프 획득");
            }
            else if (CompareTag("ReverseDeBuff"))
            {

            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); //바닥에 충돌하면 제거
        }
    }
}

