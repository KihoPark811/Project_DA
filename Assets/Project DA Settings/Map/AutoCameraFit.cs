// GridManager에 정의된 맵(그리드)의 크기에 맞춰 카메라의 위치와 시야를 자동으로 조절합니다.
// 어떤 화면 비율(해상도)에서도 맵 전체가 여백을 포함하여 화면에 꽉 차도록 만들어줍니다.

using UnityEngine; // 유니티 엔진의 기본 기능을 사용하기 위해 선언합니다.

// - 이 스크립트가 부착된 게임 오브젝트에는 반드시 Camera 컴포넌트가 있도록 강제합니다.
[RequireComponent(typeof(Camera))]
// - 카메라 자동 맞춤 기능을 수행하는 클래스입니다.
public class AutoCameraFit : MonoBehaviour
{
    // - 인스펙터 창에서 GridManager 컴포넌트를 가진 오브젝트를 연결할 변수입니다.
    public GridManager grid;
    // - 맵 주변에 추가할 여백의 크기를 설정합니다.
    public float margin = 0.5f;

    // - 게임이 시작될 때 한 번만 호출되는 유니티 내장 함수입니다.
    void Start()
    {
        // - 이 스크립트가 부착된 오브젝트의 Camera 컴포넌트를 가져옵니다.
        var cam = GetComponent<Camera>();
        // - 카메라의 투영 방식을 2D에 적합한 직교(Orthographic) 방식으로 설정합니다.
        cam.orthographic = true;

        // --- 카메라 위치를 맵의 정중앙으로 이동 ---
        // - 그리드의 가로 중앙 좌표를 계산합니다. (좌표는 0부터 시작하므로 -1)
        float cx = (grid.width - 1) * 0.5f;
        // - 그리드의 세로 중앙 좌표를 계산합니다.
        float cy = (grid.height - 1) * 0.5f;
        // - 계산된 중앙 좌표로 카메라의 위치를 설정합니다. (z축은 뒤로 물러나게 설정)
        transform.position = new Vector3(cx, cy, -10f);

        // --- 화면 비율에 맞춰 카메라 시야(크기) 조절 ---
        // - 카메라가 보여줘야 할 목표 너비를 계산합니다. (그리드 너비 + 양쪽 여백)
        float targetWidth = grid.width + margin * 2f;
        // - 카메라가 보여줘야 할 목표 높이를 계산합니다. (그리드 높이 + 위아래 여백)
        float targetHeight = grid.height + margin * 2f;

        // - 현재 게임 화면의 가로/세로 비율을 계산합니다.
        float screenAspect = (float)Screen.width / Screen.height;
        // - 카메라가 보여줘야 할 맵 영역의 가로/세로 비율을 계산합니다.
        float mapAspect = targetWidth / targetHeight;

        // - 화면 비율이 맵 비율보다 크거나 같은지(즉, 화면이 맵보다 옆으로 더 넓은지) 확인합니다.
        if (screenAspect >= mapAspect)
        {
            // - 화면이 더 넓으므로, 카메라 크기를 맵의 '높이'에 정확히 맞춥니다.
            cam.orthographicSize = targetHeight * 0.5f;
        }
        // - 화면이 맵보다 세로로 더 긴 경우입니다.
        else
        {
            // - 화면이 더 길므로, 카메라 크기를 맵의 '너비'에 맞추되, 화면 비율을 고려하여 계산합니다.
            cam.orthographicSize = (targetWidth * 0.5f) / screenAspect;
        }
    }
}