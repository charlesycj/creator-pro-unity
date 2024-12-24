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
    public GameObject playerA; // Player A 오브젝트
    public float playerASpeed = 400; // Player A 속도
    public KeyCode playerALeftKey = KeyCode.A; // Player A 왼쪽 이동 키
    public KeyCode playerARightKey = KeyCode.D; // Player A 오른쪽 이동 키
    public bool playerAHide;
    public bool playerAHasShield;


    [Header("Player B Settings")]
    public GameObject playerB; // Player B 오브젝트
    public float playerBSpeed = 400; // Player B 속도
    public KeyCode playerBLeftKey = KeyCode.LeftArrow; // Player B 왼쪽 이동 키
    public KeyCode playerBRightKey = KeyCode.RightArrow; // Player B 오른쪽 이동 키
    public bool playerBHide;
    public bool playerBHasShield;

    //1회용 쉴드 활성화 상태 추적
    private bool playerAIsShieldActive, playerBIsShieldActive;

    [Header("Ground Settings")]
    public GameObject ground; // Ground 오브젝트

    private float minX, maxX; // Ground 내 이동 가능한 x축 범위

    private Rigidbody2D rigidA, rigidB; // 각각의 Rigidbody2D
    private SpriteRenderer rendererA, rendererB; // 각각의 SpriteRenderer

    private bool isGameOver = false; // 게임오버 상태 플래그

    public string DeadPlayer;

    public GameObject playerShieldA; // 쉴드A
    public GameObject playerShieldB; // 쉴드B

    public SoundManager soundManager; // SoundManager 참조

    void Start()
    {
        // Player A 초기화
        rigidA = playerA.GetComponent<Rigidbody2D>();
        rendererA = playerA.GetComponent<SpriteRenderer>();

        // Player B 초기화
        rigidB = playerB.GetComponent<Rigidbody2D>();
        rigidB.gravityScale = -100; // 중력 반전
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
        if (!isGameOver)
        {

            // 플레이어 A와 B의 이동을 처리
            HandlePlayerMovement(playerA, rigidA, rendererA, playerASpeed, playerALeftKey, playerARightKey);
            HandlePlayerMovement(playerB, rigidB, rendererB, playerBSpeed, playerBLeftKey, playerBRightKey);

        }
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
        rigid.velocity = new Vector2(h * speed, rigid.velocity.y); // y축 속도는 그대로 두고 x축 속도만 변경

        // Ground 범위 내로 이동 제한
        Vector3 clampedPosition = player.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        player.transform.position = clampedPosition;

        // 애니메이션 처리
        Animator playerAnim = player.GetComponent<Animator>();
        if (h == 0) // 멈춘 상태
        {
            playerAnim.SetBool("isRunning", false);
        }
        else // 움직이는 상태
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

    // 1회용 실드가 어떤플레이어에게 활성화되어 있는지 확인
    public bool IsShieldActive(bool isPlayerA)
    {
        return isPlayerA ? playerAIsShieldActive : playerBIsShieldActive;
    }

    // 실드 적용 함수
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

    // 실드 해제 함수
    public void RemoveShieldBuff(bool isPlayerA)
    {
        if (isPlayerA)
        {
            playerAIsShieldActive = false;
            playerAHasShield = false; // 실드 상태를 명확히 해제
            playerShieldA.SetActive(false);
            soundManager.PlayRemoveShield();
        }
        else
        {
            playerBIsShieldActive = false;
            playerBHasShield = false; // 실드 상태를 명확히 해제
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
            // 반투명 효과 (알파 값 50%)
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.5f);

            // 장애물 충돌 비활성화
            foreach (GameObject Avoid in GameObject.FindGameObjectsWithTag("Avoid"))
            {
                Collider2D obstacleCollider = Avoid.GetComponent<Collider2D>();
                if (obstacleCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, obstacleCollider, true);
                }
            }

            // 은신 상태 유지 및 충돌 무시 상태 유지
            StartCoroutine(KeepCollisionIgnoredDuringHide(duration, playerCollider));
        }
        else
        {
            // 반투명 효과 해제
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);

            // 장애물 충돌 활성화
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

    // 은신 상태 동안 충돌 무시를 지속적으로 유지하는 코루틴
    private IEnumerator KeepCollisionIgnoredDuringHide(float duration, Collider2D playerCollider)
    {
        float remainingTime = duration;

        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(0.1f); // 매 0.1초마다 계속 충돌을 무시
            remainingTime -= 0.1f;

            // 은신 상태 동안 장애물 충돌 무시
            foreach (GameObject Avoid in GameObject.FindGameObjectsWithTag("Avoid"))
            {
                Collider2D obstacleCollider = Avoid.GetComponent<Collider2D>();
                if (obstacleCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, obstacleCollider, true);
                }
            }
        }

        // 은신 상태가 끝나면 충돌을 다시 활성화
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
        // 기존 코루틴 중지
        if (coroutineDict.ContainsKey(isPlayerA) && coroutineDict[isPlayerA] != null)
        {
            StopCoroutine(coroutineDict[isPlayerA]);
        }
        // 새 코루틴 시작
        Coroutine newCoroutine = StartCoroutine(coroutineMethod);
        coroutineDict[isPlayerA] = newCoroutine;
        return newCoroutine;
    }
}
  