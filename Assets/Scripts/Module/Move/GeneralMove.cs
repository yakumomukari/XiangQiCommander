using System.Collections.Generic;
using UnityEngine;

public class GeneralMoveModule : BaseMoveModule
{
    public override List<Vector2Int> GetMovableRange(BasePiece owner, BattleManager manager)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };

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

        // 注意：这里没写“老将不能照面（飞将）”的逻辑
        // 因为“飞将”通常是全局规则验证，写在底层模组里会导致耦合度过高，建议之后在输入校验层单独写。
        return moves;
    }
}