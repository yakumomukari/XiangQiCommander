using System.Collections.Generic;
using UnityEngine;

// 工具箱：判断目标是否为己方棋子
public static class MoveHelper
{
    public static bool IsFriendly(BasePiece owner, BasePiece target)
    {
        if (target == null) return false;
        // 只要双方的类名一样，就认为是友军（红认红，黑认黑）
        return owner.GetType() == target.GetType();
    }
}
