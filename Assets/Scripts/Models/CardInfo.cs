using System;
using UnityEngine;

public class CardInfo : IComparable
{
    public string cardName; //卡牌图片名
    public CardTypes cardType; //牌的类型
    public int cardIndex;      //牌在所在类型的索引3-10,J,Q,K,A,2(0-12)
    public bool isSelected;    //是否选中


    public CardInfo(string cardName)
    {
        this.cardName = cardName;
        var splits = cardName.Split('_');

        switch (splits[1])
        {
            case "1":
                cardType = CardTypes.Hearts;
                cardIndex = (int.Parse(splits[2]) + 10) % 13;
                break;
            case "2":
                cardType = CardTypes.Spades;
                cardIndex = (int.Parse(splits[2]) + 10) % 13;
                break;
            case "3":
                cardType = CardTypes.Diamonds;
                cardIndex = (int.Parse(splits[2]) + 10) % 13;
                break;
            case "4":
                cardType = CardTypes.Clubs;
                cardIndex = (int.Parse(splits[2]) + 10) % 13;
                break;
            case "joker":
                cardType = CardTypes.Joker;
                cardIndex = (int.Parse(splits[2]) + 10) % 13;
                break;
            default:
                throw new Exception(string.Format("卡牌文件名{0}非法！", cardName));
        }
    }

    //卡牌大小比较
    public int CompareTo(object obj)
    {
        CardInfo other = obj as CardInfo;

        if (other == null)
            throw new Exception("比较对象类型非法！");

        //如果当前是大小王
        if (cardType == CardTypes.Joker)
        {
            //对方也是大小王
            if (other.cardType == CardTypes.Joker)
            {
                return cardIndex.CompareTo(other.cardIndex);
            }
            //对方不是大小王
            return 1;
        }
        //如果是一般的牌
        else
        {
            //对方是大小王
            if (other.cardType == CardTypes.Joker)
            {
                return -1;
            }
            //如果对方也是一般的牌
            else
            {
                //计算牌力
                if (cardIndex == other.cardIndex)
                {
                    return -cardType.CompareTo(other.cardType);
                }

                return cardIndex.CompareTo(other.cardIndex);
            }
        }
    }

}
