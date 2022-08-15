using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingCore;

namespace WuXingBase
{
    public class DeadCard
    {
        GameObject m_deadSignal;
        List<GameObject> m_deadCard;


        public DeadCard(GameObject gameObject)
        {
            m_deadSignal = gameObject;
            m_deadCard = new List<GameObject>();
        }



        public bool AddCard(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            if(card.GetCardType() == CardType.HandCard)
                card.SetCardType(CardType.DeadHand);
            else if(card.GetCardType() == CardType.SiteCard)
                card.SetCardType(CardType.DeadSite);

            targetCard.transform.SetParent(null);
            //targetCard.transform.localPosition = new Vector3(10000, 10000, 0);
            m_deadCard.Add(targetCard);
            return true;
        }
        public bool RemoveCard(GameObject targetCard)
        {
            return m_deadCard.Remove(targetCard);
        }


        /// <summary>
        /// 清空死亡牌堆（销毁其中的所有卡牌）
        /// </summary>
        /// <returns></returns>
        public bool ClearCard(GamePlayer player)
        {
            for(int i = 0; i < m_deadCard.Count; i++)
            {
                player.AddFold(m_deadCard[i]);
                GameListener.Instance().GetSkillControl().RemoveChainSkill(m_deadCard[i]);
                GameObject.Destroy(m_deadCard[i]);
            }
            m_deadCard.Clear();
            return true;
        }



        public List<GameObject> GetDeadCard()
        {
            return m_deadCard;
        }
        public GameObject GetDeadSignal()
        {
            return m_deadSignal;
        }
    }
}

