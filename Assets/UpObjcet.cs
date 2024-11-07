using UnityEngine;

public class ObjectUp : MonoBehaviour
{
    public GameObject objectPrefab;      // 올라갈 오브젝트 프리팹
    public float spawnInterval = 1f;     // 오브젝트 생성간격 (초)
    public float destroyYPosition = -0.25f; // 파괴될 Y 위치 (지면에 도달시 파괴됨)
    private float timeElapsed = 0f;      // 경과 시간
    private float initialSpeed = 5f;     // 오브젝트의 초기 속도

    void Start()
    {
        // 1초에 한 번씩 오브젝트를 생성하는 메서드 호출
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    void Update()
    {
        // 시간 3초가 경과할 때마다 spawnInterval을 0.1씩 감소
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= 3f) //3초가 지날때마다
        {
            spawnInterval -= 0.01f; // spawnInterval을 0.01씩 감소 (오프젝트 생성속도)
            initialSpeed += 0.3f;   // 오브젝트의 속도를 0.3씩 증가시킴 (오프젝트의 이동속도)
            timeElapsed = 0f;       // 경과 시간 초기화

            CancelInvoke("SpawnObject"); // 기존 InvokeRepeating 취소
            InvokeRepeating("SpawnObject", 0f, spawnInterval); // 새로운 spawnInterval로 반복 호출 시작
        }
    }

    public void SpawnObject()
    {
        // 화면의 무작위 X 좌표에서 오브젝트를 생성
        float randomX = Random.Range(-8f, 8f); // 화면 범위에 맞게 X 범위 설정
        Vector3 spawnPosition = new Vector3(randomX, -10f, 0f); // Y는 -10으로 설정 (아래에서 올라오기 시작)

        // 오브젝트 생성
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        // 생성된 오브젝트에 "Up" 스크립트 추가하고 속도 설정
        Up upScript = newObject.AddComponent<Up>();
        upScript.speed = initialSpeed; // 초기 속도 전달
    }
}

public class Up : MonoBehaviour
{
    public float speed;               // ObjectUp에서 전달받는 초기 속도 및 증가 속도 

    void Update()
    {

        // 오브젝트가 위로 올라가도록 이동
        transform.Translate(Vector3.up * Time.deltaTime * speed);

        // Y가 지정된 위치에 도달하면 오브젝트 파괴
        if (transform.position.y >= -0.25f)
        {
            Destroy(gameObject);
        }
    }
}
