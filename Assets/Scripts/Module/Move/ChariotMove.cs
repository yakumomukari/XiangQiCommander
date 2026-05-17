using System.Collections.Generic;
using UnityEngine;

public class ChariotMoveModule : BaseMoveModule
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

            while (manager.IsValidGrid(nx, ny))
            {
                BasePiece target = manager.Board[nx, ny];
                if (target == null)
                {
                    moves.Add(new Vector2Int(nx, ny));
                }
                else
                {
                    // 撞到棋子了，如果是敌军，把这个位置加入（吃子），然后跳出射线
                    if (!MoveHelper.IsFriendly(owner, target)) moves.Add(new Vector2Int(nx, ny));
                    break;
                }
                nx += dx[i];
                ny += dy[i];
            }
        }
        return moves;
    }
}