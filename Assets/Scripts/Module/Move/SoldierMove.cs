using System.Collections.Generic;
using UnityEngine;

public class SoldierMoveModule : BaseMoveModule
{
    public override List<Vector2Int> GetMovableRange(BasePiece owner, BattleManager manager)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        bool isRed = owner is RedPiece;
        int forwardY = isRed ? 1 : -1; // 红兵往上走（Y增加），黑卒往下走（Y减少）
        bool crossedRiver = isRed ? owner.GridY >= 5 : owner.GridY <= 4;

        // 1. 向前走
        int nx = owner.GridX;
        int ny = owner.GridY + forwardY;
        if (manager.IsValidGrid(nx, ny))
        {
            BasePiece target = manager.Board[nx, ny];
            if (!MoveHelper.IsFriendly(owner, target)) moves.Add(new Vector2Int(nx, ny));
        }

        // 2. 过河后可以左右平移
        if (crossedRiver)
        {
            int[] dx = { -1, 1 };
            for (int i = 0; i < 2; i++)
            {
                nx = owner.GridX + dx[i];
                ny = owner.GridY;
                if (manager.IsValidGrid(nx, ny))
                {
                    BasePiece target = manager.Board[nx, ny];
                    if (!MoveHelper.IsFriendly(owner, target)) moves.Add(new Vector2Int(nx, ny));
                }
            }
        }

        return moves;
    }
}