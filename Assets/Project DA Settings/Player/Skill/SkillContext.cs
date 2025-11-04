using UnityEngine;

public struct SkillContext
{
    public Vector2 point;          // 충돌 지점
    public Vector2 normal;         // 충돌 법선(벽)
    public GameObject target;      // 몬스터(없을 수 있음)
    public PartyProjectile2D owner; // 발사체 주체
    public int chainIndex;         // 같은 프레임 내 파생 순서(옵션)
}
