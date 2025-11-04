using UnityEngine;

public class MageMagicMissileSkill : MonoBehaviour, ISkill
{
    public string Name => "MageMagicMissile";
    public TriggerType Trigger => TriggerType.WallAll;      // 벽 충돌 시
    public ConditionType Condition => ConditionType.Hit;    // 히트 기준
    [SerializeField] int conditionCount = 1;
    public int ConditionCount => conditionCount;

    [Header("Missile Spawn")]
    public GameObject projectilePrefab;   // ← MagicMissile 프리팹
    public int missiles = 2;              // 벽 1번 충돌당 생성 수
    public float spreadDeg = 15f;         // ±15도 무작위
    public float speed = 10f;             // 미사일 속도
    public float spawnOffset = 0.05f;     // 벽에서 조금 띄워 스폰

    public void Execute(in SkillContext ctx)
    {
        if (!projectilePrefab || ctx.owner == null) return;

        // 벽 반사 방향을 기준으로 '앞을 향한' 기본 방향
        var ownerVel = ctx.owner.RB.linearVelocity;
        Vector2 baseDir = ownerVel.sqrMagnitude > 1e-6f
            ? Vector2.Reflect(ownerVel.normalized, ctx.normal)
            : (ctx.normal == Vector2.zero ? Vector2.right : Vector2.Reflect(Vector2.right, ctx.normal));

        Vector3 spawnPos = new Vector3(ctx.point.x, ctx.point.y, 0f) + (Vector3)(ctx.normal * spawnOffset);

        for (int i = 0; i < missiles; i++)
        {
            float rand = Random.Range(-spreadDeg, spreadDeg);
            Vector2 dir = Quaternion.Euler(0, 0, rand) * baseDir;

            var go = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            var p = go.GetComponent<PartyProjectile2D>();
            p.attackDamage = Mathf.Max(1, ctx.owner.attackDamage);     // 데미지는 원본과 맞춤
            p.stopOnHit = false;                                       // 관통형
            // 관통 횟수는 프리fab의 PartyProjectile2D.pierceCount 값을 사용(2)

            // 미사일은 런처(Owner) 마스크를 그대로 써도 되고, null로 둬도 동작함
            p.Launch((Vector2)spawnPos, dir * speed, ctx.owner.Owner);
            go.SetActive(true);
        }
    }
}
