using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingAssetManage;
using WuXingCore;

namespace WuXingBase
{
    public class HandCard
    {
        private List<GameObject> m_handRoleCard;//角色牌
        private List<GameObject> m_handLeaderCard;//主角牌
        private List<GameObject> m_handSpellCard;//法术牌
        private Transform m_handTransform;//手牌区位置
        private int m_maxHandCardNum;//最大角色卡牌数量

        public  HandCard(Transform handTrans)
        {
            m_handRoleCard = new List<GameObject>();
            m_handLeaderCard = new List<GameObject>();
            m_handSpellCard = new List<GameObject>();
            m_handTransform = handTrans;
            m_maxHandCardNum = 5;
        }


        /// <summary>
        /// 将卡加入手牌
        /// </summary>
        /// <param name="card"></param>
        /// <param name="cardCategory"></param>
        /// <returns></returns>
        public bool AddCard(GameObject targetCard, PlayerType playerType)
        {
            Card card = targetCard.GetComponent<Card>();

            switch (card.GetCardCategory())
            {
                case CardCategory.LeaderCard:
                    m_handLeaderCard.Add(targetCard);
                    break;
                case CardCategory.ExtraCard:
                case CardCategory.RoleCard:
                    m_handRoleCard.Add(targetCard);
                    break;
                case CardCategory.SpellCard:
                    m_handSpellCard.Add(targetCard);
                    break;
                default:
                    return false;
            }

            card.SetPlayerType(playerType);
            card.SetCardType(CardType.HandCard);
            card.ClearResistance();
            card.ClearSkillConstraint();
            card.ClearSkillImmune();
            card.ClearSkillTime();

            card.GetComponent<Transform>().SetParent(m_handTransform);
            card.GetComponent<Transform>().localRotation = Quaternion.Euler(0, 0, 0);
            card.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, AssertManager.CardWidth);
            card.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, AssertManager.CardHeight);
            card.GetComponent<Transform>().localScale = Vector3.one;


            if(card.GetPlayerType() == PlayerType.Enemy)
                DisplayActor.HideHandCard(targetCard);

            SettleHandCard();

            return true;
        }
        /// <summary>
        /// 将卡从手牌移除
        /// </summary>
        /// <param name="targetCardTrans"></param>
        /// <returns></returns>
        public bool RemoveCard(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            switch (card.GetCardCategory())
            {
                case CardCategory.LeaderCard:
                    m_handLeaderCard.Remove(targetCard);
                    break;
                case CardCategory.ExtraCard:
                case CardCategory.RoleCard:
                    m_handRoleCard.Remove(targetCard);
                    break;
                case CardCategory.SpellCard:
                    m_handSpellCard.Remove(targetCard);
                    break;
                default:
                    break;
            }

            DisplayActor.DisplayHandCard(targetCard);

            SettleHandCard();
            return true;
        }


        /// <summary>
        /// 返回手牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetHandCard()
        {
            List<GameObject> card = new List<GameObject>();
            card.AddRange(m_handRoleCard);
            card.AddRange(m_handLeaderCard);
            card.AddRange(m_handSpellCard);
            return card;
        }
        /// <summary>
        /// 返回手卡位置
        /// </summary>
        /// <returns></returns>
        public Transform GetHandPosition()
        {
            return m_handTransform;
        }
        /// <summary>
        /// 返回非主角角色牌数量
        /// </summary>
        /// <returns></returns>
        public int GetRoleCardNum()
        {
            return m_handRoleCard.Count;
        }


        /// <summary>
        /// 整理手牌
        /// </summary>
        public void SettleHandCard()
        {
            List<GameObject> allCard = new List<GameObject>();
            allCard.AddRange(m_handSpellCard);
            allCard.AddRange(m_handLeaderCard);
            allCard.AddRange(m_handRoleCard);
            
            if (allCard.Count <= 0)
                return;

            float cardWidth = allCard[0].GetComponent<RectTransform>().rect.width;
            if (cardWidth* 1.1f * allCard.Count > 900)
            {
                float width = (900f - cardWidth) / (allCard.Count - 1);
                for (int j = 0; j < allCard.Count; j++)
                {
                    float x = (j - (allCard.Count - 1) / 2f) * width;

                    RectTransform trans = allCard[j].GetComponent<RectTransform>();
                    trans.anchorMin = Vector2.one / 2;
                    trans.anchorMax = Vector2.one / 2;
                    trans.pivot = Vector2.one / 2;
                    trans.localScale = Vector3.one;
                    trans.localPosition = new Vector3(x, 0, 0);
                }
            }
            else
            {
                float width = cardWidth * 1.1f;
                for (int j = 0; j < allCard.Count; j++)
                {
                    float x = (j - (allCard.Count - 1) / 2f) * width;

                    RectTransform trans = allCard[j].GetComponent<RectTransform>();
                    trans.anchorMin = Vector2.one / 2;
                    trans.anchorMax = Vector2.one / 2;
                    trans.pivot = Vector2.one / 2;
                    trans.localScale = Vector3.one;
                    trans.localPosition = new Vector3(x, 0, 0);
                }
            }
        }

        

        /// <summary>
        /// 手牌是否达到上限
        /// </summary>
        /// <returns></returns>
        public bool IsHandCardMax()
        {
            return m_handRoleCard.Count >= m_maxHandCardNum;
        }
    }
}

