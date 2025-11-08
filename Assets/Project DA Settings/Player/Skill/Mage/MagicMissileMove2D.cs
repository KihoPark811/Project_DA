using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MagicMissileMove2D : MonoBehaviour
{
    [Header("Move")]
    public float speed = 10f;            // 미사일 속도
    public float homingTurnRate = 720f;  // 초당 회전 각도(도)
    public float searchRadius = 50f;     // 타겟 탐색 반경

    [Header("Damage")]
    public int damage = 5;

    Rigidbody2D rb;
    Transform target;

    int enemyLayer;
    int wallLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        enemyLayer = LayerMask.NameToLayer("Enemy");
        wallLayer = LayerMask.NameToLayer("Wall");
    }

    void OnEnable()
    {
        // 혹시 속도가 0이면 위로 기본 속도 부여
        if (rb.linearVelocity.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = Vector2.up * speed;
        }

        AcquireTarget();
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            AcquireTarget();
            if (target == null) return; // 타겟 없으면 그냥 지금 방향 유지
        }

        Vector2 toTarget = (Vector2)target.position - rb.position;
        if (toTarget.sqrMagnitude < 0.0001f) return;

        Vector2 desiredDir = toTarget.normalized;
        Vector2 currentDir = rb.linearVelocity.sqrMagnitude > 0.001f
            ? rb.linearVelocity.normalized
            : desiredDir;

        // 이번 프레임에 회전 가능한 최대 라디안
        float maxRadians = homingTurnRate * Mathf.Deg2Rad * Time.fixedDeltaTime;

        // 🔧 여기 부분이 수정된 부분!
        // Vector3.RotateTowards 로 회전 계산 후, 2D 방향으로 다시 사용
        Vector3 cur3 = currentDir;
        Vector3 des3 = desiredDir;
        Vector2 newDir = Vector3.RotateTowards(cur3, des3, maxRadians, 0f);

        rb.linearVelocity = newDir * speed;
    }

    void AcquireTarget()
    {
        MonsterInstance[] monsters = FindObjectsOfType<MonsterInstance>();
        if (monsters == null || monsters.Length == 0)
        {
            target = null;
            return;
        }

        Transform best = null;
        float bestSqr = searchRadius * searchRadius;

        foreach (var m in monsters)
        {
            if (!m || !m.isActiveAndEnabled) continue;

            Vector2 pos = m.transform.position;
            float sqr = (pos - (Vector2)transform.position).sqrMagnitude;

            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = m.transform;
            }
        }

        target = best;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        int layer = other.gameObject.layer;

        // Enemy 충돌
        if (layer == enemyLayer)
        {
            var monster = other.GetComponent<MonsterInstance>();
            if (monster != null)
            {
                uint dmg = (uint)Mathf.Max(1, damage);
                monster.TakeDamage(dmg);
            }

            Destroy(gameObject);
            return;
        }

        // Wall 충돌 시 바로 소멸
        if (layer == wallLayer)
        {
            Destroy(gameObject);
        }
    }
}
