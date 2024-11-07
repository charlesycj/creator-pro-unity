using UnityEngine;

public class ObjectUp : MonoBehaviour
{
    public GameObject objectPrefab;  // �ö� ������Ʈ ������
    public float spawnInterval = 1f; // ������Ʈ �������� (��)
    public float destroyYPosition = -0.25f; // �ı��� Y ��ġ (���� �ö� �� �ı���)

    void Start()
    {
        // 1�ʿ� �� ���� ������Ʈ�� �����ϴ� �޼��� ȣ��
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    public void SpawnObject()  // public���� ����
    {
        // ȭ���� ������ X ��ǥ���� ������Ʈ�� ����
        float randomX = Random.Range(-8f, 8f); // ȭ�� ������ �°� X ���� ����
        Vector3 spawnPosition = new Vector3(randomX, -10f, 0f); // Y�� -10���� ���� (�Ʒ����� �ö���� ����)

        // ������Ʈ ����
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        // ������ ������Ʈ�� "Up" ��ũ��Ʈ �߰�
        newObject.AddComponent<Up>();
    }
}

public class Up : MonoBehaviour
{
    void Update()
    {
        // ������Ʈ�� ���� �ö󰡵��� �̵� (�ӵ��� ���� ����)
        transform.Translate(Vector3.up * Time.deltaTime * 5f);  // Y������ ���� �ö󰡱�

        // Y�� ������ ��ġ�� �����ϸ� ������Ʈ �ı�
        if (transform.position.y >= -0.25f)
        {
            Destroy(gameObject);
        }
    }
}
