using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAMove : MonoBehaviour
{
    public float maxSpeed; // 최대 속도
    private Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        // A와 D 키 입력으로 이동
        float h = 0;

        if (Input.GetKey(KeyCode.A)) // A 키가 눌렸다면
        {
            h = -1; // 왼쪽으로 이동
        }
        else if (Input.GetKey(KeyCode.D)) // D 키가 눌렸다면
        {
            h = 1; // 오른쪽으로 이동
        }

        // 속도 설정 (미끄러짐 방지)
        rigid.velocity = new Vector2(h * maxSpeed, rigid.velocity.y);
    }
}
