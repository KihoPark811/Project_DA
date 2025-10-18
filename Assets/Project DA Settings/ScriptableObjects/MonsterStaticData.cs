using UnityEngine;

[CreateAssetMenu(menuName = "DA/Monster Static Data")]
public class MonsterStaticData : ScriptableObject
{
    [Header("Identification")]
    [Range(0, 255)] public byte monster_id;      // 표: monster_id
    public string monster_name;                 // 표: monster_name
    [TextArea] public string monster_guide;     // 표: monster_guide (설명)

    [Header("Stats")]
    [Range(0, 2)] public byte monster_grade;     // 0=normal,1=elite,2=boss
    public uint monster_max_hp;                 // 표: monster_max_hp
    [Range(0, 100)] public byte monster_def;     // 표: monster_def

    [Header("Skill Meta")]
    public string skill_type;                   // melee / ranged
    [Range(0, 15)] public byte skill_range;      // 표: skill_range
    [Range(0, 3)] public byte skill_cool;        // 표: skill_cool
    public bool skill_applies;                  // 표: skill_applies (0/1)

    [Header("Reward")]
    public byte reward_id;                      // 표: reward_id
}
