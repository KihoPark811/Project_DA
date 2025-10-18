// 유니티 엔진의 기능(컴포넌트, 물리, 수학 등)을 쓰기 위해 반드시 포함하는 네임스페이스
using UnityEngine;

// 본 스크립트를 사용하려면 Rigidbody2D와 Collider2D가 반드시 필요하다.
// 만약 없다면 자동으로 추가해준다.
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]

//유니티 컴포넌트 스크립트의 기본 형태
public class WallBounceReflect : MonoBehaviour
{
    // speed는 이동 속도의 크기(스칼라)이다.
    // SerializeField 덕분에 private처럼 감추면서도 인스펙터에서 값 조절이 가능하다.
    // Range는 인스펙터에 슬라이더 UI를 만들어 1~20 사이로만 바꾸게 한다.
    [SerializeField][Range(1f, 20f)] float speed = 10f;

    // 유니티 2D 물리의 핵심.속도·질량·힘을 가진다.
    Rigidbody2D rb;
    // 현재 진행 방향을 저장하는 단위 벡터(길이=1)다.
    // 왜 단위 벡터인가? → 속도 크기와 방향을 분리해서 velocity = 방향 × 속도크기로 깔끔하게 관리하기 위함이다.
    Vector2 moveDir;

    // Awake는 컴포넌트가 활성화될 때 가장 먼저 호출된다.(유니티 생명주기 참고)
    void Awake()
    {
        // GetComponent<Rigidbody2D>(): 같은 오브젝트에 붙은 Rigidbody2D를 찾아서 변수에 보관한다.
        rb = GetComponent<Rigidbody2D>();
        // gravityScale = 0: 브릭브레이커/탑다운류 게임은 중력이 필요 없으니 0으로 꺼 둔다.
        rb.gravityScale = 0;          // 중력 제거 (탑다운/벽돌깨기 느낌)
        // collisionDetectionMode = Continuous: 빠르게 움직일 때 콜라이더를 관통하는 문제를 줄인다. (기본 Discrete보다 충돌 검사 빈도가 높다)
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // Start는 Awake 뒤, 첫 프레임에 한 번 호출된다. 실제 “출발 상태”를 만든다.
    void Start()
    {
        // 시작 방향 랜덤
        // Random.Range(-1, 1): X/Y를 각각 -1~1 사이에서 뽑아 랜덤 방향을 만든다.
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        // .normalized: 벡터의 길이를 1로 만든다(단위 벡터). 방향만 유지하고 크기는 버린다.
        moveDir = new Vector2(randomX, randomY).normalized;

        // 위치를 직접 바꾸지 말고(transform.position += … 금지), 리지드바디의 속도로 움직여야 유니티 물리와 충돌이 정상 동작한다.
        // 속도 벡터 = 방향(단위) × 속도 크기
        rb.linearVelocity = moveDir * speed;
    }

    // OnCollisionEnter2D는 이 오브젝트의 Collider2D가 IsTrigger=false이고, 다른 콜라이더와 물리 충돌했을 때 한 번 호출된다.
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 첫 번째 접점에서 법선 벡터 가져오기
        // collision.contacts[0].normal
        // 법선(normal)은 충돌 표면에 수직인 방향 벡터다.
        // 예: 위쪽을 향한 바닥의 법선은(0,1), 오른쪽 벽의 법선은(-1, 0).
        Vector2 normal = collision.contacts[0].normal;

        // 현재 진행 방향과 충돌 법선으로 반사 벡터 계산
        // Vector2.Reflect(입사벡터, 법선)
        // 수학 원리: 반사 벡터 = v - 2 * dot(v, n) * n
        // 여기서 v는 입사 방향, n은 단위 법선이다.
        // 이 함수가 위 공식을 대신 계산해 입사각 = 반사각을 만족하는 새 방향을 돌려준다.
        // 다시 .normalized로 방향만 단위화
        moveDir = Vector2.Reflect(moveDir, normal).normalized;

        // 속도 갱신
        // rb.velocity = moveDir * speed;로 속도 크기는 유지한 채 방향만 교체한다.
        rb.linearVelocity = moveDir * speed;
    }
}
