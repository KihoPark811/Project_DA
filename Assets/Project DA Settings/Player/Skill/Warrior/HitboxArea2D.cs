using UnityEngine;
public class HitboxArea2D : MonoBehaviour
{
    public float radius = 1.2f;
    public int damage = 10;
    public LayerMask enemyMask;
    public float life = 0.15f; // 이펙트 수명

    void OnEnable()
    {
        // 즉시 판정 1회
        var hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        foreach (var h in hits)
            if (h.TryGetComponent(out MonsterInstance m))
                m.TakeDamage((uint)Mathf.Max(1, damage));
        Invoke(nameof(Die), life);
    }
    void Die() => Destroy(gameObject);
    void OnDrawGizmosSelected() { Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, radius); }
}
