using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingTool;
using WuXingBase;

namespace WuXingCore
{
    /// <summary>
    /// 状态父类
    /// </summary>
    public class GameStates
    {
        protected List<GameObject> m_selectCard;
        protected PlayerType m_playerType;
        protected GameStates m_beforeState;
        protected GameStates m_afterState;

        public GameStates()
        {
            m_selectCard = new List<GameObject>();
            m_afterState = null;
            m_beforeState = null;
        }


        /// <summary>
        /// 进入状态后预处理
        /// </summary>
        virtual public bool PreProcess(GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);
            DisplayActor.DisplayControlPlayer(player.GetPlayerType());
            return true;
        }
        /// <summary>
        /// 结束当前阶段后处理
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        virtual public bool EndProcess(GamePlayer player1, GamePlayer player2)
        {
            return true; ;
        }

        

        /// <summary>
        /// 控制事件通知
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="trigger"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        virtual public bool ControlEventNotify(ControlEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            switch (eventID)
            {
                default:
                    return true;
            }
        }
        virtual public bool ChainEventNotify(ChainEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            switch (eventID)
            {
                case ChainEventID.evt_left:
                    return EventSiteLeft(player1, player2, notifyPara);
                case ChainEventID.evt_drawspell:
                    return EventDrawSpell(player1, player2, notifyPara);
                case ChainEventID.evt_destory:
                    return EventDestory(player1, player2, notifyPara);
                case ChainEventID.evt_handdiscard:
                    return EventAbandon(player1, player2, notifyPara);
                case ChainEventID.evt_clear_deadcard:
                    return EventClearDeadCard(player1, player2);
                case ChainEventID.evt_drawrole:
                    return EventDrawRole(player1, player2, notifyPara);
                case ChainEventID.evt_drawspecific:
                    return EventDrawSpecific(player1, player2, notifyPara);
                case ChainEventID.evt_tohand:
                    return EventCardToHand(player1, player2, notifyPara);
                case ChainEventID.evt_setresistance:
                    return EventSetResistance(notifyPara);
                case ChainEventID.evt_skillnum:
                    return EventSetSkillNum(notifyPara);
                case ChainEventID.evt_skillconstraint:
                    return EventSetSkillConstranint(notifyPara);
                case ChainEventID.evt_siteproperty:
                    return EventSetSiteProperty(player1, player2, notifyPara);
                case ChainEventID.evt_cardpara:
                    return EventSetCardPara(player1, player2, notifyPara);
                case ChainEventID.evt_playerpara:
                    return EventSetPlayerPara(player1, player2, notifyPara);
                case ChainEventID.evt_siteconstraint:
                    return EventSetSiteConstraint(player1, player2, notifyPara);
                case ChainEventID.evt_come:
                    return EventCardCome(player1, player2, notifyPara);
                case ChainEventID.evt_epochactive:
                    return EventEpochActive(player1, player2, notifyPara);
                case ChainEventID.evt_epochclose:
                    return EventEpochClose(player1, player2, notifyPara);
                case ChainEventID.evt_showcard:
                    return EventShowCard(player1, player2, notifyPara);
                case ChainEventID.evt_setskillimmune:
                    return EventSetSkillImmune(player1, player2, notifyPara);
                default:
                    return true;
            }
        }



        /// <summary>
        /// 返回当前状态的下一个状态
        /// </summary>
        /// <returns></returns>
        virtual public GameStates GetNextStates()
        {
            return null;
        }
        /// <summary>
        /// 返回当前状态名称
        /// </summary>
        /// <returns></returns>
        virtual public string GetCurrentStatusName()
        {
            return "";
        }
        /// <summary>
        /// 获取当前控制玩家
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        public GamePlayer GetPlayer(GamePlayer player1, GamePlayer player2)
        {
            if (player1.GetPlayerType() == m_playerType)
                return player1;
            else
                return player2;
        }
        /// <summary>
        /// 获取敌对玩家
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        public GamePlayer GetEnemy(GamePlayer player1, GamePlayer player2)
        {
            if (player1.GetPlayerType() == m_playerType)
                return player2;
            else
                return player1;
        }
        virtual public GameStates GetBeforeStates()
        {
            return m_beforeState;
        }
        virtual public GameStates GetAfterStates()
        {
            return m_afterState;
        }


        virtual public void SetBeforeStates(GameStates gameStates)
        {
            m_beforeState = gameStates;
        }
        virtual public void SetAfterStates(GameStates gameStates)
        {
            m_afterState = gameStates;
        }



        /// <summary>
        /// 允许切换阶段
        /// </summary>
        /// <returns></returns>
        virtual public bool IsChangeStageEnable()
        {
            return true;
        }
        /// <summary>
        /// 允许连锁技能
        /// </summary>
        /// <returns></returns>
        virtual public bool IsChainEnable()
        {
            return false;
        }


        /// <summary>
        /// 重新进入该阶段
        /// </summary>
        /// <returns></returns>
        virtual public bool ReEntry(GamePlayer player1, GamePlayer player2)
        {
            if(m_afterState == null)
            {
                GamePlayer player = GetPlayer(player1, player2);
                DisplayActor.DisplayControlPlayer(player.GetPlayerType());
            }
            
            return true;
        }



        /// <summary>
        /// 抽法术卡事件
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool EventDrawSpell(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;

            GamePlayer player;

            if(player1.GetPlayerType() == eventPara.m_playerType)
                player = player1;
            else
                player = player2;

            GameObject gameObject = DeckActor.DrawSpellCard(player);
            CardActor.DeckToHand(player, player, null, gameObject);
            return true;
        }
        /// <summary>
        /// 场上卡牌离场事件
        /// </summary>
        /// <param name="player"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        private bool EventSiteLeft(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject originCard = eventPara.m_trigger;
            GameObject targetCard = eventPara.m_target;

            GamePlayer originPlayer = null;
            GamePlayer targetPlayer = null;

            if (originCard != null)
            {
                if (originCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    originPlayer = player1;
                else
                    originPlayer = player2;
            }

            if (targetCard != null)
            {
                if (targetCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    targetPlayer = player1;
                else
                    targetPlayer = player2;
            }

            CardActor.CardLeftCard(originPlayer, targetPlayer, originCard, targetCard);
            return true;
        }
        /// <summary>
        /// 卡牌破坏事件
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <param name="originCard"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        private bool EventDestory(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject originCard = eventPara.m_trigger;
            GameObject targetCard = eventPara.m_target;

            GamePlayer originPlayer = null;
            GamePlayer targetPlayer = null;

            if (originCard != null)
            {
                if (originCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    originPlayer = player1;
                else
                    originPlayer = player2;
            }

            if (targetCard != null)
            {
                if (targetCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    targetPlayer = player1;
                else
                    targetPlayer = player2;
            }

            return CardActor.CardDestoryCard(originPlayer, targetPlayer, originCard, targetCard);
        }
        /// <summary>
        /// 丢弃手牌事件
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <param name="originCard"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        private bool EventAbandon(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject originCard = eventPara.m_trigger;
            GameObject targetCard = eventPara.m_target;

            GamePlayer originPlayer = null;
            GamePlayer targetPlayer = null;

            if (originCard != null)
            {
                if (originCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    originPlayer = player1;
                else
                    originPlayer = player2;
            }

            if (targetCard != null)
            {
                if (targetCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    targetPlayer = player1;
                else
                    targetPlayer = player2;
            }

            CardActor.CardAbandonCard(originPlayer, targetPlayer, originCard, targetCard);

            return true;
        }
        /// <summary>
        /// 清空死亡卡牌事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        private bool EventClearDeadCard(GamePlayer player1, GamePlayer player2)
        {
            player1.ClearDeadCard();
            player2.ClearDeadCard();
            return true;
        }
        /// <summary>
        /// 抽取角色卡事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="originCard"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        private bool EventDrawRole(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject originCard = eventPara.m_trigger;
            GameObject targetCard = eventPara.m_target;

            GamePlayer originPlayer = null;
            GamePlayer targetPlayer = null;

            if (originCard != null)
            {
                if (originCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    originPlayer = player1;
                else
                    originPlayer = player2;
            }

            if (targetCard != null)
            {
                if (targetCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    targetPlayer = player1;
                else
                    targetPlayer = player2;
            }

            GameObject newCard = DeckActor.DrawRoleCardFree(targetPlayer, targetCard);
            return CardActor.DeckToHand(originPlayer, targetPlayer, originCard, newCard);
        }
        /// <summary>
        /// 设置卡牌抗性事件
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="resistance"></param>
        /// <returns></returns>
        private bool EventSetResistance(NotifyPara notifyPara)
        {
            ResistPara resistPara = notifyPara as ResistPara;
            GameObject targetCard = resistPara.m_target;
            Resistance resistance = resistPara.m_resistance;

            CardActor.SetCardResistance(targetCard, resistance);
            return true;
        }
        /// <summary>
        /// 抽取特定卡牌事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="originCard"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        private bool EventDrawSpecific(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject originCard = eventPara.m_trigger;
            GameObject targetCard = eventPara.m_target;

            GamePlayer originPlayer = null;
            GamePlayer targetPlayer = null;

            if (originCard != null)
            {
                if (originCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    originPlayer = player1;
                else
                    originPlayer = player2;
            }

            if (targetCard != null)
            {
                if (targetCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    targetPlayer = player1;
                else
                    targetPlayer = player2;
            }

            GameObject newCard = null;
            if (targetCard != null)
            {
                Card card = targetCard.GetComponent<Card>();
                newCard = DeckActor.DrawSpecificCard(originPlayer, card.GetCardInfo());
            }


            return CardActor.DeckToHand(originPlayer, targetPlayer, originCard, newCard);
        }
        /// <summary>
        /// 设置技能使用次数事件
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="index"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private bool EventSetSkillNum(NotifyPara notifyPara)
        {
            SkillPara skillPara = notifyPara as SkillPara;
            GameObject targetCard = skillPara.m_target;
            int index = skillPara.m_index;
            int num = skillPara.m_num;


            CardActor.SetCardSkillNum(targetCard, index, num);
            return true;
        }
        /// <summary>
        /// 设置技能约束事件
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="skillConstraint"></param>
        /// <returns></returns>
        private bool EventSetSkillConstranint(NotifyPara notifyPara)
        {
            SkillPara skillPara = notifyPara as SkillPara;
            GameObject targetCard = skillPara.m_target;
            SkillConstraint skillConstraint = skillPara.m_skillConstraint;

            CardActor.SetCardSkillConstraint(targetCard, skillConstraint);
            return true;
        }
        /// <summary>
        /// 将卡堆外卡牌加入手牌事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="originCard"></param>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        private bool EventCardToHand(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject originCard = eventPara.m_trigger;
            GameObject targetCard = eventPara.m_target;

            GamePlayer cardPlayer = null;
            GamePlayer targetPlayer = null;


            if (eventPara.m_playerType == player1.GetPlayerType())
                targetPlayer = player1;
            else
                targetPlayer = player2;


            if (targetCard != null)
            {
                if (targetCard.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                    cardPlayer = player1;
                else
                    cardPlayer = player2;
            }

            return CardActor.CardToHandCard(cardPlayer, targetPlayer, originCard, targetCard);
        }
        /// <summary>
        /// 设置盘的属性事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventSetSiteProperty(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            CardPara cardPara = notifyPara as CardPara;

            GamePlayer player = null;
            if (cardPara.m_playerType == player1.GetPlayerType())
                player = player1;
            else
                player = player2;

            player.SetSiteproperty(cardPara.m_owner[0], cardPara.m_property[0]);
            return true;
        }
        /// <summary>
        /// 设置卡的参数事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventSetCardPara(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            CardPara cardPara = notifyPara as CardPara;
            GameObject targetCard = cardPara.m_target;
            Card card = targetCard.GetComponent<Card>();

            List<SanCai> sanCais = new List<SanCai>();
            sanCais.AddRange(cardPara.m_owner);

            List<WuXing> wuXings = new List<WuXing>();
            wuXings.AddRange(cardPara.m_property);

            if(cardPara.m_isadd)
            {
                card.AddRegardOwner(sanCais, 1);
                card.AddRegardProperty(wuXings, 1);
            }
            else
            {
                card.AddRegardOwner(sanCais, -1);
                card.AddRegardProperty(wuXings, -1);
            }
            

            return true;
        }
        /// <summary>
        /// 设置玩家参数事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventSetPlayerPara(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            PlayerPara playerPara = notifyPara as PlayerPara;

            GamePlayer player = null;
            if (playerPara.m_playerType == player1.GetPlayerType())
                player = player1;
            else
                player = player2;

            if(playerPara.m_isadd)
            {
                player.AddSiteCamp(playerPara.m_camp, 1);
            }
            else
            {
                player.AddSiteCamp(playerPara.m_camp, -1);
            }
            return true;
        }
        /// <summary>
        /// 设置场地约束
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventSetSiteConstraint(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            SitePara sitePara = notifyPara as SitePara;
            GamePlayer player = null;
            if (sitePara.m_playerType == player1.GetPlayerType())
                player = player1;
            else
                player = player2;

            player.SetSiteConstraint(sitePara.m_siteConstraint);
            return true;
        }
        /// <summary>
        /// 卡牌登场事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventCardCome(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;
            GamePlayer cardplayer = null;
            GamePlayer siteplayer = null;

            if (eventPara.m_trigger.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                cardplayer = player1;
            else
                cardplayer = player2;

            if (eventPara.m_target.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                siteplayer = player1;
            else
                siteplayer = player2;

            return CardActor.CardComeCard(cardplayer, siteplayer, eventPara.m_origin, eventPara.m_trigger, eventPara.m_target);
        }
        /// <summary>
        /// 激活纪行事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventEpochActive(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;
            GamePlayer player = null;


            if (eventPara.m_target.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                player = player1;
            else
                player = player2;

            player.AddEpochCard(eventPara.m_target);

            return true;
        }
        /// <summary>
        /// 关闭纪行事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventEpochClose(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;
            GamePlayer player = null;


            if (eventPara.m_target.GetComponent<Card>().GetPlayerType() == player1.GetPlayerType())
                player = player1;
            else
                player = player2;

            player.RemoveEpochCard(eventPara.m_target);

            return true;
        }
        /// <summary>
        /// 显示卡牌事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventShowCard(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            EventPara eventPara = notifyPara as EventPara;

            GameObject targetCard = eventPara.m_target;

            DisplayActor.DisplayHandCard(targetCard);

            return true;
        }
        /// <summary>
        /// 设置卡牌技能抗性事件
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventSetSkillImmune(GamePlayer player1, GamePlayer player2, NotifyPara notifyPara)
        {
            ImmunePara immunePara = notifyPara as ImmunePara;
            GameObject target = immunePara.m_target;
            SkillImmune skillImmune = immunePara.m_skillImmune;

            CardActor.SetCardSkillImmune(target, skillImmune);
            return true;
        }

    }




    /// <summary>
    /// 游戏结束阶段
    /// </summary>
    public class GameFinishStates : GameStates
    {
        public override GameStates GetNextStates()
        {
            GameFinishStates gameFinishStates = new GameFinishStates();
            return gameFinishStates;
        }
        public override string GetCurrentStatusName()
        {
            return "游戏结束";
        }

        public override bool IsChangeStageEnable()
        {
            return false;
        }
    }
}

