public interface ISkill
{
    string Name { get; }
    TriggerType Trigger { get; }         // 언제 발동하나
    ConditionType Condition { get; }     // Hit/Kill 등
    int ConditionCount { get; }          // N회 후 발동(기획 조건)
    void Execute(in SkillContext ctx);   // 실제 효과
}
