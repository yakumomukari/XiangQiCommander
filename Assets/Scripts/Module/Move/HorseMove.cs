using System.Collections.Generic;
using UnityEngine;

public class HorseMoveModule : BaseMoveModule
{
    public override List<Vector2Int> GetMovableRange(BasePiece owner, BattleManager manager)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        // 马走日：8个落点方向
        int[] dx = { 1, 2, 2, 1, -1, -2, -2, -1 };
        int[] dy = { 2, 1, -1, -2, -2, -1, 1, 2 };
        // 蹩马腿判定点：和落点方向对应的阻挡点
        int[] bx = { 0, 1, 1, 0, 0, -1, -1, 0 };
        int[] by = { 1, 0, 0, -1, -1, 0, 0, 1 };

        for (int i = 0; i < 8; i++)
        {
            int nx = owner.GridX + dx[i];
            int ny = owner.GridY + dy[i];
            int blockX = owner.GridX + bx[i];
            int blockY = owner.GridY + by[i];

            if (manager.IsValidGrid(nx, ny))
            {
                // 检查蹩马腿
                if (manager.Board[blockX, blockY] == null)
                {
                    BasePiece target = manager.Board[nx, ny];
                    if (!MoveHelper.IsFriendly(owner, target))
                    {
                        moves.Add(new Vector2Int(nx, ny));
                    }
                }
            }
        }
        return moves;
    }
}