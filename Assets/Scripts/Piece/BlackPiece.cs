using UnityEngine;

// 黑方棋子：保留古典象棋的“移动即秒杀”机制
public class BlackPiece : BasePiece
{
    public override void StartTurn(BattleManager manager)
    {
        // 待接入极大极小值AI
    }

    public void ExecuteMoveAndKill(BattleManager manager, int targetX, int targetY)
    {
        BasePiece targetNode = manager.Board[targetX, targetY];
        if (targetNode != null && targetNode != this)
        {
            targetNode.TakeDamage(99999); // 秒杀机制
        }

        manager.Board[GridX, GridY] = null;
        GridX = targetX;
        GridY = targetY;
        manager.Board[GridX, GridY] = this;

        UpdateVisualPosition();
        EndTurn();
    }
}