using System.Collections.Generic;
using UnityEngine;

// 所有攻击模组的父类
public abstract class BaseAttackModule : MonoBehaviour
{
    // 获取火力覆盖范围
    public abstract List<Vector2Int> GetAttackRange(BasePiece owner, BattleManager manager);

    // 执行攻击的特效与结算逻辑
    public abstract void ExecuteAttack(BasePiece attacker, BasePiece target);
}