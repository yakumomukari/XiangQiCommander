using UnityEngine;

public class RedPiece : BasePiece
{
    [Header("RPG与养成属性")]
    public int AttackPower;
    public int CurrentExp;
    public int Level = 1;

    [Header("部署限制")]
    public int DeployCost;

    [Header("行为模组")]
    public BaseMoveModule MoveModule;
    public BaseAttackModule AttackModule;

    // 以前的 UI 槽位和 SetSelectedVisual() 全删了，清爽多了

    public override void StartTurn(BattleManager manager)
    {
        Debug.Log($"【回合开始】轮到红棋 {gameObject.name} 行动！等待玩家下达指令...");

        // 轮到自己时，直接通知管理器铺绿点
        if (MoveModule != null)
        {
            var validMoves = MoveModule.GetMovableRange(this, manager);
            HighlightManager.Instance.ShowMoveRange(validMoves);
        }
    }

    public void PerformAttack(BasePiece target)
    {
        if (AttackModule != null) AttackModule.ExecuteAttack(this, target);
        EndTurn();
    }

    public void PerformMove(BattleManager manager, int targetX, int targetY)
    {
        manager.Board[GridX, GridY] = null;
        GridX = targetX;
        GridY = targetY;
        manager.Board[GridX, GridY] = this;

        UpdateVisualPosition();
        EndTurn();
    }

    public void GainExp(int exp)
    {
        CurrentExp += exp;
        if (CurrentExp >= Level * 100)
        {
            Level++;
            CurrentExp = 0;
            Debug.Log($"{gameObject.name} 晋升到了 Level {Level}!");
        }
    }
}