using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;

namespace WuXingManual
{
    public class SiteInfo : MonoBehaviour
    {
        [SerializeField]
        public string m_cardName;//卡牌名称
        public SanCai m_cardOwner;//卡牌三才从属
        public WuXing m_cardProperty;//卡牌五行属性
        public CardType m_cardType; //卡牌类型
        public PlayerType m_playerType;//卡牌拥有者

        // Start is called before the first frame update
        void Start()
        {
            CardInfo cardInfo = new CardInfo("", m_cardName, PropertyTool.SanCaiToString(m_cardOwner), PropertyTool.WuXingToString(m_cardProperty), new List<string>(), null);
            this.GetComponent<Card>().Initial(cardInfo, m_cardType, m_playerType, CardCategory.FakeCard, null, true);
        }

        // Update is called once per frame
    }
}

