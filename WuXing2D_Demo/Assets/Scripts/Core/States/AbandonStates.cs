using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;
using System;
using WuXingSkill;


namespace WuXingCore
{
    public class AbandonStates : GameStates
    {
        public override bool PreProcess(GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);
            GamePlayer enemy = GetEnemy(player1, player2);

            CardWuXingDestory(player, enemy);
            GameListener.Instance().GetSkillControl().AddWaitEvent(player.GetDeadSignal(), player.GetDeadSignal(), null, TriggerType.StageAbondan);
            

            base.PreProcess(player1, player2);
            return true;
        }
        public override bool EndProcess(GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);

            ResetHandCard();

            GameListener.Instance().GetSkillControl().CardProcess();

            UpdateSiteState(player);

            return true;
        }


        public override bool ControlEventNotify(ControlEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);
            GamePlayer enemy = GetEnemy(player1, player2);

            switch (eventID)
            {
                case ControlEventID.evt_card_click:
                    return EventCardClick(notifyPara, player);
                case ControlEventID.evt_card_abandon:
                    return EventCardAbandon(player);
                default:
                    return true;
            }
        }



        /// <summary>
        /// 卡牌点击响应
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool EventCardClick(NotifyPara notifyPara, GamePlayer player)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject gameObject = eventPara.m_trigger;

            Card card = gameObject.GetComponent<Card>();
            switch (card.GetCardType())
            {
                case CardType.HandCard:
                    ClickHandCard(gameObject, player);
                    return true;
                default:
                    return true;
            }
        }
        /// <summary>
        /// 手牌丢弃按钮响应
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool EventCardAbandon(GamePlayer player)
        {
            for (int i = 0; i < m_selectCard.Count; i++)
            {
                CardActor.HandToFold(player, player, m_selectCard[i]);
            }
            m_selectCard.Clear();
            return true;
        }



        /// <summary>
        /// 点击手牌
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool ClickHandCard(GameObject gameObject, GamePlayer player)
        {
            Card card = gameObject.GetComponent<Card>();
            if (card.GetPlayerType() == player.GetPlayerType())
            {
                if (m_selectCard.Contains(gameObject))
                {
                    DisplayActor.PutBackHandCard(gameObject);
                    m_selectCard.Remove(gameObject);
                }
                else
                {
                    DisplayActor.ExtractHandCard(gameObject);
                    m_selectCard.Add(gameObject);
                }
            }
            return true;
        }

        /// <summary>
        /// 根据五行进行破坏结算
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private bool CardWuXingDestory(GamePlayer player, GamePlayer enemy)
        {
            for(int i=0; i<3; i++)
            {
                SanCai sanCai = (SanCai)i;
                GameObject siteCard1 = player.GetSiteCardCurrent(sanCai);
                GameObject siteCard2 = enemy.GetSiteCardCurrent(sanCai);
                int sign = SortRestriction(siteCard1, siteCard2);
                if (sign == 1)
                {
                    CardActor.SiteToFold(player, enemy, siteCard2);
                    //CardActor.CardDestoryCard(player, enemy, siteCard1, siteCard2);
                }
                else if (sign == -1)
                {
                    CardActor.SiteToFold(enemy, player, siteCard1);
                    //CardActor.CardDestoryCard(enemy, player, siteCard2, siteCard1);
                }
            }

            return true;
        }
        /// <summary>
        /// 将卡牌按破坏关系排序（前面破坏后面返回1， 后面破坏前面返回-1 ，否则0）
        /// </summary>
        /// <param name="gameObject1"></param>
        /// <param name="gameObject2"></param>
        /// <returns></returns>
        private int SortRestriction(GameObject gameObject1, GameObject gameObject2)
        {
            if (gameObject1 == null || gameObject2 == null)
                return 0;

            Card card1 = gameObject1.GetComponent<Transform>().parent.GetComponent<Card>();
            Card card2 = gameObject2.GetComponent<Transform>().parent.GetComponent<Card>();

            if (card1.GetBaseProprty() == PropertyTool.WuXingRestriction(card2.GetBaseProprty()))
            {
                return -1;
            }
            else if (card2.GetBaseProprty() == PropertyTool.WuXingRestriction(card1.GetBaseProprty()))
            {
                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 恢复所有手牌状态
        /// </summary>
        /// <returns></returns>
        private bool ResetHandCard()
        {
            for (int i = 0; i < m_selectCard.Count; i++)
            {
                DisplayActor.PutBackHandCard(m_selectCard[i]);
            }
            m_selectCard.Clear();
            return true;
        }


        private void UpdateSiteState(GamePlayer player)
        {
            player.UpdateSite();
        }

        public override bool IsChainEnable()
        {
            return false;
        }
    }


    public class PlayerAbandonStates : AbandonStates
    {
        public PlayerAbandonStates()
        {
            m_playerType = PlayerType.Player;
        }

        public override GameStates GetNextStates()
        {
            EnemyReadyStates enemyReadyStates = new EnemyReadyStates();
            return enemyReadyStates;
        }
        public override string GetCurrentStatusName()
        {
            return "我方弃卡阶段";
        }
    }

    public class EnemyAbandonStates : AbandonStates
    {
        public EnemyAbandonStates()
        {
            m_playerType = PlayerType.Enemy;
        }

        public override GameStates GetNextStates()
        {
            PlayerReadyStates playerReadyStates = new PlayerReadyStates();
            return playerReadyStates;
        }
        public override string GetCurrentStatusName()
        {
            return "敌方弃牌阶段";
        }
    }
}

