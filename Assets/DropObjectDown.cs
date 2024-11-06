using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject dropObjectPrefab; // 떨어질 오브젝트 프리팹
    public float dropSpeed = 5f; // 오브젝트가 떨어지는 속도
    private Camera mainCamera;
    private float screenLeft;
    private float screenRight;

    // 파괴할 y 좌표 설정
    public float destroyYPosition = 0.25f;

    void Start()
    {
        // 메인 카메라의 경계를 가져옵니다.
        mainCamera = Camera.main;
        Vector3 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // 화면의 좌측과 우측 경계 설정
        screenLeft = -screenBounds.x;
        screenRight = screenBounds.x;

        // 1초마다 SpawnDropObject 메서드 호출
        InvokeRepeating("SpawnDropObject", 0f, 1f);
    }

    void SpawnDropObject()
    {
        // 무작위 x 위치 설정
        float randomX = Random.Range(screenLeft, screenRight);
        float startY = mainCamera.orthographicSize;

        // 새로운 오브젝트 생성 및 위치 설정
        GameObject newDropObject = Instantiate(dropObjectPrefab, new Vector2(randomX, startY), Quaternion.identity);

        // DropObject 컴포넌트에 속도 전달
        DropObject dropBehavior = newDropObject.GetComponent<DropObject>();
        dropBehavior.dropSpeed = dropSpeed;
    }
}

public class DropObject : MonoBehaviour
{
    public float dropSpeed = 5f; // 오브젝트가 떨어지는 속도
    public float destroyYPosition = 0.25f; // 오브젝트가 사라질 Y 좌표

    void Update()
    {
        // 오브젝트가 떨어지도록 이동
        transform.Translate(Vector2.down * dropSpeed * Time.deltaTime);

        // y 좌표가 destroyYPosition 보다 작으면 오브젝트 삭제
        Debug.Log("Current Y Position: " + transform.position.y); // y 값 확인

        if (transform.position.y < destroyYPosition)
        {
            Debug.Log("Destroying object at position: " + transform.position.y); // 파괴 조건 확인
            Destroy(gameObject); // 오브젝트 파괴
        }
    }
}
