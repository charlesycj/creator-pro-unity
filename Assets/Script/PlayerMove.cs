using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player A Settings")]
    public GameObject playerA; // Player A ������Ʈ
    public float playerASpeed; // Player A �ӵ�
    public KeyCode playerALeftKey = KeyCode.A; // Player A ���� �̵� Ű
    public KeyCode playerARightKey = KeyCode.D; // Player A ������ �̵� Ű

    [Header("Player B Settings")]
    public GameObject playerB; // Player B ������Ʈ
    public float playerBSpeed; // Player B �ӵ�
    public KeyCode playerBLeftKey = KeyCode.LeftArrow; // Player B ���� �̵� Ű
    public KeyCode playerBRightKey = KeyCode.RightArrow; // Player B ������ �̵� Ű

    [Header("Ground Settings")]
    public GameObject ground; // Ground ������Ʈ

    private float minX, maxX; // Ground �� �̵� ������ x�� ����

    private Rigidbody2D rigidA, rigidB; // ������ Rigidbody2D
    private SpriteRenderer rendererA, rendererB; // ������ SpriteRenderer

    void Start()
    {
        // Player A �ʱ�ȭ
        rigidA = playerA.GetComponent<Rigidbody2D>();
        rendererA = playerA.GetComponent<SpriteRenderer>();

        // Player B �ʱ�ȭ
        rigidB = playerB.GetComponent<Rigidbody2D>();
        rigidB.gravityScale = -200; // �߷� ����
        rendererB = playerB.GetComponent<SpriteRenderer>();

        // Ground ���� ����
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
        // Player A �̵�
        HandlePlayerMovement(playerA, rigidA, rendererA, playerASpeed, playerALeftKey, playerARightKey);

        // Player B �̵�
        HandlePlayerMovement(playerB, rigidB, rendererB, playerBSpeed, playerBLeftKey, playerBRightKey);
    }

    void HandlePlayerMovement(GameObject player, Rigidbody2D rigid, SpriteRenderer renderer, float speed, KeyCode leftKey, KeyCode rightKey)
    {
        float h = 0;

        if (Input.GetKey(leftKey)) // ���� �̵� Ű�� ���ȴٸ�
        {
            h = -1; // �������� �̵�
            renderer.flipX = false; // �̹��� �״��
        }
        else if (Input.GetKey(rightKey)) // ������ �̵� Ű�� ���ȴٸ�
        {
            h = 1; // ���������� �̵�
            renderer.flipX = true; // �̹��� ����
        }

        // �ӵ� ����
        rigid.velocity = new Vector2(h * speed, rigid.velocity.y);

        // Ground ���� ���� �̵� ����
        Vector3 clampedPosition = player.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        player.transform.position = clampedPosition;
    }
}
