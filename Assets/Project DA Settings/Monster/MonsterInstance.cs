// MonsterInstance.cs  (monster_sprite 사용 안 함)
using UnityEngine;
using UnityEngine.UI;

public class MonsterInstance : MonoBehaviour
{
    [Header("정적 데이터(스프라이트 없음 가정)")]
    public MonsterStaticData staticData;

    [Header("동적 데이터")]
    public uint instance_id;
    public byte monster_id;
    public uint current_hp;
    public Vector2Int position;
    public byte[] buff = new byte[5];

    [Header("UI(선택)")]
    public Image hpBar;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();       // 프리팹에 있는 스프라이트를 그대로 사용한다.
        if (instance_id == 0)
            instance_id = (uint)Random.Range(1, int.MaxValue);
    }

    void Start()
    {
        // 에디터에서 테스트용으로 배치했을 때도 안전하게 초기화한다.
        if (staticData != null && current_hp == 0)
        {
            monster_id = staticData.monster_id;
            current_hp = staticData.monster_max_hp;
            UpdateHP();
        }
    }

    
    // SpawnManager가 호출하는 초기화 함수다.
    public void Init(uint instanceId, MonsterStaticData data, Vector2Int gridPos, Vector2 cellSize)
    {
        staticData = data;
        instance_id = instanceId;
        monster_id = data.monster_id;
        current_hp = data.monster_max_hp;
        position = gridPos;

        // 프리팹의 스프라이트를 그대로 쓴다(별도 데이터 스프라이트가 없어도 OK).
        if (!sr) sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = GradeTint(data.monster_grade); // 등급 색 틴트만 적용(원치 않으면 삭제)

        // 셀 크기에 맞춘 균등 스케일(짧은 변 기준)이다.
        float s = Mathf.Min(cellSize.x, cellSize.y);
        transform.localScale = new Vector3(s, s, 1f);

        UpdateHP();
    }

    // 등급에 따른 색 보정(선택)
    Color GradeTint(byte grade)
    {
        return grade == 0 ? Color.white
             : grade == 1 ? new Color(1f, 0.9f, 0.6f)
                          : new Color(1f, 0.7f, 0.7f);
    }

    public void TakeDamage(uint damage)
    {
        // DEF는 0~100% 비율로 가정한다.
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
