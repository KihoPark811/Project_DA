using System.Collections.Generic; // List�� ����ϱ� ���� �߰��մϴ�.
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("������ ����")]
    // GridManager�� �ν����Ϳ��� �����ؾ� �մϴ�.
    public GridManager gridManager;

    [Header("���� ����")]
    // ������ ������ ���� �������� �����մϴ�.
    public GameObject monsterPrefab;
    // ���� ���� �� ������ ���� ���� �����մϴ�.
    public int initialMonsterCount = 6;
    // ������ ������ ����(ScriptableObject)�� ����Ʈ�� �����մϴ�.
    public List<MonsterStaticData> monsterDatas;

    // ������ ��� ���� �ν��Ͻ��� �����ϱ� ���� ����Ʈ�Դϴ�.
    private List<MonsterInstance> monsterInstances = new List<MonsterInstance>();
    // ���Ϳ��� �ο��� ���� ID ī�����Դϴ�.
    private uint nextInstanceId = 1;

    void Start()
    {
        // ���� �����Ͱ� �����Ǿ����� Ȯ���մϴ�.
        if (monsterDatas == null || monsterDatas.Count == 0)
        {
            Debug.LogError("������ ���� ������(monsterDatas)�� �������� �ʾҽ��ϴ�!");
            return; // ���� �����Ͱ� ������ ������ �ߴ��մϴ�.
        }

        // ������ ���۵Ǹ� ������ ����ŭ ���͸� �����մϴ�.
        SpawnInitialMonsters();
    }

    /// <summary>
    /// ������ ��(initialMonsterCount)��ŭ ���͸� �����ϰ� ��ġ�ϴ� �Լ��Դϴ�.
    /// </summary>
    void SpawnInitialMonsters()
    {
        RectInt spawnRect = gridManager.startSpawnRect;

        // ���� ������ ��� ��ġ�� ����Ʈ�� �̸� ��ƵӴϴ�.
        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int y = spawnRect.yMin; y < spawnRect.yMax; y++)
        {
            for (int x = spawnRect.xMin; x < spawnRect.xMax; x++)
            {
                availablePositions.Add(new Vector2Int(x, y));
            }
        }

        // ������ ����ŭ ���͸� �����մϴ�.
        for (int i = 0; i < initialMonsterCount; i++)
        {
            if (availablePositions.Count == 0)
            {
                Debug.LogWarning("���͸� ������ ��ġ�� �����մϴ�. ������ �ߴ��մϴ�.");
                break;
            }

            // 1. ���� ��ġ ������ ���� (�ߺ� ����)
            int posIndex = Random.Range(0, availablePositions.Count);
            Vector2Int gridPos = availablePositions[posIndex];
            availablePositions.RemoveAt(posIndex); // ����� ��ġ�� ����Ʈ���� ����

            // 2. ������ ���� ���� ������ ����
            int dataIndex = Random.Range(0, monsterDatas.Count);
            MonsterStaticData selectedData = monsterDatas[dataIndex];

            // 3. ���� ���� �� �ʱ�ȭ
            Vector2 worldPos = gridManager.CellToWorld(gridPos.x, gridPos.y);
            GameObject monsterObj = Instantiate(monsterPrefab, worldPos, Quaternion.identity);
            MonsterInstance monster = monsterObj.GetComponent<MonsterInstance>();

            if (monster != null)
            {
                // Init �Լ��� ȣ���Ͽ� ���͸� �ʱ�ȭ�մϴ�.
                monster.Init(nextInstanceId++, selectedData, gridPos, new Vector2(gridManager.cellSize, gridManager.cellSize));
                monsterInstances.Add(monster);
            }
        }
    }
}
