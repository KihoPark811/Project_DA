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
    public float projectileSpeed = 12f;

    [Header("Return")]
    public float returnCatchRadius = 1.2f;
    public float returnLerpSpeed = 12f;

    float _aimFromUp;

    // 발사 입력 버퍼: Space를 눌렀지만 아직 못 쏜 상태
    bool _fireRequested = false;

    // 마지막으로 발사한 캐릭터 (R 키로 되돌릴 때 사용)
    PartyProjectile2D _lastFired;

    void Update()
    {
        // 게임 오버 체크를 쓰고 싶으면 여기에서 막기
        // if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        HandleAim();
        DrawPredictLine();

        // 1) Space 입력은 "발사 요청"만 기록
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _fireRequested = true;
        }

        // 2) 발사 요청이 있으면, 매 프레임 발사를 시도
        if (_fireRequested)
        {
            // TryFireOne()이 true면 실제로 한 명 발사 성공
            if (TryFireOne())
            {
                _fireRequested = false;
            }
        }

        // ================== R키: 직전 발사 되돌리기 ==================
        // 나중에 이 기능을 빼고 싶으면, 아래 if 블럭과 UndoLastShot() 함수만 통째로 지우면 됨
        if (Input.GetKeyDown(KeyCode.R))
        {
            UndoLastShot();
        }
        // ============================================================
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

    /// <summary>
    /// 발사가 가능한 캐릭터 한 명을 발사 시도.
    /// 성공하면 true, 아무도 못 쐈으면 false.
    /// </summary>
    bool TryFireOne()
    {
        foreach (var proj in partyQueue)
        {
            if (!proj) continue;

            // 아직 공중에 날아가는 중이면 패스
            if (proj.IsLaunched) continue;

            // 여기까지 왔다 = 발사 가능한 상태
            proj.Launch(
                muzzle.position,
                DirFromUp(_aimFromUp) * projectileSpeed,
                this
            );

            _lastFired = proj;   // 직전에 발사한 캐릭터 기억
            return true;         // 실제로 한 명 발사함
        }

        // 쏠 수 있는 캐릭터가 없으면 false
        return false;
    }

    // ================== R 키로 "직전 발사 되돌리기" 기능 ==================
    void UndoLastShot()
    {
        if (_lastFired == null) return;

        var p = _lastFired;

        // 이미 발사 상태가 아니고, 발사대 근처에 있다면 굳이 손댈 필요 없음
        if (!p.IsLaunched && IsNearLauncher(p.transform.position))
            return;

        // 즉시 리턴 처리
        p.SetKinematic(true);
        p.RB.linearVelocity = Vector2.zero;

        // 발사대 위치로 순간이동
        p.transform.position = muzzle.position;

        p.SetKinematic(false);
        p.OnReturned();  // launched = false, collider 비활성화 등 리턴 처리
    }
    // =================================================================

    public bool IsNearLauncher(Vector2 pos)
        => Vector2.Distance(pos, muzzle.position) <= returnCatchRadius;

    public void RequestReturn(PartyProjectile2D p)
        => StartCoroutine(ReturnRoutine(p));

    IEnumerator ReturnRoutine(PartyProjectile2D p)
    {
        // 리턴 시작 시점에 전투/충돌 상태 종료
        p.OnReturned();

        p.SetKinematic(true);
        p.RB.linearVelocity = Vector2.zero;

        while (Vector2.Distance(p.transform.position, muzzle.position) > 0.05f)
        {
            p.transform.position = Vector3.Lerp(
                p.transform.position,
                muzzle.position,
                Time.deltaTime * returnLerpSpeed
            );
            yield return null;
        }

        p.SetKinematic(false);
    }

    Vector2 DirFromUp(float deg)
        => (Vector2)(Quaternion.Euler(0, 0, deg) * Vector2.up);
}
