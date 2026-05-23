using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance { get; private set; }

    [Header("UI 预制体")]
    public GameObject GreenDotPrefab;
    public GameObject SelectionCursorPrefab;
    [Tooltip("鼠标悬停在侧栏头像上时，场上棋子脚下的白色高光预制体")]
    public GameObject HoverCursorPrefab; // <--- 新增

    private List<GameObject> dotPool = new List<GameObject>();
    private GameObject cursorInstance;
    private GameObject hoverCursorInstance; // <--- 新增

    void Awake()
    {
        Instance = this;

        if (SelectionCursorPrefab != null)
        {
            cursorInstance = Instantiate(SelectionCursorPrefab, transform);
            cursorInstance.SetActive(false);
        }

        // ==================== 新增：初始化白色悬停游标 ====================
        if (HoverCursorPrefab != null)
        {
            hoverCursorInstance = Instantiate(HoverCursorPrefab, transform);
            hoverCursorInstance.SetActive(false);
        }
        // ================================================================
    }

    public void ShowSelectionCursor(Vector3 worldPos)
    {
        if (cursorInstance != null)
        {
            cursorInstance.transform.position = worldPos;
            cursorInstance.SetActive(true);
        }
    }

    public void HideSelectionCursor()
    {
        if (cursorInstance != null)
        {
            cursorInstance.SetActive(false);
        }
    }

    // ==================== 新增：悬停白色高光控制接口 ====================
    public void ShowHoverCursor(Vector3 worldPos)
    {
        if (hoverCursorInstance != null)
        {
            hoverCursorInstance.transform.position = worldPos;
            hoverCursorInstance.SetActive(true);
        }
    }

    public void HideHoverCursor()
    {
        if (hoverCursorInstance != null)
        {
            hoverCursorInstance.SetActive(false);
        }
    }
    // ===================================================================

    public void ShowMoveRange(List<Vector2Int> validMoves)
    {
        ClearMoveRange();
        for (int i = 0; i < validMoves.Count; i++)
        {
            if (i >= dotPool.Count)
            {
                GameObject newDot = Instantiate(GreenDotPrefab, transform);
                dotPool.Add(newDot);
            }
            Vector3 worldPos = BattleManager.Instance.GridToWorld(validMoves[i].x, validMoves[i].y);
            dotPool[i].transform.position = worldPos;
            dotPool[i].SetActive(true);
        }
    }

    public void ClearMoveRange()
    {
        foreach (var dot in dotPool)
        {
            dot.SetActive(false);
        }
    }
}