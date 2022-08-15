using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingDisplay;

namespace WuXingCore
{
    public class AnimaStates : GameStates
    {
        protected GameObject m_origin;
        //protected GameObject m_displayCard;
        

        public AnimaStates(EventPara eventPara, GameStates gameStates)
        {
            m_origin = eventPara.m_origin;

            m_beforeState = gameStates;
        }

        

        public override bool PreProcess(GamePlayer player1, GamePlayer player2)
        {
            Card card = m_origin.GetComponent<Card>();
            card.GetComponent<AnimationDisplay>().Initial(this);
            //m_displayCard = CardCreator.CreateTempCard(card.GetCardInfo(), card.GetCardCategory(), card.GetPlayerType());

            //DisplayActor.DisplaySkillLaunchCard(m_displayCard, this);


            return true;
        }
        public override bool EndProcess(GamePlayer player1, GamePlayer player2)
        {
            //GameObject.Destroy(m_displayCard);
            return true;
        }


        public override bool ControlEventNotify(ControlEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            return true;
        }
        public override bool ChainEventNotify(ChainEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            //return true;
            return base.ChainEventNotify(eventID, notifyPara, player1, player2);
        }

        public override bool IsChainEnable()
        {
            return false;
        }
        public override bool IsChangeStageEnable()
        {
            return false;
        }


        public override bool ReEntry(GamePlayer player1, GamePlayer player2)
        {
            return true;
        }

        public override string GetCurrentStatusName()
        {
            return "动画阶段";
        }

    }

    public class PlayerAnimaStates : AnimaStates
    {
        public PlayerAnimaStates(EventPara eventPara, GameStates gameStates)
            : base(eventPara, gameStates)
        {
            m_playerType = PlayerType.Player;
        }
    }

    public class EnemyAnimaStates : AnimaStates
    {
        public EnemyAnimaStates(EventPara eventPara, GameStates gameStates)
            : base(eventPara, gameStates)
        {
            m_playerType = PlayerType.Enemy;
        }
    }
}

