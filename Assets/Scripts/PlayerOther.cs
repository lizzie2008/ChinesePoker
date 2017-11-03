using UnityEngine;

public class PlayerOther : Player
{
    void Update()
    {
        //如果当前是自己回合，模拟对手叫牌
        if (isMyTerm)
        {
            if (CardManager._instance.cardManagerState == CardManagerStates.Bid)
            {
                if (Input.GetKeyDown(KeyCode.Q))    //叫牌
                {
                    ForBid();
                }
                if (Input.GetKeyDown(KeyCode.W))    //不叫
                {
                    NotBid();
                }
            }
            if (CardManager._instance.cardManagerState == CardManagerStates.Playing)
            {
                if (Input.GetKeyDown(KeyCode.Q))    //出牌
                {
                    ForFollow();
                }
                if (Input.GetKeyDown(KeyCode.W))    //不出
                {
                    NotFollow();
                }
            }
        }
    }
}
