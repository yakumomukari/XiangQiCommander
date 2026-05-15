using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("棋盘配置")]
    public int BoardWidth = 9;
    public int BoardHeight = 10;

    [Header("视觉映射")]
    public Vector3 OriginPosition;
    public float CellSize = 1f;
    [Tooltip("楚河汉界导致的额外Y轴偏移量")]
    public float RiverOffset = 0.5f;

    public static BattleManager Instance { get; private set; }

    // 逻辑棋盘，纯数据层
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

    // 逻辑坐标转世界坐标 (附带过河偏移量)
    public Vector3 GridToWorld(int gridX, int gridY)
    {
        float worldX = OriginPosition.x + (gridX * CellSize);
        float worldY = OriginPosition.y + (gridY * CellSize);

        if (gridY >= 5)
        {
            worldY += RiverOffset;
        }
        return new Vector3(worldX, worldY, 0);
    }

    // 世界坐标转逻辑坐标 (附带过河偏移量反推)
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float relativeX = worldPos.x - OriginPosition.x;
        float relativeY = worldPos.y - OriginPosition.y;

        float riverCenterY = (4 * CellSize) + (CellSize / 2f) + (RiverOffset / 2f);
        if (relativeY > riverCenterY)
        {
            relativeY -= RiverOffset;
        }

        int x = Mathf.RoundToInt(relativeX / CellSize);
        int y = Mathf.RoundToInt(relativeY / CellSize);
        return new Vector2Int(x, y);
    }

    public bool IsValidGrid(int x, int y)
    {
        return x >= 0 && x < BoardWidth && y >= 0 && y < BoardHeight;
    }

    // ---------------- 编辑器辅助调试线 (格点版) ----------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float crossSize = CellSize * 0.2f; // 十字准星的大小

        // 遍历所有逻辑格点，画出十字交叉线，完美对应象棋的网格交点
        for (int x = 0; x < BoardWidth; x++)
        {
            for (int y = 0; y < BoardHeight; y++)
            {
                Vector3 pos = GridToWorld(x, y);

                // 画横线和竖线形成十字
                Gizmos.DrawLine(pos - new Vector3(crossSize, 0, 0), pos + new Vector3(crossSize, 0, 0));
                Gizmos.DrawLine(pos - new Vector3(0, crossSize, 0), pos + new Vector3(0, crossSize, 0));
            }
        }

        // 画一条蓝线示意楚河汉界的视觉中心点
        Gizmos.color = Color.cyan;
        float riverCenterY = OriginPosition.y + (4 * CellSize) + (CellSize / 2f) + (RiverOffset / 2f);
        Vector3 leftPt = new Vector3(OriginPosition.x - CellSize, riverCenterY, 0);
        Vector3 rightPt = new Vector3(OriginPosition.x + BoardWidth * CellSize, riverCenterY, 0);
        Gizmos.DrawLine(leftPt, rightPt);
    }
}