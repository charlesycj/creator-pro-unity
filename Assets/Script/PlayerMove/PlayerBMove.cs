using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBMove : MonoBehaviour
{
    public float maxSpeed; // �ִ� �ӵ�
    private Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = -1; // �߷� ����
    }

    void FixedUpdate()
    {
        // ȭ��ǥ Ű �Է����� �̵�
        float h = 0;

        if (Input.GetKey(KeyCode.LeftArrow)) // ���� ȭ��ǥ Ű�� ���ȴٸ�
        {
            h = -1; // �������� �̵�
        }
        else if (Input.GetKey(KeyCode.RightArrow)) // ������ ȭ��ǥ Ű�� ���ȴٸ�
        {
            h = 1; // ���������� �̵�
        }

        // �ӵ� ���� (�̲����� ����)
        rigid.velocity = new Vector2(h * maxSpeed, rigid.velocity.y);
    }
}
