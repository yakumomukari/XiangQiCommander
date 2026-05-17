using System.Collections.Generic;
using UnityEngine;

public class ElephantMoveModule : BaseMoveModule
{
    public override List<Vector2Int> GetMovableRange(BasePiece owner, BattleManager manager)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        // 象飞田：4个落点
        int[] dx = { 2, 2, -2, -2 };
        int[] dy = { 2, -2, -2, 2 };
        // 塞象眼判定点
        int[] bx = { 1, 1, -1, -1 };
        int[] by = { 1, -1, -1, 1 };

        bool isRed = owner is RedPiece;

        for (int i = 0; i < 4; i++)
        {
            int nx = owner.GridX + dx[i];
            int ny = owner.GridY + dy[i];
            int blockX = owner.GridX + bx[i];
            int blockY = owner.GridY + by[i];

            if (manager.IsValidGrid(nx, ny))
            {
                // 过河判定：象不能过河
                if (isRed && ny > 4) continue;
                if (!isRed && ny < 5) continue;

                // 塞象眼判定
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