using UnityEngine;
using UnityEngine.UI;   // �� HP_Fill Image �����


/// ���� �����տ� �̸� �ٿ��� �ǰ�, ������ �÷��̾��� ���� ������Ʈ�� ��Ÿ�ӿ� �ڵ����� AddComponent �մϴ�.
/// MonsterStaticData�� ���� ��� �ִ�ü���� ������ ����, ������ fallbackMaxHp ���.
[DisallowMultipleComponent]
public class MonsterHealthRuntime : MonoBehaviour
{
    [Header("Static (Optional)")]
    public MonsterStaticData staticData;   // ������ �� �� ��� (��� ��)
    public uint fallbackMaxHp = 3;         // staticData ���� �� �⺻ �ִ�ü��

    [Header("State")]
    public uint currentHp;
    public uint maxHp;

    [Header("Behaviour")]
    public bool disableOnDeath = true;     // true�� SetActive(false), false�� Destroy

    void Awake()
    {
        if (maxHp == 0)
        {
            maxHp = staticData ? staticData.monster_max_hp : fallbackMaxHp;
        }
        if (currentHp == 0 || currentHp > maxHp) currentHp = maxHp;

        // �߰����� ������ ���� ����
        if (maxHp == 0) maxHp = staticData ? staticData.monster_max_hp : fallbackMaxHp;
        if (currentHp == 0 || currentHp > maxHp) currentHp = maxHp;

        // hpFill ������ �� �ڵ� Ž��(���� ���� �״�� ���)
        if (!hpFill)
            hpFill = transform.Find("HP_Canvas/HP_Fill")?.GetComponent<Image>();

        UpdateBar();   // �� ���� �� ������ �ݿ�
    }

    // �߰����� ������ ���� ����
    [SerializeField] private Image hpFill;  // Inspector���� HP_Fill �巡��(������ �ڵ� Ž��)

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

        // ���� ������ �ʿ��ϸ� �Ʒ� �� �ٷ� ���� ���� ����:
        // byte def = staticData ? staticData.monster_def : (byte)0;
        // float scale = Mathf.Clamp01(1f - def * 0.01f);
        // dmg = (uint)Mathf.Max(0, Mathf.RoundToInt(amount * scale));

        currentHp = (dmg >= currentHp) ? 0u : currentHp - dmg;

        // ������ ���� ����
        UpdateBar();

        // TODO: ��Ʈ ����Ʈ/����/���� �� ���� ��

        if (currentHp == 0)
        {
            if (disableOnDeath) gameObject.SetActive(false);
            else Destroy(gameObject);
        }
    }
}
