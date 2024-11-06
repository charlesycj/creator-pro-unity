using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject dropObjectPrefab; // ������ ������Ʈ ������
    public float dropSpeed = 5f; // ������Ʈ�� �������� �ӵ�
    private Camera mainCamera;
    private float screenLeft;
    private float screenRight;

    // �ı��� y ��ǥ ����
    public float destroyYPosition = 0.25f;

    void Start()
    {
        // ���� ī�޶��� ��踦 �����ɴϴ�.
        mainCamera = Camera.main;
        Vector3 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // ȭ���� ������ ���� ��� ����
        screenLeft = -screenBounds.x;
        screenRight = screenBounds.x;

        // 1�ʸ��� SpawnDropObject �޼��� ȣ��
        InvokeRepeating("SpawnDropObject", 0f, 1f);
    }

    void SpawnDropObject()
    {
        // ������ x ��ġ ����
        float randomX = Random.Range(screenLeft, screenRight);
        float startY = mainCamera.orthographicSize;

        // ���ο� ������Ʈ ���� �� ��ġ ����
        GameObject newDropObject = Instantiate(dropObjectPrefab, new Vector2(randomX, startY), Quaternion.identity);

        // DropObject ������Ʈ�� �ӵ� ����
        DropObject dropBehavior = newDropObject.GetComponent<DropObject>();
        dropBehavior.dropSpeed = dropSpeed;
    }
}

public class DropObject : MonoBehaviour
{
    public float dropSpeed = 5f; // ������Ʈ�� �������� �ӵ�
    public float destroyYPosition = 0.25f; // ������Ʈ�� ����� Y ��ǥ

    void Update()
    {
        // ������Ʈ�� ���������� �̵�
        transform.Translate(Vector2.down * dropSpeed * Time.deltaTime);

        // y ��ǥ�� destroyYPosition ���� ������ ������Ʈ ����
        Debug.Log("Current Y Position: " + transform.position.y); // y �� Ȯ��

        if (transform.position.y < destroyYPosition)
        {
            Debug.Log("Destroying object at position: " + transform.position.y); // �ı� ���� Ȯ��
            Destroy(gameObject); // ������Ʈ �ı�
        }
    }
}
