using System.Collections.Generic;
using UnityEngine;

public class CannonMoveModule : BaseMoveModule
{
    public override List<Vector2Int> GetMovableRange(BasePiece owner, BattleManager manager)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = owner.GridX + dx[i];
            int ny = owner.GridY + dy[i];
            bool jumped = false; // 是否已经翻过了一座“炮架”

            while (manager.IsValidGrid(nx, ny))
            {
                BasePiece target = manager.Board[nx, ny];
                if (!jumped)
                {
                    // 没翻山前：空地能走，遇到任何人（不管敌我）当作炮架
                    if (target == null) moves.Add(new Vector2Int(nx, ny));
                    else jumped = true;
                }
                else
                {
                    // 翻山后：遇到空地跳过，遇到棋子结算
                    if (target != null)
                    {
                        if (!MoveHelper.IsFriendly(owner, target)) moves.Add(new Vector2Int(nx, ny));
                        break; // 炮只能隔一座山打人，结算完立刻阻断射线
                    }
                }
                nx += dx[i];
                ny += dy[i];
            }
        }
        return moves;
    }
}