using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WuXingBase
{
    public enum WuXing { Metal, Wood, Water, Fire, Earth, Chaos, Epoch, Spell};
    public enum SanCai { Sky, Earth, Human };

    public class CardInfo
    {
        public string m_ID { set; get; }//卡牌ID
        public string m_cardName { set; get; }//卡牌名称
        public string m_cardOwner { set; get; }//卡牌三才从属
        public string m_cardProperty { set; get; }//卡牌五行属性
        public List<string> m_cardCamp { set; get; }//卡牌阵营

        public List<SkillInfo> m_skillInfo;//技能信息
        public CardInfo()
        {
            m_cardCamp = new List<string>();
            m_skillInfo = new List<SkillInfo>();
        }
        public CardInfo(string id, string name, string owner, string property, List<string> camp, List<SkillInfo> skillInfo)
        {
            m_ID = id;
            m_cardName = name;
            m_cardOwner = owner;
            m_cardProperty = property;
            m_cardCamp = camp;
            if (skillInfo != null)
                m_skillInfo = skillInfo;
            else
                m_skillInfo = new List<SkillInfo>();
        }
        /// <summary>
        /// 返回属性
        /// </summary>
        /// <returns></returns>
        public WuXing GetProperty()
        {
            switch (m_cardProperty)
            {
                case "金":
                    return WuXing.Metal;
                case "木":
                    return WuXing.Wood;
                case "水":
                    return WuXing.Water;
                case "火":
                    return WuXing.Fire;
                case "土":
                    return WuXing.Earth;
                default:
                    return WuXing.Chaos;
            }
        }
        /// <summary>
        /// 返回从属
        /// </summary>
        /// <returns></returns>
        public SanCai GetOwner()
        {
            switch(m_cardOwner)
            {
                case "天":
                    return SanCai.Sky;
                case "地":
                    return SanCai.Earth;
                case "人":
                    return SanCai.Human;
                default: 
                    return SanCai.Sky;
            }
        }
    }
}

