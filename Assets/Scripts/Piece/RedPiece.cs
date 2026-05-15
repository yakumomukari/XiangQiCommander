using UnityEngine;

// 红方棋子：RPG动作机制，移动与攻击分离，带经验成长
public class RedPiece : BasePiece
{
    public int AttackPower;
    public int CurrentExp;
    public int Level = 1;

    public BaseMoveModule MoveModule;
    public BaseAttackModule AttackModule;

    public override void StartTurn(BattleManager manager)
    {
        // 激活前端输入，等待玩家操作
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