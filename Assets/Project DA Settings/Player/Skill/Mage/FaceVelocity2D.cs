using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FaceVelocity2D : MonoBehaviour
{
    public float angleOffsetDeg = 0f; // 스프라이트 기본 방향이 오른쪽이면 0, 위쪽이면 -90 등
    Rigidbody2D rb;
    void Awake() { rb = GetComponent<Rigidbody2D>(); }
    void LateUpdate()
    {
        var v = rb.linearVelocity;
        if (v.sqrMagnitude < 1e-6f) return;
        float ang = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg + angleOffsetDeg;
        transform.rotation = Quaternion.Euler(0, 0, ang);
    }
}
