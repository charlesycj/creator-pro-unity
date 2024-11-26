using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [Header("Player A Settings")]
    public GameObject playerA; // Player A 오브젝트
    public float playerASpeed; // Player A 속도
    public KeyCode playerALeftKey = KeyCode.A; // Player A 왼쪽 이동 키
    public KeyCode playerARightKey = KeyCode.D; // Player A 오른쪽 이동 키

    [Header("Player B Settings")]
    public GameObject playerB; // Player B 오브젝트
    public float playerBSpeed; // Player B 속도
    public KeyCode playerBLeftKey = KeyCode.LeftArrow; // Player B 왼쪽 이동 키
    public KeyCode playerBRightKey = KeyCode.RightArrow; // Player B 오른쪽 이동 키

    [Header("Ground Settings")]
    public GameObject ground; // Ground 오브젝트

    private float minX, maxX; // Ground 내 이동 가능한 x축 범위

    private Rigidbody2D rigidA, rigidB; // 각각의 Rigidbody2D
    private SpriteRenderer rendererA, rendererB; // 각각의 SpriteRenderer

    private bool isGameOver = false; // 게임오버 상태 플래그

    void Start()
    {
        // Player A 초기화
        rigidA = playerA.GetComponent<Rigidbody2D>();
        rendererA = playerA.GetComponent<SpriteRenderer>();

        // Player B 초기화
        rigidB = playerB.GetComponent<Rigidbody2D>();
        rigidB.gravityScale = -200; // 중력 반전
        rendererB = playerB.GetComponent<SpriteRenderer>();

        // Ground 범위 설정
        if (ground != null)
        {
            float groundWidth = ground.GetComponent<SpriteRenderer>().bounds.size.x;
            float groundCenter = ground.transform.position.x;

            minX = (groundCenter - groundWidth / 2) + 30;
            maxX = (groundCenter + groundWidth / 2) - 30;
        }
    }

    void FixedUpdate()
    {
        if (isGameOver)
        {
            // 게임오버 시 이동 정지
            StopPlayerMovement(rigidA);
            StopPlayerMovement(rigidB);
            return; // 더 이상 이동하지 않도록 Early Return
        }

        // 게임오버 상태가 아니라면 플레이어 이동 처리
        HandlePlayerMovement(playerA, rigidA, rendererA, playerASpeed, playerALeftKey, playerARightKey);
        HandlePlayerMovement(playerB, rigidB, rendererB, playerBSpeed, playerBLeftKey, playerBRightKey);
    }

    void HandlePlayerMovement(GameObject player, Rigidbody2D rigid, SpriteRenderer renderer, float speed, KeyCode leftKey, KeyCode rightKey)
    {
        float h = 0;

        if (Input.GetKey(leftKey)) // 왼쪽 이동 키가 눌렸다면
        {
            h = -1; // 왼쪽으로 이동
            renderer.flipX = false; // 이미지 그대로
        }
        else if (Input.GetKey(rightKey)) // 오른쪽 이동 키가 눌렸다면
        {
            h = 1; // 오른쪽으로 이동
            renderer.flipX = true; // 이미지 반전
        }

        // 속도 설정
        rigid.velocity = new Vector2(h * speed, rigid.velocity.y);

        // Ground 범위 내로 이동 제한
        Vector3 clampedPosition = player.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        player.transform.position = clampedPosition;
    }

    public void TriggerGameOver()
    {
        isGameOver = true; // 게임오버 상태로 전환
        Debug.Log("게임오버: 플레이어 움직임 정지");

        // 플레이어 A와 B의 Rigidbody2D 정지
        playerASpeed = 0;
        playerBSpeed = 0;
    }

    private void StopPlayerMovement(Rigidbody2D rigid)
    {
        if (rigid != null)
        {
            rigid.velocity = Vector2.zero; // 속도 초기화
            rigid.isKinematic = true; // 물리 연산 비활성화
        }
    }

}
