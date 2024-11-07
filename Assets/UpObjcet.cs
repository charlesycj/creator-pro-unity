using UnityEngine;

public class ObjectUp : MonoBehaviour
{
    public GameObject objectPrefab;  // 올라갈 오브젝트 프리팹
    public float spawnInterval = 1f; // 오브젝트 생성간격 (초)
    public float destroyYPosition = -0.25f; // 파괴될 Y 위치 (위로 올라갈 때 파괴됨)

    void Start()
    {
        // 1초에 한 번씩 오브젝트를 생성하는 메서드 호출
        InvokeRepeating("SpawnObject", 0f, spawnInterval);
    }

    public void SpawnObject()  // public으로 변경
    {
        // 화면의 무작위 X 좌표에서 오브젝트를 생성
        float randomX = Random.Range(-8f, 8f); // 화면 범위에 맞게 X 범위 설정
        Vector3 spawnPosition = new Vector3(randomX, -10f, 0f); // Y는 -10으로 설정 (아래에서 올라오기 시작)

        // 오브젝트 생성
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

        // 생성된 오브젝트에 "Up" 스크립트 추가
        newObject.AddComponent<Up>();
    }
}

public class Up : MonoBehaviour
{
    void Update()
    {
        // 오브젝트가 위로 올라가도록 이동 (속도는 조정 가능)
        transform.Translate(Vector3.up * Time.deltaTime * 5f);  // Y축으로 위로 올라가기

        // Y가 지정된 위치에 도달하면 오브젝트 파괴
        if (transform.position.y >= -0.25f)
        {
            Destroy(gameObject);
        }
    }
}
