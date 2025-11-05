using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SynergyBuffOnCollision : MonoBehaviour
{
    [Header("Who am I?")]
    public bool isWarrior;   // WarriorBall 에서만 체크
    public bool isMage;      // MageBall 에서만 체크

    [Header("Buff Settings")]
    public float damageMultiplier = 2f;  // 강화 배율
    public float buffDuration = 2f;      // 유지 시간 초

    [Header("Skill Color Settings")]
    public Color warriorBuffSlashColor = Color.yellow; // 전사 슬래시 색
    public Color mageBuffMissileColor = Color.cyan;    // 마법사 미사일 색

    // 가져와서 조작할 대상들
    WarriorSlashSkill warriorSkill;
    MageEnhancedSkillTrigger mageSkill;

    // 슬래시 / 미사일 프리팹의 SpriteRenderer 캐시
    SpriteRenderer slashSR;
    SpriteRenderer missileSR;

    // 원래 색 저장용
    Color originalSlashColor;
    Color originalMissileColor;
    bool hasSlashColor;
    bool hasMissileColor;

    float buffEndTime;
    bool buffActive;

    // 🔹 추가: 발사체 상태 확인용
    PartyProjectile2D proj;

    void Awake()
    {
        // 같은 오브젝트에 붙어있는 스킬 컴포넌트들 가져오기
        warriorSkill = GetComponent<WarriorSlashSkill>();
        mageSkill = GetComponent<MageEnhancedSkillTrigger>();
        proj = GetComponent<PartyProjectile2D>();   // 🔹 추가

        // 전사 슬래시 프리팹에서 SpriteRenderer 찾아서 원래 색 저장
        if (warriorSkill != null && warriorSkill.slashPrefab != null)
        {
            slashSR = warriorSkill.slashPrefab.GetComponentInChildren<SpriteRenderer>();
            if (slashSR != null)
            {
                originalSlashColor = slashSR.color;
                hasSlashColor = true;
            }
        }

        // 마법사 미사일 프리팹에서 SpriteRenderer 찾아서 원래 색 저장
        // (MageEnhancedSkillTrigger 대신 MageMagicMissileSkill을 쓰고 있다면,
        //  여기 코드를 그쪽으로 맞춰줘야 함)
        /*
        MageMagicMissileSkill missileSkill = GetComponent<MageMagicMissileSkill>();
        if (missileSkill != null && missileSkill.projectilePrefab != null)
        {
            missileSR = missileSkill.projectilePrefab.GetComponentInChildren<SpriteRenderer>();
            if (missileSR != null)
            {
                originalMissileColor = missileSR.color;
                hasMissileColor      = true;
            }
        }
        */
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 🔹 발사 중이 아닐 때(리턴/대기 상태)는 시너지 발동 X
        if (proj != null && !proj.IsLaunched)
            return;

        // 같은 Layer(Party)끼리만 반응 = WarriorBall <-> MageBall
        if (collision.gameObject.layer != gameObject.layer)
            return;

        // 여기까지 왔다 = 캐릭터끼리 부딪힌 상황
        ActivateBuff();
    }

    void ActivateBuff()
    {
        buffActive = true;
        buffEndTime = Time.time + buffDuration;
        ApplyBuff();   // 바로 한 번 적용
    }

    void Update()
    {
        if (!buffActive) return;

        if (Time.time >= buffEndTime)
        {
            buffActive = false;
            ClearBuff();
        }
    }

    // 실제 강화 적용 (데미지 + 스킬 프리팹 색)
    void ApplyBuff()
    {
        if (isWarrior && warriorSkill != null)
            warriorSkill.damageMultiplier = damageMultiplier;

        if (isMage && mageSkill != null)
            mageSkill.enhancedDamageMultiplier = damageMultiplier;

        if (hasSlashColor && slashSR != null)
            slashSR.color = warriorBuffSlashColor;

        if (hasMissileColor && missileSR != null)
            missileSR.color = mageBuffMissileColor;
    }

    // 버프 해제 (배율 + 프리팹 색 원상복구)
    void ClearBuff()
    {
        if (isWarrior && warriorSkill != null)
            warriorSkill.damageMultiplier = 1f;

        if (isMage && mageSkill != null)
            mageSkill.enhancedDamageMultiplier = 1f;

        if (hasSlashColor && slashSR != null)
            slashSR.color = originalSlashColor;

        if (hasMissileColor && missileSR != null)
            missileSR.color = originalMissileColor;
    }
}
