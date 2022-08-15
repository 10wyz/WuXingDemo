using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;
using WuXingAssetManage;

namespace WuXingCore
{
    public class GamePlayer
    {
        private Dictionary<string, int> m_campMemberNum;//玩家场上各个阵营的数量
        private CardSite m_siteCard;//场地上的牌
        private HandCard m_handCard;//手牌
        private DeadCard m_deadCard;//死亡卡牌（每回合清空）
        private EpochCard m_epochCard;//纪行卡牌
        private PlayerType m_playerType;//玩家属性
        private CardDeck m_deck;//玩家卡组
        private SkillControl m_skillControl;//连锁条目表


        public GamePlayer(Transform handTrans, PlayerType playerType, CardDeck cardDeck, EpochCard epochCard,SkillControl skillControl)
        {
            m_handCard = new HandCard(handTrans);
            m_siteCard = new CardSite(playerType);
            m_deadCard = new DeadCard(handTrans.parent.Find("ExtraCard").gameObject);
            m_epochCard = epochCard;
            m_campMemberNum = new Dictionary<string, int>();
            m_playerType = playerType;
            m_deck = cardDeck;
            m_skillControl = skillControl;
        }



        /// <summary>
        /// 抽角色卡
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public GameObject DrawRoleCard(GameObject targetDeck)
        {
            if (m_handCard.IsHandCardMax())
                return null;

            Card card = targetDeck.GetComponent<Card>();
            SanCai sanCai = card.GetBaseOwner();
            WuXing wuXing = card.GetBaseProprty();
            CardInfo cardInfo = m_deck.DrawRoleCard(sanCai, wuXing);
            if(cardInfo != null)
            {
                GameObject newcard = CardCreator.CreateRoleCard(cardInfo, CardType.HandCard, m_playerType, m_skillControl);

                return newcard;
            }
            else
                return null;
            
        }
        /// <summary>
        /// 抽角色牌，不受上限限制
        /// </summary>
        /// <param name="targetDeck"></param>
        /// <returns></returns>
        public GameObject DrawRoleCardFree(GameObject targetDeck)
        {
            Card card = targetDeck.GetComponent<Card>();
            SanCai sanCai = card.GetBaseOwner();
            WuXing wuXing = card.GetBaseProprty();
            CardInfo cardInfo = m_deck.DrawRoleCard(sanCai, wuXing);
            if (cardInfo != null)
            {
                GameObject newcard = CardCreator.CreateRoleCard(cardInfo, CardType.HandCard, m_playerType, m_skillControl);

                return newcard;
            }
            else
                return null;
        }
        /// <summary>
        /// 抽取特定卡牌
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        public GameObject DrawSpecificCard(CardInfo cardInfo)
        {
            KeyValuePair<CardInfo, int> card = m_deck.DrawSpecificCard(cardInfo);

            GameObject newcard = null;
            switch(card.Value)
            {
                case 0:
                    newcard = CardCreator.CreateRoleCard(cardInfo, CardType.HandCard, m_playerType, m_skillControl);
                    break;
                case 1:
                    newcard = CardCreator.CreateExtraCard(cardInfo, CardType.HandCard, m_playerType, m_skillControl);
                    break;
                default:
                    break;
            }
            return newcard;
        }
        /// <summary>
        /// 抽主角卡
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public List<GameObject> DrawLeaderCard()
        {
            List<CardInfo> cardInfo = m_deck.DrawLeaderCard(m_playerType);
            List<GameObject> list = new List<GameObject>();
            for (int i=0; i<cardInfo.Count;i++)
            {
                GameObject newcard = CardCreator.CreateLeaderCard(cardInfo[i], CardType.HandCard, m_playerType, m_skillControl);
                list.Add(newcard);
            }

            return list;
        }
        /// <summary>
        /// 抽法术牌
        /// </summary>
        /// <returns></returns>
        public GameObject DrawSpellCard()
        {
            CardInfo cardInfo = m_deck.DrawSpellCard(m_playerType);
            if (cardInfo != null)
            {
                GameObject newcard = CardCreator.CreateSpellCard(cardInfo, CardType.HandCard, m_playerType, m_skillControl);

                return newcard;
            }
            else
                return null;
        }



        /// <summary>
        /// 将卡牌加入手牌
        /// </summary>
        /// <param name="card"></param>
        public bool AddHandCard(GameObject targetCard)
        {
            if(m_handCard.AddCard(targetCard, m_playerType))
            {
                targetCard.GetComponent<Card>().SetPlayerType(m_playerType);
            }
            return true;
        }
        /// <summary>
        /// 将卡牌从手牌移除
        /// </summary>
        /// <param name="targetCardTrans"></param>
        /// <returns></returns>
        public bool RemoveHandCard(GameObject targetCard)
        {
            return m_handCard.RemoveCard(targetCard);
        }
        /// <summary>
        /// 在场上放置卡牌
        /// </summary>
        /// <param name="targrtSiteTrans"></param>
        /// <param name="targetCardTrans"></param>
        /// <returns></returns>
        public bool AddSiteCard(GameObject targetCard, GameObject targetSite)
        {
            if(m_siteCard.AddCard(targetCard, targetSite))
            {
                targetCard.GetComponent<Card>().SetPlayerType(m_playerType);
                AddSiteCamp(targetCard);
            }
            return true;
        }
        /// <summary>
        /// 在场上放置卡牌，不改变盘属性
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public bool AddSiteCardFree(GameObject targetCard, GameObject targetSite)
        {
            if (m_siteCard.AddCardFree(targetCard, targetSite))
            {
                targetCard.GetComponent<Card>().SetPlayerType(m_playerType);
                AddSiteCamp(targetCard);
            }
            return true;
        }
        /// <summary>
        /// 移除场上的牌
        /// </summary>
        /// <param name="targrtSiteTrans"></param>
        /// <returns></returns>
        public GameObject RemoveSiteCard(GameObject taegetCard, GameObject targrtSite)
        {
            GameObject gameObject = m_siteCard.RemoveCard(targrtSite, taegetCard);
            if(gameObject != null)
            {
                RemoveSiteCamp(gameObject);
            }
            return gameObject;
        }
        /// <summary>
        /// 将卡牌加入弃牌堆
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public bool AddFold(GameObject targetCard)
        {
            return m_deck.CardToFold(targetCard);
        }
        /// <summary>
        /// 将卡牌加入卡组
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public bool AddDeck(GameObject targetCard)
        {
            return m_deck.CardToDeck(targetCard);
        }
        /// <summary>
        /// 将卡牌加入死亡牌堆
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public bool AddDeadCard(GameObject targetCard)
        {
            return m_deadCard.AddCard(targetCard);
        }
        /// <summary>
        /// 将卡牌移除死亡牌堆
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public bool RemoveDeadCard(GameObject targetCard)
        {
            return m_deadCard.RemoveCard(targetCard);
        }
        /// <summary>
        /// 清空所有死亡牌堆中的牌
        /// </summary>
        /// <returns></returns>
        public bool ClearDeadCard()
        {
            return m_deadCard.ClearCard(this);
        }
        /// <summary>
        /// 修改阵营数
        /// </summary>
        /// <param name="camps"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool AddSiteCamp(List<string> camps, int num)
        {
            for(int i = 0; i < camps.Count; i++)
            {
                if(!m_campMemberNum.ContainsKey(camps[i]))
                {
                    m_campMemberNum.Add(camps[i], num);
                }
                else
                {
                    m_campMemberNum[camps[i]] += num;
                }

                if(m_campMemberNum[camps[i]] == 0)
                    m_campMemberNum.Remove(camps[i]);
            }

            return true;
        }
        /// <summary>
        /// 激活纪行
        /// </summary>
        /// <param name="targetEpoch"></param>
        /// <returns></returns>
        public bool AddEpochCard(GameObject targetEpoch)
        {
            return m_epochCard.ActiveEpoch(targetEpoch);
        }
        /// <summary>
        /// 关闭纪行
        /// </summary>
        /// <param name="targetEpoch"></param>
        /// <returns></returns>
        public bool RemoveEpochCard(GameObject targetEpoch)
        {
            return m_epochCard.CloseEpoch(targetEpoch);
        }



        /// <summary>
        /// 获取玩家属性
        /// </summary>
        /// <returns></returns>
        public PlayerType GetPlayerType()
        {
            return m_playerType;
        }
        /// <summary>
        /// 获取场地上的所有牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetSiteCardAll()
        {
            return m_siteCard.GetSiteCardAll();
        }
        /// <summary>
        /// 获取所选盘上当前盘属性对应的卡牌
        /// </summary>
        /// <param name="sanCai"></param>
        /// <returns></returns>
        public GameObject GetSiteCardCurrent(SanCai sanCai)
        {
            return m_siteCard.GetSiteCardCurrent(sanCai);
        }
        /// <summary>
        /// 返回场地上对应从属和属性的牌
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public GameObject GetSiteCard(SanCai sanCai, WuXing wuXing)
        {
            return m_siteCard.GetSiteCard(sanCai, wuXing);
        }
        /// <summary>
        /// 返回场地上对应从属的牌
        /// </summary>
        /// <param name="sanCai"></param>
        /// <returns></returns>
        public List<GameObject> GetSiteCardOwner(SanCai sanCai)
        {
            return m_siteCard.GetSiteCardOwner(sanCai);
        }
        /// <summary>
        /// 返回场地上对应属性的牌
        /// </summary>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public List<GameObject> GetSiteCardProperty(WuXing wuXing)
        {
            return m_siteCard.GetSiteCardProperty(wuXing);
        }
        /// <summary>
        /// 返回场上所有主角牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetSiteCardLeader()
        {
            return m_siteCard.GetSiteCardLeader();
        }
        /// <summary>
        /// 返回场上每个阵营的数量
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetCampNum()
        {
            return m_campMemberNum;
        }
        /// <summary>
        /// 返回手牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetHandCardAll()
        {
            return m_handCard.GetHandCard();
        }
        /// <summary>
        /// 返回死亡卡牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetDeadCardAll()
        {
            return m_deadCard.GetDeadCard();
        }
        /// <summary>
        /// 返回死亡卡堆标志物
        /// </summary>
        /// <returns></returns>
        public GameObject GetDeadSignal()
        {
            return m_deadCard.GetDeadSignal();
        }
        /// <summary>
        /// 返回手卡位置
        /// </summary>
        /// <returns></returns>
        public Transform GetHandPosition()
        {
            return m_handCard.GetHandPosition();
        }
        /// <summary>
        /// 返回卡组
        /// </summary>
        /// <returns></returns>
        public CardDeck GetCardDeck()
        {
            return m_deck;
        }
        /// <summary>
        /// 返回卡组中的角色牌
        /// </summary>
        /// <returns></returns>
        public List<CardInfo> GetDeckRoleCardAll()
        {
            return m_deck.GetDeckRoleCard();
        }
        /// <summary>
        /// 返回卡组中的额外牌
        /// </summary>
        /// <returns></returns>
        public List<CardInfo> GetDeckExtraCardAll()
        {
            return m_deck.GetDeckExtraCard();
        }
        /// <summary>
        /// 返回场地标志牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetSiteSignal()
        {
            return m_siteCard.GetSiteSignal();
        }
        /// <summary>
        /// 返回纪行牌
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetEpochCard()
        {
            return m_epochCard.GetEpochCard();
        }
        /// <summary>
        /// 返回手牌中非主角角色牌数量
        /// </summary>
        /// <returns></returns>
        public int GetHandRoleCardNum()
        {
            return m_handCard.GetRoleCardNum();
        }


        /// <summary>
        /// 设置场地的属性
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public bool SetSiteproperty(SanCai sanCai, WuXing wuXing)
        {
            return m_siteCard.SetSiteProperty(sanCai, wuXing);
        }
        /// <summary>
        /// 设置场地约束
        /// </summary>
        /// <param name="siteConstraint"></param>
        /// <returns></returns>
        public bool SetSiteConstraint(SiteConstraint siteConstraint)
        {
            return m_siteCard.SetSiteContraint(siteConstraint);
        }
        /// <summary>
        /// 设置终牌
        /// </summary>
        /// <returns></returns>
        public bool SetFinalCard()
        {
            return m_siteCard.SetFinalCard();
        }
        /// <summary>
        /// 取消终牌
        /// </summary>
        /// <returns></returns>
        public bool CancelFinalCard()
        {
            return m_siteCard.CancelCard();
        }
        /// <summary>
        /// 设置卡组
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="index"></param>
        public void SetCardDeck(List<string> idList, int index)
        {
            m_deck.UpdateCardDeck(index, idList);
        }


        /// <summary>
        /// 所选卡牌放置在所选场地上是否合法
        /// </summary>
        /// <param name="handCardTrans"></param>
        /// <param name="siteCardTrans"></param>
        /// <returns></returns>
        public bool IsCardToSiteEnable(GameObject taegetCard, GameObject targetSite)
        {
            return m_siteCard.IsCardMatchSite(taegetCard, targetSite);
        }
        /// <summary>
        /// 所选卡牌放置在所选场地上是否合法（不需要满足五行相生）
        /// </summary>
        /// <param name="taegetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public bool IsCardToSiteFreeEnable(GameObject taegetCard, GameObject targetSite)
        {
            return m_siteCard.IsCardMatchSiteFree(taegetCard, targetSite);
        }



        /// <summary>
        /// 重新计算盘的属性
        /// </summary>
        /// <param name="sanCai"></param>
        /// <returns></returns>
        public bool ReCalculationSiteProperty(SanCai sanCai)
        {
            return m_siteCard.ReCalculationSiteProperty(sanCai);
        }


        /// <summary>
        /// 添加场上阵营
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        private bool AddSiteCamp(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            List<string> list = card.GetCamp();
            for(int i = 0; i < list.Count; i++)
            {
                if(m_campMemberNum.ContainsKey(list[i]))
                {
                    m_campMemberNum[list[i]]++;
                }
                else
                {
                    m_campMemberNum.Add(list[i], 1);
                }
                if(m_campMemberNum[list[i]] == 0)
                    m_campMemberNum.Remove(list[i]);
            }
            return true;
        }
        /// <summary>
        /// 移除场上阵营
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        private bool RemoveSiteCamp(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            List<string> list = card.GetCamp();
            for (int i = 0; i < list.Count; i++)
            {
                m_campMemberNum[list[i]]--;
                if(m_campMemberNum[list[i]] <= 0)
                    m_campMemberNum.Remove(list[i]);
            }
            return true;
        }


        /// <summary>
        /// 更新场地各参数
        /// </summary>
        /// <returns></returns>
        public bool UpdateSite()
        {
            m_siteCard.UpdateSite();
            return true;
        }
    }
}
