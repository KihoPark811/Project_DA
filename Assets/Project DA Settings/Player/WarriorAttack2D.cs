using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class WarriorAttack2D : MonoBehaviour
{
    [Header("Damage")]
    public int baseDamage = 1;
    public float perTargetCooldown = 0.15f;  // ���� ��� ����Ÿ ����
    public bool useTrigger = true;           // ���� �ݶ��̴��� isTrigger�� true

    [Header("Filters")]
    public LayerMask monsterMask;            // ���� ���̾ Ÿ��

    // ��� ������ �ǰ� �ð�
    readonly Dictionary<Collider2D, float> _lastHitAt = new();

    void OnTriggerEnter2D(Collider2D other) { if (useTrigger) TryHit(other); }
    void OnTriggerStay2D(Collider2D other) { if (useTrigger) TryHit(other); }

    void OnCollisionEnter2D(Collision2D c) { if (!useTrigger) TryHit(c.collider); }
    void OnCollisionStay2D(Collision2D c) { if (!useTrigger) TryHit(c.collider); }

    void TryHit(Collider2D other)
    {
        // ���̾� ����
        if (((1 << other.gameObject.layer) & monsterMask) == 0) return;

        float now = Time.time;
        if (_lastHitAt.TryGetValue(other, out var t) && now - t < perTargetCooldown) return;
        _lastHitAt[other] = now;

        // ���� HP ��Ÿ�� Ȯ��(������ �ڵ� ����)
        var hp = other.GetComponentInParent<MonsterHealthRuntime>();
        if (!hp) hp = other.attachedRigidbody ? other.attachedRigidbody.GetComponent<MonsterHealthRuntime>() : other.GetComponent<MonsterHealthRuntime>();
        if (!hp) hp = other.gameObject.AddComponent<MonsterHealthRuntime>(); // �ڵ� ����

        Vector2 p = other.bounds.ClosestPoint(transform.position);
        hp.ApplyDamage(baseDamage, p, gameObject);
    }
}
