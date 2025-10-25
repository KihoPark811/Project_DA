using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyLauncher2D : MonoBehaviour
{
    [Header("Aim")]
    [Range(10f, 150f)] public float maxAimDegrees = 150f;
    public bool useMouseAim = true;
    public float aimSensitivity = 90f;

    [Header("Refs")]
    public Transform muzzle;
    public LineRenderer line;
    public LayerMask wallMask, enemyMask, groundMask;

    [Header("Firing")]
    public List<PartyProjectile2D> partyQueue = new();
    public float fireInterval = 0.18f;
    public float projectileSpeed = 12f;

    [Header("Return")]
    public float returnCatchRadius = 1.2f;
    public float returnLerpSpeed = 12f;

    float _aimFromUp;
    bool _isFiring;

    void Update()
    {
        HandleAim();
        DrawPredictLine();

        if (Input.GetKeyDown(KeyCode.Space) && !_isFiring)
            StartCoroutine(FireSeq());
    }

    void HandleAim()
    {
        if (useMouseAim)
        {
            Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - muzzle.position);
            dir.y = Mathf.Max(dir.y, 0.001f);
            float a = Vector2.SignedAngle(Vector2.up, dir.normalized);
            _aimFromUp = Mathf.Clamp(a, -maxAimDegrees * 0.5f, maxAimDegrees * 0.5f);
        }
        else
        {
            _aimFromUp += Input.GetAxis("Horizontal") * aimSensitivity * Time.deltaTime;
            _aimFromUp = Mathf.Clamp(_aimFromUp, -maxAimDegrees * 0.5f, maxAimDegrees * 0.5f);
        }

        transform.up = Quaternion.Euler(0, 0, _aimFromUp) * Vector3.up;
    }

    void DrawPredictLine()
    {
        if (!line) return;

        Vector2 p0 = muzzle.position;
        Vector2 dir = DirFromUp(_aimFromUp);

        RaycastHit2D h1 = Physics2D.Raycast(p0, dir, 50f, wallMask | enemyMask);
        Vector2 p1 = h1 ? h1.point : p0 + dir * 12f;

        if (h1 && ((1 << h1.collider.gameObject.layer) & wallMask) != 0)
        {
            Vector2 r = Vector2.Reflect(dir, h1.normal);
            RaycastHit2D h2 = Physics2D.Raycast(p1 + r * 0.01f, r, 50f, wallMask | enemyMask);
            Vector2 p2 = h2 ? h2.point : p1 + r * 8f;

            line.positionCount = 3;
            line.SetPositions(new Vector3[] { p0, p1, p2 });
        }
        else
        {
            line.positionCount = 2;
            line.SetPositions(new Vector3[] { p0, p1 });
        }
    }

    IEnumerator FireSeq()
    {
        _isFiring = true;
        foreach (var proj in partyQueue)
        {
            if (!proj) continue;
            // 새 3인자 Launch 사용 (시작위치, 초기속도, 소유자)
            proj.Launch(muzzle.position, DirFromUp(_aimFromUp) * projectileSpeed, this);
            yield return new WaitForSeconds(fireInterval);
        }
        _isFiring = false;
    }

    public bool IsNearLauncher(Vector2 pos)
        => Vector2.Distance(pos, muzzle.position) <= returnCatchRadius;

    public void RequestReturn(PartyProjectile2D p)
        => StartCoroutine(ReturnRoutine(p));

    IEnumerator ReturnRoutine(PartyProjectile2D p)
    {
        // isKinematic(폐기) 대신 bodyType을 설정하는 래퍼 사용
        p.SetKinematic(true);
        p.RB.linearVelocity = Vector2.zero;

        while (Vector2.Distance(p.transform.position, muzzle.position) > 0.05f)
        {
            p.transform.position = Vector3.Lerp(p.transform.position, muzzle.position, Time.deltaTime * returnLerpSpeed);
            yield return null;
        }

        p.SetKinematic(false);
        p.OnReturned(); // 이제 존재!
    }

    Vector2 DirFromUp(float deg) => (Vector2)(Quaternion.Euler(0, 0, deg) * Vector2.up);
}
