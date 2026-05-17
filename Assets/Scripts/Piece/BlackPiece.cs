using System.Collections.Generic;
using UnityEngine;

public class BlackPiece : BasePiece
{
    [Header("行为模组")]
    public BaseMoveModule MoveModule; // 黑棋也需要挂载移动模组（比如挂个 SoldierMoveModule）

    public override void StartTurn(BattleManager manager)
    {
        Debug.Log($"【回合开始】轮到黑棋 {gameObject.name} 行动！AI正在思考...");

        // 简单延迟一下，防止AI瞬间走完玩家看不清（实战建议用协程，这里为了简单直接调用）
        Invoke(nameof(ExecuteDummyAI), 0.5f);
    }

    private void ExecuteDummyAI()
    {
        // 1. 获取所有合法步数
        if (MoveModule != null)
        {
            List<Vector2Int> validMoves = MoveModule.GetMovableRange(this, BattleManager.Instance);

            if (validMoves.Count > 0)
            {
                // 2. 随机挑一个能走的地方
                Vector2Int targetMove = validMoves[Random.Range(0, validMoves.Count)];

                // 3. 执行移动和秒杀结算
                ExecuteMoveAndKill(BattleManager.Instance, targetMove.x, targetMove.y);
                return;
            }
        }

        // 如果被卡死了无路可走，直接结束回合防止死锁
        Debug.Log($"黑棋 {gameObject.name} 动弹不得，原地罚站。");
        EndTurn();
    }

    public void ExecuteMoveAndKill(BattleManager manager, int targetX, int targetY)
    {
        BasePiece targetNode = manager.Board[targetX, targetY];
        if (targetNode != null && targetNode != this)
        {
            // 如果刚好踩到红棋，秒杀！
            targetNode.TakeDamage(99999);
            Debug.Log($"黑棋踩死了红棋！");
        }

        manager.Board[GridX, GridY] = null;
        GridX = targetX;
        GridY = targetY;
        manager.Board[GridX, GridY] = this;

        UpdateVisualPosition();
        EndTurn(); // 把时间轴推给下一个人
    }
}