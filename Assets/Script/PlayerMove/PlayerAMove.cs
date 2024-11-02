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

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // 최대 속도 제한
        if (rigid.velocity.x > maxSpeed) // 오른쪽 최대 속도
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < -maxSpeed) // 왼쪽 최대 속도
        {
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
        }
    }
}
