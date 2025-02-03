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
    //�������� ��ֹ� ����
    [SerializeField] private GameObject[] stage1Prefabs;
    [SerializeField] private GameObject[] stage2Prefabs;
    [SerializeField] private GameObject[] stage3Prefabs;

    //�������� ���� ����
    [SerializeField] private GameObject SpeedBuffPrefab; //���ǵ�
    [SerializeField] private GameObject ShieldBuffPrefab; //��ȣ��
    [SerializeField] private GameObject HideBuffPrefab; // 3�ʹ���

    [SerializeField] private GameObject SpeedDebuffPrefab; //���ǵ尨��
    [SerializeField] private GameObject reverseDebuffPrefab; //���ǵ尨��
    
    [SerializeField] public GameObject scorePrefab; // Score ������ ����
    [SerializeField] private TextMeshProUGUI scoreTextMesh1; //���� ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI scoreTextMesh2; //���� ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI bestScoreTextMesh; // �ְ� ���� �ؽ�Ʈ

    [SerializeField] private Animator playerAnimatorA; // Animator ����
    [SerializeField] private Animator playerAnimatorB; // Animator ����

    [SerializeField] public float objectSpawnInterval = 4f; // ������Ʈ ���� ����
    [SerializeField] public float minobjectSpawnInterval = 0.4f; // �ּ� ������Ʈ ���� ����
    [SerializeField] public float spawnIntervalStep = 0.1f; // ��ֹ� �����ֱ� ���Ұ�

    [SerializeField] public float buffSpawnInterval = 6f;   // ���� ���� ���� 
    [SerializeField] public float minbuffSpawnInterval = 3f; // �ּ� ���� ���� ����
    [SerializeField] public float buffspawnIntervalStep = 0.05f; //���� �����ֱ� ���Ұ�

    [SerializeField] public float DebuffSpawnInterval = 6f;   // ���� ���� ���� 
    [SerializeField] public float minDebuffSpawnInterval = 3f; // �ּ� ���� ���� ����
    [SerializeField] public float DebuffspawnIntervalStep = 0.05f; //���� �����ֱ� ���Ұ�

    [SerializeField] private float gravityScale = 20f; //�߷°�

    public SoundManager soundManager;
    private float timeElapsed = 0f;
    private float timeElapsedobject = 0f;
    public bool gameStopped = false;

    public int currentStage = 1;
    private GameObject scoreObject;

    public int Score = 0; //���� ����
    public int BestScore; //�ְ����� 
    public int AccumulatedScore = 0; //��������

    public GameObject gameOverCanvas; // Canvas-GameOver ������Ʈ�� ������ ����
    public GameObject DeadPlayerA; // Canvas-GameOver ������Ʈ�� ������ ����
    public GameObject DeadPlayerB; // Canvas-GameOver ������Ʈ�� ������ ����

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

        // �ְ� ���� �ҷ�����
        BestScore = PlayerPrefs.GetInt("BestScore", 0); // �⺻���� 0

        // ���� ���� �ҷ�����
        AccumulatedScore = PlayerPrefs.GetInt("AccumulatedScore", 0);

        // �ʱ� ���� ����
        UpdateScoreText();

        // ���ھ� �ؽ�Ʈ ����
        if (scorePrefab != null)
        {
            scoreTextMesh1 = scorePrefab.GetComponentInChildren<TextMeshProUGUI>();

            if (scoreTextMesh1 != null)
            {
                UpdateScoreText(); // �ʱ� ���� ǥ��
            }
            if (scoreTextMesh2 != null)
            {
                scoreTextMesh2.text = $"Score: {Score:D5}"; // �� ��° ���� UI �ʱ�ȭ
            }
        }
        // SoundManager �ν��Ͻ� ��������
        SoundManager soundManager = FindObjectOfType<SoundManager>();
        if (soundManager != null)
        {
            soundManager.PlayStageBGM(1); // �ʱ� �������� 1 BGM ���
        }
    }

    //���� ����
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
            // �ְ� ���� ����
            PlayerPrefs.SetInt("BestScore", BestScore);
        }
        PlayerPrefs.Save(); // ���� ������ ����
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        // 5�ڸ��� ���� ǥ��
        scoreTextMesh1.text = $"Score: {Score:D5}"; 
        scoreTextMesh2.text = $"Score: {Score:D5}";

        // �ְ� ������ 5�ڸ��� ǥ��
        bestScoreTextMesh.text = $"Best Score: {BestScore:D5}";
    }

    void Update()
    {
        if (gameStopped) return;

        timeElapsed += Time.deltaTime;
        timeElapsedobject += Time.deltaTime;

        // 1�и��� �������� ��ȯ
        if (timeElapsed >= 60f)
        {
            gravityScale += 5f;
            timeElapsed = 0f; 
            currentStage = currentStage == 3 ? 1 : currentStage + 1;
            soundManager.playLevelTransition();
            soundManager.PlayStageBGM(currentStage);

            // ������Ʈ ���� �缳��
            ResetObjectSpawns();
        }

        // 5�ʸ��� ������Ʈ ���� ���� ����
        if (timeElapsedobject >= 5f)
        {
            timeElapsedobject = 0f;

            // ������Ʈ ���� ���� ����
            if (objectSpawnInterval > minobjectSpawnInterval)
            {
                objectSpawnInterval -= spawnIntervalStep;
                objectSpawnInterval = Mathf.Round(objectSpawnInterval * 1000f) / 1000f; // �Ҽ��� ��° �ڸ����� �ݿø�
                ResetObjectSpawns();
            }

            // ���� ���� ���� ����
            if (buffSpawnInterval > minbuffSpawnInterval)
            {
                buffSpawnInterval -= buffspawnIntervalStep;
                buffSpawnInterval = Mathf.Round(buffSpawnInterval * 1000f) / 1000f; // �Ҽ��� ��° �ڸ����� �ݿø�
                ResetbuffSpawns();
            }
            // ����� ���� ���� ����
            if (DebuffSpawnInterval > minDebuffSpawnInterval)
            {
                DebuffSpawnInterval -= DebuffspawnIntervalStep;
                DebuffSpawnInterval = Mathf.Round(DebuffSpawnInterval * 1000f) / 1000f; // �Ҽ��� ��° �ڸ����� �ݿø�
                ResetDebuffSpawns();
            }
        }

        if (Input.GetKeyDown(KeyCode.F1)) // �׽�Ʈ�� �ְ����� �ʱ�ȭ �ڵ�
        {
            BestScore = 0; // F1�� ������ BestScore�� �ʱ�ȭ
            Debug.Log("�ְ������� �ʱ�ȭ �Ǿ����ϴ�");
            bestScoreTextMesh.text = $"Best Score: {BestScore:D5}";
        }
    }


    private void ResetObjectSpawns()
    {
       
        CancelInvoke(nameof(RunSpawnsObject));
      
        // �ּҰ� ���Ϸ� ���������� �ּҰ����� �����Ͽ� InvokeRepeating ����
        if (objectSpawnInterval < minobjectSpawnInterval)
        {
            objectSpawnInterval = minobjectSpawnInterval;
        }

        InvokeRepeating(nameof(RunSpawnsObject), 0f, objectSpawnInterval);
        
    }

    private void ResetbuffSpawns()
    {
       
        CancelInvoke(nameof(RunSpawnsBuff));

        // �ּҰ� ���Ϸ� ���������� �ּҰ����� �����Ͽ� InvokeRepeating ����
        if (buffSpawnInterval < minbuffSpawnInterval)
        {
            buffSpawnInterval = minbuffSpawnInterval;
        }

        InvokeRepeating(nameof(RunSpawnsBuff), 0f, buffSpawnInterval);
       
    }
    private void ResetDebuffSpawns()
    {
       
        CancelInvoke(nameof(RunSpawnsDeBuff));

        // �ּҰ� ���Ϸ� ���������� �ּҰ����� �����Ͽ� InvokeRepeating ����
        if (DebuffSpawnInterval < minDebuffSpawnInterval)
        {
            DebuffSpawnInterval = minDebuffSpawnInterval;
        }

        InvokeRepeating(nameof(RunSpawnsDeBuff), 0f, DebuffSpawnInterval);
    }

    public void RunSpawnsObject()
    {
        if (gameStopped) return;

        // ���ʰ� �Ʒ��� ��ġ���� ���� ������ ����
        SpawnObject(GetStagePrefab(currentStage), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale); // ����
        SpawnObject(GetStagePrefab(currentStage), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);   // �Ʒ���
    }


    public void RunSpawnsBuff() //���������� ��������ǥ�� ����
    {
        if (gameStopped) return;
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale);
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);
    }

    public void RunSpawnsDeBuff() //���������� ��������ǥ�� ����
    {
        if (gameStopped) return;
        SpawnDeBuff(GetRandomDebuffPrefab(), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale);
        SpawnDeBuff(GetRandomDebuffPrefab(), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);
    }

    //�������� ����
    private GameObject GetStagePrefab(int stage)
    {
        // �������� ������ �迭 ����
        GameObject[] prefabArray = stage switch
        {
            1 => stage1Prefabs,
            2 => stage2Prefabs,
            3 => stage3Prefabs,
            _ => stage1Prefabs // �⺻��: �������� 1 ������ �迭
        };

        if (prefabArray == null || prefabArray.Length == 0)
            return null; // ������ �迭�� ��������� null ��ȯ

        // �迭���� �������� ������ ����
        return prefabArray[Random.Range(0, prefabArray.Length)];
    }

    private GameObject GetRandomBuffPrefab() //���������� ����������
    {
        int buffType = Random.Range(1, 4);
        return buffType switch
        {
            1 => SpeedBuffPrefab, //1�� �ӵ����� ����
            2 => ShieldBuffPrefab,//2�� 1ȸ�뺸ȣ��
            3 => HideBuffPrefab, //3�� 3�ʹ���
            _ => null
        };
    }
    private GameObject GetRandomDebuffPrefab() //���������� ����������
    {
        int DebuffType = Random.Range(1, 3);
        return DebuffType switch
        {
            1 => SpeedDebuffPrefab, //1�� �ӵ����� �����
            2 => reverseDebuffPrefab, //2�� Ű���� ���� �����
            _ => null
        };

    }

    //���뽺�� �Լ�
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

    public void StopGame() //���������� ������Ʈ ���� ����
    {
        soundManager.PlayGameOverSound(); // ���ӿ��� ���� ���
        gameStopped = true; // ���� ���� ���� �÷��� ����
        CancelInvoke(nameof(RunSpawnsObject));
        CancelInvoke(nameof(RunSpawnsBuff));
        CancelInvoke(nameof(RunSpawnsDeBuff));

        // Player ����� �ִϸ��̼� ����
        if (playerAnimatorA != null || playerAnimatorB != null)
        {
            playerAnimatorA.SetTrigger("dead");
            playerAnimatorB.SetTrigger("dead");
        }

        // ȭ�鿡 �ִ� ��� ��ֹ� �� ���� ����
        ClearAllObstaclesAndBuffs();


        // Canvas-GameOver Ȱ��ȭ
        if (gameOverCanvas != null)
        {
            GameObject playerManagerObject = GameObject.Find("PlayerManager");
            if (playerManagerObject != null)
            {
                PlayerManager playerManager = playerManagerObject.GetComponent<PlayerManager>();

                gameOverCanvas.SetActive(true);
                if (playerManager != null)
                    //DeadPlayer �� Ȯ��
                    if (playerManager.DeadPlayer == "Player A")
                    {
                        // Player A ���� Canvas Ȱ��ȭ
                        DeadPlayerA.SetActive(true);

                    }
                    else if (playerManager.DeadPlayer == "Player B")
                    {
                        // Player B ���� Canvas Ȱ��ȭ                                       
                        DeadPlayerB.SetActive(true);
                    }
            }
        }
    }

    // ��ֹ� �� ���� ���� �Լ�
    private void ClearAllObstaclesAndBuffs()
    {
       
        // ��ֹ� ����
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Avoid");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        // ������ �迭�� ����
        string[] buffTags = { "SpeedBuff", "ShieldBuff", "HideBuff","SpeedDeBuff","ReverseDeBuff" };
        //���� ����
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

public class ObjectMover : MonoBehaviour  //��ֹ� �浹����
{
    public GameManager spawnerScript;
    private bool isExploding = false; // explosion ���� �÷���

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Player�� �浹 ó��
        if (collision.gameObject.CompareTag("Player"))
        {
            // explosion ���¶�� �浹 ����
            if (isExploding)
            {
                return;
            }

            GameObject playerManager = GameObject.Find("PlayerManager");
            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

            // ���� ������ �� �浹 ����
            if (playerController.IsPlayerHide(isPlayerA))
            {
                Debug.Log("���� ���·� ��ֹ� ����");
                Destroy(gameObject); // ��ֹ� ����
                return;
            }

            // �ǵ尡 Ȱ��ȭ�� ��� ��ֹ��� ���
            if (playerController.IsShieldActive(isPlayerA))
            {
                Debug.Log("�ǵ尡 Ȱ��ȭ�Ǿ� ��ֹ� ���");
                playerController.RemoveShieldBuff(isPlayerA); // �ǵ� ���� ����
                Destroy(gameObject); // ��ֹ� ����
                return;
            }

            // ���� ���� ó��
            if (collision.gameObject == playerController.playerA)
            {
                Debug.Log("Player A�� �׾����ϴ�. ���� ����!");
                playerController.DeadPlayer = "Player A";
            }
            else if (collision.gameObject == playerController.playerB)
            {
                Debug.Log("Player B�� �׾����ϴ�. ���� ����!");
                playerController.DeadPlayer = "Player B";
            }
            spawnerScript.StopGame();
            Debug.Log("�浹�� ���� ����!");
            playerController.playerASpeed = 0;
            playerController.playerBSpeed = 0;
        }

        // Ground�� �浹 ó��
        if (collision.gameObject.CompareTag("Ground"))
        {
            spawnerScript.IncreaseScore(1); // ���� 1 ����

            Animator enemyAnimator = gameObject.GetComponent<Animator>();
            if (enemyAnimator != null)

            {
                enemyAnimator.SetTrigger("explosion"); // �ִϸ��̼� Ʈ���� ȣ��
                isExploding = true; // explosion ���·� ����
            }

            // explosion ���¿��� �浹�� Ʈ���ŷ� ��ȯ : explosion���¿��� enemy�� �浹 ����
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            // Rigidbody2D �߷� ��Ȱ��ȭ �� �ӵ� �ʱ�ȭ : enemy explosion���¿��� collider�� ��� �������� ���� ����
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;       // �ӵ� �ʱ�ȭ
                rb.gravityScale = 0;             // �߷� ��Ȱ��ȭ
            }

            // �ִϸ��̼��� ���� �Ŀ� ���� ������Ʈ�� �����ϵ��� Coroutine ȣ��
            StartCoroutine(DestroyAfterAnimation(enemyAnimator));
        }
    }

    IEnumerator DestroyAfterAnimation(Animator animator)
    {
        // �ִϸ��̼� ���̸�ŭ ���
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // ��� �� ���� ������Ʈ ����
        Destroy(gameObject);
    }
}

public class BuffMover : MonoBehaviour //���� �浹 ����
{
    public GameManager spawnerScript;

    public static Action onItemAcquired;

    // �浹 ó��
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject playerManager = GameObject.Find("PlayerManager");
            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

            SoundManager soundManager = FindObjectOfType<SoundManager>(); // SoundManager ã��

            if (CompareTag("SpeedBuff"))
            {
                // �ӵ� ���� �ڷ�ƾ�� ���� �� ����
                playerController.StartSpeedCoroutine(
                    playerController.ApplySpeedBuff(playerController, isPlayerA),
                    isPlayerA
                );

                soundManager.PlaySpeedBuffSound(); // �ӵ� ���� ���� ���
                Debug.Log("���ǵ�� ���� ȹ��");
            }

            else if (CompareTag("HideBuff"))
            {
                float hideBuffDuration = 3f; // ���� ���� ���� �ð� (��)

                // ���� ������ �ӵ� ������ ���������� ����
                playerController.StartHideCoroutine(
                    playerController.ApplyHideBuff(hideBuffDuration, isPlayerA),
                    isPlayerA
                );

                soundManager.PlayHideBuffSound(); // ���� ���� ���� ���
                Debug.Log("���� ���� ȹ��");
            }

            else if (CompareTag("ShieldBuff"))
            {
                if (!playerController.HasShield(isPlayerA)) // �̹� �ǵ尡 ���� ��쿡�� ����
                {
                    playerController.ApplyShieldBuff(isPlayerA);
                    Debug.Log("�ǵ� ���� ȹ��");
                }
                else
                {
                    Debug.Log("�ǵ尡 �̹� Ȱ��ȭ�Ǿ� �ֽ��ϴ�.");
                }
                soundManager.PlayShieldBuffSound(); // �ǵ� ���� ���� ���
            }
            onItemAcquired?.Invoke();

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); //�ٴڿ� �浹�ϸ� ����
        }
    }
}
public class DeBuffMover : MonoBehaviour //����� �浹 ����
{
    public GameManager spawnerScript;

    // �浹 ó��
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
       
            GameObject playerManager = GameObject.Find("PlayerManager");

            if (playerManager == null)
            {
                Debug.LogError("PlayerManager ������Ʈ�� ã�� �� �����ϴ�.");
                return;
            }

            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

            SoundManager soundManager = FindObjectOfType<SoundManager>(); // SoundManager ã��
            if (soundManager == null)
            {
                Debug.LogError("SoundManager�� ã�� �� �����ϴ�.");
            }

            if (CompareTag("SpeedDeBuff"))
            {


                // �ӵ� ����� �ڷ�ƾ�� ���� �� ����
                playerController.StartSpeedCoroutine(
                    playerController.ApplySpeedDeBuff(playerController, isPlayerA),
                    isPlayerA
                );

                soundManager.PlaySpeedDeBuffSound(); // �ӵ� ���� ���� ���
                Debug.Log("���ǵ�ٿ� ����� ȹ��");
            }
            else if (CompareTag("ReverseDeBuff"))
            {

            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); //�ٴڿ� �浹�ϸ� ����
        }
    }
}

