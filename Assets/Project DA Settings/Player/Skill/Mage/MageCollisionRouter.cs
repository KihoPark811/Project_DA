using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class MageCollisionRouter : MonoBehaviour
{
    [Header("Layer Masks")]
    public LayerMask warriorMask;
    public LayerMask monsterMask;

    [Header("Collision Events")]
    public UnityEvent<Collision2D> onHitWarrior;
    public UnityEvent<Collision2D> onHitMonster;

    void OnCollisionEnter2D(Collision2D collision)
    {
        int layer = collision.collider.gameObject.layer;

        // 전사
        if ((warriorMask.value & (1 << layer)) != 0)
        {
            onHitWarrior?.Invoke(collision);
        }
        // 몬스터
        else if ((monsterMask.value & (1 << layer)) != 0)
        {
            onHitMonster?.Invoke(collision);
        }
    }
}
