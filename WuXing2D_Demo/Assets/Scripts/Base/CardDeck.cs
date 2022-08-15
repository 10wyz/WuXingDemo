using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using WuXingCore;

namespace WuXingBase
{
    public class CardDeck
    {
        private Dictionary<SanCai, Dictionary<WuXing, List<CardInfo>>> m_cardDeck;//角色卡组
        private Dictionary<SanCai, Dictionary<WuXing, List<CardInfo>>> m_cardFold;//角色弃牌堆
        private Dictionary<Clique, Dictionary<CardInfo, int>> m_leaderDeck;//主角卡组
        private List<CardInfo> m_extraDeck;//额外卡组
        private Dictionary<Clique, List<CardInfo>> m_spellDeck;//法术卡组
        private Dictionary<Clique, List<CardInfo>> m_spellFold;//法术弃牌堆
        private ScreenPlay m_screenPlay;
        public CardDeck(ScreenPlay screenPlay)
        {
            m_cardDeck = new Dictionary<SanCai, Dictionary<WuXing, List<CardInfo>>>();
            m_cardFold = new Dictionary<SanCai, Dictionary<WuXing, List<CardInfo>>>();
            m_leaderDeck = new Dictionary<Clique, Dictionary<CardInfo, int>>();
            m_extraDeck = new List<CardInfo>();
            m_spellDeck = new Dictionary<Clique, List<CardInfo>>();
            m_spellFold = new Dictionary<Clique, List<CardInfo>>();

            for (int i=0; i<3; i++)
            {
                SanCai sanCai = (SanCai)i;
                m_cardDeck[sanCai] = new Dictionary<WuXing, List<CardInfo>>();
                m_cardFold[sanCai] = new Dictionary<WuXing, List<CardInfo>>();
                for (int j=0; j<5; j++)
                {
                    WuXing wuXing = (WuXing)j;
                    m_cardDeck[sanCai][wuXing] = new List<CardInfo>();
                    m_cardFold[sanCai][wuXing] = new List<CardInfo>();
                }
            }

            m_leaderDeck[Clique.Yang] = new Dictionary<CardInfo, int>();
            m_leaderDeck[Clique.Yin] = new Dictionary<CardInfo, int>();

            m_spellDeck[Clique.Yang] = new List<CardInfo>();
            m_spellDeck[Clique.Yin] = new List<CardInfo>();

            m_spellFold[Clique.Yang] = new List<CardInfo>();
            m_spellFold[Clique.Yin] = new List<CardInfo>();

            m_screenPlay = screenPlay;
        }


        /// <summary>
        /// 读取并生成卡组
        /// </summary>
        /// <param name="cardPile"></param>
        /// <returns></returns>
        public bool LoadCardDeck(List<CardInfo> cardPile)
        {
            for (int i = 0; i < cardPile.Count; i++)
            {
                if (m_screenPlay.IsLeader(cardPile[i].m_ID, Clique.Yang))
                {
                    m_leaderDeck[Clique.Yang].Add(cardPile[i], 0);
                }
                else if (m_screenPlay.IsLeader(cardPile[i].m_ID, Clique.Yin))
                {
                    m_leaderDeck[Clique.Yin].Add(cardPile[i], 0);
                }
                else
                {
                    SanCai sanCai = cardPile[i].GetOwner();
                    WuXing wuXing = cardPile[i].GetProperty();

                    m_cardDeck[sanCai][wuXing].Add(cardPile[i]);
                }
            }

            //判断卡组是否可用 Todo

            return true;
        }
        public bool LoadExtraDeck(List<CardInfo> cardPile)
        {
            m_extraDeck.AddRange(cardPile);
            return true;
        }
        public bool LoadSpellDeck(List<CardInfo> cardPile, List<CardInfo> cardPile2)
        {
            m_spellDeck[m_screenPlay.GetClique(PlayerType.Player)].AddRange(cardPile);
            m_spellDeck[m_screenPlay.GetClique(PlayerType.Enemy)].AddRange(cardPile2);
            return true;
        }



        /// <summary>
        /// 将弃牌堆的牌放入卡组
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        private bool FoldToDeck(SanCai sanCai, WuXing wuXing)
        {
            m_cardDeck[sanCai][wuXing].AddRange(m_cardFold[sanCai][wuXing]);
            m_cardFold[sanCai][wuXing].Clear();
            return true;
        }
        /// <summary>
        /// 将卡牌加入卡组
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        public bool CardToDeck(GameObject gameObject)
        {
            Card card = gameObject.GetComponent<Card>();
            CardInfo cardInfo = card.GetCardInfo();

            switch (card.GetCardCategory())
            {
                case CardCategory.LeaderCard:
                    LeaderCardToDeck(cardInfo, card.GetPlayerType(), 0);
                    break;
                case CardCategory.ExtraCard:
                    ExtraCardToDeck(cardInfo);
                    break;
                case CardCategory.RoleCard:
                    RoleCardToDeck(cardInfo);
                    break;
                case CardCategory.SpellCard:
                    break;
                default:
                    break;
            }

            return true;
        }
        /// <summary>
        /// 将卡牌加入弃牌堆
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        public bool CardToFold(GameObject gameObject)
        {
            Card card = gameObject.GetComponent<Card>();
            CardInfo cardInfo = card.GetCardInfo();

            switch (card.GetCardCategory())
            {
                case CardCategory.LeaderCard:
                    LeaderCardToDeck(cardInfo, card.GetPlayerType(), 2);
                    break;
                case CardCategory.ExtraCard:
                    ExtraCardToDeck(cardInfo);
                    break;
                case CardCategory.RoleCard:
                    RoleCardToFold(cardInfo);
                    break;
                case CardCategory.SpellCard:
                    SpellCardToFold(cardInfo, card.GetPlayerType());
                    break;
                default:
                    break;
            }

            return true;
        }
        /// <summary>
        /// 角色牌进卡组
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        private bool RoleCardToDeck(CardInfo cardInfo)
        {
            SanCai sanCai = cardInfo.GetOwner();
            WuXing wuXing = cardInfo.GetProperty();

            m_cardDeck[sanCai][wuXing].Add(cardInfo);
            CardShuffle(m_cardDeck[sanCai][wuXing]);
            return true;
        }
        /// <summary>
        /// 角色牌进弃牌堆
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        private bool RoleCardToFold(CardInfo cardInfo)
        {
            SanCai sanCai = cardInfo.GetOwner();
            WuXing wuXing = cardInfo.GetProperty();

            m_cardFold[sanCai][wuXing].Add(cardInfo);
            return true;
        }
        /// <summary>
        /// 将卡牌加入主角卡组
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <param name="playerType"></param>
        /// <returns></returns>
        private bool LeaderCardToDeck(CardInfo cardInfo, PlayerType playerType, int time)
        {
            m_leaderDeck[m_screenPlay.GetClique(playerType)].Add(cardInfo, time);
            return true;
        }
        /// <summary>
        /// 将额外牌加入额外卡组
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        private bool ExtraCardToDeck(CardInfo cardInfo)
        {
            m_extraDeck.Add(cardInfo);
            return true;
        }
        /// <summary>
        /// 法术牌进卡组
        /// </summary>
        /// <returns></returns>
        private bool SpellCardToDeck()
        {
            //Todo
            return true;
        }
        /// <summary>
        /// 法术牌进弃牌堆
        /// </summary>
        /// <returns></returns>
        private bool SpellCardToFold(CardInfo cardInfo, PlayerType playerType)
        {
            m_spellFold[m_screenPlay.GetClique(playerType)].Add(cardInfo);
            return true;
        }



        /// <summary>
        /// 洗对应卡组
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        private bool CardShuffle(List<CardInfo> deck)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                int index = UnityEngine.Random.Range(i, deck.Count - 1);

                CardInfo temp = deck[i];
                deck[i] = deck[index];
                deck[index] = temp;
            }

            return true;
        }
        public bool CardShuffle(int index)
        {
            List<CardInfo> deck = null;
            if(index>=0 && index <15)
            {
                SanCai sanCai = (SanCai)(index / 5);
                WuXing wuXing = (WuXing)(index % 5);
                deck = m_cardDeck[sanCai][wuXing];
            }
            else
            {
                switch (index)
                {
                    case 15:
                        deck = m_extraDeck;
                        break;
                    case 16:
                        deck = m_spellDeck[Clique.Yang];
                        break;
                    case 17:
                        deck = m_spellDeck[Clique.Yin];
                        break;
                    default:
                        break;
                }
            }

            if(deck != null)
            {
                if(GameListener.Instance().IsPlayerControl())
                {
                    CardShuffle(deck);
                    List<string> idList = new List<string>();
                    for(int i=0; i<deck.Count; i++)
                    {
                        idList.Add(deck[i].m_ID);
                    }
                    GameListener.Instance().GetGameNetworkManager().SendDeck(idList, index);
                }
            }
            return true;
        }
        /// <summary>
        /// 全部卡组洗牌
        /// </summary>
        /// <returns></returns>
        public bool CardShuffleAll()
        {
            for(int i = 0; i <= 17; i++)
            {
                CardShuffle(i);
            }

            return true;
        }
        /// <summary>
        /// 全部洗牌初始化
        /// </summary>
        /// <returns></returns>
        public bool CardShuffleInitial()
        {
            for(int index=0; index <= 17; index++)
            {
                List<CardInfo> deck = null;
                if (index >= 0 && index < 15)
                {
                    SanCai sanCai = (SanCai)(index / 5);
                    WuXing wuXing = (WuXing)(index % 5);
                    deck = m_cardDeck[sanCai][wuXing];
                }
                else
                {
                    switch (index)
                    {
                        case 15:
                            deck = m_extraDeck;
                            break;
                        case 16:
                            deck = m_spellDeck[Clique.Yang];
                            break;
                        case 17:
                            deck = m_spellDeck[Clique.Yin];
                            break;
                        default:
                            break;
                    }
                }

                if (deck != null)
                {
                    CardShuffle(deck);
                    List<string> idList = new List<string>();
                    for (int i = 0; i < deck.Count; i++)
                    {
                        idList.Add(deck[i].m_ID);
                    }
                    GameListener.Instance().GetGameNetworkManager().SendDeck(idList, index);
                }
            }

            return true;
        }
        public bool UpdateCardDeck(int index, List<string> idList)
        {
            List<CardInfo> deck = null;
            if (index >= 0 && index < 15)
            {
                SanCai sanCai = (SanCai)(index / 5);
                WuXing wuXing = (WuXing)(index % 5);
                deck = m_cardDeck[sanCai][wuXing];
            }
            else
            {
                switch (index)
                {
                    case 15:
                        deck = m_extraDeck;
                        break;
                    case 16:
                        deck = m_spellDeck[Clique.Yang];
                        break;
                    case 17:
                        deck = m_spellDeck[Clique.Yin];
                        break;
                    default:
                        break;
                }
            }

            List<CardInfo> newDeck = new List<CardInfo>();

            for(int i = 0; i < idList.Count; i++)
            {
                for(int j=0; j<deck.Count; j++)
                {
                    if(deck[j].m_ID == idList[i])
                    {
                        newDeck.Add(deck[j]);
                        deck.Remove(deck[j]);
                        break;
                    }
                }
            }

            deck.Clear();
            deck.AddRange(newDeck);

            return true;
        }


        /// <summary>
        /// 返回卡组和弃牌堆中对应类型卡牌剩余数量
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public int GetCardNum(SanCai sanCai, WuXing wuXing)
        {
            return m_cardDeck[sanCai][wuXing].Count + m_cardFold[sanCai][wuXing].Count;
        }
        /// <summary>
        /// 返回卡组中卡牌数量
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public int GetDeckNum(SanCai sanCai, WuXing wuXing)
        {
            return m_cardDeck[sanCai][wuXing].Count;
        }
        /// <summary>
        /// 返回弃牌堆中卡牌数量
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public int GetFoldNum(SanCai sanCai, WuXing wuXing)
        {
            return m_cardFold[sanCai][wuXing].Count;
        }
        public List<CardInfo> GetDeckRoleCard()
        {
            List<CardInfo> list = new List<CardInfo>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    list.AddRange(m_cardDeck[(SanCai)i][(WuXing)j]);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    list.AddRange(m_cardFold[(SanCai)i][(WuXing)j]);
                }
            }

            return list;

        }
        public List<CardInfo> GetDeckExtraCard()
        {
            return m_extraDeck;
        }



        /// <summary>
        /// 抽对应角色卡
        /// </summary>
        /// <param name="sanCai"></param>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public CardInfo DrawRoleCard(SanCai sanCai, WuXing wuXing)
        {
            //卡组为空则将弃牌堆卡牌加入卡组，并洗牌
            if(m_cardDeck[sanCai][wuXing].Count == 0)
            {
                FoldToDeck(sanCai, wuXing);
                CardShuffle(m_cardDeck[sanCai][wuXing]);
            }

            if(m_cardDeck[sanCai][wuXing].Count >0)
            {
                CardInfo cardInfo = m_cardDeck[sanCai][wuXing][0];
                m_cardDeck[sanCai][wuXing].RemoveAt(0);
                //抽空卡组则将弃牌堆卡牌加入卡组，并洗牌
                if (m_cardDeck[sanCai][wuXing].Count == 0)
                {
                    FoldToDeck(sanCai, wuXing);
                    CardShuffle(m_cardDeck[sanCai][wuXing]);
                }
                return cardInfo;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 抽主角卡
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public List<CardInfo> DrawLeaderCard(PlayerType playerType)
        {
            Clique clique = m_screenPlay.GetClique(playerType);

            List<CardInfo> list = new List<CardInfo>();

            for(int i=0; i<m_leaderDeck[clique].Count; i++)
            {
                var item = m_leaderDeck[clique].ElementAt(i);
                if (item.Value <= 0)
                {
                    list.Add(item.Key);
                }
                else
                {
                    m_leaderDeck[clique][item.Key]--;
                }
            }

            for(int i = 0; i < list.Count; i++)
            {
                m_leaderDeck[clique].Remove(list[i]);
            }

            return list;
        }
        /// <summary>
        /// 抽法术卡
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public CardInfo DrawSpellCard(PlayerType playerType)
        {
            Clique clique = m_screenPlay.GetClique(playerType);

            if (m_spellDeck[clique].Count > 0)
            {
                CardInfo cardInfo = m_spellDeck[clique][0];
                m_spellDeck[clique].RemoveAt(0);
                return cardInfo;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 抽特定卡牌
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        public KeyValuePair<CardInfo, int> DrawSpecificCard(CardInfo cardInfo)
        {
            int k = -1;

            for (int i=0;i<3;i++)
            {
                for(int j=0;j<5;j++)
                {
                    k = m_cardDeck[(SanCai)i][(WuXing)j].IndexOf(cardInfo);
                    if (k != -1)
                    {
                        m_cardDeck[(SanCai)i][(WuXing)j].RemoveAt(k);
                        return new KeyValuePair<CardInfo, int>(cardInfo, 0);
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    k = m_cardFold[(SanCai)i][(WuXing)j].IndexOf(cardInfo);
                    if (k != -1)
                    {
                        m_cardFold[(SanCai)i][(WuXing)j].RemoveAt(k);
                        return new KeyValuePair<CardInfo, int>(cardInfo, 0);
                    }
                }
            }

            k = m_extraDeck.IndexOf(cardInfo);
            if(k != -1)
            {
                m_extraDeck.RemoveAt(k);
                return new KeyValuePair<CardInfo, int>(cardInfo, 1);
            }

            return new KeyValuePair<CardInfo, int>(null, -1);
        }

        
    }
}

