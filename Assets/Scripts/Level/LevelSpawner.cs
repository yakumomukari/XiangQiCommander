using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [Header("关卡军费与出生地配置")]
    public int MaxDeployCost = 10;
    private int currentUsedCost = 0;

    public Vector2Int RedSpawnRangeX = new Vector2Int(0, 8);
    public Vector2Int RedSpawnRangeY = new Vector2Int(0, 4);

    public Vector2Int BlackSpawnRangeX = new Vector2Int(0, 8);
    public Vector2Int BlackSpawnRangeY = new Vector2Int(5, 9);

    [Header("实体预制体池")]
    public List<RedPiece> RedPiecePrefabs;
    // 把单一引用改成预制体列表
    public List<BlackPiece> BlackPiecePrefabs;

    void Start()
    {
        RandomDeployRedPieces();
        RandomDeployBlackPieces();

        BattleManager.Instance.TickTurn();
    }

    private void RandomDeployRedPieces()
    {
        if (RedPiecePrefabs.Count == 0) return;

        int maxRetries = 100;
        int retries = 0;

        while (currentUsedCost < MaxDeployCost && retries < maxRetries)
        {
            RedPiece randomPrefab = RedPiecePrefabs[Random.Range(0, RedPiecePrefabs.Count)];

            if (currentUsedCost + randomPrefab.DeployCost <= MaxDeployCost)
            {
                int rx = Random.Range(RedSpawnRangeX.x, RedSpawnRangeX.y + 1);
                int ry = Random.Range(RedSpawnRangeY.x, RedSpawnRangeY.y + 1);

                if (BattleManager.Instance.Board[rx, ry] == null)
                {
                    SpawnPiece(randomPrefab, rx, ry);
                    currentUsedCost += randomPrefab.DeployCost;
                }
            }
            retries++;
        }
    }

    private void RandomDeployBlackPieces()
    {
        // 加个安全锁，防空列表报错
        if (BlackPiecePrefabs.Count == 0) return;

        int blackCount = 5; // 之后你们可以通过读取关卡配置文件来动态改这个值
        int spawned = 0;
        int maxRetries = 100;
        int retries = 0;

        while (spawned < blackCount && retries < maxRetries)
        {
            // 从黑方卡池里随机抽一个兵种
            BlackPiece randomBlackPrefab = BlackPiecePrefabs[Random.Range(0, BlackPiecePrefabs.Count)];

            int bx = Random.Range(BlackSpawnRangeX.x, BlackSpawnRangeX.y + 1);
            int by = Random.Range(BlackSpawnRangeY.x, BlackSpawnRangeY.y + 1);

            if (BattleManager.Instance.Board[bx, by] == null)
            {
                SpawnPiece(randomBlackPrefab, bx, by);
                spawned++;
            }
            retries++;
        }
    }

    private void SpawnPiece(BasePiece prefab, int startX, int startY)
    {
        BasePiece newPiece = Instantiate(prefab);
        BattleManager.Instance.RegisterPiece(newPiece, startX, startY);
    }
}