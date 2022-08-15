using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WuXingTool;
using WuXingCore;
using WuXingSkill;
using WuXingAssetManage;

namespace WuXingBase
{
    public class CardSite
    {
        private Dictionary<SanCai, WuXing> m_siteProperty;//场地上三个盘的属性
        private Dictionary<SanCai, Dictionary<WuXing, GameObject>> m_siteCard;//场地上三个盘各个属性的牌
        private Dictionary<SanCai, GameObject> m_firstCard;//本回合始牌
        private Dictionary<SanCai, GameObject> m_lastCard;//本回合终牌
        private SiteConstraint m_siteConstraint;//卡牌登场约束
        private Dictionary<SanCai, Dictionary<WuXing, GameObject>> m_siteSignal;

        public CardSite(PlayerType playerType)
        {
            m_siteProperty = new Dictionary<SanCai, WuXing>();
            m_siteCard = new Dictionary<SanCai, Dictionary<WuXing, GameObject>>();
            m_firstCard = new Dictionary<SanCai, GameObject>();
            m_lastCard = new Dictionary<SanCai, GameObject>();
            for(int i= 0; i<3; i++)
            {
                m_siteProperty[(SanCai)i] = WuXing.Chaos;
                m_firstCard[(SanCai)i] = null;
                m_lastCard[(SanCai)i] = null;
                m_siteCard[(SanCai)i] = new Dictionary<WuXing, GameObject>();
                for(int j=0; j<5; j++)
                {
                    m_siteCard[(SanCai)i][(WuXing)j] = null;
                }
            }

            m_siteConstraint = new SiteConstraint();
            m_siteSignal = AssertManager.GetSiteSignal(playerType);
        }



        /// <summary>
        /// 将某张牌添加到场上
        /// </summary>
        /// <param name="targetCardTrans"></param>
        /// <param name="targrtSiteTrans"></param>
        /// <returns></returns>
        public bool AddCard(GameObject targetCard, GameObject targetSite)
        {
            Card site = targetSite.GetComponent<Card>();
            Card card = targetCard.GetComponent<Card>();
            WuXing wuXing = site.GetBaseProprty();
            SanCai sanCai = site.GetBaseOwner();


            m_siteCard[sanCai][wuXing] = targetCard;
            m_siteProperty[sanCai] = wuXing;
            DisplayActor.DisplayFieldProperty(sanCai, wuXing, site.GetPlayerType());

            card.SetPlayerType(site.GetPlayerType());
            card.SetCardType(CardType.SiteCard);
            card.SetRegardOwner(new List<SanCai>(){ sanCai});
            card.SetRegardProperty(new List<WuXing>() { wuXing });

            if (m_firstCard[sanCai] == null)
            {
                card.SetTriggerPhase(TriggerPhase.Initial);
                m_firstCard[sanCai] = card.gameObject;
            }
            else
            {
                card.SetTriggerPhase(TriggerPhase.Normal);
                m_lastCard[sanCai] = card.gameObject;
            }


 
            m_siteConstraint.CostTime(targetCard, targetSite);

            MoveCardToSite(targetCard, targetSite);

            return true;
        }
        /// <summary>
        /// 将某张牌添加到场上，但是不改变盘的属性，也不计入始牌终牌
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public bool AddCardFree(GameObject targetCard, GameObject targetSite)
        {
            Card site = targetSite.GetComponent<Card>();
            Card card = targetCard.GetComponent<Card>();
            WuXing wuXing = site.GetBaseProprty();
            SanCai sanCai = site.GetBaseOwner();
            m_siteCard[sanCai][wuXing] = targetCard;

            card.SetPlayerType(site.GetPlayerType());
            card.SetCardType(CardType.SiteCard);
            card.SetRegardOwner(new List<SanCai>() { sanCai });
            card.SetRegardProperty(new List<WuXing>() { wuXing });

            card.SetTriggerPhase(TriggerPhase.Normal);


            m_siteConstraint.CostTime(targetCard, targetSite);

            MoveCardToSite(targetCard, targetSite);

            return true;
        }
        /// <summary>
        /// 移除对应场地的牌/对应的牌
        /// </summary>
        /// <param name="targrtSiteTrans"></param>
        /// <returns></returns>
        public GameObject RemoveCard(GameObject targetSite, GameObject targetCard)
        {
            if(targetSite == null && targetCard != null)
            {
                targetSite = targetCard.GetComponent<Transform>().parent.gameObject;
            }

            if(targetSite == null)
                return null;

            Card site = targetSite.GetComponent<Card>();
            WuXing wuXing = site.GetBaseProprty();
            SanCai sanCai = site.GetBaseOwner();

            GameObject gameObject = m_siteCard[sanCai][wuXing];

            m_siteCard[sanCai][wuXing] = null;

            ReCalculationSiteProperty(sanCai);



            return gameObject;
        }



        private bool IsMatchIndex(GameObject targetCard, GameObject targetSite, int index)
        {
            switch(index)
            {
                case 1:
                    return IsMatchCard(targetCard, targetSite);
                case 2:
                    return IsMatchSite(targetSite);
                default:
                    return true;
            }
        }
        /// <summary>
        /// 所选卡牌与所选场地是否匹配
        /// </summary>
        /// <param name="handCardTrans"></param>
        /// <param name="siteCardTrans"></param>
        /// <returns></returns>
        private bool IsMatchCard(GameObject targetCard, GameObject targetSite)
        {
            Card handCard = targetCard.GetComponent<Card>();
            Card siteCard = targetSite.GetComponent<Card>();

            if (handCard == null || siteCard == null)
            {
                int pp = 1;
            }
                

            return handCard.IsMatchOwner(siteCard.GetBaseOwner()) && handCard.IsMatchProperty(siteCard.GetBaseProprty());
        }
        /// <summary>
        /// 所选场地与盘属性是否匹配（放置卡牌符合五行相生规则）
        /// </summary>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        private bool IsMatchSite(GameObject targetSite)
        {
            Card siteCard = targetSite.GetComponent<Card>();
            WuXing currentWuXing = m_siteProperty[siteCard.GetBaseOwner()];
            if(currentWuXing == WuXing.Chaos || PropertyTool.WuXingGeneration(currentWuXing) == siteCard.GetBaseProprty())
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 所选卡牌与场地是否满足场地约束
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public bool IsCardMatchSite(GameObject targetCard, GameObject targetSite)
        {
            int k = m_siteConstraint.IsComeEnable(targetCard, targetSite, 0);
            switch (k)
            {
                case 1:
                    return true;
                case -1:
                    return false;
                default:
                    break;
            }

            for (int i=1; i<3; i++)
            {
                k = m_siteConstraint.IsComeEnable(targetCard, targetSite, i);
                switch (k)
                {
                    case 1:
                        continue;
                    case -1:
                        return false;
                    default:
                        if (IsMatchIndex(targetCard, targetSite, i))
                            continue;
                        else
                            return false;
                }
            }
            
            return true;
        }
        /// <summary>
        /// 所选卡牌与场地是否满足场地约束（不需要按照五行相生放置）
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public bool IsCardMatchSiteFree(GameObject targetCard, GameObject targetSite)
        {
            int k = m_siteConstraint.IsComeEnable(targetCard, targetSite, 0);
            switch (k)
            {
                case 1:
                    return true;
                case -1:
                    return false;
                default:
                    break;
            }

            for (int i = 1; i < 2; i++)
            {
                k = m_siteConstraint.IsComeEnable(targetCard, targetSite, i);
                switch (k)
                {
                    case 1:
                        continue;
                    case -1:
                        return false;
                    default:
                        if (IsMatchIndex(targetCard, targetSite, i))
                            continue;
                        else
                            return false;
                }
            }

            return true;
        }



        /// <summary>
        /// 返回所选盘上当前盘属性对应的卡牌
        /// </summary>
        /// <param name="sanCai"></param>
        /// <returns></returns>
        public GameObject GetSiteCardCurrent(SanCai sanCai)
        {
            if (m_siteProperty[sanCai] != WuXing.Chaos)
                return m_siteCard[sanCai][m_siteProperty[sanCai]];
            return null;
        }
        /// <summary>
        /// 返回场地上对应从属和属性的牌
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public GameObject GetSiteCard(SanCai sanCai, WuXing wuXing)
        {
            if (wuXing != WuXing.Chaos)
                return m_siteCard[sanCai][wuXing];
            return null;
        }
        /// <summary>
        /// 返回场地上对应从属的牌
        /// </summary>
        /// <param name="sanCai"></param>
        /// <returns></returns>
        public List<GameObject> GetSiteCardOwner(SanCai sanCai)
        {
            List<GameObject> list = new List<GameObject>();
            for(int i=0; i<5; i++)
            {
                if (m_siteCard[sanCai][(WuXing)i] != null)
                {
                    list.Add(m_siteCard[sanCai][(WuXing)i]);
                }
            }
            
            return list;
        }
        /// <summary>
        /// 返回场地上对应属性的牌
        /// </summary>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public List<GameObject> GetSiteCardProperty(WuXing wuXing)
        {
            List<GameObject> list = new List<GameObject>();
            for(int i=0; i<3; i++)
            {
                list.Add(m_siteCard[(SanCai)i][wuXing]);
            }

            return list;
        }
        /// <summary>
        /// 返回场地上所有卡牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetSiteCardAll()
        {
            List<GameObject> list = new List<GameObject>();
            for(int i=0; i<3; i++)
            {
                for(int j=0; j<5; j++)
                {
                    if (m_siteCard[(SanCai)i][(WuXing)j] != null)
                    {
                        list.Add(m_siteCard[(SanCai)i][(WuXing)j]);
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// 返回场上所有主角牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetSiteCardLeader()
        {
            List<GameObject> list = new List<GameObject>();
            for(int i=0; i<3; i++)
            {
                for(int j=0; j<5; j++)
                {
                    if (m_siteCard[(SanCai)i][(WuXing)j] != null)
                    {
                        Card card = m_siteCard[(SanCai)i][(WuXing)j].GetComponent<Card>();
                        if (card.GetCardCategory() == CardCategory.LeaderCard)
                        {
                            list.Add(m_siteCard[(SanCai)i][(WuXing)j]);
                        }
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// 返回场地
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetSiteSignal()
        {
            List<GameObject> list = new List<GameObject>();
            for(int i=0; i<3; i++)
            {
                for(int j=0; j<5; j++)
                {
                    list.Add(m_siteSignal[(SanCai)i][(WuXing)j]);
                }
            }
            return list;
        }


        /// <summary>
        /// 设置盘的属性
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public bool SetSiteProperty(SanCai sanCai, WuXing wuXing)
        {
            if(m_siteProperty[sanCai] != WuXing.Chaos)
            {
                m_siteProperty[sanCai] = wuXing;
                DisplayActor.DisplayFieldProperty(sanCai, wuXing, m_siteSignal[sanCai][wuXing].GetComponent<Card>().GetPlayerType());
            }
            return true;
        }
        /// <summary>
        /// 设置场地约束
        /// </summary>
        /// <param name="siteConstraint"></param>
        /// <returns></returns>
        public bool SetSiteContraint(SiteConstraint siteConstraint)
        {
            m_siteConstraint.SetConstraint(siteConstraint);
            return true;
        }
        /// <summary>
        /// 设置终牌
        /// </summary>
        /// <returns></returns>
        public bool SetFinalCard()
        {
            for(int i=0; i<3; i++)
            {
                GameObject targetCard = m_lastCard[(SanCai)i];
                if(targetCard != null)
                {
                    targetCard.GetComponent<Card>().SetTriggerPhase(TriggerPhase.Final);
                }
            }
            return true;
        }
        /// <summary>
        /// 取消终牌和始牌设置
        /// </summary>
        /// <returns></returns>
        public bool CancelCard()
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject targetCard = m_lastCard[(SanCai)i];
                if (targetCard != null)
                {
                    targetCard.GetComponent<Card>().SetTriggerPhase(TriggerPhase.Normal);
                }

                targetCard = m_firstCard[(SanCai)i];
                if (targetCard != null)
                {
                    targetCard.GetComponent<Card>().SetTriggerPhase(TriggerPhase.Normal);
                }
            }
            return true;
        }


        /// <summary>
        /// 重新计算盘的属性
        /// </summary>
        /// <param name="sanCai"></param>
        /// <returns></returns>
        public bool ReCalculationSiteProperty(SanCai sanCai)
        {
            bool isEmptySite = true;
            for(int i=0; i<5; i++)
            {
                if (m_siteCard[sanCai][(WuXing)i] != null)
                {
                    isEmptySite = false;
                    break;
                }
            }

            if (isEmptySite)
            {
                m_siteProperty[sanCai] = WuXing.Chaos;

                PlayerType playerType = m_siteSignal[SanCai.Sky][WuXing.Metal].GetComponent<Card>().GetPlayerType();
                DisplayActor.DisplayFieldProperty(sanCai, WuXing.Chaos, playerType);
            }
                
            return true;
        }
        


        /// <summary>
        /// 移动卡牌到场地上
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        private bool MoveCardToSite(GameObject targetCard, GameObject targetSite)
        {
            Transform targetCardTrans = targetCard.GetComponent<Transform>();
            Transform targetSiteTrans = targetSite.GetComponent<Transform>();

            targetCardTrans.SetParent(targetSiteTrans);
            targetCardTrans.GetComponent<RectTransform>().pivot = Vector2.one / 2;
            targetCardTrans.localPosition = Vector3.zero;
            targetCardTrans.localScale = Vector3.one;
            targetCardTrans.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 25);
            targetCardTrans.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50);

            

            return true;
        }
        

        /// <summary>
        /// 更新场地
        /// </summary>
        /// <returns></returns>
        public bool UpdateSite()
        {
            m_siteConstraint.TurnUpdate();
            ResetSiteState();
            return true;
        }
        private bool ResetSiteState()
        {
            for (int i = 0; i < 3; i++)
            {
                m_firstCard[(SanCai)i] = null;
                m_lastCard[(SanCai)i] = null;
            }
            return true;
        }
    }
}

