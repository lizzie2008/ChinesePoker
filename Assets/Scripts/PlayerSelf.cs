using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 自身玩家
/// </summary>
public class PlayerSelf : Player
{
    public GameObject prefab;   //预制件

    private Transform originPos1; //牌的初始位置
    private Transform originPos2; //牌的初始位置
    private List<GameObject> cards = new List<GameObject>();
    private bool canSelectCard = false;     //玩家是否可以选牌

    void Awake()
    {
        originPos1 = transform.Find("OriginPos1");
        originPos2 = transform.Find("OriginPos2");
    }

    //整理手牌
    public void GenerateAllCards()
    {
        //排序
        Sort();
        //计算每张牌的偏移
        var offsetX = originPos2.position.x - originPos1.position.x;
        //获取最左边的起点
        int leftCount = (cardInfos.Count / 2);
        var startPos = originPos1.position + Vector3.left * offsetX * leftCount;

        for (int i = 0; i < cardInfos.Count; i++)
        {
            //生成卡牌
            var card = Instantiate(prefab, originPos1.position, Quaternion.identity, transform);
            card.GetComponent<RectTransform>().localScale = Vector3.one * 0.6f;
            card.GetComponent<Card>().InitImage(cardInfos[i]);
            card.transform.SetAsLastSibling();
            //动画移动
            var tween = card.transform.DOMoveX(startPos.x + offsetX * i, 1f);
            if (i == cardInfos.Count - 1) //最后一张动画
            {
                tween.OnComplete(() => { canSelectCard = true; });
            }
            cards.Add(card);
        }
    }
    /// <summary>
    /// 销毁所有卡牌对象
    /// </summary>
    public void DestroyAllCards()
    {
        cards.ForEach(Destroy);
        cards.Clear();
    }
    /// <summary>
    /// 开始叫牌
    /// </summary>
    public override void ToBiding()
    {
        base.ToBiding();
        CardManager._instance.SetBidButtonActive(true);
    }
    /// <summary>
    /// 点击卡牌处理
    /// </summary>
    /// <param name="data"></param>
    public void CardClick(BaseEventData data)
    {
        //叫牌或出牌阶段才可以选牌
        if (canSelectCard &&
            (CardManager._instance.cardManagerState == CardManagerStates.Bid ||
             CardManager._instance.cardManagerState == CardManagerStates.Playing))
        {
            var eventData = data as PointerEventData;
            if (eventData == null) return;

            var card = eventData.pointerCurrentRaycast.gameObject.GetComponent<Card>();
            if (card == null) return;

            card.SetSelectState();
        }
    }
}
