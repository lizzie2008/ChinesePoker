using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class Player : MonoBehaviour
{
    #region 初始化
    public GameObject prefabSmall;   //预制件

    protected List<CardInfo> cardInfos = new List<CardInfo>();  //个人所持卡牌

    private Text cardCoutText;
    private Text countDownText;
    private int consideratingTime = 6; //玩家考虑时间
    protected bool isMyTerm = false;  //当前是否是自己回合

    private Transform smallCardPos;     //出牌的原始位置
    private List<GameObject> smallCards = new List<GameObject>();


    void Start()
    {
        cardCoutText = transform.Find("HeapPos/Text").GetComponent<Text>();
        countDownText = transform.Find("CountDown/Text").GetComponent<Text>();
        smallCardPos = transform.Find("SmallCardPos");
    } 
    #endregion

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

        //选择的牌，添加到出牌区域
        var selectedCards = cardInfos.Where(s => s.isSelected).ToList();
        var offset = 5;
        for (int i = 0; i < selectedCards.Count(); i++)
        {
            var card = Instantiate(prefabSmall, smallCardPos.position + Vector3.right * offset * i, Quaternion.identity, smallCardPos.transform);
            card.GetComponent<RectTransform>().localScale = Vector3.one * 0.3f;
            card.GetComponent<Image>().sprite = Resources.Load("Images/Cards/" + selectedCards[i].cardName, typeof(Sprite)) as Sprite;
            card.transform.SetAsLastSibling();

            smallCards.Add(card);
        }
        cardInfos = cardInfos.Where(s => !s.isSelected).ToList();

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
    /// <summary>
    /// 销毁出牌对象
    /// </summary>
    public void DropAllSmallCards()
    {
        smallCards.ForEach(Destroy);
        smallCards.Clear();
    }
    #endregion
}


