using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private Image image;        //牌的图片
    private CardInfo cardInfo;  //卡牌信息
    private float offsetY;

    void Awake()
    {
        image = GetComponent<Image>();

        //计算世界坐标系移动的位置
        var distance = GetComponent<RectTransform>().sizeDelta.y / 10;
        var pos1 = transform.position;
        var pos2 = transform.position + Vector3.up * distance;
        var pos11 = transform.TransformPoint(pos1);
        var pos22 = transform.TransformPoint(pos2);
        offsetY = pos22.y - pos11.y;
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
            if (cardInfo.isSelected)
            {
                transform.DOMoveY(transform.position.y - offsetY, 0.5f);
            }
            else
            {
                transform.DOMoveY(transform.position.y + offsetY, 0.5f);
            }
            cardInfo.isSelected = !cardInfo.isSelected;
        }

    }
}
