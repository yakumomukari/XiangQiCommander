using UnityEngine;
using UnityEngine.EventSystems;

// 挂在行动条头像预制体（PortraitPrefab）上的脚本，处理鼠标悬停交互
public class TurnOrderPortrait : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 当前头像在虚拟时序里引用的战场棋子实体
    public BasePiece LinkedPiece { get; set; }

    // 鼠标指针移入头像时由 EventSystem 自动回调
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LinkedPiece != null)
        {
            // 让高亮管理器把白色悬停高光瞬移到对应棋子的 Transform 世界位置
            HighlightManager.Instance.ShowHoverCursor(LinkedPiece.transform.position);
        }
    }

    // 鼠标指针移出头像时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        // 隐去白色高光
        HighlightManager.Instance.HideHoverCursor();
    }

    // 防御性策略：万一棋子在乱斗中途被打死了，头像隐藏时顺手把高光关了，防止画面残留 Bug
    private void OnDisable()
    {
        if (HighlightManager.Instance != null)
        {
            HighlightManager.Instance.HideHoverCursor();
        }
    }
}