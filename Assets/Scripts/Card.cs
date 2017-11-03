using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private Image image;        //牌的图片
    private CardInfo cardInfo;  //卡牌信息
    private bool isSelected;    //是否选中

    void Awake()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// 初始化图片
    /// </summary>
    /// <param name="cardInfo"></param>
    public void InitImage(CardInfo cardInfo)
    {
        this.cardInfo = cardInfo;
        image.sprite = Resources.Load("Images/Cards/" + cardInfo.cardName, typeof(Sprite)) as Sprite;
    }
    /// <summary>
    /// 设置选择状态
    /// </summary>
    public void SetSelectState()
    {
        if (!DOTween.IsTweening(transform))
        {
            if (isSelected)
            {
                transform.DOMoveY(transform.position.y - 10f, 0.5f);
            }
            else
            {
                transform.DOMoveY(transform.position.y + 10f, 0.5f);
            }
            isSelected = !isSelected;
        }

    }
}
