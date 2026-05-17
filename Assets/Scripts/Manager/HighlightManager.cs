using System.Collections.Generic;
using UnityEngine;

// 专职处理网格范围高亮的管理类
public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance { get; private set; }

    [Tooltip("绿点预制体，建议使用SpriteRenderer而非UGUI")]
    public GameObject GreenDotPrefab;

    // 对象池，杜绝 Instantiate/Destroy 带来的内存碎片
    private List<GameObject> dotPool = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    // 显示可移动的绿点范围
    public void ShowMoveRange(List<Vector2Int> validMoves)
    {
        ClearHighlights(); // 先把旧的藏起来

        for (int i = 0; i < validMoves.Count; i++)
        {
            // 池子不够就扩容
            if (i >= dotPool.Count)
            {
                GameObject newDot = Instantiate(GreenDotPrefab, transform);
                dotPool.Add(newDot);
            }

            // O(1) 取物理坐标
            Vector3 worldPos = BattleManager.Instance.GridToWorld(validMoves[i].x, validMoves[i].y);

            // 部署绿点
            dotPool[i].transform.position = worldPos;
            dotPool[i].SetActive(true);
        }
    }

    // 隐藏所有绿点
    public void ClearHighlights()
    {
        foreach (var dot in dotPool)
        {
            dot.SetActive(false);
        }
    }
}