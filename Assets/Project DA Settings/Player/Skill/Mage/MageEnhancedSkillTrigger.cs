using UnityEngine;

[RequireComponent(typeof(MageMagicMissileSkill))]
public class MageEnhancedSkillTrigger : MonoBehaviour
{
    [Header("Skill Reference")]
    public GameObject projectilePrefab;

    public float enhancedDamageMultiplier = 1f;
    private MageMagicMissileSkill baseSkill;

    void Awake()
    {
        baseSkill = GetComponent<MageMagicMissileSkill>();
    }

    // MageCollisionRouter에서 호출됨
    public void TriggerEnhancedSkill(Collision2D c)
    {
        if (baseSkill == null) return;

        Vector2 reflectNormal = c.contacts[0].normal;
        Vector2 baseDir = Vector2.Reflect(Vector2.right, reflectNormal);

        // 매직미사일 생성 로직 (기존과 동일)
        for (int i = 0; i < baseSkill.missiles; i++)
        {
            float rand = Random.Range(-baseSkill.spreadDeg, baseSkill.spreadDeg);
            Vector2 dir = Quaternion.Euler(0, 0, rand) * baseDir;

            var go = Instantiate(baseSkill.projectilePrefab,
                                 c.contacts[0].point + reflectNormal * 0.1f,
                                 Quaternion.identity);

            var p = go.GetComponent<PartyProjectile2D>();
            if (p)
            {
                p.attackDamage = Mathf.RoundToInt(p.attackDamage * enhancedDamageMultiplier);
                p.stopOnHit = false;
                p.Launch((Vector2)c.contacts[0].point, dir * baseSkill.speed,
                         baseSkill.GetComponent<PartyLauncher2D>());
            }
            go.SetActive(true);
        }
    }
}
