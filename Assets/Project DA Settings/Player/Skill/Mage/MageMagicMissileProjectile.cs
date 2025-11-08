using UnityEngine;

public class MageMagicMissileProjectile : MonoBehaviour
{
    private Transform target;
    private float speed;

    // 필요하면 데미지값도 여기서 관리
    public int damage = 1;

    // 스킬에서 호출해주는 초기화 함수
    public void Init(Transform targetTransform, float moveSpeed)
    {
        target = targetTransform;
        speed = moveSpeed;
    }

    void Update()
    {
        // 타겟이 죽었거나 씬에서 사라졌으면 미사일도 제거
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 current = transform.position;
        Vector2 dest = target.position;
        Vector2 dir = (dest - current).normalized;

        // 타겟을 향해서 직선 이동
        Vector2 nextPos = current + dir * speed * Time.deltaTime;
        transform.position = nextPos;

        // 충분히 가까워지면 "맞은 것"으로 처리
        if ((dest - nextPos).sqrMagnitude < 0.01f)
        {
            HitTarget(target);
        }
    }

    void HitTarget(Transform t)
    {
        // 여기서 적에게 데미지 주기 (프로젝트 규칙에 맞게 수정)
        var monster = t.GetComponent<MonsterInstance>();
        if (monster != null)
        {
            Debug.Log($"[Mage Basic Skill] {monster.name} 공격! 피해량: {damage}");
            // monster.Damage(damage);  // 실제 데미지 적용
        }

        Destroy(gameObject);
    }

    // 콜라이더 트리거로도 처리 가능 (필요하면 사용)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (target == null) return;

        if (other.transform == target)
        {
            HitTarget(target);
        }
    }
}
