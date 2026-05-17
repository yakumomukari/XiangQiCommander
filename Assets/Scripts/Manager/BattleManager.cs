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

    public void TickTurn()
    {
        if (allPieces.Count == 0) return;
        allPieces.Sort((a, b) => a.CurrentAV.CompareTo(b.CurrentAV));
        BasePiece nextActor = allPieces[0];

        float timePassed = nextActor.CurrentAV;
        foreach (var piece in allPieces)
        {
            piece.CurrentAV -= timePassed;
        }
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