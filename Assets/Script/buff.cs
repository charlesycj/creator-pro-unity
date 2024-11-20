using UnityEngine;

public class BuffSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject SpeedBuffPrefab;    // 스피드 증가 프리팹
    [SerializeField]
    public GameObject ShieldBuffPrefab;   // 1회용 보호막 프리팹
    [SerializeField]
    public GameObject HideBuffPrefab;  // 무적 프리팹

    public float spawnInterval = 1.0f;    // 오브젝트 생성 간격 (초)
    private bool gameStopped = false;      // 게임 상태 확인
    private float gravityScale = 10f;      // 중력 스케일 초기값

    void Start()
    {
        // 위 아래로 버프 생성
        InvokeRepeating("SpawnUpwardBuff", 0f, spawnInterval);  // 위에서 내려오는 버프
        InvokeRepeating("SpawnDownwardBuff", spawnInterval / 2, spawnInterval);  // 아래에서 올라오는 버프
    }

    void Update()
    {
        if (gameStopped) return;
    }

    public void SpawnUpwardBuff()
{
    if (gameStopped) return;

    // 무작위로 버프 선택 (3가지 중에서 랜덤)
    int buffType = Random.Range(1, 4);  // 1, 2, 3 중 하나 선택
    float randomX = Random.Range(-850f, 850f);
    Vector3 spawnPosition = new Vector3(randomX, -425f, 0f);  // 아래에서 위로 올라오는 위치

    GameObject selectedPrefab = null;

    // 버프 종류에 따라 선택
    switch (buffType)
    {
        case 1:
            selectedPrefab = SpeedBuffPrefab;  // 스피드 증가 버프
            Debug.Log("올라가는 스피드 선택");
            break;
        case 2:
            selectedPrefab = ShieldBuffPrefab; // 1회용 보호막 버프
            Debug.Log("올라가는 보호막 선택");
            break;
        case 3:
            selectedPrefab = HideBuffPrefab;  // 무적 버프
            Debug.Log("올라가는 무적 선택");
            break;
    }

    // 선택된 버프 생성
    if (selectedPrefab != null)
    {
        Debug.Log("selectedPrefab name: " + selectedPrefab.name); // 생성되는 프리팹 이름 확인
        Spawnbuff(selectedPrefab, spawnPosition, -gravityScale);
    }
    else
    {
        Debug.LogError("selectedPrefab is null!");
    }
}

public void SpawnDownwardBuff()
{
    if (gameStopped) return;

    // 무작위로 버프 선택 (3가지 중에서 랜덤)
    int buffType = Random.Range(1, 4);  // 1, 2, 3 중 하나 선택
    float randomX = Random.Range(-850f, 850f);
    Vector3 spawnPosition = new Vector3(randomX, 425f, 0f);  // 위에서 아래로 내려오는 위치

    GameObject selectedPrefab = null;

    // 버프 종류에 따라 선택
    switch (buffType)
    {
        case 1:
            selectedPrefab = SpeedBuffPrefab;  // 스피드 증가 버프
            Debug.Log("내려가는 스피드 선택");
            break;
        case 2:
            selectedPrefab = ShieldBuffPrefab; // 1회용 보호막 버프
            Debug.Log("내려가는 보호막 선택");
            break;
        case 3:
            selectedPrefab = HideBuffPrefab;  // 무적 버프
            Debug.Log("내려가는 무적 선택");
            break;
    }

    // 선택된 버프 생성
    if (selectedPrefab != null)
    {
        Debug.Log("selectedPrefab name: " + selectedPrefab.name); // 생성되는 프리팹 이름 확인
        Spawnbuff(selectedPrefab, spawnPosition, gravityScale);
    }
    else
    {
        Debug.LogError("selectedPrefab is null!");
    }
}


    private void Spawnbuff(GameObject prefab, Vector3 position, float gravity)
    {
        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);

        // Rigidbody2D 추가 및 중력 설정
        Rigidbody2D rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // 공통 스크립트 추가 및 스포너 참조 전달
        BuffMover moverScript = newObject.AddComponent<BuffMover>();
        moverScript.spawnerScript = this;
    }

    public void StopGame()
    {
        gameStopped = true;
        CancelInvoke("SpawnUpwardBuff");
        CancelInvoke("SpawnDownwardBuff");
    }
}

public class BuffMover : MonoBehaviour
{
    public BuffSpawner spawnerScript;  // BuffSpawner 참조

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥 충돌 시 버프 파괴
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        // 플레이어 충돌 시 버프 습득
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
