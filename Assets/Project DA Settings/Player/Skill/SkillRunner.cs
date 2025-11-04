using UnityEngine;
using System.Collections.Generic;

public class SkillRunner : MonoBehaviour
{
    [SerializeField] List<MonoBehaviour> skillBehaviours; // WarriorSlashSkill 등 컴포넌트 참조
    readonly List<ISkill> _skills = new();
    readonly Dictionary<(TriggerType, ConditionType), int> _counts = new();

    void Awake()
    {
        foreach (var mb in skillBehaviours)
            if (mb is ISkill s) _skills.Add(s);

        var router = GetComponent<SkillTriggerRouter>();
        router.OnSkillTrigger += HandleTrigger;
    }

    void HandleTrigger(SkillContext ctx, TriggerType trig, ConditionType cond)
    {
        // 카운트 증가
        var key = (trig, cond);
        _counts.TryGetValue(key, out var c);
        c++; _counts[key] = c;

        foreach (var s in _skills)
        {
            if (s.Trigger != trig || s.Condition != cond) continue;
            if (s.ConditionCount > 1 && c % s.ConditionCount != 0) continue; // N회마다
            s.Execute(ctx);
        }
    }
}
