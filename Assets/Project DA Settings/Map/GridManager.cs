// 파일 역할 및 용도:
// 이 스크립트는 '벽돌깨기' 게임의 그리드(격자) 맵 시스템을 총괄하여 관리합니다.
// 맵의 크기와 형태를 정의하고, 몬스터나 캐릭터가 배치될 수 있는 좌표 시스템의 기준을 마련합니다.
// 또한, 유니티 에디터 상에서 맵의 영역을 시각적으로 표시해 주어 레벨 디자인의 편의성을 제공하는 역할도 담당합니다.

using UnityEngine; // 유니티 엔진의 기본 기능을 사용하기 위해 선언합니다.

public class GridManager : MonoBehaviour // 유니티 게임 오브젝트에 부착될 수 있는 컴포넌트 클래스로 선언합니다.
{
    [Header("Grid Size (자료 기준: 7 x 14)")] // 인스펙터 창에 제목을 표시하여 변수들을 그룹화합니다.
    public int width = 7; // 그리드의 가로 칸 수를 7로 설정합니다.
    public int height = 14; // 그리드의 세로 칸 수를 14로 설정합니다.
    public float cellSize = 1f; // 각 그리드 칸의 크기를 1 유니티 유닛으로 설정합니다.

    [Header("Spawn Area Settings")] // 인스펙터 창에 스폰 영역 관련 제목을 표시합니다.
    public RectInt startSpawnRect = new RectInt(0, 9, 7, 5); // 몬스터가 처음 생성될 사각형 영역을 (x:0, y:9)에서 너비 7, 높이 5로 정의합니다.

    // 그리드 좌표(x, y)를 실제 게임 월드 좌표로 변환해주는 함수입니다.
    public Vector2 CellToWorld(int x, int y)
    {
        return new Vector2(x * cellSize, y * cellSize); // 입력받은 x, y 좌표에 셀 크기를 곱하여 실제 위치를 반환합니다.
    }

    // 입력받은 그리드 좌표(x, y)가 맵 내부에 있는지 확인하는 함수입니다.
    public bool IsInside(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height); // 좌표가 0과 맵 최대 크기 사이에 있는지 논리 연산으로 확인 후 결과를 반환합니다.
    }

    // 유니티 에디터의 Scene 뷰에서만 보이는 시각적 도우미(기즈모)를 그리는 내장 함수입니다.
    private void OnDrawGizmos()
    {
        // --- 전체 그리드 영역 그리기 ---
        Gizmos.color = new Color(0.7f, 0.7f, 0.7f, 1f); // 기즈모의 색상을 회색으로 설정합니다.
        for (int x = 0; x < width; x++) // 가로 칸 수만큼 반복문을 실행합니다.
        {
            for (int y = 0; y < height; y++) // 세로 칸 수만큼 반복문을 실행합니다.
            {
                Vector2 center = CellToWorld(x, y); // 현재 순번의 그리드 칸(x, y)의 월드 좌표를 계산합니다.
                Gizmos.DrawWireCube(center, Vector3.one * (cellSize * 0.95f)); // 계산된 위치에 테두리만 있는 사각형을 그립니다. (칸 구분을 위해 약간 작게)
            }
        }

        // --- 몬스터 초기 스폰 영역 그리기 ---
        Gizmos.color = new Color(0.3f, 1f, 0.3f, 0.25f); // 기즈모의 색상을 반투명 녹색으로 설정합니다.

        // 스폰 영역의 중심점 좌표를 계산합니다.
        Vector3 spawnAreaCenter = new Vector3(
            startSpawnRect.x + startSpawnRect.width / 2f - (cellSize / 2f), // 중심 x 좌표를 계산합니다.
            startSpawnRect.y + startSpawnRect.height / 2f - (cellSize / 2f), // 중심 y 좌표를 계산합니다.
            0f); // 2D 게임이므로 z 좌표는 0으로 설정합니다.

        // 스폰 영역의 전체 크기를 계산합니다.
        Vector3 spawnAreaSize = new Vector3(
            startSpawnRect.width * cellSize, // 영역의 실제 너비를 계산합니다.
            startSpawnRect.height * cellSize, // 영역의 실제 높이를 계산합니다.
            0.1f); // 다른 기즈모와 겹치지 않도록 z축 크기를 약간 부여합니다.

        Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize); // 계산된 중심점과 크기로 속이 채워진 사각형을 그립니다.
    }
}

