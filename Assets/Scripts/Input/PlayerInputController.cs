using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private RedPiece selectedRedPiece;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        if (Input.GetMouseButtonDown(1) && selectedRedPiece != null)
        {
            selectedRedPiece = null;
            Debug.Log("取消选中");
        }
    }

    private void HandleLeftClick()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2Int clickedGrid = BattleManager.Instance.WorldToGrid(mouseWorldPos);
        if (!BattleManager.Instance.IsValidGrid(clickedGrid.x, clickedGrid.y)) return;

        BasePiece targetPiece = BattleManager.Instance.Board[clickedGrid.x, clickedGrid.y];

        // 状态一：还没选中己方棋子
        if (selectedRedPiece == null)
        {
            if (targetPiece is RedPiece red)
            {
                selectedRedPiece = red;
                Debug.Log($"选中了红棋: 坐标 ({clickedGrid.x}, {clickedGrid.y})");
            }
        }
        // 状态二：已经选中了，尝试移动
        else
        {
            if (selectedRedPiece.MoveModule != null)
            {
                // 实时获取该棋子当前能走的所有格子
                List<Vector2Int> validMoves = selectedRedPiece.MoveModule.GetMovableRange(selectedRedPiece, BattleManager.Instance);

                // O(N) 暴力比对落点是否合法
                if (validMoves.Contains(clickedGrid))
                {
                    selectedRedPiece.PerformMove(BattleManager.Instance, clickedGrid.x, clickedGrid.y);
                    selectedRedPiece = null; // 移动完清空选中状态
                }
                else
                {
                    Debug.Log("该位置无法到达！");
                    selectedRedPiece = null; // 点错直接清空，符合常规战棋直觉
                }
            }
        }
    }
}