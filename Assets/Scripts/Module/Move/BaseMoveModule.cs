using System.Collections.Generic;
using UnityEngine;

// 所有移动模组的父类
public abstract class BaseMoveModule : MonoBehaviour
{
    // 获取当前能够移动的所有坐标（考虑蹩马腿、地形阻挡等）
    public abstract List<Vector2Int> GetMovableRange(BasePiece owner, BattleManager manager);
}