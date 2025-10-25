using UnityEngine;
using UnityEngine.UI;   // ← HP_Fill Image 제어용


/// 몬스터 프리팹에 미리 붙여도 되고, 없으면 플레이어의 공격 컴포넌트가 런타임에 자동으로 AddComponent 합니다.
/// MonsterStaticData가 있을 경우 최대체력을 가져다 쓰고, 없으면 fallbackMaxHp 사용.
[DisallowMultipleComponent]
public class MonsterHealthRuntime : MonoBehaviour
{
    [Header("Static (Optional)")]
    public MonsterStaticData staticData;   // 있으면 이 값 사용 (없어도 됨)
    public uint fallbackMaxHp = 3;         // staticData 없을 때 기본 최대체력

    [Header("State")]
    public uint currentHp;
    public uint maxHp;

    [Header("Behaviour")]
    public bool disableOnDeath = true;     // true면 SetActive(false), false면 Destroy

    void Awake()
    {
        if (maxHp == 0)
        {
            maxHp = staticData ? staticData.monster_max_hp : fallbackMaxHp;
        }
        if (currentHp == 0 || currentHp > maxHp) currentHp = maxHp;

        // 추가본임 삭제할 수도 있음
        if (maxHp == 0) maxHp = staticData ? staticData.monster_max_hp : fallbackMaxHp;
        if (currentHp == 0 || currentHp > maxHp) currentHp = maxHp;

        // hpFill 미지정 시 자동 탐색(현재 계층 그대로 사용)
        if (!hpFill)
            hpFill = transform.Find("HP_Canvas/HP_Fill")?.GetComponent<Image>();

        UpdateBar();   // ← 시작 시 게이지 반영
    }

    // 추가본임 삭제할 수도 있음
    [SerializeField] private Image hpFill;  // Inspector에서 HP_Fill 드래그(없으면 자동 탐색)

    private void UpdateBar()
    {
        if (!hpFill) return;
        float f = (maxHp == 0) ? 0f : Mathf.Clamp01((float)currentHp / maxHp);
        hpFill.fillAmount = f;
    }


    public void ApplyDamage(int amount, Vector2 hitPoint, GameObject source = null)
    {
        if (currentHp == 0) return;
        uint dmg = (uint)Mathf.Max(0, amount);

        // 방어력 적용이 필요하면 아래 한 줄로 쉽게 보정 가능:
        // byte def = staticData ? staticData.monster_def : (byte)0;
        // float scale = Mathf.Clamp01(1f - def * 0.01f);
        // dmg = (uint)Mathf.Max(0, Mathf.RoundToInt(amount * scale));

        currentHp = (dmg >= currentHp) ? 0u : currentHp - dmg;

        // 삭제할 수도 있음
        UpdateBar();

        // TODO: 히트 이펙트/사운드/경직 등 연출 훅

        if (currentHp == 0)
        {
            if (disableOnDeath) gameObject.SetActive(false);
            else Destroy(gameObject);
        }
    }
}
