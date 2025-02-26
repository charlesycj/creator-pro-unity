using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{

    private Dictionary<bool, Coroutine> speedBuffCoroutines = new Dictionary<bool, Coroutine>();
    private Dictionary<bool, Coroutine> hideBuffCoroutines = new Dictionary<bool, Coroutine>();
    private Dictionary<bool, Coroutine> ReverseDeBuffCoroutines = new Dictionary<bool, Coroutine>();
   

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

    public GameObject playerConfusionA;
    public GameObject playerConfusionB;

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
    public void SetPlayerReverse(bool isReverse, bool isPlayerA, float duration = 0f)
    {

        if (isReverse)
        {
            // �̹� ������ ���¶�� ���� ���¸� �״�� ����
            if (isPlayerA)
            {
                (playerALeftKey, playerARightKey) = (KeyCode.D, KeyCode.A);
                Debug.Log("A �÷��̾� Ű���� ���� �����");
            }
            else
            {
                (playerBLeftKey, playerBRightKey) = (KeyCode.RightArrow, KeyCode.LeftArrow);
                Debug.Log("B �÷��̾� Ű���� ���� �����");
            }
        }
        else
        {
            // ���� ���� ����
            if (isPlayerA)
            {
                (playerALeftKey, playerARightKey) = (KeyCode.A, KeyCode.D);
                Debug.Log("A �÷��̾� Ű���� ���� ������");
            }
            else
            {
                (playerBLeftKey, playerBRightKey) = (KeyCode.LeftArrow, KeyCode.RightArrow);
                Debug.Log("B �÷��̾� Ű���� ���� ������");
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


    // �ӵ� ���� ���� �ڷ�ƾ ����
    public void StartSpeedCoroutine(IEnumerator coroutineMethod, bool isPlayerA)
    {
        // ���� �ӵ� ���� �ڷ�ƾ ����
        if (speedBuffCoroutines.ContainsKey(isPlayerA) && speedBuffCoroutines[isPlayerA] != null)
        {
            StopCoroutine(speedBuffCoroutines[isPlayerA]);
        }

        // �� �ڷ�ƾ ����
        Coroutine newCoroutine = StartCoroutine(coroutineMethod);
        speedBuffCoroutines[isPlayerA] = newCoroutine;
    }

    // ���� ���� ���� �ڷ�ƾ ����
    public void StartHideCoroutine(IEnumerator coroutineMethod, bool isPlayerA)
    {
        // ���� ���� ���� �ڷ�ƾ ����
        if (hideBuffCoroutines.ContainsKey(isPlayerA) && hideBuffCoroutines[isPlayerA] != null)
        {
            StopCoroutine(hideBuffCoroutines[isPlayerA]);
        }

        // �� �ڷ�ƾ ����
        Coroutine newCoroutine = StartCoroutine(coroutineMethod);
        hideBuffCoroutines[isPlayerA] = newCoroutine;
    }

    //Ű���� ���� �ڷ�ƾ ����
    public void StartReverseDeBuffCoroutine(IEnumerator coroutineMethod, bool isPlayerA)
    {
        //���� Ű���� ���� �ڷ�ƾ ����
        if (ReverseDeBuffCoroutines.ContainsKey(isPlayerA) && ReverseDeBuffCoroutines[isPlayerA] != null)
        {
            StopCoroutine(ReverseDeBuffCoroutines[isPlayerA]);
        }


        // �� �ڷ�ƾ ����
        Coroutine newCoroutine = StartCoroutine(coroutineMethod);
        ReverseDeBuffCoroutines[isPlayerA] = newCoroutine;
    }

    // ���ǵ�� ���� ����
    public IEnumerator ApplySpeedBuff(PlayerManager playerController, bool isPlayerA)
    {
        // ���� �ӵ� �ʱ�ȭ
        if (isPlayerA)
            playerController.playerASpeed = 400;
        else
            playerController.playerBSpeed = 400;
        // Animator ��������
        GameObject player = isPlayerA ? playerController.playerA : playerController.playerB;
        Animator playerAnimator = player.GetComponent<Animator>();

        if (playerAnimator == null)
        {
            Debug.LogError("Animator�� ã�� �� �����ϴ�. �÷��̾ Animator�� �߰��Ǿ� �ִ��� Ȯ���ϼ���.");
            yield break;
        }
        // �ִϸ��̼� �ӵ� 2��� ����
        playerAnimator.speed = 2.5f;

        // ���� ����
        if (isPlayerA)
            playerController.playerASpeed +=200 ;
        else
            playerController.playerBSpeed +=200;

        yield return new WaitForSeconds(2f); // ù 2�� ���� ������ ���� ����

        // ���� 2�� ���� 0.5�� �������� �ӵ��� ����
        int decrementAmount = 50; // 0.5�ʸ��� �پ��� ��
        int steps = 4; // 2�� ���� 0.5�� ���� == �ӵ��� �����ϴ� Ƚ��
        for (int i = 0; i < steps; i++)
        {
            if (isPlayerA)
            {
                playerController.playerASpeed -= decrementAmount;
                Debug.Log($"A�� �ӵ� 50���� ����ӵ�: {playerASpeed}");
            }

            else
            {
                playerController.playerBSpeed -= decrementAmount;
                Debug.Log($"B�� �ӵ� 50���� ����ӵ�:{playerBSpeed}");
            }

            yield return new WaitForSeconds(0.5f);
        }

        // �ִϸ��̼� �ӵ� ����
        playerAnimator.speed = 1f;

        Debug.Log("���ǵ�� ���� ����");
    }
    // ���ǵ�ٿ� ����� ����
    public IEnumerator ApplySpeedDeBuff(PlayerManager playerController, bool isPlayerA)
    {
        
        // ���� �ӵ� �ʱ�ȭ
        if (isPlayerA)
            playerController.playerASpeed = 400;
        else
            playerController.playerBSpeed = 400;

        // Animator ��������
        GameObject player = isPlayerA ? playerController.playerA : playerController.playerB;
        Animator playerAnimator = player.GetComponent<Animator>();

        if (playerAnimator == null)
        {
            Debug.LogError("Animator�� ã�� �� �����ϴ�. �÷��̾ Animator�� �߰��Ǿ� �ִ��� Ȯ���ϼ���.");
            yield break;
        }

        // �ִϸ��̼� �ӵ� ����
        playerAnimator.speed = 0.5f;

        // ���� ���� (�ӵ� 200 ����)
        if (isPlayerA)
            playerController.playerASpeed -= 200;
        else
            playerController.playerBSpeed -= 200;

        yield return new WaitForSeconds(2f); // ù 2�� ���� ������ ���� ����

        // ���� 0.5�ʸ��� �ӵ��� �������Ѽ� 400���� ����
        int incrementAmount = 50; // 0.5�ʸ��� �����ϴ� ��
        int steps = 4; // 2�� ���� 0.5�� ���� == �ӵ��� �����ϴ� Ƚ��
        for (int i = 0; i < steps; i++)
        {
            if (isPlayerA)
            {
                playerController.playerASpeed += incrementAmount;
                Debug.Log($"A�� �ӵ� 50 ���� ����ӵ�: {playerController.playerASpeed}");
            }
            else
            {
                playerController.playerBSpeed += incrementAmount;
                Debug.Log($"B�� �ӵ� 50 ���� ����ӵ�: {playerController.playerBSpeed}");
            }

            yield return new WaitForSeconds(0.5f);
        }

        // �ִϸ��̼� �ӵ� ����
        playerAnimator.speed = 1f;

        Debug.Log("���ǵ� �ٿ� ���� ����");
    }

    // ���� ���� ����
    public IEnumerator ApplyHideBuff(float duration, bool isPlayerA)
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        PlayerManager playerController = playerManager.GetComponent<PlayerManager>();

        // ���� ���� Ȱ��ȭ
        playerController.SetPlayerHide(true, isPlayerA, duration);

        float remainingTime = duration;

        //���� ���Žð� ����s
        while (remainingTime > 0)
        {
            Debug.Log($"{(isPlayerA ? "Aĳ����" : "Bĳ����")}" +
                   $"�� ���� ȿ�� ���� �ð�: {remainingTime:F2}��");
            yield return new WaitForSeconds(0.5f); // 0.5�� �������� ������Ʈ
            remainingTime -= 0.5f;   
        }

        // ���� ���� ��Ȱ��ȭ
        playerController.SetPlayerHide(false, isPlayerA, duration);
    }
    public IEnumerator ApplyReverseDeBuff(float duration, bool isPlayerA)
    {
        GameObject playerManager = GameObject.Find("PlayerManager");
        PlayerManager playerController = playerManager.GetComponent<PlayerManager>();

        // Ű������� ���� Ȱ��ȭ
        playerController.SetPlayerReverse(true, isPlayerA, duration);

        float remainingTime = duration;

        // ������ Ű ���� ���� �� ���� �ð� üũ
        while (remainingTime > 0)
        {
                Debug.Log($"{(isPlayerA ? "Aĳ����" : "Bĳ����")}" +
                    $"�� Ű���� ���� ȿ�� ���� �ð�: {remainingTime:F2}��");
                yield return new WaitForSeconds(0.5f); // 0.5�� �������� ������Ʈ
                remainingTime -= 0.5f;     
        }
        //Ű���� ���� ���� ����
        playerController.SetPlayerReverse(false, isPlayerA, duration);
    }
}
