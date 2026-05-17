using UnityEngine;

// 黑方棋子：保留古典象棋的“移动即秒杀”机制
public class BlackPiece : BasePiece
{
    public override void StartTurn(BattleManager manager)
    {
        // 临时防死锁：让黑棋强行原地挂机并结束回合，或者你随便调个 MoveModule 里的合法格子走一步
        Debug.Log($"轮到黑棋 {gameObject.name} 行动，但AI还没写，原地结束回合。");
        EndTurn();
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