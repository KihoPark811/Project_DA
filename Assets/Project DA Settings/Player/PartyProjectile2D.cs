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

    [Header("Combat")]
    public int attackDamage = 12;          // 전사 기본 피해
    public bool stopOnHit = true;          // 맞으면 멈출지
    public int pierceCount = 0;            // 관통 횟수(전사는 0)

    [Header("Debug")]
    public bool drawDebug = false;

    // 내부 상태
    public Rigidbody2D RB { get; private set; }
    public PartyLauncher2D Owner { get; private set; }

    private bool launched;
    private Vector2 travelDir = Vector2.right;

    // 스킬 트리거 라우터
    private SkillTriggerRouter _router;

    // 🔹 추가: 자신의 Collider 캐시
    private Collider2D col;

    // 발사 중인지 외부에서 알 수 있게 프로퍼티
    public bool IsLaunched => launched;

    void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        RB.gravityScale = 0f;
        RB.freezeRotation = true;
        RB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        _router = GetComponent<SkillTriggerRouter>();
    }

    void OnEnable()
    {
        // 예전: 자동 Launch(travelDir, launchSpeed) → 문제 유발 가능
        // 지금: 활성화될 때는 "대기 상태"로 시작
        launched = false;
        RB.linearVelocity = Vector2.zero;
        if (col != null) col.enabled = false;
    }

    // [A] 기존 2인자 Launch(방향, 속도)
    public void Launch(Vector2 dir, float speed)
    {
        travelDir = dir.sqrMagnitude > 1e-6f ? dir.normalized : Vector2.right;
        RB.position = transform.position;
        RB.linearVelocity = travelDir * Mathf.Max(0f, speed);
        launched = true;
        if (col != null) col.enabled = true;   // 🔹 발사 시 충돌 켜기
    }

    // [B] 런처가 호출하는 3인자 Launch(시작위치, 초기속도, 소유자)
    public void Launch(Vector2 startPos, Vector2 initialVelocity, PartyLauncher2D owner)
    {
        Owner = owner;
        RB.position = startPos;
        RB.linearVelocity = initialVelocity;
        travelDir = initialVelocity.sqrMagnitude > 1e-6f ? initialVelocity.normalized : travelDir;
        launched = true;
        if (col != null) col.enabled = true;   // 🔹 발사 시 충돌 켜기
        gameObject.SetActive(true);
    }

    public void SetDirection(Vector2 newDir, float newSpeed = -1f)
    {
        travelDir = newDir.sqrMagnitude > 1e-6f ? newDir.normalized : travelDir;
        if (newSpeed > 0f) RB.linearVelocity = travelDir * newSpeed;
    }

    public void SetKinematic(bool enable)
    {
        RB.bodyType = enable ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        if (enable) RB.linearVelocity = Vector2.zero;
    }

    // 🔹 런처에서 "리턴 완료" 시점에 반드시 호출해주기
    public void OnReturned()
    {
        launched = false;
        RB.linearVelocity = Vector2.zero;
        if (col != null) col.enabled = false;   // 🔹 리턴 후 충돌 끄기
    }

    void Update() { /* 비물리 로직만 */ }

    void FixedUpdate()
    {
        if (!launched) return;
        float spd = RB.linearVelocity.magnitude;
        if (spd > maxSpeed) RB.linearVelocity = RB.linearVelocity.normalized * maxSpeed;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        int lay = c.collider.gameObject.layer;

        // Owner가 지정한 마스크로 충돌 대상 판별
        if (Owner && ((Owner.enemyMask.value & (1 << lay)) != 0))
        {
            HandleEnemyHit(c);     // 적 피격 처리 + 스킬 트리거
            return;
        }
        if (Owner && ((Owner.wallMask.value & (1 << lay)) != 0))
        {
            HandleWallHit(c);      // 스킬 트리거 알림 후 반사
            return;
        }

        // 그 외는 기존 반사 로직
        HandleWallBounce(c);
    }

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.contactCount == 0) return;
        var cp = c.GetContact(0);
        Vector2 n = cp.normal.normalized;
        float vn = Vector2.Dot(RB.linearVelocity, n);
        if (vn < 0f) RB.linearVelocity -= vn * n; // 안쪽 성분 제거
    }

    [SerializeField] float enemyBounceCooldown = 0.05f;
    float _lastEnemyBounceTime = -999f;

    private void HandleEnemyHit(Collision2D c)
    {
        var cp = c.GetContact(0);

        // 1) 데미지
        if (c.collider.TryGetComponent(out MonsterInstance mi))
        {
            mi.TakeDamage((uint)Mathf.Max(1, attackDamage));
            _router?.NotifyMobHit(c.collider.gameObject, cp.point);
        }

        // 2) 리코셰(리바운드)
        if (Time.time - _lastEnemyBounceTime < enemyBounceCooldown) return;
        _lastEnemyBounceTime = Time.time;

        Vector2 n = cp.normal.normalized;
        Vector2 v = RB.linearVelocity;
        float speed = v.magnitude;
        if (speed < 0.001f) speed = launchSpeed;

        RB.position = RB.position + n * Mathf.Min(skinPushOut, 0.02f);

        Vector2 r = Vector2.Reflect(v.normalized, n);
        float outSpeed = Mathf.Max(speed * Mathf.Max(0f, reflectDamping), minReflectSpeed);
        RB.linearVelocity = r * outSpeed;
        travelDir = RB.linearVelocity.normalized;

        if (pierceCount > 0)
        {
            pierceCount--;
        }

        if (stopOnHit)
            RB.linearVelocity = Vector2.zero;
    }

    private void HandleWallHit(Collision2D c)
    {
        var cp = c.GetContact(0);
        Vector2 n = cp.normal.normalized;
        bool isTop = Mathf.Abs(n.y) > 0.9f;

        _router?.NotifyWallHit(cp.point, n, isTop);
        HandleWallBounce(c);
    }

    private void HandleWallBounce(Collision2D c)
    {
        if (c.contactCount <= 0) return;

        Vector2 v = RB.linearVelocity;
        if (v.sqrMagnitude < 1e-6f) v = c.relativeVelocity;
        if (v.sqrMagnitude < 1e-6f) return;

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

        const float snapEps = 0.15f;
        if (Mathf.Abs(n.x) > Mathf.Abs(n.y))
        {
            if (Mathf.Abs(n.x) > (1f - snapEps)) n = new Vector2(Mathf.Sign(n.x), 0f);
        }
        else
        {
            if (Mathf.Abs(n.y) > (1f - snapEps)) n = new Vector2(0f, Mathf.Sign(n.y));
        }

        float vn = Vector2.Dot(RB.linearVelocity, n);
        if (vn < 0f) RB.position = RB.position + n * Mathf.Min(skinPushOut, 0.02f);

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
