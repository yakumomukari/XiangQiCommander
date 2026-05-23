using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUI : MonoBehaviour
{
    public static TurnOrderUI Instance { get; private set; }

    [Header("UI 引用")]
    public GameObject PortraitPrefab;
    public Transform Container;

    private List<GameObject> portraitPool = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public void UpdateTurnOrder(List<BasePiece> sortedPieces)
    {
        while (portraitPool.Count < sortedPieces.Count)
        {
            GameObject newPortrait = Instantiate(PortraitPrefab, Container);
            portraitPool.Add(newPortrait);
        }

        for (int i = 0; i < portraitPool.Count; i++)
        {
            if (i < sortedPieces.Count)
            {
                portraitPool[i].SetActive(true);

                Image img = portraitPool[i].GetComponent<Image>();
                if (img != null && sortedPieces[i].Icon != null)
                {
                    img.sprite = sortedPieces[i].Icon;
                }

                // ==================== 新增：动态绑定时序数据 ====================
                // 获取或动态挂载刚才写的悬停交互组件
                TurnOrderPortrait portraitScript = portraitPool[i].GetComponent<TurnOrderPortrait>();
                if (portraitScript == null)
                {
                    portraitScript = portraitPool[i].AddComponent<TurnOrderPortrait>();
                }

                // 将推算出的对应棋子引用灌进去（多动机制下，不同的头像可能会指向同一个红棋实例）
                portraitScript.LinkedPiece = sortedPieces[i];
                // ===============================================================
            }
            else
            {
                portraitPool[i].SetActive(false);
            }
        }
    }
}