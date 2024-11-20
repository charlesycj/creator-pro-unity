using UnityEngine;

public class BuffSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject SpeedBuffPrefab;    // ���ǵ� ���� ������
    [SerializeField]
    public GameObject ShieldBuffPrefab;   // 1ȸ�� ��ȣ�� ������
    [SerializeField]
    public GameObject HideBuffPrefab;  // ���� ������

    public float spawnInterval = 1.0f;    // ������Ʈ ���� ���� (��)
    private bool gameStopped = false;      // ���� ���� Ȯ��
    private float gravityScale = 10f;      // �߷� ������ �ʱⰪ

    void Start()
    {
        // �� �Ʒ��� ���� ����
        InvokeRepeating("SpawnUpwardBuff", 0f, spawnInterval);  // ������ �������� ����
        InvokeRepeating("SpawnDownwardBuff", spawnInterval / 2, spawnInterval);  // �Ʒ����� �ö���� ����
    }

    void Update()
    {
        if (gameStopped) return;
    }

    public void SpawnUpwardBuff()
{
    if (gameStopped) return;

    // �������� ���� ���� (3���� �߿��� ����)
    int buffType = Random.Range(1, 4);  // 1, 2, 3 �� �ϳ� ����
    float randomX = Random.Range(-850f, 850f);
    Vector3 spawnPosition = new Vector3(randomX, -425f, 0f);  // �Ʒ����� ���� �ö���� ��ġ

    GameObject selectedPrefab = null;

    // ���� ������ ���� ����
    switch (buffType)
    {
        case 1:
            selectedPrefab = SpeedBuffPrefab;  // ���ǵ� ���� ����
            Debug.Log("�ö󰡴� ���ǵ� ����");
            break;
        case 2:
            selectedPrefab = ShieldBuffPrefab; // 1ȸ�� ��ȣ�� ����
            Debug.Log("�ö󰡴� ��ȣ�� ����");
            break;
        case 3:
            selectedPrefab = HideBuffPrefab;  // ���� ����
            Debug.Log("�ö󰡴� ���� ����");
            break;
    }

    // ���õ� ���� ����
    if (selectedPrefab != null)
    {
        Debug.Log("selectedPrefab name: " + selectedPrefab.name); // �����Ǵ� ������ �̸� Ȯ��
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

    // �������� ���� ���� (3���� �߿��� ����)
    int buffType = Random.Range(1, 4);  // 1, 2, 3 �� �ϳ� ����
    float randomX = Random.Range(-850f, 850f);
    Vector3 spawnPosition = new Vector3(randomX, 425f, 0f);  // ������ �Ʒ��� �������� ��ġ

    GameObject selectedPrefab = null;

    // ���� ������ ���� ����
    switch (buffType)
    {
        case 1:
            selectedPrefab = SpeedBuffPrefab;  // ���ǵ� ���� ����
            Debug.Log("�������� ���ǵ� ����");
            break;
        case 2:
            selectedPrefab = ShieldBuffPrefab; // 1ȸ�� ��ȣ�� ����
            Debug.Log("�������� ��ȣ�� ����");
            break;
        case 3:
            selectedPrefab = HideBuffPrefab;  // ���� ����
            Debug.Log("�������� ���� ����");
            break;
    }

    // ���õ� ���� ����
    if (selectedPrefab != null)
    {
        Debug.Log("selectedPrefab name: " + selectedPrefab.name); // �����Ǵ� ������ �̸� Ȯ��
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

        // Rigidbody2D �߰� �� �߷� ����
        Rigidbody2D rb = newObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = gravity;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // ���� ��ũ��Ʈ �߰� �� ������ ���� ����
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
    public BuffSpawner spawnerScript;  // BuffSpawner ����

    void OnCollisionEnter2D(Collision2D collision)
    {
        // �ٴ� �浹 �� ���� �ı�
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        // �÷��̾� �浹 �� ���� ����
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
