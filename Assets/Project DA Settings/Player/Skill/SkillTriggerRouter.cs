using UnityEngine;
using System;

[RequireComponent(typeof(PartyProjectile2D))]
public class SkillTriggerRouter : MonoBehaviour
{
    public event Action<SkillContext, TriggerType, ConditionType> OnSkillTrigger;
    PartyProjectile2D _proj;

    void Awake() { _proj = GetComponent<PartyProjectile2D>(); }

    // PartyProjectile2D에서 호출하게 해줌
    public void NotifyWallHit(Vector2 point, Vector2 normal, bool isTop)
    {
        var ctx = new SkillContext { point = point, normal = normal, owner = _proj };
        var trig = isTop ? TriggerType.WallTop : TriggerType.WallSide;
        OnSkillTrigger?.Invoke(ctx, trig, ConditionType.Hit);
        OnSkillTrigger?.Invoke(ctx, TriggerType.WallAll, ConditionType.Hit);
    }

    public void NotifyMobHit(GameObject mob, Vector2 point)
    {
        var ctx = new SkillContext { point = point, normal = Vector2.zero, target = mob, owner = _proj };
        OnSkillTrigger?.Invoke(ctx, TriggerType.Mob, ConditionType.Hit);
    }

    public void NotifyMobKill(GameObject mob, Vector2 point)
    {
        var ctx = new SkillContext { point = point, target = mob, owner = _proj };
        OnSkillTrigger?.Invoke(ctx, TriggerType.Mob, ConditionType.Kill);
    }
}
