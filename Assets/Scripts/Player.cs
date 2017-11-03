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
    /// 用户考虑时间
    /// </summary>
    private IEnumerator Considerating()
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
    /// 开始叫地主
    /// </summary>
    public virtual void ToBiding()
    {
        isMyTerm = true;

        //开始倒计时
        countDownText.transform.parent.gameObject.SetActive(true);
        StartCoroutine("Considerating");
    }
    /// <summary>
    /// 抢地主
    /// </summary>
    public void ForBid()
    {
        //关闭倒计时
        countDownText.transform.parent.gameObject.SetActive(false);
        StopCoroutine("Considerating");

        CardManager._instance.ForBid();
        isMyTerm = false;
    }
    /// <summary>
    /// 不抢地主
    /// </summary>
    public void NotBid()
    {
        //关闭倒计时
        countDownText.transform.parent.gameObject.SetActive(false);
        StopCoroutine("Considerating");

        CardManager._instance.NotBid();
        isMyTerm = false;
    }
    /// <summary>
    /// 卡牌排序（从大到小）
    /// </summary>
    protected void Sort()
    {
        cardInfos.Sort();
        cardInfos.Reverse();
    }

}


