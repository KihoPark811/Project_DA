// ���� ���� �� �뵵:
// �� ��ũ��Ʈ�� '��������' ������ �׸���(����) �� �ý����� �Ѱ��Ͽ� �����մϴ�.
// ���� ũ��� ���¸� �����ϰ�, ���ͳ� ĳ���Ͱ� ��ġ�� �� �ִ� ��ǥ �ý����� ������ �����մϴ�.
// ����, ����Ƽ ������ �󿡼� ���� ������ �ð������� ǥ���� �־� ���� �������� ���Ǽ��� �����ϴ� ���ҵ� ����մϴ�.

using UnityEngine; // ����Ƽ ������ �⺻ ����� ����ϱ� ���� �����մϴ�.

public class GridManager : MonoBehaviour // ����Ƽ ���� ������Ʈ�� ������ �� �ִ� ������Ʈ Ŭ������ �����մϴ�.
{
    [Header("Grid Size (�ڷ� ����: 7 x 14)")] // �ν����� â�� ������ ǥ���Ͽ� �������� �׷�ȭ�մϴ�.
    public int width = 7; // �׸����� ���� ĭ ���� 7�� �����մϴ�.
    public int height = 14; // �׸����� ���� ĭ ���� 14�� �����մϴ�.
    public float cellSize = 1f; // �� �׸��� ĭ�� ũ�⸦ 1 ����Ƽ �������� �����մϴ�.

    [Header("Spawn Area Settings")] // �ν����� â�� ���� ���� ���� ������ ǥ���մϴ�.
    public RectInt startSpawnRect = new RectInt(0, 9, 7, 5); // ���Ͱ� ó�� ������ �簢�� ������ (x:0, y:9)���� �ʺ� 7, ���� 5�� �����մϴ�.

    // �׸��� ��ǥ(x, y)�� ���� ���� ���� ��ǥ�� ��ȯ���ִ� �Լ��Դϴ�.
    public Vector2 CellToWorld(int x, int y)
    {
        return new Vector2(x * cellSize, y * cellSize); // �Է¹��� x, y ��ǥ�� �� ũ�⸦ ���Ͽ� ���� ��ġ�� ��ȯ�մϴ�.
    }

    // �Է¹��� �׸��� ��ǥ(x, y)�� �� ���ο� �ִ��� Ȯ���ϴ� �Լ��Դϴ�.
    public bool IsInside(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height); // ��ǥ�� 0�� �� �ִ� ũ�� ���̿� �ִ��� �� �������� Ȯ�� �� ����� ��ȯ�մϴ�.
    }

    // ����Ƽ �������� Scene �信���� ���̴� �ð��� �����(�����)�� �׸��� ���� �Լ��Դϴ�.
    private void OnDrawGizmos()
    {
        // --- ��ü �׸��� ���� �׸��� ---
        Gizmos.color = new Color(0.7f, 0.7f, 0.7f, 1f); // ������� ������ ȸ������ �����մϴ�.
        for (int x = 0; x < width; x++) // ���� ĭ ����ŭ �ݺ����� �����մϴ�.
        {
            for (int y = 0; y < height; y++) // ���� ĭ ����ŭ �ݺ����� �����մϴ�.
            {
                Vector2 center = CellToWorld(x, y); // ���� ������ �׸��� ĭ(x, y)�� ���� ��ǥ�� ����մϴ�.
                Gizmos.DrawWireCube(center, Vector3.one * (cellSize * 0.95f)); // ���� ��ġ�� �׵θ��� �ִ� �簢���� �׸��ϴ�. (ĭ ������ ���� �ణ �۰�)
            }
        }

        // --- ���� �ʱ� ���� ���� �׸��� ---
        Gizmos.color = new Color(0.3f, 1f, 0.3f, 0.25f); // ������� ������ ������ ������� �����մϴ�.

        // ���� ������ �߽��� ��ǥ�� ����մϴ�.
        Vector3 spawnAreaCenter = new Vector3(
            startSpawnRect.x + startSpawnRect.width / 2f - (cellSize / 2f), // �߽� x ��ǥ�� ����մϴ�.
            startSpawnRect.y + startSpawnRect.height / 2f - (cellSize / 2f), // �߽� y ��ǥ�� ����մϴ�.
            0f); // 2D �����̹Ƿ� z ��ǥ�� 0���� �����մϴ�.

        // ���� ������ ��ü ũ�⸦ ����մϴ�.
        Vector3 spawnAreaSize = new Vector3(
            startSpawnRect.width * cellSize, // ������ ���� �ʺ� ����մϴ�.
            startSpawnRect.height * cellSize, // ������ ���� ���̸� ����մϴ�.
            0.1f); // �ٸ� ������ ��ġ�� �ʵ��� z�� ũ�⸦ �ణ �ο��մϴ�.

        Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize); // ���� �߽����� ũ��� ���� ä���� �簢���� �׸��ϴ�.
    }
}

