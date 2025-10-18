using System.Collections.Generic; // List를 사용하기 위해 추가합니다.
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("관리자 연결")]
    // GridManager를 인스펙터에서 연결해야 합니다.
    public GridManager gridManager;

    [Header("몬스터 설정")]
    // 생성할 몬스터의 원본 프리팹을 연결합니다.
    public GameObject monsterPrefab;
    // 게임 시작 시 생성할 몬스터 수를 설정합니다.
    public int initialMonsterCount = 6;
    // 스폰할 몬스터의 종류(ScriptableObject)를 리스트로 관리합니다.
    public List<MonsterStaticData> monsterDatas;

    // 생성된 모든 몬스터 인스턴스를 관리하기 위한 리스트입니다.
    private List<MonsterInstance> monsterInstances = new List<MonsterInstance>();
    // 몬스터에게 부여할 고유 ID 카운터입니다.
    private uint nextInstanceId = 1;

    void Start()
    {
        // 몬스터 데이터가 설정되었는지 확인합니다.
        if (monsterDatas == null || monsterDatas.Count == 0)
        {
            Debug.LogError("스폰할 몬스터 데이터(monsterDatas)가 설정되지 않았습니다!");
            return; // 몬스터 데이터가 없으면 실행을 중단합니다.
        }

        // 게임이 시작되면 지정된 수만큼 몬스터를 스폰합니다.
        SpawnInitialMonsters();
    }

    /// <summary>
    /// 지정된 수(initialMonsterCount)만큼 몬스터를 생성하고 배치하는 함수입니다.
    /// </summary>
    void SpawnInitialMonsters()
    {
        RectInt spawnRect = gridManager.startSpawnRect;

        // 생성 가능한 모든 위치를 리스트에 미리 담아둡니다.
        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int y = spawnRect.yMin; y < spawnRect.yMax; y++)
        {
            for (int x = spawnRect.xMin; x < spawnRect.xMax; x++)
            {
                availablePositions.Add(new Vector2Int(x, y));
            }
        }

        // 지정된 수만큼 몬스터를 생성합니다.
        for (int i = 0; i < initialMonsterCount; i++)
        {
            if (availablePositions.Count == 0)
            {
                Debug.LogWarning("몬스터를 생성할 위치가 부족합니다. 스폰을 중단합니다.");
                break;
            }

            // 1. 스폰 위치 무작위 선택 (중복 방지)
            int posIndex = Random.Range(0, availablePositions.Count);
            Vector2Int gridPos = availablePositions[posIndex];
            availablePositions.RemoveAt(posIndex); // 사용한 위치는 리스트에서 제거

            // 2. 스폰할 몬스터 종류 무작위 선택
            int dataIndex = Random.Range(0, monsterDatas.Count);
            MonsterStaticData selectedData = monsterDatas[dataIndex];

            // 3. 몬스터 생성 및 초기화
            Vector2 worldPos = gridManager.CellToWorld(gridPos.x, gridPos.y);
            GameObject monsterObj = Instantiate(monsterPrefab, worldPos, Quaternion.identity);
            MonsterInstance monster = monsterObj.GetComponent<MonsterInstance>();

            if (monster != null)
            {
                // Init 함수를 호출하여 몬스터를 초기화합니다.
                monster.Init(nextInstanceId++, selectedData, gridPos, new Vector2(gridManager.cellSize, gridManager.cellSize));
                monsterInstances.Add(monster);
            }
        }
    }
}
