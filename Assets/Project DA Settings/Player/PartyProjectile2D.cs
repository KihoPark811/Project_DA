using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PartyProjectile2D : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    PartyLauncher2D _launcher;

    // 반환 요청 후 잠시 유예 시간 (즉시 반환 방지)
    const float RETURN_GRACE = 0.12f;
    float _timeSinceLaunch;

    [Header("Combat")]
    public string role = "Warrior"; // "Warrior" / "Mage" �� ����׿� ��
    public int attackDamage = 12;
    public int pierceCount = 0;   // 0=ù ���Ϳ��� ����, >0=�� ����ŭ ����
    public bool stopOnHit = true; // true�� ���� ���ڸ��� �ӵ� 0

    [Header("Move")]
    public float maxSpeed = 18f;
    public float minSpeedToReturn = 0.6f;

    [Header("Wall Skill (�ɼ�)")]
    public bool triggerSkillOnWall = false;
    public GameObject burstProjectilePrefab; // ������: �� �浹 �� �̴�ź
    public float burstSpeed = 10f;
    public int burstCount = 4; // 4��

    bool _launched;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false); // �߻� ������ ��Ȱ��
    }

    public void Launch(Vector2 pos, Vector2 velocity, PartyLauncher2D launcher)
    {
        _launcher = launcher;
        transform.position = pos;

        // ▼ 중요: 반드시 velocity를 '확실히' 넣어준다.
        rb.linearVelocity = velocity;               // Unity 6에서도 velocity 사용
        // rb.linearVelocity = velocity;      // (만약 velocity가 먹지 않으면 이 라인을 사용)

        _timeSinceLaunch = 0f;                // 유예시간 초기화
        gameObject.SetActive(true);           // 혹시나 안전 차원에서 다시 켜두기
    }

    void Update()
    {
        // ▼ 발사 직후 유예시간 경과 전에는 회수 로직을 건드리지 않는다.
        _timeSinceLaunch += Time.deltaTime;

        if (_timeSinceLaunch > RETURN_GRACE)
        {
            // 속도가 너무 느려지거나, 발사대 근처로 돌아왔을 때만 회수
            if (_launcher.IsNearLauncher((Vector2)transform.position) ||
                rb.linearVelocity.magnitude < minSpeedToReturn)
            {
                _launcher.RequestReturn(this);
                // 여기서 바로 false/return으로 끊어주지 않아도, RequestReturn 코루틴이 알아서 회수
            }
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        int lay = c.collider.gameObject.layer;

        // 1) 몬스터 피격
        if (((1 << lay) & _launcher.enemyMask) != 0)
        {
            if (c.collider.TryGetComponent(out MonsterInstance mi))
            {
                mi.TakeDamage((uint)Mathf.Max(1, attackDamage)); // 몬스터 HP 감소
            }

            if (pierceCount > 0)
            {
                pierceCount--; // 관통 남았으면 계속 진행
            }
            else
            {
                if (stopOnHit)
                {
                    rb.linearVelocity = Vector2.zero; // 멈추어 곧 복귀
                    return;
                }
            }
        }

        // 2) 벽 반사 (물리 재질 없이 코드로 확실하게)
        if (((1 << lay) & _launcher.wallMask) != 0)
        {
            // 첫 번째 접촉 지점의 법선으로 반사
            Vector2 n = c.contacts.Length > 0 ? c.contacts[0].normal : Vector2.up;
            Vector2 v = rb.linearVelocity;

            if (v.sqrMagnitude < 0.0001f) v = transform.up; // 혹시 0이라면 보정
            Vector2 reflected = Vector2.Reflect(v, n);

            // 약간의 감쇠를 주면 벽에 들러붙는 현상 방지 (원하면 1.0f로)
            rb.linearVelocity = reflected * 0.98f;

            // 마법사 벽-스킬 (옵션)
            if (triggerSkillOnWall && burstProjectilePrefab)
            {
                for (int i = 0; i < burstCount; i++)
                {
                    float a = 360f * i / burstCount;
                    var go = Instantiate(burstProjectilePrefab);
                    var p = go.GetComponent<PartyProjectile2D>();
                    p.attackDamage = Mathf.Max(1, attackDamage / 2);
                    p.Launch(transform.position,
                             (Quaternion.Euler(0, 0, a) * Vector2.up) * burstSpeed,
                             _launcher);
                    go.SetActive(true);
                }
            }
        }
    }


    public void OnReturned()
    {
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}
