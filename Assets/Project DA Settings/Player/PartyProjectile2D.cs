using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PartyProjectile2D : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] private float launchSpeed = 12f;
    [SerializeField] private float maxSpeed = 20f;

    [Header("Bounce")]
    [Tooltip("반사 후 속도 감쇠 (1 = 완전탄성)")]
    [Range(0f, 1.1f)] public float reflectDamping = 1.0f;
    [Tooltip("반사 후 최소 속도")]
    public float minReflectSpeed = 2.0f;
    [Tooltip("관통 방지용 분리 거리(법선 방향). 너무 크면 위로 밀려날 수 있음")]
    [Range(0f, 0.05f)] public float skinPushOut = 0.015f;

    [Header("Debug")]
    public bool drawDebug = false;

    // 내부 상태
    public Rigidbody2D RB { get; private set; }   // <- 공개 getter
    public PartyLauncher2D Owner { get; private set; }

    private bool launched;
    private Vector2 travelDir = Vector2.right;

    void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        RB.gravityScale = 0f;
        RB.freezeRotation = true;
        RB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void OnEnable()
    {
        // 기본 발사(필요 없으면 외부 Launch만 사용)
        Launch(travelDir, launchSpeed);
    }

    // [A] 기존 2인자 Launch(방향, 속도) — 유지
    public void Launch(Vector2 dir, float speed)
    {
        travelDir = dir.sqrMagnitude > 1e-6f ? dir.normalized : Vector2.right;
        RB.position = transform.position;
        RB.linearVelocity = travelDir * Mathf.Max(0f, speed);
        launched = true;
    }

    // [B] 런처가 호출하는 3인자 Launch(시작위치, 초기속도, 소유자)
    public void Launch(Vector2 startPos, Vector2 initialVelocity, PartyLauncher2D owner)
    {
        Owner = owner;
        RB.position = startPos;
        RB.linearVelocity = initialVelocity;
        travelDir = initialVelocity.sqrMagnitude > 1e-6f ? initialVelocity.normalized : travelDir;
        launched = true;
        gameObject.SetActive(true);
    }

    public void SetDirection(Vector2 newDir, float newSpeed = -1f)
    {
        travelDir = newDir.sqrMagnitude > 1e-6f ? newDir.normalized : travelDir;
        if (newSpeed > 0f) RB.linearVelocity = travelDir * newSpeed;
    }

    // 런처에서 사용할 안전한 설정자
    public void SetKinematic(bool enable)
    {
        RB.bodyType = enable ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        if (enable) RB.linearVelocity = Vector2.zero;
    }

    // 런처가 반환 완료 시점에 호출하는 콜백(필요 작업 정리)
    public void OnReturned()
    {
        launched = false;
        RB.linearVelocity = Vector2.zero;
        // 필요시 파티 슬롯 복귀, 이펙트, 상태 초기화 등 추가
    }

    void Update() { /* 비물리 로직만 */ }

    void FixedUpdate()
    {
        if (!launched) return;
        float spd = RB.linearVelocity.magnitude;
        if (spd > maxSpeed) RB.linearVelocity = RB.linearVelocity.normalized * maxSpeed;
    }

    void OnCollisionEnter2D(Collision2D c) { HandleWallBounce(c); }
    void OnCollisionStay2D(Collision2D c)
    {
        if (c.contactCount == 0) return;
        var cp = c.GetContact(0);
        Vector2 n = cp.normal.normalized;
        float vn = Vector2.Dot(RB.linearVelocity, n);
        if (vn < 0f) RB.linearVelocity -= vn * n; // 안쪽 성분 제거
    }

    private void HandleWallBounce(Collision2D c)
    {
        if (c.contactCount <= 0) return;

        Vector2 v = RB.linearVelocity;
        if (v.sqrMagnitude < 1e-6f) v = c.relativeVelocity;
        if (v.sqrMagnitude < 1e-6f) return;

        // 가장 정면 접점 선택
        Vector2 minusV = -v.normalized;
        ContactPoint2D best = c.GetContact(0);
        float bestDot = Vector2.Dot(minusV, best.normal);
        for (int i = 1; i < c.contactCount; i++)
        {
            var cp = c.GetContact(i);
            float d = Vector2.Dot(minusV, cp.normal);
            if (d > bestDot) { bestDot = d; best = cp; }
        }

        Vector2 n = best.normal.normalized;

        // 수직/수평 스냅
        const float snapEps = 0.15f;
        if (Mathf.Abs(n.x) > Mathf.Abs(n.y))
        {
            if (Mathf.Abs(n.x) > (1f - snapEps)) n = new Vector2(Mathf.Sign(n.x), 0f);
        }
        else
        {
            if (Mathf.Abs(n.y) > (1f - snapEps)) n = new Vector2(0f, Mathf.Sign(n.y));
        }

        // 조건부 분리
        float vn = Vector2.Dot(RB.linearVelocity, n);
        if (vn < 0f) RB.position = RB.position + n * Mathf.Min(skinPushOut, 0.02f);

        // 반사
        Vector2 dir = RB.linearVelocity.normalized;
        Vector2 reflectedDir = Vector2.Reflect(dir, n).normalized;

        float inSpeed = RB.linearVelocity.magnitude;
        float outSpeed = Mathf.Max(inSpeed * Mathf.Max(0f, reflectDamping), minReflectSpeed);

        RB.linearVelocity = reflectedDir * outSpeed;
        travelDir = RB.linearVelocity.sqrMagnitude > 1e-6f ? RB.linearVelocity.normalized : reflectedDir;

        if (drawDebug)
        {
            Debug.DrawRay(best.point, n * 0.5f, Color.green, 0.25f);
            Debug.DrawRay(best.point, RB.linearVelocity.normalized * 0.5f, Color.yellow, 0.25f);
        }
    }
}
