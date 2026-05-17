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

    [Header("UI表现")]
    [Tooltip("拖入代表选中的高亮子物体（如脚底光圈）")]
    public GameObject SelectionHighlight;

    public override void StartTurn(BattleManager manager)
    {
        // 留空，交给 PlayerInputController 接管
    }

    // 控制自身高亮显隐
    public void SetSelectedVisual(bool isSelected)
    {
        if (SelectionHighlight != null)
        {
            SelectionHighlight.SetActive(isSelected);
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