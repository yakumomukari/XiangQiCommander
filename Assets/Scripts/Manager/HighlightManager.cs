using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance { get; private set; }

    [Header("UI 预制体")]
    [Tooltip("绿点预制体 (SpriteRenderer)")]
    public GameObject GreenDotPrefab;
    [Tooltip("选中时的脚底光圈预制体 (SpriteRenderer)")]
    public GameObject SelectionCursorPrefab;

    private List<GameObject> dotPool = new List<GameObject>();
    private GameObject cursorInstance; // 全局唯一的选中光标

    void Awake()
    {
        Instance = this;

        // 初始化时直接生成一个光标，并把它藏起来
        if (SelectionCursorPrefab != null)
        {
            cursorInstance = Instantiate(SelectionCursorPrefab, transform);
            cursorInstance.SetActive(false);
        }
    }

    // --- 选中光圈控制 ---
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

    // --- 绿点范围控制 ---
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