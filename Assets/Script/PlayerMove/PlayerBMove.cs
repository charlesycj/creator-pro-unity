using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBMove : MonoBehaviour
{
    public float curSpeed; // �ִ� �ӵ�
    private Rigidbody2D rigid;
    public GameObject ground; //Ground ������Ʈ
    private float minX, maxX; // �̵� ������ x�� ����
    private SpriteRenderer rendererB; // �̹��� �¿����

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = -200; // �߷� ����
        rendererB = GetComponent<SpriteRenderer>();

        if (ground != null)
        {   // Ground ������Ʈ�� �߽���ġ,ũ�� ������
            float groundWidth = ground.GetComponent<SpriteRenderer>().bounds.size.x;
            float groundCenter = ground.transform.position.x;

            // �̵� ������ x�� �ּ�/�ִ� ���� ����(+30,-30�ִ� ���� : Grond �������� �̵�)  
            minX = (groundCenter - groundWidth / 2) + 30;
            maxX = (groundCenter + groundWidth / 2) - 30;
        }
    }

    void FixedUpdate()
    {
        // ȭ��ǥ Ű �Է����� �̵�
        float h = 0;

        if (Input.GetKey(KeyCode.LeftArrow)) // ���� ȭ��ǥ Ű�� ���ȴٸ�
        {
            h = -1; // �������� �̵�
            rendererB.flipX = true; // �̹��� ����           
        }
        else if (Input.GetKey(KeyCode.RightArrow)) // ������ ȭ��ǥ Ű�� ���ȴٸ�
        {
            h = 1; // ���������� �̵�
            rendererB.flipX = false; // �̹��� �״��
        }

        // �ӵ� ���� (�̲����� ����)
        rigid.velocity = new Vector2(h * curSpeed, rigid.velocity.y);

        //Ground ���� ���� �̵� ����
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        transform.position = clampedPosition;
    }
}
