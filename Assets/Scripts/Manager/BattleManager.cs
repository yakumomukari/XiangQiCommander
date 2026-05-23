using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("棋盘配置")]
    [Tooltip("可通过外部脚本（如关卡管理器）在Awake前修改这两个值")]
    public int BoardWidth = 9;
    public int BoardHeight = 10;

    [Header("视觉与河界映射")]
    public Vector3 OriginPosition;
    public float CellSize = 1f;

    public bool HasRiver = true;
    [Tooltip("越过哪一排（逻辑Y索引）开始计算河界偏移？标准象棋是5")]
    public int RiverStartY = 5;
    [Tooltip("河导致的额外Y轴偏移量")]
    public float RiverOffset = 0.5f;

    public static BattleManager Instance { get; private set; }
    public BasePiece[,] Board { get; private set; }
    private List<BasePiece> allPieces = new List<BasePiece>();
    // 在 BattleManager.cs 开头加上这个属性
    public BasePiece CurrentActivePiece { get; private set; }
    void Awake()
    {
        Instance = this;
        Board = new BasePiece[BoardWidth, BoardHeight];
    }

    public void RegisterPiece(BasePiece piece, int startX, int startY)
    {
        piece.GridX = startX;
        piece.GridY = startY;
        Board[startX, startY] = piece;
        allPieces.Add(piece);
        piece.CurrentAV = 10000f / piece.Speed;
        piece.UpdateVisualPosition();
    }

    public void RemovePiece(BasePiece piece)
    {
        Board[piece.GridX, piece.GridY] = null;
        allPieces.Remove(piece);
        Destroy(piece.gameObject);
    }

    // ==================== 新增：虚拟时序推算节点 ====================
    private struct SimulatedTimelineNode
    {
        public BasePiece Piece;
        public float TempAV;

        public SimulatedTimelineNode(BasePiece piece, float av)
        {
            Piece = piece;
            TempAV = av;
        }
    }

    /// <summary>
    /// 核心预测算法：在不污染真实数据的前提下，模拟推算未来 N 个行动席位
    /// </summary>
    public List<BasePiece> PredictFutureTurns(int previewCount)
    {
        List<BasePiece> predictionList = new List<BasePiece>();
        if (allPieces.Count == 0) return predictionList;

        // 1. 拷贝一份当前所有活棋的实时真实AV，放进虚拟队列
        List<SimulatedTimelineNode> simQueue = new List<SimulatedTimelineNode>();
        foreach (var p in allPieces)
        {
            simQueue.Add(new SimulatedTimelineNode(p, p.CurrentAV));
        }

        // 2. 循环离散事件模拟，推演未来多动席位
        for (int i = 0; i < previewCount; i++)
        {
            // 排序裁决逻辑必须与真实的 TickTurn 严格一致
            simQueue.Sort((a, b) =>
            {
                int avCompare = a.TempAV.CompareTo(b.TempAV);
                if (avCompare != 0) return avCompare;

                int speedCompare = b.Piece.Speed.CompareTo(a.Piece.Speed);
                if (speedCompare != 0) return speedCompare;

                bool aIsRed = a.Piece is RedPiece;
                bool bIsRed = b.Piece is RedPiece;
                if (aIsRed && !bIsRed) return -1;
                if (!aIsRed && bIsRed) return 1;

                return 0;
            });

            // 抓出这轮虚拟推演里最快的人
            SimulatedTimelineNode nextActor = simQueue[0];
            predictionList.Add(nextActor.Piece);

            // 模拟当前单位行动结束：在虚拟未来里，它的虚拟AV需要加上跑完一轮的消耗量
            nextActor.TempAV += (10000f / nextActor.Piece.Speed);

            // 结构体是值类型，必须写回列表生效
            simQueue[0] = nextActor;
        }

        return predictionList;
    }

    // 修改现有的 TickTurn 函数
    public void TickTurn()
    {
        if (allPieces.Count == 0) return;

        // 真实的当前回合排序，用于抓出眼前是谁该动
        allPieces.Sort((a, b) =>
        {
            int avCompare = a.CurrentAV.CompareTo(b.CurrentAV);
            if (avCompare != 0) return avCompare;

            int speedCompare = b.Speed.CompareTo(a.Speed);
            if (speedCompare != 0) return speedCompare;

            bool aIsRed = a is RedPiece;
            bool bIsRed = b is RedPiece;
            if (aIsRed && !bIsRed) return -1;
            if (!aIsRed && bIsRed) return 1;

            return 0;
        });

        // ==================== 修改：通知 UI 时改用虚拟预测列表 ====================
        if (TurnOrderUI.Instance != null)
        {
            // 传入 6 代表侧栏会预显接下来的 6 次行动序列。如果红棋够快，这 6 个格子里能同时出现它好几次
            List<BasePiece> futureTurns = PredictFutureTurns(6);
            TurnOrderUI.Instance.UpdateTurnOrder(futureTurns);
        }
        // =========================================================================

        BasePiece nextActor = allPieces[0];

        // 真实的时间流逝流转
        float timePassed = nextActor.CurrentAV;
        foreach (var piece in allPieces)
        {
            piece.CurrentAV -= timePassed;
        }

        CurrentActivePiece = nextActor;
        nextActor.StartTurn(this);
    }

    // 动态河界的世界坐标映射
    public Vector3 GridToWorld(int gridX, int gridY)
    {
        float worldX = OriginPosition.x + (gridX * CellSize);
        float worldY = OriginPosition.y + (gridY * CellSize);

        // 只有开启了河界，且跨过了设定的 RiverStartY 才加偏移
        if (HasRiver && gridY >= RiverStartY)
        {
            worldY += RiverOffset;
        }
        return new Vector3(worldX, worldY, 0);
    }

    // 动态河界的逻辑坐标映射
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float relativeX = worldPos.x - OriginPosition.x;
        float relativeY = worldPos.y - OriginPosition.y;

        if (HasRiver)
        {
            // 计算视觉上的河界中心点
            float riverCenterY = ((RiverStartY - 1) * CellSize) + (CellSize / 2f) + (RiverOffset / 2f);
            if (relativeY > riverCenterY)
            {
                relativeY -= RiverOffset;
            }
        }

        int x = Mathf.RoundToInt(relativeX / CellSize);
        int y = Mathf.RoundToInt(relativeY / CellSize);
        return new Vector2Int(x, y);
    }

    public bool IsValidGrid(int x, int y)
    {
        return x >= 0 && x < BoardWidth && y >= 0 && y < BoardHeight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float crossSize = CellSize * 0.2f;

        for (int x = 0; x < BoardWidth; x++)
        {
            for (int y = 0; y < BoardHeight; y++)
            {
                Vector3 pos = GridToWorld(x, y);
                Gizmos.DrawLine(pos - new Vector3(crossSize, 0, 0), pos + new Vector3(crossSize, 0, 0));
                Gizmos.DrawLine(pos - new Vector3(0, crossSize, 0), pos + new Vector3(0, crossSize, 0));
            }
        }

        if (HasRiver)
        {
            Gizmos.color = Color.cyan;
            float riverCenterY = OriginPosition.y + ((RiverStartY - 1) * CellSize) + (CellSize / 2f) + (RiverOffset / 2f);
            Vector3 leftPt = new Vector3(OriginPosition.x - CellSize, riverCenterY, 0);
            Vector3 rightPt = new Vector3(OriginPosition.x + BoardWidth * CellSize, riverCenterY, 0);
            Gizmos.DrawLine(leftPt, rightPt);
        }
    }
}