using System.Collections.Generic;
using UnityEngine;

public class AdvisorMoveModule : BaseMoveModule
{
    public override List<Vector2Int> GetMovableRange(BasePiece owner, BattleManager manager)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int[] dx = { 1, 1, -1, -1 };
        int[] dy = { 1, -1, -1, 1 };

        bool isRed = owner is RedPiece;

        for (int i = 0; i < 4; i++)
        {
            int nx = owner.GridX + dx[i];
            int ny = owner.GridY + dy[i];

            if (manager.IsValidGrid(nx, ny))
            {
                // 九宫格限制
                if (nx < 3 || nx > 5) continue;
                if (isRed && (ny < 0 || ny > 2)) continue;
                if (!isRed && (ny < 7 || ny > 9)) continue;

                BasePiece target = manager.Board[nx, ny];
                if (!MoveHelper.IsFriendly(owner, target))
                {
                    moves.Add(new Vector2Int(nx, ny));
                }
            }
        }
        return moves;
    }
}