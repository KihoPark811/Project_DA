// MonsterInstance.cs  (monster_sprite ��� �� ��)
using UnityEngine;
using UnityEngine.UI;

public class MonsterInstance : MonoBehaviour
{
    [Header("���� ������(��������Ʈ ���� ����)")]
    public MonsterStaticData staticData;

    [Header("���� ������")]
    public uint instance_id;
    public byte monster_id;
    public uint current_hp;
    public Vector2Int position;
    public byte[] buff = new byte[5];

    [Header("UI(����)")]
    public Image hpBar;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();       // �����տ� �ִ� ��������Ʈ�� �״�� ����Ѵ�.
        if (instance_id == 0)
            instance_id = (uint)Random.Range(1, int.MaxValue);
    }

    void Start()
    {
        // �����Ϳ��� �׽�Ʈ������ ��ġ���� ���� �����ϰ� �ʱ�ȭ�Ѵ�.
        if (staticData != null && current_hp == 0)
        {
            monster_id = staticData.monster_id;
            current_hp = staticData.monster_max_hp;
            UpdateHP();
        }
    }

    
    // SpawnManager�� ȣ���ϴ� �ʱ�ȭ �Լ���.
    public void Init(uint instanceId, MonsterStaticData data, Vector2Int gridPos, Vector2 cellSize)
    {
        staticData = data;
        instance_id = instanceId;
        monster_id = data.monster_id;
        current_hp = data.monster_max_hp;
        position = gridPos;

        // �������� ��������Ʈ�� �״�� ����(���� ������ ��������Ʈ�� ��� OK).
        if (!sr) sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = GradeTint(data.monster_grade); // ��� �� ƾƮ�� ����(��ġ ������ ����)

        // �� ũ�⿡ ���� �յ� ������(ª�� �� ����)�̴�.
        float s = Mathf.Min(cellSize.x, cellSize.y);
        transform.localScale = new Vector3(s, s, 1f);

        UpdateHP();
    }

    // ��޿� ���� �� ����(����)
    Color GradeTint(byte grade)
    {
        return grade == 0 ? Color.white
             : grade == 1 ? new Color(1f, 0.9f, 0.6f)
                          : new Color(1f, 0.7f, 0.7f);
    }

    public void TakeDamage(uint damage)
    {
        // DEF�� 0~100% ������ �����Ѵ�.
        float reduceRate = Mathf.Clamp01(staticData != null ? staticData.monster_def / 100f : 0f);
        int real = Mathf.Max(1, Mathf.RoundToInt(damage * (1f - reduceRate)));

        current_hp = (uint)Mathf.Max(0, (int)current_hp - real);
        UpdateHP();
        if (current_hp == 0) Die();
    }

    void UpdateHP()
    {
        if (hpBar != null && staticData != null && staticData.monster_max_hp > 0)
            hpBar.fillAmount = (float)current_hp / staticData.monster_max_hp;
    }

    void Die() => Destroy(gameObject);
}
