using UnityEngine;

public class PlayerOther : Player
{
    void Update()
    {
        //如果当前是自己回合，模拟对手叫牌
        if (isMyTerm)
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
    }
}
