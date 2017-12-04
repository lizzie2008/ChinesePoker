using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models.FollowCards
{
    /// <summary>
    /// 单张
    /// </summary>
    public class SingleCards : FollowCardsBase
    {
        /// <summary>
        /// 验证类型
        /// </summary>
        /// <returns></returns>
        public override bool Validate(List<CardInfo> cardInfos)
        {
            cardInfos.Sort();

            if (cardInfos.Count == 1)   //单张
            {
                return true;
            }
            else if (cardInfos.Count >= 5)  //顺子
            {
                //如果最大的牌是王或者2，则不是顺子
                if (cardInfos.Last().cardType == CardTypes.Joker || cardInfos.Last().cardIndex == 12)
                    return false;

                for (int i = 0; i < cardInfos.Count - 2; i++)
                {
                    if (cardInfos[i].cardIndex + 1 != cardInfos[i + 1].cardIndex)
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 找到最小满足的牌组
        /// </summary>
        /// <returns></returns>
        public override List<CardInfo> FindBigger(List<CardInfo> handCardInfos, List<CardInfo> cardInfos)
        {
            cardInfos.Sort();
            handCardInfos.Sort();

            if (cardInfos.Count == 1)   //单张
            {
                var cardInfo = handCardInfos.FirstOrDefault(s => s.CompareTo(cardInfos[0]) > 0 && s.cardIndex != cardInfos[0].cardIndex);
                if (cardInfo != null)
                {
                    var result = new List<CardInfo>();
                    result.Add(cardInfo);
                    return result;
                }
                return null;
            }

            else if (cardInfos.Count >= 5)  //顺子
            {
                var count = handCardInfos.Count - cardInfos.Count;

                if (count >= 0)
                {
                    //手牌比牌组多count，则有count + 1可能满足牌组
                    for (int i = 0; i < count + 1; i++)
                    {
                        var mayBiggerCardInfos = handCardInfos.Skip(i).Take(cardInfos.Count).ToList();
                        //是顺子，且最小的牌比要比较的牌组最小牌要大
                        if (Validate(mayBiggerCardInfos) && mayBiggerCardInfos[0].CompareTo(cardInfos[0]) > 0 && mayBiggerCardInfos[0].cardIndex != cardInfos[0].cardIndex)

                        {
                            return mayBiggerCardInfos;
                        }
                    }
                    return null;
                }
                return null;
            }
            return null;
        }
        /// <summary>
        /// 判断是否牌大过要比较的牌组
        /// </summary>
        /// <param name="handCardInfos"></param>
        /// <param name="cardInfos"></param>
        /// <returns></returns>
        public override bool IsBigger(List<CardInfo> handCardInfos, List<CardInfo> cardInfos)
        {
            cardInfos.Sort();
            handCardInfos.Sort();

            //牌数一样且最小牌比要比较的牌组的最小牌大
            if (handCardInfos.Count == cardInfos.Count && Validate(handCardInfos) && Validate(cardInfos))
            {
                if (handCardInfos[0].CompareTo(cardInfos[0]) > 0)
                    return true;
            }
            return false;
        }
    }
}
