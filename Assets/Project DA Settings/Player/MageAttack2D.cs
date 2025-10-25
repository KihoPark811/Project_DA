using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class MageAttack2D : MonoBehaviour
{
    [Header("Damage")]
    public int baseDamage = 1;
    [Range(0f, 10f)] public float skillScale = 1.0f; // 스킬 배율(스킬마다 교체)
    [Range(0f, 1f)] public float critChance = 0.0f; // 크리 확률
    public float critMultiplier = 1.5f;
    public float perTargetCooldown = 0.2f;
    public bool useTrigger = true;

    [Header("Filters")]
    public LayerMask monsterMask;

    readonly Dictionary<Collider2D, float> _lastHitAt = new();

    void OnTriggerEnter2D(Collider2D other) { if (useTrigger) TryHit(other); }
    void OnTriggerStay2D(Collider2D other) { if (useTrigger) TryHit(other); }

    void OnCollisionEnter2D(Collision2D c) { if (!useTrigger) TryHit(c.collider); }
    void OnCollisionStay2D(Collision2D c) { if (!useTrigger) TryHit(c.collider); }

    int ComputeDamage()
    {
        float d = baseDamage * Mathf.Max(0f, skillScale);
        if (critChance > 0f && Random.value < critChance) d *= Mathf.Max(1f, critMultiplier);
        return Mathf.Max(0, Mathf.RoundToInt(d));
    }

    void TryHit(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & monsterMask) == 0) return;

        float now = Time.time;
        if (_lastHitAt.TryGetValue(other, out var t) && now - t < perTargetCooldown) return;
        _lastHitAt[other] = now;

        var hp = other.GetComponentInParent<MonsterHealthRuntime>();
        if (!hp) hp = other.attachedRigidbody ? other.attachedRigidbody.GetComponent<MonsterHealthRuntime>() : other.GetComponent<MonsterHealthRuntime>();
        if (!hp) hp = other.gameObject.AddComponent<MonsterHealthRuntime>(); // 자동 장착

        Vector2 p = other.bounds.ClosestPoint(transform.position);
        hp.ApplyDamage(ComputeDamage(), p, gameObject);
    }
}
