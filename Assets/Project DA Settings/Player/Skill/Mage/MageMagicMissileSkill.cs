using UnityEngine;

public class MageMagicMissileSkill : MonoBehaviour, ISkill
{
    public string Name => "MageMagicMissile";
    public TriggerType Trigger => TriggerType.WallAll;
    public ConditionType Condition => ConditionType.Hit;
    [SerializeField] int conditionCount = 1;
    public int ConditionCount => conditionCount;

    [Header("Missile Spawn")]
    public GameObject projectilePrefab;   // 미사일 프리팹(아래 Projectile 스크립트가 붙어 있어야 함)
    public int missiles = 1;              // 강화 트리에서 쓰는 값 – 필요하면 늘어나도 됨
    public float spreadDeg = 0f;          // 지금은 안 쓰지만, 다른 코드가 참조하므로 남겨둠
    public float speed = 10f;             // 미사일 이동 속도
    public float spawnOffset = 0.1f;      // 벽에서 살짝 떨어진 위치

    // origin 기준으로 가장 가까운 몬스터 찾기
    MonsterInstance FindNearestMonster(Vector2 origin, float maxRadius = 999f)
    {
        MonsterInstance[] monsters = FindObjectsOfType<MonsterInstance>();
        if (monsters == null || monsters.Length == 0)
            return null;

        MonsterInstance nearest = null;
        float bestSqr = maxRadius * maxRadius;

        foreach (var m in monsters)
        {
            if (!m || !m.isActiveAndEnabled) continue;

            Vector2 pos = m.transform.position;
            float sqr = (pos - origin).sqrMagnitude;

            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                nearest = m;
            }
        }

        return nearest;
    }

    public void Execute(in SkillContext ctx)
    {
        if (!projectilePrefab || ctx.owner == null)
            return;

        // 1) 미사일 생성 위치 : 벽 충돌 지점에서 조금 떨어진 곳
        Vector2 spawnPos2D = (Vector2)ctx.point + ctx.normal * spawnOffset;
        Vector3 spawnPos3D = new Vector3(spawnPos2D.x, spawnPos2D.y, 0f);

        // 2) 그 지점 기준, 가장 가까운 몬스터 찾기
        MonsterInstance target = FindNearestMonster(spawnPos2D);
        if (target == null)
        {
            // 몬스터 없으면 그냥 안 쏨
            return;
        }

        // 3) missiles 수만큼 미사일 생성 (모두 같은 타겟으로 간다)
        for (int i = 0; i < missiles; i++)
        {
            GameObject go = Instantiate(projectilePrefab, spawnPos3D, Quaternion.identity);

            var proj = go.GetComponent<MageMagicMissileProjectile>();
            if (proj != null)
            {
                proj.Init(target.transform, speed);
            }

            go.SetActive(true);
        }
    }
}
