using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBMove : MonoBehaviour
{
    public float curSpeed; // 최대 속도
    private Rigidbody2D rigid;
    public GameObject ground; //Ground 오브젝트
    private float minX, maxX; // 이동 가능한 x축 범위
    private SpriteRenderer rendererB; // 이미지 좌우반전

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.gravityScale = -200; // 중력 반전
        rendererB = GetComponent<SpriteRenderer>();

        if (ground != null)
        {   // Ground 오브젝트의 중심위치,크기 가져옴
            float groundWidth = ground.GetComponent<SpriteRenderer>().bounds.size.x;
            float groundCenter = ground.transform.position.x;

            // 이동 가능한 x축 최소/최대 범위 설정(+30,-30주는 이유 : Grond 내에서만 이동)  
            minX = (groundCenter - groundWidth / 2) + 30;
            maxX = (groundCenter + groundWidth / 2) - 30;
        }
    }

    void FixedUpdate()
    {
        // 화살표 키 입력으로 이동
        float h = 0;

        if (Input.GetKey(KeyCode.LeftArrow)) // 왼쪽 화살표 키가 눌렸다면
        {
            h = -1; // 왼쪽으로 이동
            rendererB.flipX = true; // 이미지 반전           
        }
        else if (Input.GetKey(KeyCode.RightArrow)) // 오른쪽 화살표 키가 눌렸다면
        {
            h = 1; // 오른쪽으로 이동
            rendererB.flipX = false; // 이미지 그대로
        }

        // 속도 설정 (미끄러짐 방지)
        rigid.velocity = new Vector2(h * curSpeed, rigid.velocity.y);

        //Ground 범위 내로 이동 제한
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        transform.position = clampedPosition;
    }
}
