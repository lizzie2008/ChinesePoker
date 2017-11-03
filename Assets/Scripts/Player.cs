using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Player : MonoBehaviour
{
    protected List<CardInfo> cardInfos = new List<CardInfo>();  //个人所持卡牌

    private Text cardCoutText;
    private Text countDownText;
    private int consideratingTime = 6; //玩家考虑时间
    protected bool isMyTerm = false;  //当前是否是自己回合

    void Start()
    {
        cardCoutText = transform.Find("HeapPos/Text").GetComponent<Text>();
        countDownText = transform.Find("CountDown/Text").GetComponent<Text>();
    }

    #region 卡牌管理
    /// <summary>
    /// 增加一张卡牌
    /// </summary>
    /// <param name="cardName"></param>
    public void AddCard(string cardName)
    {
        cardInfos.Add(new CardInfo(cardName));
        cardCoutText.text = cardInfos.Count.ToString();
    }
    /// <summary>
    /// 清空所有卡片
    /// </summary>
    public void DropCards()
    {
        cardInfos.Clear();
    }
    /// <summary>
    /// 卡牌排序（从大到小）
    /// </summary>
    protected void Sort()
    {
        cardInfos.Sort();
        cardInfos.Reverse();
    }
    #endregion

    #region 倒计时
    private enum CountDownTypes
    {
        Bid,
        Follow
    }
    /// <summary>
    /// 用户考虑是否叫牌中
    /// </summary>
    private IEnumerator BidConsiderating()
    {
        //倒计时
        var time = consideratingTime;
        while (time > 0)
        {
            countDownText.text = time.ToString();

            yield return new WaitForSeconds(1);
            time--;
        }
        NotBid();
    }
    /// <summary>
    /// 用户考虑是否出牌中
    /// </summary>
    private IEnumerator FollowConsiderating()
    {
        //倒计时
        var time = consideratingTime;
        while (time > 0)
        {
            countDownText.text = time.ToString();

            yield return new WaitForSeconds(1);
            time--;
        }
        NotFollow();
    }
    /// <summary>
    /// 开始倒计时
    /// </summary>
    private void StartCountDown(CountDownTypes countDownType)
    {
        countDownText.transform.parent.gameObject.SetActive(true);
        if (countDownType == CountDownTypes.Bid)
        {
            StartCoroutine("BidConsiderating");
        }
        if (countDownType == CountDownTypes.Follow)
        {
            StartCoroutine("FollowConsiderating");
        }
    }
    /// <summary>
    /// 停止倒计时
    /// </summary>
    private void StopCountDown(CountDownTypes countDownType)
    {
        countDownText.transform.parent.gameObject.SetActive(false);
        if (countDownType == CountDownTypes.Bid)
        {
            StopCoroutine("BidConsiderating");
        }
        if (countDownType == CountDownTypes.Follow)
        {
            StopCoroutine("FollowConsiderating");
        }
    }
    #endregion

    #region 叫牌逻辑
    /// <summary>
    /// 开始叫地主
    /// </summary>
    public virtual void ToBiding()
    {
        isMyTerm = true;

        //开始倒计时
        StartCountDown(CountDownTypes.Bid);
    }
    /// <summary>
    /// 抢地主
    /// </summary>
    public void ForBid()
    {
        //关闭倒计时
        StopCountDown(CountDownTypes.Bid);

        CardManager._instance.ForBid();
        isMyTerm = false;
    }
    /// <summary>
    /// 不抢地主
    /// </summary>
    public void NotBid()
    {
        //关闭倒计时
        StopCountDown(CountDownTypes.Bid);

        CardManager._instance.NotBid();
        isMyTerm = false;
    }

    #endregion

    #region 出牌逻辑
    /// <summary>
    /// 开始出牌
    /// </summary>
    public virtual void ToFollowing()
    {
        isMyTerm = true;

        //关闭倒计时
        StopCountDown(CountDownTypes.Follow);

        //开始倒计时
        StartCountDown(CountDownTypes.Follow);
    }
    /// <summary>
    /// 出牌
    /// </summary>
    public void ForFollow()
    {
        //关闭倒计时
        StopCountDown(CountDownTypes.Follow);

        CardManager._instance.ForFollow();
        isMyTerm = false;
    }
    /// <summary>
    /// 不出
    /// </summary>
    public void NotFollow()
    {
        //关闭倒计时
        StopCountDown(CountDownTypes.Follow);

        CardManager._instance.NotFollow();
        isMyTerm = false;
    }
    #endregion

}


