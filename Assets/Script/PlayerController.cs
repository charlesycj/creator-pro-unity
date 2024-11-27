using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
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
        }
        else if (!isPlayerA && !playerBIsShieldActive)
        {
            playerBIsShieldActive = true;
            playerBHasShield = true;
        }
    }

    // 실드 해제 함수
    public void RemoveShieldBuff(bool isPlayerA)
    {
        if (isPlayerA)
        {
            playerAIsShieldActive = false;
            playerAHasShield = false; // 실드 상태를 명확히 해제
        }
        else
        {
            playerBIsShieldActive = false;
            playerBHasShield = false; // 실드 상태를 명확히 해제
        }
    }

    public void SetPlayerHide(bool isHide, bool isPlayerA)
    {
        if (isPlayerA)
        {
            playerAHide = isHide; // 은신 상태 업데이트
            playerA.SetActive(!isHide); // 플레이어 오브젝트 활성화/비활성화
        }
        else
        {
            playerBHide = isHide;
            playerB.SetActive(!isHide);
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
  