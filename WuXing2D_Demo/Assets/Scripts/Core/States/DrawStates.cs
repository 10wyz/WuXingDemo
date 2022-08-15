using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;
using WuXingWindow;

namespace WuXingCore
{
    public class DrawStates : GameStates
    {
        GameObject m_window;

        public DrawStates()
        {
            m_window = null;
        }

        public override bool PreProcess(GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);

            DrawLeaderCard(player);
            m_window = WindowCreator.CreateCardDeckWindow(player.GetCardDeck(), Mathf.Max(0, 5-player.GetHandRoleCardNum()));

            base.PreProcess(player, player2);
            return true;
        }
        public override bool EndProcess(GamePlayer player1, GamePlayer player2)
        {
            if(m_window != null)
            {
                GameObject.Destroy(m_window);
            }
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
                default:
                    return base.ControlEventNotify(eventID, notifyPara, player, enemy);
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
            GameObject targetCard = eventPara.m_trigger;

            Card card = targetCard.GetComponent<Card>();
            switch (card.GetCardType())
            {
                case CardType.CardDeck:
                    ClickCardDeck(targetCard, player);
                    return true;
                default:
                    return true;
            }
        }



        /// <summary>
        /// 点击卡组
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool ClickCardDeck(GameObject gameObject, GamePlayer player)
        {
            GameObject card = DeckActor.DrawRoleCard(player, gameObject);
            CardActor.DeckToHand(player, player, null, card);
            if(card != null)
                m_window.GetComponent<CardDeckWindow>().UpdateNumText(gameObject, player.GetCardDeck());
            return true;
        }



        /// <summary>
        /// 抽取主角牌
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool DrawLeaderCard(GamePlayer player)
        {
            List<GameObject> cards = DeckActor.DrawLeaderCard(player);
            for (int i = 0; i < cards.Count; i++)
                CardActor.DeckToHand(player, player, null, cards[i]);
            return true;
        }


        public override bool IsChainEnable()
        {
            return false;
        }
    }

    public class PlayerDrawStates : DrawStates
    {
        public PlayerDrawStates()
        {
            m_playerType = PlayerType.Player;
        }

        public override GameStates GetNextStates()
        {
            PlayerMainStates playermainStates = new PlayerMainStates();
            return playermainStates;
        }
        public override string GetCurrentStatusName()
        {
            return "我方抽卡阶段";
        }
    }

    public class EnemyDrawStates : DrawStates
    {
        public EnemyDrawStates()
        {
            m_playerType= PlayerType.Enemy;
        }

        public override GameStates GetNextStates()
        {
            EnemyMainStates enemyMainStates = new EnemyMainStates();
            return enemyMainStates;
        }
        public override string GetCurrentStatusName()
        {
            return "敌方抽卡阶段";
        }
    }
}

