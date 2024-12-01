using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] stage1UpwardPrefabs;
    [SerializeField] private GameObject[] stage1DownwardPrefabs;
    [SerializeField] private GameObject[] stage2UpwardPrefabs;
    [SerializeField] private GameObject[] stage2DownwardPrefabs;
    [SerializeField] private GameObject[] stage3UpwardPrefabs;
    [SerializeField] private GameObject[] stage3DownwardPrefabs;

    [SerializeField] private GameObject SpeedBuffPrefab;
    [SerializeField] private GameObject ShieldBuffPrefab;
    [SerializeField] private GameObject HideBuffPrefab;

    [SerializeField] public GameObject scorePrefab; // Score ������ ����
    [SerializeField] private TextMeshProUGUI scoreTextMesh;

    public float objectSpawnInterval = 1f; // ������Ʈ ���� ����
    public float buffSpawnInterval = 15f;   // ���� ���� ���� (������� ����)
    private float timeElapsed = 0f;
    private bool gameStopped = false;
    private float gravityScale = 20f;
    public int currentStage = 1;
    private GameObject scoreObject;


    public int Score = 0; //���� ����
    public int MaxScore; //�ְ����� 

    void Start()
    {
        // �ְ� ���� �ҷ�����
        MaxScore = PlayerPrefs.GetInt("MaxScore", 0); // �⺻���� 0
        UpdateScoreText(); // �ʱ� ���� ����

        //���ھ� �ؽ�Ʈ ����
        if (scorePrefab != null)
        {
            // scorePrefab�� ���� �����Ѵٰ� �����ϰ�, �� �ȿ� �ִ� TextMeshProUGUI�� ã��
            scoreTextMesh = scorePrefab.GetComponentInChildren<TextMeshProUGUI>();

            if (scoreTextMesh != null)
            {
                UpdateScoreText(); // �ʱ� ���� ǥ��
            }
        }

        // ������Ʈ ���� ����
        InvokeRepeating(nameof(SpawnUpwardObject), 0f, objectSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardObject), 0f, objectSpawnInterval);

        // ���� ���� ���� (���������� ���� ���� ����)
        InvokeRepeating(nameof(SpawnUpwardBuff), 0f, buffSpawnInterval);
        InvokeRepeating(nameof(SpawnDownwardBuff), buffSpawnInterval / 2, buffSpawnInterval);
    }

    public void IncreaseScore(int amount)
    {
        Score += amount;

        if (Score > MaxScore)
        {
            MaxScore = Score;
            Debug.Log($"�ְ� ������ ���ŵǾ����ϴ�: {MaxScore}");

            // �ְ� ���� ����
            PlayerPrefs.SetInt("MaxScore", MaxScore);
            PlayerPrefs.Save(); // ���� ������ ����
        }
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreTextMesh != null)
        {
            scoreTextMesh.text = $"Score:{Score:D5}"; // 5�ڸ��� ���� ǥ��
        }
    }
    void Update()
    {
        if (gameStopped) return;

        timeElapsed += Time.deltaTime;

        // 3�и��� �������� ��ȯ
        if (timeElapsed >= 180.0f)
        {
            gravityScale += 5f;
            timeElapsed = 0f;
            currentStage = currentStage == 3 ? 1 : currentStage + 1;

            // ������Ʈ ���� ���� ���� (�ּҰ� ����)
            if (objectSpawnInterval > 0.1f) objectSpawnInterval -= 0.1f;

            // ������Ʈ ���� ȣ�� �缳��
            ResetObjectSpawns();
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
    public void SpawnUpwardBuff() //�ö󰡴� ���� ��������ǥ�� ����
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
        gameStopped = true;
        CancelInvoke(nameof(SpawnUpwardObject));
        CancelInvoke(nameof(SpawnDownwardObject));
        CancelInvoke(nameof(SpawnUpwardBuff));
        CancelInvoke(nameof(SpawnDownwardBuff));
    }
}

public class ObjectMover : MonoBehaviour
{
    public GameManager spawnerScript;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
            spawnerScript.StopGame();
            Destroy(gameObject);
            Debug.Log("�浹�� ���� ����!");
            playerController.playerASpeed = 0;
            playerController.playerBSpeed = 0;
            Destroy(gameObject); // ��ֹ� ����
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            spawnerScript.IncreaseScore(1); // ���� 1 ����
            Destroy(gameObject); // �ٴڿ� �浹�ϸ� ����
        }
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
                Debug.Log("���ǵ�� ���� ȹ��");
            }
            else if (CompareTag("HideBuff"))
            {
                float hideBuffDuration = 5f; // ���� ���� ���� �ð� (��)
                // ���� ���� �ڷ�ƾ ����
                playerController.StartManagedCoroutine(
                    playerController.activeHideBuffCoroutines, isPlayerA,
                    ApplyHideBuff(hideBuffDuration, isPlayerA)
                );
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
        // ���� ����
        if (isPlayerA)
            playerController.playerASpeed += 200;
        else
            playerController.playerBSpeed += 200;

        yield return new WaitForSeconds(10f); // 10�� ����

        // ���� ����
        if (isPlayerA)
            playerController.playerASpeed -= 200;
        else
            playerController.playerBSpeed -= 200;

        Debug.Log("���ǵ�� ���� ����");
    }

    // ���� ���� ����
    IEnumerator ApplyHideBuff(float duration, bool isPlayerA)
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        PlayerManager playerController = playerManager.GetComponent<PlayerManager>();

        // ���� ���� Ȱ��ȭ
        playerController.SetPlayerHide(true, isPlayerA, duration);

        yield return new WaitForSeconds(duration);

        // ���� ���� ��Ȱ��ȭ
        playerController.SetPlayerHide(false, isPlayerA, duration);
    }
}