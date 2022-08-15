using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingSkill;

namespace WuXingCore
{
    public class ReadyStates : GameStates
    {
        public override bool PreProcess(GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);

            CardOnExist(player);
            UpdateCardState(player);

            GameListener.Instance().GetSkillControl().AddWaitEvent(player.GetDeadSignal(), player.GetDeadSignal(), null, TriggerType.StageReady);
            
            base.PreProcess(player1, player2);
            return true;
        }
        public override bool EndProcess(GamePlayer player1, GamePlayer player2)
        {
            GameListener.Instance().GetSkillControl().CardProcess();
            return true;
        }

        public override bool ControlEventNotify(ControlEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);
            GamePlayer enemy = GetEnemy(player1, player2);

            
            switch (eventID)
            {
                case ControlEventID.evt_card_click:
                    return EventCardClick(notifyPara, player, enemy);
                default:
                    return base.ControlEventNotify(eventID, notifyPara, player1, player2);
            }

        }
        public override bool ChainEventNotify(ChainEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);
            GamePlayer enemy = GetEnemy(player1, player2);

            switch (eventID)
            {
                default:
                    return base.ChainEventNotify(eventID, notifyPara, player, enemy);
            }
        }



        /// <summary>
        /// 卡牌点击响应
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private bool EventCardClick(NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject gameObject = eventPara.m_trigger;

            GamePlayer player = GetPlayer(player1, player2);

            Card card = gameObject.GetComponent<Card>();
            switch (card.GetCardType())
            {
                case CardType.SiteCenter:
                    Reincarnation(gameObject, player);
                    return true;
                default:
                    return true;
            }
        }



        /// <summary>
        /// 轮回选中的盘
        /// </summary>
        /// <param name="targetSite"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool Reincarnation(GameObject targetSite, GamePlayer player)
        {
            Card card = targetSite.GetComponent<Card>();
            List<GameObject> list = player.GetSiteCardOwner(card.GetBaseOwner());
            bool IsReincarnation = list.Count == 5;
            for (int i = 0; i < list.Count; i++)
            {
                if(IsReincarnation)
                {
                    CardActor.SiteToFold(player, player, list[i]);
                    list[i].GetComponent<Card>().OnAction(list[i], null, WuXingSkill.TriggerType.Rinne);
                }
                else
                {
                    CardActor.SiteToFold(player, player, list[i]);
                }
                
            }
            if (IsReincarnation)
            {
                EventPara eventPara = new EventPara();
                eventPara.m_playerType = player.GetPlayerType();
                GameListener.Instance().ChainEventNotify(ChainEventID.evt_drawspell, eventPara);
            }
            return IsReincarnation;
        }
        /// <summary>
        /// 在场主角牌触发存在事件
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool CardOnExist(GamePlayer player)
        {
            List<GameObject> list = player.GetSiteCardLeader();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].GetComponent<Card>().OnAction(list[i], null, WuXingSkill.TriggerType.Exist);
            }
            return true;
        }
   

        /// <summary>
        /// 更新卡牌状态
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool UpdateCardState(GamePlayer player)
        {
            List<GameObject> list = player.GetSiteCardAll();
            list.AddRange(player.GetEpochCard());
            for (int i = 0; i < list.Count; i++)
            {
                Card card = list[i].GetComponent<Card>();
                card.UpdateCardState();
            }
            return true;
        }


        public override bool IsChainEnable()
        {
            return false;
        }
    }


    public class PlayerReadyStates : ReadyStates
    {
        public PlayerReadyStates()
        {
            m_playerType = PlayerType.Player;
        }

        public override GameStates GetNextStates()
        {
            PlayerDrawStates playerDrawStates = new PlayerDrawStates();
            return playerDrawStates;
        }
        public override string GetCurrentStatusName()
        {
            return "我方预备阶段";
        }
    }

    public class EnemyReadyStates : ReadyStates
    {
        public EnemyReadyStates()
        {
            m_playerType= PlayerType.Enemy;
        }
        public override GameStates GetNextStates()
        {
            EnemyDrawStates enemyDrawStates = new EnemyDrawStates();
            return enemyDrawStates;
        }
        public override string GetCurrentStatusName()
        {
            return "敌方预备阶段";
        }
    }
}

