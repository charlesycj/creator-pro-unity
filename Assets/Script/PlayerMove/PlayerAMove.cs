using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAMove : MonoBehaviour
{
    public float maxSpeed; // �ִ� �ӵ�
    private Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // A�� D Ű �Է����� �̵�
        float h = 0;

        if (Input.GetKey(KeyCode.A)) // A Ű�� ���ȴٸ�
        {
            h = -1; // �������� �̵�
        }
        else if (Input.GetKey(KeyCode.D)) // D Ű�� ���ȴٸ�
        {
            h = 1; // ���������� �̵�
        }

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // �ִ� �ӵ� ����
        if (rigid.velocity.x > maxSpeed) // ������ �ִ� �ӵ�
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < -maxSpeed) // ���� �ִ� �ӵ�
        {
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
        }
    }
}
