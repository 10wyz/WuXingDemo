using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;


namespace WuXingSkill
{
    public class SpecialCardCondition
    {
        public static bool IsMatch(GameObject origin, GameObject target, int index)
        {
            switch (index)
            {
                case 0:
                    return IsMatch00(target);
                case 1:
                    return IsMatch01(target);
                case 2:
                    return IsMatch02(target);
                case 3:
                    return IsMatch03(target);
                default:
                    return false;
            }
        }
        public static bool IsMatch(CardInfo cardInfo, int index)
        {
            switch (index)
            {
                default:
                    return false;
            }
        }


        /// <summary>
        /// 目标卡牌被视为的属性和基础属性不一致
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static bool IsMatch00(GameObject target)
        {
            Card card = target.GetComponent<Card>();
            if (card.GetBaseProprty() != card.GetRegardProperty()[0])
                return true;
            return false;
        }
        /// <summary>
        /// 目标卡牌是所处盘上的主导牌
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static bool IsMatch01(GameObject target)
        {
            Card card = target.GetComponent<Card>();
            GamePlayer player = GameListener.Instance().GetPlayer(target);
            if (player.GetSiteCardCurrent(card.GetRegardOwner()[0]) == target)
                return true;

            return false;
        }
        /// <summary>
        /// 目标卡牌是始牌
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static bool IsMatch02(GameObject target)
        {
            Card card = target.GetComponent<Card>();
            if (card.GetTriggerPhase() == TriggerPhase.Initial)
                return true;

            return false;
        }
        /// <summary>
        /// 包含破坏技能
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static bool IsMatch03(GameObject target)
        {
            Transform skillItem01 = target.transform.Find("Skill/SkillItem01");
            if (skillItem01 != null)
                return true;

            return false;
        }
    }
}

