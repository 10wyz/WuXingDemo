using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingAssetManage;
using WuXingTool;
using WuXingCore;

namespace WuXingBase
{
    public class EpochCard
    {
        private List<GameObject> m_epochCard;
        private Dictionary<SanCai, GameObject> m_epochCardinal;//纪行位置
        private Dictionary<SanCai, List<GameObject>> m_activeEpoch;//激活纪行
        private GameObject m_epochStore;//事纪存储库
        public EpochCard()
        {
            m_epochCard = new List<GameObject>();
            m_epochCardinal = null;

            m_activeEpoch = new Dictionary<SanCai, List<GameObject>>();
            for(int i = 0; i < 3; i++)
            {
                m_activeEpoch[(SanCai)i] = new List<GameObject>();
            }
        }

        public void LoadCard(List<CardInfo> cardInfos, PlayerType playerType, ScreenPlay screenPlay, SkillControl skillControl)
        {
            m_epochStore = GameObject.Find("EpochStore");
            for (int i = 0; i < cardInfos.Count; i++)
            {
                if(screenPlay.IsMatchPlayer(cardInfos[i].m_ID, playerType))
                {
                    m_epochCard.Add(CardCreator.CreateEpochCard(cardInfos[i], CardType.HandCard, playerType, skillControl));
                    m_epochCard[m_epochCard.Count - 1].transform.SetParent(m_epochStore.transform);
                    m_epochCard[m_epochCard.Count - 1].GetComponent<RectTransform>().localPosition = new Vector3(1000, 1000, 0);
                }
            }

            m_epochCardinal = AssertManager.GetEpochSignal(playerType);
        }
        public void LoginNetwork()
        {
            for(int i = 0; i < m_epochCard.Count; i++)
            {
                GameListener.Instance().GetGameNetworkManager().AddCard(m_epochCard[i]);
            }
        }


        /// <summary>
        /// 激活纪行
        /// </summary>
        /// <param name="targetEpoch"></param>
        /// <returns></returns>
        public bool ActiveEpoch(GameObject targetEpoch)
        {
            if(m_epochCard.Contains(targetEpoch))
            {
                Card card = targetEpoch.GetComponent<Card>();
                if(!m_activeEpoch[card.GetBaseOwner()].Contains(targetEpoch))
                {
                    card.SetCardType(CardType.SiteCard);
                    m_activeEpoch[card.GetBaseOwner()].Add(targetEpoch);
                    AllignEpoch(card.GetBaseOwner());

                    //card.OnAction(null, targetEpoch, WuXingSkill.TriggerType.Come);
                }
            }

            return true;
        }
        /// <summary>
        /// 结束纪行
        /// </summary>
        /// <param name="targetEpoch"></param>
        /// <returns></returns>
        public bool CloseEpoch(GameObject targetEpoch)
        {
            if (m_epochCard.Contains(targetEpoch))
            {
                Card card = targetEpoch.GetComponent<Card>();
                card.SetCardType(CardType.HandCard);
                targetEpoch.transform.SetParent(m_epochStore.transform);
                targetEpoch.GetComponent<Transform>().localPosition = new Vector3(1000, 1000, 0);
                m_activeEpoch[card.GetBaseOwner()].Remove(targetEpoch);
                AllignEpoch(card.GetBaseOwner());

                //card.OnAction(null, targetEpoch, WuXingSkill.TriggerType.Left);
            }

            return true;
        }


        public List<GameObject> GetEpochCard()
        {
            return m_epochCard;
        }

        private void AllignEpoch(SanCai sanCai)
        {
            if (m_activeEpoch[sanCai].Count == 0)
                return;

            float width = AssertManager.CardWidth / 2 * 1.1f; 

            GameObject site = m_epochCardinal[sanCai];
            for(int i=0; i<m_activeEpoch[sanCai].Count; i++)
            {
                GameObject epoch = m_activeEpoch[sanCai][i];

                epoch.GetComponent<RectTransform>().SetParent(site.transform);
                epoch.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, AssertManager.CardWidth / 2);
                epoch.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, AssertManager.CardHeight / 2);
                epoch.GetComponent<RectTransform>().localPosition = new Vector3(-width * i, 0, 0);
                epoch.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
                epoch.GetComponent<RectTransform>().localScale = Vector3.one;
            }
        }
    }
}

