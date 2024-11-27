using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{

    public Dictionary<bool, Coroutine> activeSpeedBuffCoroutines = new Dictionary<bool, Coroutine>();
    public Dictionary<bool, Coroutine> activeHideBuffCoroutines = new Dictionary<bool, Coroutine>();

    [Header("Player A Settings")]
    public GameObject playerA; // Player A ������Ʈ
    public float playerASpeed = 400; // Player A �ӵ�
    public KeyCode playerALeftKey = KeyCode.A; // Player A ���� �̵� Ű
    public KeyCode playerARightKey = KeyCode.D; // Player A ������ �̵� Ű
    public bool playerAHide;
    public bool playerAHasShield;


    [Header("Player B Settings")]
    public GameObject playerB; // Player B ������Ʈ
    public float playerBSpeed = 400; // Player B �ӵ�
    public KeyCode playerBLeftKey = KeyCode.LeftArrow; // Player B ���� �̵� Ű
    public KeyCode playerBRightKey = KeyCode.RightArrow; // Player B ������ �̵� Ű
    public bool playerBHide;
    public bool playerBHasShield;

    //1ȸ�� ���� Ȱ��ȭ ���� ����
    private bool playerAIsShieldActive, playerBIsShieldActive;

    [Header("Ground Settings")]
    public GameObject ground; // Ground ������Ʈ

    private float minX, maxX; // Ground �� �̵� ������ x�� ����

    private Rigidbody2D rigidA, rigidB; // ������ Rigidbody2D
    private SpriteRenderer rendererA, rendererB; // ������ SpriteRenderer

    private bool isGameOver = false; // ���ӿ��� ���� �÷���

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
        if (!isGameOver)
        {

            // �÷��̾� A�� B�� �̵��� ó��
            HandlePlayerMovement(playerA, rigidA, rendererA, playerASpeed, playerALeftKey, playerARightKey);
            HandlePlayerMovement(playerB, rigidB, rendererB, playerBSpeed, playerBLeftKey, playerBRightKey);
        }
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
        rigid.velocity = new Vector2(h * speed, rigid.velocity.y); // y�� �ӵ��� �״�� �ΰ� x�� �ӵ��� ����

        // Ground ���� ���� �̵� ����
        Vector3 clampedPosition = player.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        player.transform.position = clampedPosition;
    }


    public bool IsPlayerHide(bool isPlayerA)
    {
        return isPlayerA ? playerAHide : playerBHide;
    }

    public bool HasShield(bool isPlayerA)
    {
        return isPlayerA ? playerAHasShield : playerBHasShield;
    }

    // 1ȸ�� �ǵ尡 ��÷��̾�� Ȱ��ȭ�Ǿ� �ִ��� Ȯ��
    public bool IsShieldActive(bool isPlayerA)
    {
        return isPlayerA ? playerAIsShieldActive : playerBIsShieldActive;
    }

    // �ǵ� ���� �Լ�
    public void ApplyShieldBuff(bool isPlayerA)
    {
        if (isPlayerA && !playerAIsShieldActive)
        {
            playerAIsShieldActive = true;
            playerAHasShield = true;
        }
        else if (!isPlayerA && !playerBIsShieldActive)
        {
            playerBIsShieldActive = true;
            playerBHasShield = true;
        }
    }

    // �ǵ� ���� �Լ�
    public void RemoveShieldBuff(bool isPlayerA)
    {
        if (isPlayerA)
        {
            playerAIsShieldActive = false;
            playerAHasShield = false; // �ǵ� ���¸� ��Ȯ�� ����
        }
        else
        {
            playerBIsShieldActive = false;
            playerBHasShield = false; // �ǵ� ���¸� ��Ȯ�� ����
        }
    }

    public void SetPlayerHide(bool isHide, bool isPlayerA)
    {
        if (isPlayerA)
        {
            playerAHide = isHide; // ���� ���� ������Ʈ
            playerA.SetActive(!isHide); // �÷��̾� ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
        }
        else
        {
            playerBHide = isHide;
            playerB.SetActive(!isHide);
        }
    }
    public Coroutine StartManagedCoroutine(Dictionary<bool, Coroutine> coroutineDict, bool isPlayerA, IEnumerator coroutineMethod)
    {
        // ���� �ڷ�ƾ ����
        if (coroutineDict.ContainsKey(isPlayerA) && coroutineDict[isPlayerA] != null)
        {
            StopCoroutine(coroutineDict[isPlayerA]);
        }
        // �� �ڷ�ƾ ����
        Coroutine newCoroutine = StartCoroutine(coroutineMethod);
        coroutineDict[isPlayerA] = newCoroutine;
        return newCoroutine;
    }
}
  