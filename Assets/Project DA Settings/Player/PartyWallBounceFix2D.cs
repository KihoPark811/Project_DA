using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PartyWallBounceFix2D : MonoBehaviour
{
    public LayerMask wallMask;        // 일반 벽
    public LayerMask enemyMask;       // 적(부딪혀도 반사 X 또는 별도 처리)
    public LayerMask returnWallMask;  // ⬅️ 아래벽 전용 (새로 추가)

    public float skinPushOut = 0.04f;
    public float minReflectSpeed = 2.0f;
    [Range(0.9f, 1f)] public float reflectDamping = 0.98f;

    Rigidbody2D rb;
    PartyProjectile2D proj;           // ⬅️ 캐시

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        proj = GetComponent<PartyProjectile2D>();
    }

    void OnCollisionEnter2D(Collision2D c) => HandleBounce(c);
    void OnCollisionStay2D(Collision2D c) => HandleBounce(c);

    void HandleBounce(Collision2D c)
    {
        // 리턴 중(키네마틱)이면 무시
        if (rb.bodyType == RigidbodyType2D.Kinematic) return;

        int lay = c.collider.gameObject.layer;

        // 0) 아래벽에 닿으면: 반사하지 말고 즉시 리턴
        if (((1 << lay) & returnWallMask) != 0)
        {
            if (proj != null && proj.Owner != null)
                proj.Owner.RequestReturn(proj);
            return; // 여기서 끝 (반사 X)
        }

        // 1) 적에 닿으면: 반사 로직 스킵(원래 하던 정책 유지)
        if (((1 << lay) & enemyMask) != 0) return;

        // 2) 일반 벽이 아니면 무시
        if (((1 << lay) & wallMask) == 0) return;

        // 3) 여기부터는 기존 반사 처리
        Vector2 n = Vector2.zero;
        for (int i = 0; i < c.contactCount; i++) n += c.GetContact(i).normal;
        if (n.sqrMagnitude < 1e-6f) return;
        n.Normalize();

        Vector2 v = rb.linearVelocity;
        if (Vector2.Dot(v, n) >= 0f) return;

        rb.position += n * skinPushOut;

        float speed = v.magnitude;
        Vector2 dir = (speed > 1e-4f) ? (v / speed) : (-n);

        Vector2 reflected = Vector2.Reflect(dir, n);
        float outSpeed = Mathf.Max(speed * reflectDamping, minReflectSpeed);
        rb.linearVelocity = reflected * outSpeed;
    }
}
