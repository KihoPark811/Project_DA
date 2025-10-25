using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class WarriorAttack2D : MonoBehaviour
{
    [Header("Damage")]
    public int baseDamage = 1;
    public float perTargetCooldown = 0.15f;  // 같은 대상 연속타 제한
    public bool useTrigger = true;           // 공격 콜라이더가 isTrigger면 true

    [Header("Filters")]
    public LayerMask monsterMask;            // 몬스터 레이어만 타격

    // 대상별 마지막 피격 시각
    readonly Dictionary<Collider2D, float> _lastHitAt = new();

    void OnTriggerEnter2D(Collider2D other) { if (useTrigger) TryHit(other); }
    void OnTriggerStay2D(Collider2D other) { if (useTrigger) TryHit(other); }

    void OnCollisionEnter2D(Collision2D c) { if (!useTrigger) TryHit(c.collider); }
    void OnCollisionStay2D(Collision2D c) { if (!useTrigger) TryHit(c.collider); }

    void TryHit(Collider2D other)
    {
        // 레이어 필터
        if (((1 << other.gameObject.layer) & monsterMask) == 0) return;

        float now = Time.time;
        if (_lastHitAt.TryGetValue(other, out var t) && now - t < perTargetCooldown) return;
        _lastHitAt[other] = now;

        // 몬스터 HP 런타임 확보(없으면 자동 부착)
        var hp = other.GetComponentInParent<MonsterHealthRuntime>();
        if (!hp) hp = other.attachedRigidbody ? other.attachedRigidbody.GetComponent<MonsterHealthRuntime>() : other.GetComponent<MonsterHealthRuntime>();
        if (!hp) hp = other.gameObject.AddComponent<MonsterHealthRuntime>(); // 자동 장착

        Vector2 p = other.bounds.ClosestPoint(transform.position);
        hp.ApplyDamage(baseDamage, p, gameObject);
    }
}
