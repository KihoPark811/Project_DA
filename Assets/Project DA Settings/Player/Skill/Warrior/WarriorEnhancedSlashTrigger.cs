using UnityEngine;

[RequireComponent(typeof(WarriorSlashSkill))]
public class WarriorEnhancedSlashTrigger : MonoBehaviour
{
    [Header("Mage Detect")]
    [Tooltip("마법사 캐릭터에 붙어 있는 태그 이름")]
    public string mageTag = "Mage";

    [Header("Enhanced Slash Settings")]
    [Tooltip("색이 다른 강화 슬래시 프리팹")]
    public GameObject enhancedSlashPrefab;   // 색 다른 슬래시 이펙트 프리팹

    [Tooltip("기본 슬래시보다 데미지 몇 배?")]
    public float enhancedDamageMultiplier = 2f;

    [Tooltip("기본 슬래시보다 범위 몇 배?")]
    public float enhancedRadiusMultiplier = 1.5f;

    private WarriorSlashSkill slashSkill;

    void Awake()
    {
        slashSkill = GetComponent<WarriorSlashSkill>();
    }

    // 전사에 붙은 Rigidbody2D + Collider2D 기준
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 마법사와 부딪힌 경우만 처리
        if (!collision.collider.CompareTag(mageTag))
            return;

        TriggerEnhancedSlash(collision);
    }

    private void TriggerEnhancedSlash(Collision2D c)
    {
        if (slashSkill == null)
            return;

        // 사용할 프리팹(없으면 기본 슬래시라도 사용)
        GameObject prefab = enhancedSlashPrefab != null
            ? enhancedSlashPrefab
            : slashSkill.slashPrefab;

        if (prefab == null)
            return;

        // 충돌 지점 + 충돌면(법선) 방향으로 오프셋
        ContactPoint2D contact = c.contacts[0];
        Vector2 normal = contact.normal;
        Vector2 dir2 = (normal == Vector2.zero) ? Vector2.right : normal;

        Vector3 spawnPos = new Vector3(contact.point.x, contact.point.y, 0f)
                         + (Vector3)dir2.normalized * slashSkill.offset;

        // 슬래시 이펙트 생성
        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Hitbox 설정
        if (go.TryGetComponent(out HitboxArea2D hb))
        {
            // 기본 데미지 × 기존 multiplier × 강화 multiplier
            float dmg = slashSkill.baseDamage * slashSkill.damageMultiplier * enhancedDamageMultiplier;
            hb.damage = Mathf.RoundToInt(dmg);

            // 기본 radius × 강화 배수
            hb.radius = slashSkill.radius * enhancedRadiusMultiplier;
            Debug.Log($"[Warrior Enhanced Skill] 강화 슬래시 발동! 피해량: {hb.damage}, 범위: {hb.radius}");


            // Enemy 레이어 마스크 (기본값)
            var mask = LayerMask.GetMask("Enemy");
            hb.enemyMask = mask;
        }
    }
}
