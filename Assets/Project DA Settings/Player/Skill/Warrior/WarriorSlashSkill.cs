using UnityEngine;

public class WarriorSlashSkill : MonoBehaviour, ISkill
{
    public string Name => "WarriorSlash";
    public TriggerType Trigger => TriggerType.Mob;          // 몹 충돌 시
    public ConditionType Condition => ConditionType.Hit;    // 히트 기준
    [SerializeField] int conditionCount = 1;                 // 1회마다
    public int ConditionCount => conditionCount;

    [Header("Damage Settings")]
    [HideInInspector] 
    public float damageMultiplier = 1f;

    [Header("Slash")]
    public GameObject slashPrefab; // HitboxArea2D가 붙은 프리팹
    public float offset = 0.6f;    // 충돌지점에서 살짝 전방
    public int baseDamage = 12;
    public float radius = 1.2f;

    public void Execute(in SkillContext ctx)
    {
        if (!slashPrefab) return;

        // 충돌면(법선) 방향으로 오프셋
        Vector2 dir2 = (ctx.normal == Vector2.zero) ? Vector2.right : ctx.normal;

        // ★ 모든 좌표 연산을 Vector3로 통일
        Vector3 spawnPos = new Vector3(ctx.point.x, ctx.point.y, 0f)
                         + new Vector3(dir2.normalized.x, dir2.normalized.y, 0f) * offset;

        var go = Instantiate(slashPrefab, spawnPos, Quaternion.identity);

        // 범위/데미지 세팅
        if (go.TryGetComponent(out HitboxArea2D hb))
        {
            hb.damage = (int)(baseDamage * damageMultiplier);
            // hb.damage = baseDamage;
            hb.radius = radius;
            Debug.Log($"[Warrior Basic Skill] 슬래시 발동! 피해량: {hb.damage}, 범위: {hb.radius}");

            // Enemy 레이어 마스크(런처에서 가져오기)
            var mask = LayerMask.GetMask("Enemy");
            if (ctx.owner && ctx.owner.Owner != null)
                mask = ctx.owner.Owner.enemyMask;
            hb.enemyMask = mask;
        }
    }

}
