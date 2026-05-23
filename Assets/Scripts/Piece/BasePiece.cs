using UnityEngine;

// 所有棋子实体的最高抽象
public abstract class BasePiece : MonoBehaviour
{
    public int MaxHP;
    public int CurrentHP;
    public int Speed;
    public float CurrentAV { get; set; }
    public int GridX { get; set; }
    public int GridY { get; set; }
    // 在 BasePiece.cs 的属性声明区加上这个
[Header("UI表现")]
public Sprite Icon; // 用于在行动条显示的头像

    public void UpdateVisualPosition()
    {
        transform.position = BattleManager.Instance.GridToWorld(GridX, GridY);
    }

    public virtual void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0) Die();
    }

    protected virtual void Die()
    {
        BattleManager.Instance.RemovePiece(this);
    }

    public abstract void StartTurn(BattleManager manager);

    public void EndTurn()
    {
        CurrentAV = 10000f / Speed;
        BattleManager.Instance.TickTurn();
    }
}