using UnityEngine;

[RequireComponent(typeof(PartyProjectile2D))]
public class MissileLimits2D : MonoBehaviour
{
    public bool destroyOnWall = true;
    public float lifeTime = 2.5f;

    PartyProjectile2D proj;
    void Awake() { proj = GetComponent<PartyProjectile2D>(); }
    void OnEnable() { if (lifeTime > 0) Invoke(nameof(Kill), lifeTime); }
    void OnDisable() { CancelInvoke(); }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (!destroyOnWall || proj.Owner == null) return;
        int lay = c.collider.gameObject.layer;
        if ((proj.Owner.wallMask.value & (1 << lay)) != 0)
            Kill();
    }
    void Kill() { Destroy(gameObject); }
}
