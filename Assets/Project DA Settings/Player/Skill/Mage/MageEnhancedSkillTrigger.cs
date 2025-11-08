using UnityEngine;

[RequireComponent(typeof(MageMagicMissileSkill))]
public class MageEnhancedSkillTrigger : MonoBehaviour
{
    [Header("Enhanced Magic Missile")]
    [Tooltip("강화 마법탄 프리팹 (색깔만 다른 버전)")]
    public GameObject projectilePrefab;           // ← 강화 전용 프리팹 배치

    [Tooltip("기본 미사일 대비 데미지 배수")]
    public float enhancedDamageMultiplier = 2f;   // 2배 데미지 기본값

    private MageMagicMissileSkill baseSkill;

    void Awake()
    {
        baseSkill = GetComponent<MageMagicMissileSkill>();
    }

    // ★ origin 기준으로 가장 가까운 몬스터 찾기
    private MonsterInstance FindNearestMonster(Vector2 origin, float maxRadius = 999f)
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

    // MageCollisionRouter에서 전사-마법사 충돌 시 호출된다고 가정
    public void TriggerEnhancedSkill(Collision2D c)
    {
        if (baseSkill == null) return;
        if (!projectilePrefab) return;   // 강화 프리팹이 비어 있으면 아무 것도 안 함

        // 1) 두 캐릭터가 부딪힌 지점 근처에서 스폰
        Vector2 spawnPos2D = c.contacts[0].point;
        Vector3 spawnPos3D = new Vector3(spawnPos2D.x, spawnPos2D.y, 0f);

        // 2) 그 지점 기준 가장 가까운 몬스터 찾기
        MonsterInstance target = FindNearestMonster(spawnPos2D);
        if (target == null) return;      // 몬스터 없으면 발사 안 함

        // 3) missiles 수만큼 강화 미사일 생성
        for (int i = 0; i < baseSkill.missiles; i++)
        {
            GameObject go = Instantiate(projectilePrefab, spawnPos3D, Quaternion.identity);

            var proj = go.GetComponent<MageMagicMissileProjectile>();
            if (proj != null)
            {
                // 기본 미사일보다 강하게
                proj.damage = Mathf.RoundToInt(proj.damage * enhancedDamageMultiplier);

                // 기본 스킬과 같은 속도로, 가장 가까운 몬스터에게 유도
                proj.Init(target.transform, baseSkill.speed);

                Debug.Log($"[Mage Enhanced Skill] 강화 미사일 발사! 피해량: {proj.damage}");

            }

            go.SetActive(true);
        }
    }
}
