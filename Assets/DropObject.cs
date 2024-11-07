using UnityEngine;



public class ObjectFall : MonoBehaviour
{
    public GameObject objectPrefab;  // ������ ������Ʈ ������
    public float spawnInterval = 1f; // ������Ʈ �������� ����
    public float destroyYPosition = 0.25f; // �ı��� Y ��ġ

    void Start()
    {
        // 1�ʿ� �� ���� ������Ʈ�� ����߸��� �ڷ�ƾ ����
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    public void SpawnObject()  // public���� ����
    {
        // ȭ���� ������ X ��ǥ���� ������Ʈ�� ����
        float randomX = Random.Range(-8f, 8f); // ȭ�� ������ �°� X ���� ����
        Vector3 spawnPosition = new Vector3(randomX, 10f, 0f); // Y�� 10���� ���� (������ �������� ����)

        // ������Ʈ ����
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        // ������ ������Ʈ�� "Fall" ��ũ��Ʈ �߰�
        newObject.AddComponent<Fall>();
    }
}

public class Fall : MonoBehaviour
{
    void Update()
    {
        // ������Ʈ�� ���������� �̵� (�ӵ��� ���� ����)
        transform.Translate(Vector3.down * Time.deltaTime * 5f);  // Y������ �������鼭 ������

        // Y�� ������ ��ġ�� �����ϸ� ������Ʈ �ı�
        if (transform.position.y <= 0.25f)
        {
            Destroy(gameObject);
        }
    }
}
