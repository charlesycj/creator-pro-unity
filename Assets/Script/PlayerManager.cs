using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class PlayerManager : MonoBehaviour
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

    public string DeadPlayer;

    public GameObject playerShieldA; // ����A
    public GameObject playerShieldB; // ����B

    public SoundManager soundManager; // SoundManager ����

    void Start()
    {
        // Player A �ʱ�ȭ
        rigidA = playerA.GetComponent<Rigidbody2D>();
        rendererA = playerA.GetComponent<SpriteRenderer>();

        // Player B �ʱ�ȭ
        rigidB = playerB.GetComponent<Rigidbody2D>();
        rigidB.gravityScale = -100; // �߷� ����
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

        // �ִϸ��̼� ó��
        Animator playerAnim = player.GetComponent<Animator>();
        if (h == 0) // ���� ����
        {
            playerAnim.SetBool("isRunning", false);
        }
        else // �����̴� ����
        {
            playerAnim.SetBool("isRunning", true);
        }
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
            playerShieldA.SetActive(true);
        }
        else if (!isPlayerA && !playerBIsShieldActive)
        {
            playerBIsShieldActive = true;
            playerBHasShield = true;
            playerShieldB.SetActive(true);
        }
    }

    // �ǵ� ���� �Լ�
    public void RemoveShieldBuff(bool isPlayerA)
    {
        if (isPlayerA)
        {
            playerAIsShieldActive = false;
            playerAHasShield = false; // �ǵ� ���¸� ��Ȯ�� ����
            playerShieldA.SetActive(false);
            soundManager.PlayRemoveShield();
        }
        else
        {
            playerBIsShieldActive = false;
            playerBHasShield = false; // �ǵ� ���¸� ��Ȯ�� ����
            playerShieldB.SetActive(false);
            soundManager.PlayRemoveShield();
        }
    }

    public void SetPlayerHide(bool isHide, bool isPlayerA, float duration = 0f)
    {
        GameObject player = isPlayerA ? playerA : playerB;
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        SpriteRenderer renderer = player.GetComponent<SpriteRenderer>();

        if (isHide)
        {
            // ������ ȿ�� (���� �� 50%)
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.5f);

            // ��ֹ� �浹 ��Ȱ��ȭ
            foreach (GameObject Avoid in GameObject.FindGameObjectsWithTag("Avoid"))
            {
                Collider2D obstacleCollider = Avoid.GetComponent<Collider2D>();
                if (obstacleCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, obstacleCollider, true);
                }
            }

            // ���� ���� ���� �� �浹 ���� ���� ����
            StartCoroutine(KeepCollisionIgnoredDuringHide(duration, playerCollider));
        }
        else
        {
            // ������ ȿ�� ����
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);

            // ��ֹ� �浹 Ȱ��ȭ
            foreach (GameObject Avoid in GameObject.FindGameObjectsWithTag("Avoid"))
            {
                Collider2D obstacleCollider = Avoid.GetComponent<Collider2D>();
                if (obstacleCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, obstacleCollider, false);
                }
            }
        }
    }

    // ���� ���� ���� �浹 ���ø� ���������� �����ϴ� �ڷ�ƾ
    private IEnumerator KeepCollisionIgnoredDuringHide(float duration, Collider2D playerCollider)
    {
        float remainingTime = duration;

        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(0.1f); // �� 0.1�ʸ��� ��� �浹�� ����
            remainingTime -= 0.1f;

            // ���� ���� ���� ��ֹ� �浹 ����
            foreach (GameObject Avoid in GameObject.FindGameObjectsWithTag("Avoid"))
            {
                Collider2D obstacleCollider = Avoid.GetComponent<Collider2D>();
                if (obstacleCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, obstacleCollider, true);
                }
            }
        }

        // ���� ���°� ������ �浹�� �ٽ� Ȱ��ȭ
        foreach (GameObject Avoid in GameObject.FindGameObjectsWithTag("Avoid"))
        {
            Collider2D obstacleCollider = Avoid.GetComponent<Collider2D>();
            if (obstacleCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, obstacleCollider, false);
            }
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
  