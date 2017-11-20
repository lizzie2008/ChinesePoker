using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models
{
    /// <summary>
    /// 出牌类型基类
    /// </summary>
    public abstract class FollowCardsBase
    {
        public List<CardInfo> CardInfos = new List<CardInfo>();

        /// <summary>
        /// 验证类型
        /// </summary>
        /// <returns></returns>
        public abstract bool Validate(List<CardInfo> cardInfos);
        /// <summary>
        /// 找到最小满足的牌组
        /// </summary>
        /// <returns></returns>
        public abstract List<CardInfo> FindBigger(List<CardInfo> handCardInfos, List<CardInfo> cardInfos);
        /// <summary>
        /// 判断是否牌大过要比较的牌组
        /// </summary>
        /// <param name="handCardInfos"></param>
        /// <param name="cardInfos"></param>
        /// <returns></returns>
        public abstract bool IsBigger(List<CardInfo> handCardInfos, List<CardInfo> cardInfos);
    }
}
