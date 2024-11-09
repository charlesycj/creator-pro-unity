using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAMove : MonoBehaviour
{
    public float maxSpeed; // �ִ� �ӵ�
    private Rigidbody2D rigid;
    public GameObject ground; //Ground ������Ʈ
    private float minX, maxX; // �̵� ������ x�� ����

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        if (ground != null)
        {   // Ground ������Ʈ�� �߽���ġ,ũ�� ������
            float groundWidth = ground.GetComponent<SpriteRenderer>().bounds.size.x;
            float groundCenter = ground.transform.position.x;

            // �̵� ������ x�� �ּ�/�ִ� ���� ����(+30,-30�ִ� ���� : Grond �������� �̵�)  
            minX = (groundCenter - groundWidth / 2) + 30;
            maxX = (groundCenter + groundWidth / 2) -30 ;
        }
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

        // �ӵ� ���� (�̲����� ����)
        rigid.velocity = new Vector2(h * maxSpeed, rigid.velocity.y);
        
        //Ground ���� ���� �̵� ����
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        transform.position = clampedPosition;
    }

}
