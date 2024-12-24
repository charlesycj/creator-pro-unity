using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
   //�������� ��ֹ� ����
    [SerializeField] private GameObject[] stage1UpwardPrefabs;
    [SerializeField] private GameObject[] stage1DownwardPrefabs;
    [SerializeField] private GameObject[] stage2UpwardPrefabs;
    [SerializeField] private GameObject[] stage2DownwardPrefabs;
    [SerializeField] private GameObject[] stage3UpwardPrefabs;
    [SerializeField] private GameObject[] stage3DownwardPrefabs;

    //�������� ���� ����
    [SerializeField] private GameObject SpeedBuffPrefab;
    [SerializeField] private GameObject ShieldBuffPrefab;
    [SerializeField] private GameObject HideBuffPrefab;


    [SerializeField] public GameObject scorePrefab; // Score ������ ����
    [SerializeField] private TextMeshProUGUI scoreTextMesh1; //���� ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI scoreTextMesh2; //���� ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI bestScoreTextMesh; // �ְ� ���� �ؽ�Ʈ

    [SerializeField] private Animator playerAnimatorA; // Animator ����
    [SerializeField] private Animator playerAnimatorB; // Animator ����

    public float objectSpawnInterval = 1f; // ������Ʈ ���� ����
    public float buffSpawnInterval = 5f;   // ���� ���� ���� (������� ����)
    private float timeElapsed = 0f;
    public bool gameStopped = false;
    private float gravityScale = 20f; //�߷°�
    public int currentStage = 1;
    private GameObject scoreObject;


    public int Score = 0; //���� ����
    public int BestScore; //�ְ����� 

    public GameObject gameOverCanvas; // Canvas-GameOver ������Ʈ�� ������ ����
    public GameObject DeadPlayerA; // Canvas-GameOver ������Ʈ�� ������ ����
    public GameObject DeadPlayerB; // Canvas-GameOver ������Ʈ�� ������ ����

    void Start()
    {

        // �ְ� ���� �ҷ�����
        BestScore = PlayerPrefs.GetInt("BestScore", 0); // �⺻���� 0
        UpdateScoreText(); // �ʱ� ���� ����
        UpdateBestScoreText(); // �ְ� ���� �ؽ�Ʈ �ʱ�ȭ

        //���ھ� �ؽ�Ʈ ����
        if (scorePrefab != null)
        {
            // scorePrefab�� ���� �����Ѵٰ� �����ϰ�, �� �ȿ� �ִ� TextMeshProUGUI�� ã��
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

        // ������Ʈ ���� ����
        InvokeRepeating(nameof(SpawnUpwardObject), 0f, objectSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardObject), 0f, objectSpawnInterval);

        // ���� ���� ���� (���������� ���� ���� ����)
        InvokeRepeating(nameof(SpawnUpwardBuff), 0f, buffSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardBuff), buffSpawnInterval / 2, buffSpawnInterval);


        // SoundManager �ν��Ͻ� ��������
        SoundManager soundManager = FindObjectOfType<SoundManager>();
        if (soundManager != null)
        {
            soundManager.PlayStageBGM(1); // �ʱ� �������� 1 BGM ���
        }
        else
        {
            Debug.LogError("SoundManager�� ã�� �� �����ϴ�!");
        }
    }


    public void IncreaseScore(int amount)
    {
        Score += amount;

        if (Score > BestScore)
        {
            BestScore = Score;
            Debug.Log($"�ְ� ������ ���ŵǾ����ϴ�: {BestScore}");

            // �ְ� ���� ����
            PlayerPrefs.SetInt("BestScore", BestScore);
            PlayerPrefs.Save(); // ���� ������ ����
        }
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        // ù ��° �ؽ�Ʈ UI�� ���� ����
        if (scoreTextMesh1 != null)
        {
            scoreTextMesh1.text = $"Score: {Score:D5}"; // 5�ڸ��� ���� ǥ��
        }

        // �� ��° �ؽ�Ʈ UI���� ���� ����
        if (scoreTextMesh2 != null)
        {
            scoreTextMesh2.text = $"Score: {Score:D5}"; // 5�ڸ��� ���� ǥ��
        }
    }
    private void UpdateBestScoreText()
    {
        if (bestScoreTextMesh != null)
        {
            bestScoreTextMesh.text = $"Best Score: {BestScore:D5}"; // �ְ� ������ 5�ڸ��� ǥ��
        }
    }
    void Update()
    {
        // ���� ���� �� �÷��̾� �¿���� ����
        if (gameStopped)
        {
            if (playerAnimatorA != null)
                FixPlayerFlip(playerAnimatorA.gameObject);

            if (playerAnimatorB != null)
                FixPlayerFlip(playerAnimatorB.gameObject);
        }


        if (gameStopped) return;

        timeElapsed += Time.deltaTime;

        // 3�и��� �������� ��ȯ
        if (timeElapsed >= 90f) 
        {
            SoundManager soundManager = FindObjectOfType<SoundManager>(); // SoundManager ã��
            soundManager.playLevelTransition(); // �ӵ� ���� ���� ���
            gravityScale += 5f;
            timeElapsed = 0f;
            currentStage = currentStage == 3 ? 1 : currentStage + 1;

            // ���������� �´� BGM ���
            soundManager.PlayStageBGM(currentStage);

            // ������Ʈ ���� ���� ���� (�ּҰ� ����)
            if (objectSpawnInterval > 0.1f) objectSpawnInterval -= 0.01f;

            // ������Ʈ ���� ȣ�� �缳��
            ResetObjectSpawns();
        }
        if (Input.GetKeyDown(KeyCode.F1)) //�׽�Ʈ�� �ְ����� �ʱ�ȭ �ڵ�
        {
            BestScore = 0; // F1�� ������ BestScore�� �ʱ�ȭ
            Debug.Log("�ְ������� �ʱ�ȭ �Ǿ����ϴ�");
            bestScoreTextMesh.text = $"Best Score: {BestScore:D5}";
        }
    }

    private void ResetObjectSpawns()
    {
        CancelInvoke(nameof(SpawnUpwardObject));
        CancelInvoke(nameof(SpawnDownwardObject));
        InvokeRepeating(nameof(SpawnUpwardObject), 0f, objectSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardObject), 0f, objectSpawnInterval);
    }

    // ������Ʈ ���� ���� �Լ�
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

    // ���� ���� ���� �Լ�
    public void SpawnUpwardBuff() //�ö� �� ���� ��������ǥ�� ����
    {
        if (gameStopped) return;
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), -425f, 0f), -gravityScale);
    }

    public void SpawnDownwardBuff() //�������� ���� ��������ǥ�� ����
    {
        if (gameStopped) return;
        SpawnBuff(GetRandomBuffPrefab(), new Vector3(Random.Range(-850f, 850f), 425f, 0f), gravityScale);
    }

    private GameObject GetRandomBuffPrefab() //���������� ����������
    {
        int buffType = Random.Range(1, 4);
        return buffType switch
        {
            1 => SpeedBuffPrefab, //1�� �ӵ����� ����
            2 => ShieldBuffPrefab,//2�� 1ȸ�뺸ȣ��
            3 => HideBuffPrefab, //3�� 5�ʹ���
            _ => null
        };
    }

    // ���� ���� �Լ�
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

    public void StopGame() //���������� ������Ʈ ���� ����
    {

        SoundManager soundManager = FindObjectOfType<SoundManager>(); // SoundManager ã��
        soundManager.PlayGameOverSound(); // �ӵ� ���� ���� ���
        gameStopped = true; // ���� ���� ���� �÷��� ����
        CancelInvoke(nameof(SpawnUpwardObject));
        CancelInvoke(nameof(SpawnDownwardObject));
        CancelInvoke(nameof(SpawnUpwardBuff));
        CancelInvoke(nameof(SpawnDownwardBuff));

        // Player ����� �ִϸ��̼� ����
        if (playerAnimatorA != null || playerAnimatorB != null)
        {
            playerAnimatorA.SetTrigger("dead");
            playerAnimatorB.SetTrigger("dead");
        }

        // ���� ������ �ְ� �������� ������ ����
        if (Score > BestScore)
        {
            BestScore = Score;
            Debug.Log($"�ְ� ������ ���ŵǾ����ϴ�: {BestScore}");

            // �ְ� ���� ����
            PlayerPrefs.SetInt("Score", BestScore);
            PlayerPrefs.Save(); // ���� ������ ����
        }

        // ȭ�鿡 �ִ� ��� ��ֹ� �� ���� ����
        ClearAllObstaclesAndBuffs();
        // �ְ� ���� �ؽ�Ʈ ����
        UpdateBestScoreText();

        
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

    // �÷��̾� ����� �¿���� ���� �Լ�
    private void FixPlayerFlip(GameObject player)
    {
        if (player != null)
        {
            var spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.flipX = false;

            // localScale.x�� ����� ����
            Vector3 fixedScale = player.transform.localScale;
            fixedScale.x = Mathf.Abs(fixedScale.x);
            player.transform.localScale = fixedScale;
        }
    }

    // ��ֹ� �� ���� ���� �Լ�
    private void ClearAllObstaclesAndBuffs()
    {

        // ��� �±׸� �迭�� ����
        string[] buffTags = { "SpeedBuff", "ShieldBuff", "HideBuff" };
        // ��ֹ� ����
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
        Debug.Log("ȭ���� ��� ��ֹ��� ������ ���ŵǾ����ϴ�.");

    }
}

public class ObjectMover : MonoBehaviour
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
        /*    if (collision.gameObject == playerController.playerA)
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
            Destroy(gameObject);
            Debug.Log("�浹�� ���� ����!");
            playerController.playerASpeed = 0;
            playerController.playerBSpeed = 0;*/

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




public class BuffMover : MonoBehaviour
{
    public GameManager spawnerScript;

    // �浹 ó��
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject playerManager = GameObject.Find("PlayerManager");
            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

            SoundManager soundManager = FindObjectOfType<SoundManager>(); // SoundManager ã��

            // ���� ���¿� �浹 ó��
            if (playerController.IsPlayerHide(isPlayerA) && !CompareTag("SpeedBuff") && !CompareTag("ShieldBuff") && !CompareTag("HideBuff"))
            {
                Debug.Log("���� ���·� ��ֹ� ����");
                Destroy(gameObject); // ��ֹ� ����
                return;
            }

            if (CompareTag("SpeedBuff"))
            {
                // ���ǵ� ���� �ڷ�ƾ ����
                playerController.StartManagedCoroutine(
                    playerController.activeSpeedBuffCoroutines, isPlayerA,
                    ApplySpeedBuff(playerController, isPlayerA)
                );

                soundManager.PlaySpeedBuffSound(); // �ӵ� ���� ���� ���
                Debug.Log("���ǵ�� ���� ȹ��");
            }

            else if (CompareTag("HideBuff"))
            {
                float hideBuffDuration = 3f; // ���� ���� ���� �ð� (��)
                // ���� ���� �ڷ�ƾ ����
                playerController.StartManagedCoroutine(
                    playerController.activeHideBuffCoroutines, isPlayerA,
                    ApplyHideBuff(hideBuffDuration, isPlayerA)
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
            Destroy(gameObject);
        }


        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("���������� ����");
            Destroy(gameObject); //�ٴڿ� �浹�ϸ� ����
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject playerManager = GameObject.Find("PlayerManager");
            PlayerManager playerController = playerManager.GetComponent<PlayerManager>();
            bool isPlayerA = collision.gameObject == playerController.playerA;

            // ���� ������ �� �浹�� ��� ����
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

    // ���ǵ�� ���� ����
    private IEnumerator ApplySpeedBuff(PlayerManager playerController, bool isPlayerA)
    {
        // Animator ��������
        GameObject player = isPlayerA ? playerController.playerA : playerController.playerB;
        Animator playerAnimator = player.GetComponent<Animator>();

        if (playerAnimator == null)
        {
            Debug.LogError("Animator�� ã�� �� �����ϴ�. �÷��̾ Animator�� �߰��Ǿ� �ִ��� Ȯ���ϼ���.");
            yield break;
        }
        // �ִϸ��̼� �ӵ� 2��� ����
        playerAnimator.speed = 2.5f;


        // ���� ����
        if (isPlayerA)
            playerController.playerASpeed += 200;
        else
            playerController.playerBSpeed += 200;

        yield return new WaitForSeconds(2f); // ù 2�� ���� ������ ���� ����

        // ���� 2�� ���� 0.5�� �������� �ӵ��� ����
        int decrementAmount = 50; // 0.5�ʸ��� �پ��� ��
        int steps = 4; // 2�� ���� 0.5�� ���� == �ӵ��� �����ϴ� Ƚ��
        for (int i = 0; i < steps; i++)
        {
            if (isPlayerA)
            {
                playerController.playerASpeed -= decrementAmount;
                Debug.Log("A�� �ӵ� 50����");
            }

            else
            {
                playerController.playerBSpeed -= decrementAmount;
                Debug.Log("B�� �ӵ� 50����");
            }
    
            yield return new WaitForSeconds(0.5f);
        }

        // �ִϸ��̼� �ӵ� ����
        playerAnimator.speed = 1f;

        Debug.Log("���ǵ�� ���� ����");
    }

    // ���� ���� ����
    IEnumerator ApplyHideBuff(float duration, bool isPlayerA)
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        PlayerManager playerController = playerManager.GetComponent<PlayerManager>();

        // ���� ���� Ȱ��ȭ
        playerController.SetPlayerHide(true, isPlayerA, duration);


        float remainingTime = duration;

        //���� ���Žð� ����s
        while (remainingTime > 0) 
        {
            if (isPlayerA)
            {
                Debug.Log($"Aĳ������ ���� ȿ�� ���� �ð�: {remainingTime:F2}��");
                yield return new WaitForSeconds(0.5f); // 0.5�� �������� ������Ʈ
                remainingTime -= 0.5f;
            }
            else if(!isPlayerA)
            {
                Debug.Log($"Bĳ������ ���� ȿ�� ���� �ð�: {remainingTime:F2}��");
                yield return new WaitForSeconds(0.5f); // 0.5�� �������� ������Ʈ
                remainingTime -= 0.5f;
            }
         }

            // ���� ���� ��Ȱ��ȭ
            playerController.SetPlayerHide(false, isPlayerA, duration);
    }
}