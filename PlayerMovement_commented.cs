using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

/// <summary>
/// 2D 플레이어 캐릭터의 움직임을 제어하는 클래스
/// 좌우 이동, 점프, 중력 적용, 충돌 처리 등을 담당
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // 컴포넌트 참조
    private new Camera camera;          // 메인 카메라 참조
    private new Rigidbody2D rigidbody;  // 물리 연산을 위한 리지드바디
    private new Collider2D collider;    // 충돌 검사를 위한 콜라이더

    // 움직임 관련 변수
    private Vector2 velocity;           // 현재 속도 벡터
    private float inputAxis;            // 수평 입력 축 (-1 ~ 1)

    // 움직임 설정 변수들
    public float moveSpeed = 8f;        // 좌우 이동 속도
    public float maxJumpHeight = 5f;    // 최대 점프 높이
    public float maxJumpTime = 1f;      // 최대 점프 시간

    // 점프 힘 계산 (물리 공식 기반)
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    
    // 중력 계산 (물리 공식 기반)
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    // 상태 확인 프로퍼티들
    public bool grounded { get; private set; }  // 바닥에 닿아있는지 여부
    public bool jumping { get; private set; }   // 점프 중인지 여부
    
    // 달리고 있는지 여부 (속도나 입력이 임계값 이상일 때)
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    
    // 미끄러지고 있는지 여부 (입력과 속도 방향이 반대일 때)
    public bool sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);
    
    // 떨어지고 있는지 여부 (아래로 움직이고 있고 바닥에 닿지 않았을 때)
    public bool falling => velocity.y < 0f && !grounded;
    
    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        camera = Camera.main;
        collider = GetComponent<Collider2D>();
    }

    /// <summary>
    /// 오브젝트가 활성화될 때 호출
    /// 물리 시뮬레이션 활성화 및 초기 상태 설정
    /// </summary>
    private void OnEnable()
    {
        rigidbody.isKinematic = false;  // 물리 시뮬레이션 활성화
        collider.enabled = true;        // 콜라이더 활성화
        velocity = Vector2.zero;        // 속도 초기화
        jumping = false;                // 점프 상태 초기화
    }

    /// <summary>
    /// 오브젝트가 비활성화될 때 호출
    /// 물리 시뮬레이션 비활성화 및 상태 초기화
    /// </summary>
    private void OnDisable()
    {
        rigidbody.isKinematic = true;   // 물리 시뮬레이션 비활성화
        collider.enabled = false;       // 콜라이더 비활성화
        velocity = Vector2.zero;        // 속도 초기화
        inputAxis = 0f;                 // 입력 초기화
        jumping = false;                // 점프 상태 초기화
    }

    /// <summary>
    /// 매 프레임 호출되는 메인 업데이트 함수
    /// 입력 처리, 바닥 체크, 움직임 및 중력 적용
    /// </summary>
    private void Update()
    {
        HorizontalMovemnet();  // 좌우 움직임 처리

        // 바닥에 닿았는지 레이캐스트로 확인
        grounded = rigidbody.Raycast(Vector2.down);

        // 바닥에 닿았을 때만 바닥 움직임 처리
        if (grounded)
        {
            GroundedMovement();
        }

        ApplyGravity();  // 중력 적용
    }

    /// <summary>
    /// 좌우 이동 처리
    /// 입력 받기, 속도 적용, 벽 충돌 체크, 캐릭터 방향 전환
    /// </summary>
    private void HorizontalMovemnet()
    {
        // 좌우 입력 받기 (-1, 0, 1)
        inputAxis = Input.GetAxisRaw("Horizontal");
        
        // 목표 속도까지 부드럽게 가속/감속
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);

        // 벽 충돌 체크 - 이동 방향으로 레이캐스트
        if(rigidbody.Raycast(Vector2.right * velocity.x))
        {
            velocity.x = 0f;  // 벽에 부딪히면 수평 속도 0
        }

        // 캐릭터 방향 전환 (스프라이트 뒤집기)
        if(velocity.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;  // 오른쪽 향함
        }
        else if (velocity.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);  // 왼쪽 향함
        }
    }

    /// <summary>
    /// 바닥에 닿았을 때의 움직임 처리
    /// 점프 입력 처리 및 수직 속도 조정
    /// </summary>
    private void GroundedMovement()
    {
        // 바닥에 닿았을 때 아래로 가는 속도 제거
        velocity.y = Mathf.Max(velocity.y, 0f);

        // 위로 움직이고 있으면 점프 중으로 판단
        jumping = velocity.y > 0f;

        // 점프 입력 처리
        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;  // 점프 힘 적용
            jumping = true;          // 점프 상태 설정
        }
    }

    /// <summary>
    /// 중력 적용 및 점프 높이 조절
    /// 떨어질 때와 올라갈 때 다른 중력 적용
    /// </summary>
    private void ApplyGravity()
    {
        // 떨어지고 있거나 점프 버튼을 놓았을 때
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        
        // 떨어질 때는 중력을 2배로 적용 (더 빠른 낙하)
        float multiplier = falling ? 2f : 1f;

        // 중력 적용
        velocity.y += gravity * multiplier * Time.deltaTime;
        
        // 최대 낙하 속도 제한
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    /// <summary>
    /// 고정된 간격으로 호출되는 물리 업데이트
    /// 실제 위치 이동 및 화면 경계 제한
    /// </summary>
    private void FixedUpdate()
    {
        // 현재 위치 가져오기
        Vector2 position = rigidbody.position;
        
        // 속도에 따른 위치 업데이트
        position += velocity * Time.fixedDeltaTime;

        // 화면 경계 계산
        Vector2 leftEdge = camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        
        // 화면 경계 내로 위치 제한 (여유 공간 0.5f)
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        // 리지드바디 위치 업데이트
        rigidbody.MovePosition(position);
    }

    /// <summary>
    /// 충돌 시작 시 호출되는 함수
    /// 적과의 충돌 처리 및 기타 오브젝트와의 상호작용
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 적과 충돌했을 때
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 적을 위에서 밟았는지 확인
            if (transform.DotTest(collision.transform, Vector2.down))
            {
                velocity.y = jumpForce / 2f;  // 작은 점프 (적 밟기)
                jumping = true;
            }
        }
        // 파워업이 아닌 다른 오브젝트와 충돌했을 때
        else if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            // 아래에서 위로 충돌했을 때 (바닥이나 플랫폼)
            if (transform.DotTest(collision.transform, Vector2.up))
            {
                velocity.y = 0f;  // 수직 속도 제거 (착지)
            }
        }
    }
}
