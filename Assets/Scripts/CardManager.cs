using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 卡牌管理
/// </summary>
public class CardManager : MonoBehaviour
{
    #region 初始化
    public static CardManager _instance;    //单例


    public float dealCardSpeed = 20;  //发牌速度
    public Player[] Players;    //玩家的集合

    public GameObject coverPrefab;      //背面排预制件
    public GameObject cardPrefab;      //背面排预制件
    public Transform heapPos;           //牌堆位置
    public Transform[] playerHeapPos;    //玩家牌堆位置
    public CardManagerStates cardManagerState;  //卡牌回合状态
    public List<CardInfo> currentCardInfos = new List<CardInfo>();  //当前出牌组合
    public int notFollowIndex = 0;  //不跟累计，如果=2，则对手要不起，继续出牌

    private string[] cardNames;  //所有牌集合
    private int termStartIndex;  //回合开始玩家索引
    private int termCurrentIndex;  //回合当前玩家索引
    private int bankerIndex;        //当前地主索引
    private GameObject bidBtns;     //玩家叫牌按钮
    private GameObject followBtns;  //玩家出牌按钮
    private GameObject startBtn;    //开始游戏按钮
    private GameObject bidCards;    //地主牌

    void Awake()
    {
        _instance = this;
        cardNames = GetCardNames();

        GameObject root = GameObject.Find("Root");

        startBtn = root.transform.Find("StartBtn").gameObject;
        bidBtns = root.transform.Find("BidBtns").gameObject;
        followBtns = root.transform.Find("FollowBtns").gameObject;
        bidCards = root.transform.Find("BidCards").gameObject;
    }
    #endregion

    #region 洗牌、发牌
    /// <summary>
    /// 洗牌
    /// </summary>
    public void ShuffleCards()
    {
        //进入洗牌阶段
        cardManagerState = CardManagerStates.ShuffleCards;
        cardNames = cardNames.OrderBy(c => Guid.NewGuid()).ToArray();
    }
    /// <summary>
    /// 开始发牌
    /// </summary>
    public IEnumerator DealCards()
    {
        //进入发牌阶段
        cardManagerState = CardManagerStates.DealCards;
        termCurrentIndex = termStartIndex;

        yield return DealHeapCards(false);
    }
    /// <summary>
    /// 发牌堆上的牌（如果现在不是抢地主阶段，发普通牌，如果是，发地主牌）
    /// </summary>
    /// <returns></returns>
    private IEnumerator DealHeapCards(bool ifForBid)
    {
        //显示牌堆
        heapPos.gameObject.SetActive(true);
        playerHeapPos.ToList().ForEach(s => { s.gameObject.SetActive(true); });

        var cardNamesNeeded = ifForBid
            ? cardNames.Skip(cardNames.Length - 3).Take(3)  //如果是抢地主牌，取最后3张
            : cardNames.Take(cardNames.Length - 3);         //如果首次发牌

        //计算每张地主牌的位置
        int cardIndex = 0;
        var width = (bidCards.GetComponent<RectTransform>().sizeDelta.x - 20) / 3;
        var centerBidPos = Vector3.zero;
        var leftBidPos = centerBidPos - Vector3.left * width;
        var rightBidPos = centerBidPos + Vector3.left * width;
        List<Vector3> bidPoss = new List<Vector3> { leftBidPos, centerBidPos, rightBidPos };
        foreach (var cardName in cardNamesNeeded)
        {
            //给当前玩家发一张牌
            Players[termCurrentIndex].AddCard(cardName);

            var cover = Instantiate(coverPrefab, heapPos.position, Quaternion.identity, heapPos.transform);
            cover.GetComponent<RectTransform>().localScale = Vector3.one;
            //移动动画，动画结束后自动销毁
            var tween = cover.transform.DOMove(playerHeapPos[termCurrentIndex].position, 0.3f);
            tween.OnComplete(() => Destroy(cover));

            yield return new WaitForSeconds(1 / dealCardSpeed);

            //如果给地主发牌
            if (ifForBid)
            {
                //显示地主牌
                var bidcard = Instantiate(cardPrefab, bidCards.transform.TransformPoint(bidPoss[cardIndex]), Quaternion.identity, bidCards.transform);
                bidcard.GetComponent<Card>().InitImage(new CardInfo(cardName));
                bidcard.GetComponent<RectTransform>().localScale = Vector3.one * 0.3f;
            }
            else
            {
                //下一个需要发牌者
                SetNextPlayer();
            }

            cardIndex++;
        }

        //隐藏牌堆
        heapPos.gameObject.SetActive(false);
        playerHeapPos[0].gameObject.SetActive(false);

        //发普通牌
        if (!ifForBid)
        {
            //显示玩家手牌
            ShowPlayerSelfCards();
            StartBiding();
        }
        //发地主牌
        else
        {
            if (Players[bankerIndex] is PlayerSelf)
            {
                //显示玩家手牌
                ShowPlayerSelfCards();
            }
            StartFollowing();
        }
    }

    /// <summary>
    /// 显示玩家手牌
    /// </summary>
    private void ShowPlayerSelfCards()
    {
        //销毁玩家手牌
        DestroyPlayerSelfCards();

        Players.ToList().ForEach(s =>
        {
            var player0 = s as PlayerSelf;
            if (player0 != null)
            {
                player0.GenerateAllCards();
            }
        });
    }
    /// <summary>
    /// 清空牌局
    /// </summary>
    public void ClearCards()
    {
        //清空所有玩家卡牌
        Players.ToList().ForEach(s => s.DropCards());

        //销毁玩家手牌
        DestroyPlayerSelfCards();
    }
    /// <summary>
    /// 
    /// </summary>
    private void DestroyPlayerSelfCards()
    {
        //销毁玩家手牌
        Players.ToList().ForEach(s =>
        {
            var player0 = s as PlayerSelf;
            if (player0 != null)
            {
                player0.DestroyAllCards();
            }
        });
    }
    /// <summary>
    /// 加载所有卡牌名
    /// </summary>
    /// <returns></returns>
    private string[] GetCardNames()
    {
        //路径  
        string fullPath = "Assets/Resources/Images/Cards/";

        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*.png", SearchOption.AllDirectories);

            return files.Select(s => Path.GetFileNameWithoutExtension(s.Name)).ToArray();
        }
        return null;
    }
    #endregion

    #region 叫牌逻辑
    /// <summary>
    /// 开始抢地主
    /// </summary>
    private void StartBiding()
    {
        cardManagerState = CardManagerStates.Bid;

        Players[termCurrentIndex].ToBiding();
    }
    /// <summary>
    /// 叫地主
    /// </summary>
    public void ForBid()
    {
        //设置当前地主
        bankerIndex = termCurrentIndex;

        //给地主发剩下的3张牌
        SetBidCardsActive(true);
        SetBidButtonActive(false);
        StartCoroutine(DealHeapCards(true));
    }
    /// <summary>
    /// 不叫
    /// </summary>
    public void NotBid()
    {
        SetBidButtonActive(false);
        SetNextPlayer();
        Players[termCurrentIndex].ToBiding();
    }
    #endregion

    #region 出牌逻辑
    /// <summary>
    /// 开始出牌阶段
    /// </summary>
    private void StartFollowing()
    {
        cardManagerState = CardManagerStates.Playing;
        //地主先出牌
        Players[bankerIndex].ToFollowing();
    }
    /// <summary>
    /// 玩家出牌
    /// </summary>
    public void ForFollow()
    {
        SetFollowButtonActive(false);

        //上轮玩家出牌清空
        Players[(termCurrentIndex + Players.Length - 1) % 3].DropAllSmallCards();
        if (Players[termCurrentIndex] is PlayerSelf)
            ShowPlayerSelfCards();

        SetNextPlayer();
        Players[termCurrentIndex].ToFollowing();
        notFollowIndex = 0;
    }
    /// <summary>
    /// 玩家不出
    /// </summary>
    public void NotFollow()
    {
        SetFollowButtonActive(false);

        //上轮玩家出牌清空
        Players[(termCurrentIndex + Players.Length - 1) % 3].DropAllSmallCards();

        SetNextPlayer();
        Players[termCurrentIndex].ToFollowing();
        notFollowIndex++;
    }
    #endregion

    #region 控制隐藏和显示
    /// <summary>
    /// 控制叫牌按钮是否显示
    /// </summary>
    /// <param name="isActive"></param>
    public void SetBidButtonActive(bool isActive)
    {
        bidBtns.SetActive(isActive);
    }
    /// <summary>
    /// 控制出牌按钮是否显示
    /// </summary>
    /// <param name="isActive"></param>
    public void SetFollowButtonActive(bool isActive)
    {
        followBtns.SetActive(isActive);
    }
    /// <summary>
    /// 控制开始游戏按钮是否显示
    /// </summary>
    /// <param name="isActive"></param>
    public void SetStartButtonActive(bool isActive)
    {
        startBtn.SetActive(isActive);
    }
    /// <summary>
    /// 设置地主牌是否显示
    /// </summary>
    /// <param name="isActive"></param>
    public void SetBidCardsActive(bool isActive)
    {
        bidCards.SetActive(isActive);
    }
    #endregion

    #region 卡牌回合控制
    //开始新回合
    public void OnStartTermClick()
    {
        //清空手牌、重新洗牌、开始发牌
        ClearCards();
        ShuffleCards();
        StartCoroutine(DealCards());
        SetStartButtonActive(false);
    }
    /// <summary>
    /// 设置下一轮开始玩家
    /// </summary>
    public void SetNextTerm()
    {
        termStartIndex = (termStartIndex + 1) % Players.Length;
    }
    /// <summary>
    /// 设置下个回合玩家
    /// </summary>
    /// <returns></returns>
    public void SetNextPlayer()
    {
        termCurrentIndex = (termCurrentIndex + 1) % Players.Length;
    }

    #endregion
}
