using UnityEngine;

public class ObjectUp : MonoBehaviour
{
    public GameObject objectPrefab;      // �ö� ������Ʈ ������
    public float spawnInterval = 1f;     // ������Ʈ �������� (��)
    public float destroyYPosition = -0.25f; // �ı��� Y ��ġ (���鿡 ���޽� �ı���)
    private float timeElapsed = 0f;      // ��� �ð�
    private float initialSpeed = 5f;     // ������Ʈ�� �ʱ� �ӵ�

    void Start()
    {
        // 1�ʿ� �� ���� ������Ʈ�� �����ϴ� �޼��� ȣ��
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    void Update()
    {
        // �ð� 3�ʰ� ����� ������ spawnInterval�� 0.1�� ����
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 3f) //3�ʰ� ����������
        {
            spawnInterval -= 0.01f; // spawnInterval�� 0.01�� ���� (������Ʈ �����ӵ�)
            initialSpeed += 0.3f;   // ������Ʈ�� �ӵ��� 0.3�� ������Ŵ (������Ʈ�� �̵��ӵ�)
            timeElapsed = 0f;       // ��� �ð� �ʱ�ȭ

            CancelInvoke("SpawnObject"); // ���� InvokeRepeating ���
            InvokeRepeating("SpawnObject", 0f, spawnInterval); // ���ο� spawnInterval�� �ݺ� ȣ�� ����
        }
    }

    public void SpawnObject()
    {
        // ȭ���� ������ X ��ǥ���� ������Ʈ�� ����
        float randomX = Random.Range(-8f, 8f); // ȭ�� ������ �°� X ���� ����
        Vector3 spawnPosition = new Vector3(randomX, -10f, 0f); // Y�� -10���� ���� (�Ʒ����� �ö���� ����)

        // ������Ʈ ����
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        // ������ ������Ʈ�� "Up" ��ũ��Ʈ �߰��ϰ� �ӵ� ����
        Up upScript = newObject.AddComponent<Up>();
        upScript.speed = initialSpeed; // �ʱ� �ӵ� ����
    }
}

public class Up : MonoBehaviour
{
    public float speed;               // ObjectUp���� ���޹޴� �ʱ� �ӵ� �� ���� �ӵ� 

    void Update()
    {

        // ������Ʈ�� ���� �ö󰡵��� �̵�
        transform.Translate(Vector3.up * Time.deltaTime * speed);

        // Y�� ������ ��ġ�� �����ϸ� ������Ʈ �ı�
        if (transform.position.y >= -0.25f)
        {
            Destroy(gameObject);
        }
    }
}
