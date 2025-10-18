using UnityEngine;

[CreateAssetMenu(menuName = "DA/Monster Static Data")]
public class MonsterStaticData : ScriptableObject
{
    [Header("Identification")]
    [Range(0, 255)] public byte monster_id;      // ǥ: monster_id
    public string monster_name;                 // ǥ: monster_name
    [TextArea] public string monster_guide;     // ǥ: monster_guide (����)

    [Header("Stats")]
    [Range(0, 2)] public byte monster_grade;     // 0=normal,1=elite,2=boss
    public uint monster_max_hp;                 // ǥ: monster_max_hp
    [Range(0, 100)] public byte monster_def;     // ǥ: monster_def

    [Header("Skill Meta")]
    public string skill_type;                   // melee / ranged
    [Range(0, 15)] public byte skill_range;      // ǥ: skill_range
    [Range(0, 3)] public byte skill_cool;        // ǥ: skill_cool
    public bool skill_applies;                  // ǥ: skill_applies (0/1)

    [Header("Reward")]
    public byte reward_id;                      // ǥ: reward_id
}
