using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null) return;

        if (BattleManager.Instance.CurrentActivePiece == null ||
            !(BattleManager.Instance.CurrentActivePiece is RedPiece activeRed))
        {
            return;
        }

        // 每次轮到红棋行动，或者玩家没操作时，确保光标跟着当前活动棋子
        // 放在 Update 里可以保证就算棋子刚刚走完，光圈也能立刻切走或者显示
        HighlightManager.Instance.ShowSelectionCursor(activeRed.transform.position);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleLeftClick(activeRed);
        }
    }

    private void HandleLeftClick(RedPiece activeRed)
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));
        mouseWorldPos.z = 0f;

        Vector2Int clickedGrid = BattleManager.Instance.WorldToGrid(mouseWorldPos);
        if (!BattleManager.Instance.IsValidGrid(clickedGrid.x, clickedGrid.y)) return;

        if (activeRed.MoveModule != null)
        {
            List<Vector2Int> validMoves = activeRed.MoveModule.GetMovableRange(activeRed, BattleManager.Instance);

            if (validMoves.Contains(clickedGrid))
            {
                // 玩家下达指令后，隐藏光圈和绿点
                HighlightManager.Instance.HideSelectionCursor();
                HighlightManager.Instance.ClearMoveRange();

                activeRed.PerformMove(BattleManager.Instance, clickedGrid.x, clickedGrid.y);
            }
            else
            {
                Debug.Log("点错地方了，这回合你只能在这颗红棋的绿点范围内行动！");
            }
        }
    }
}