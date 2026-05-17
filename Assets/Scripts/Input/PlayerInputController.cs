using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private RedPiece selectedRedPiece;

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && selectedRedPiece != null)
        {
            ClearSelection();
            Debug.Log("取消选中");
        }
    }

    private void HandleLeftClick()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));
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

                // 开启棋子本体高亮
                selectedRedPiece.SetSelectedVisual(true);

                // 计算并铺开绿点范围
                if (selectedRedPiece.MoveModule != null)
                {
                    List<Vector2Int> validMoves = selectedRedPiece.MoveModule.GetMovableRange(selectedRedPiece, BattleManager.Instance);
                    HighlightManager.Instance.ShowMoveRange(validMoves);
                }

                Debug.Log($"选中了红棋: 坐标 ({clickedGrid.x}, {clickedGrid.y})");
            }
        }
        // 状态二：已经选中了，尝试移动
        else
        {
            if (selectedRedPiece.MoveModule != null)
            {
                List<Vector2Int> validMoves = selectedRedPiece.MoveModule.GetMovableRange(selectedRedPiece, BattleManager.Instance);

                if (validMoves.Contains(clickedGrid))
                {
                    selectedRedPiece.PerformMove(BattleManager.Instance, clickedGrid.x, clickedGrid.y);
                    ClearSelection(); // 移动完清空状态和表现层
                }
                else
                {
                    // 如果点的是其他红棋，应该切换选中目标而不是直接取消
                    if (targetPiece is RedPiece newRed && newRed != selectedRedPiece)
                    {
                        ClearSelection(); // 先清空旧的

                        // 直接走一套新的选中逻辑
                        selectedRedPiece = newRed;
                        selectedRedPiece.SetSelectedVisual(true);
                        List<Vector2Int> newMoves = selectedRedPiece.MoveModule.GetMovableRange(selectedRedPiece, BattleManager.Instance);
                        HighlightManager.Instance.ShowMoveRange(newMoves);
                    }
                    else
                    {
                        Debug.Log("该位置无法到达！取消选中。");
                        ClearSelection();
                    }
                }
            }
        }
    }

    // 统一切理选中状态（含数据清理和表现层清理）
    private void ClearSelection()
    {
        if (selectedRedPiece != null)
        {
            selectedRedPiece.SetSelectedVisual(false); // 关掉棋子底光
            selectedRedPiece = null;
        }
        HighlightManager.Instance.ClearHighlights(); // 回收全部绿点
    }
}