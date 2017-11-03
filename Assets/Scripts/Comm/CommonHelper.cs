using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CommonHelper
{



}

public enum CardTypes
{
    //大小王
    Joker,
    //红桃
    Hearts,
    //黑桃
    Spades,
    //方片
    Diamonds,
    //梅花
    Clubs
}
public enum CardManagerStates
{
    //准备
    Ready,
    //洗牌
    ShuffleCards,
    //发牌
    DealCards,
    //叫牌
    Bid,
    //出牌
    Playing
}
