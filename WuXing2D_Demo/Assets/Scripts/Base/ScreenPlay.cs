using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingCore;
using WuXingSkill;
using WuXingTool;
using System;
using UnityEngine.UI;

namespace WuXingBase
{
    public enum Clique {Yang, Yin}

    abstract public class ScreenPlay
    {
        abstract public bool IsLeader(string Id, Clique clique);
        abstract public bool IsMatchPlayer(string Id, PlayerType playerType);

        abstract public bool Notify(WaitEvent waitEvent);

        abstract public string GetScreenPlayerName();
        abstract public Clique GetClique(PlayerType playerType);

        abstract public bool IsConditionEnable(int index);

        abstract protected void UpdateScoreBoard();
    }


    public class ScreenPlay01 : ScreenPlay
    {
        private Dictionary<Clique, List<string>> m_leaderID;
        private Dictionary<Clique, int> m_leaderStayTime;
        private Dictionary<PlayerType, Clique> m_playerClique;
        private int m_arrowNum;//箭支数量
        private int m_lastTime;//孤日持续时间

        GameObject m_scoreBoard;//计分板

        public ScreenPlay01(Clique playerclique)
        {
            m_arrowNum = 0;

            m_leaderID = new Dictionary<Clique, List<string>>();
            m_leaderID[Clique.Yang] = new List<string>();
            m_leaderID[Clique.Yin] = new List<string>();

            m_leaderID[Clique.Yang].Add("140001");
            m_leaderID[Clique.Yang].Add("240007");
            m_leaderID[Clique.Yang].Add("340001");

            m_leaderID[Clique.Yin].Add("130001");
            m_leaderID[Clique.Yin].Add("230007");
            m_leaderID[Clique.Yin].Add("350003");

            m_leaderStayTime = new Dictionary<Clique, int>();
            m_leaderStayTime[Clique.Yang] = 0;
            m_leaderStayTime[Clique.Yin] = 0;


            m_playerClique = new Dictionary<PlayerType, Clique>();
            if (playerclique == Clique.Yang)
            {
                m_playerClique[PlayerType.Player] = Clique.Yang;
                m_playerClique[PlayerType.Enemy] = Clique.Yin;
            }
            else
            {
                m_playerClique[PlayerType.Player] = Clique.Yin;
                m_playerClique[PlayerType.Enemy] = Clique.Yang;
            }

            m_scoreBoard = GameObject.Find("ScoreBoard");
            UpdateScoreBoard();

        }



        /// <summary>
        /// 判断是否是主角牌
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public override bool IsLeader(string Id, Clique clique)
        {
            if (m_leaderID[clique].Contains(Id))
                return true;
            return false;
        }
        /// <summary>
        /// 判断玩家是否胜利
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public bool IsWin(PlayerType playerType)
        {
            UpdateScoreBoard();

            if (m_leaderStayTime[m_playerClique[playerType]] >= 5)
                return true;

            return false;
        }
        public override bool IsMatchPlayer(string Id, PlayerType playerType)
        {
            if(Id[1] == '1')
            {
                if (m_playerClique[playerType] == Clique.Yang)
                    return true;
                else
                    return false;
            }
            else
            {
                if (m_playerClique[playerType] == Clique.Yin)
                    return true;
                else
                    return false;
            }
        }
        public override bool IsConditionEnable(int index)
        {
            switch (index)
            {
                case 0:
                    return IsShootSuccess();
                case 1:
                    return IsSunsetEnd();
                default:
                    return false;
            }
        }
        private bool IsShootSuccess()
        {
            return m_arrowNum >= 5;
        }
        private bool IsSunsetEnd()
        {
            return m_lastTime <= 0;
        }


        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="gameObjectList"></param>
        /// <param name="notifyEvent"></param>
        /// <returns></returns>
        public override bool Notify(WaitEvent waitEvent)
        {
            switch (waitEvent.m_triggerType)
            {
                case TriggerType.Rinne:
                    return EventRinne(waitEvent);
                case TriggerType.Exist:
                    return EventExist(waitEvent);
                case TriggerType.Skill:
                    return EventSkill(waitEvent);
                case TriggerType.StageReady:
                    return EventStageReady(waitEvent);
                default:
                    return true;
            }
        }


        private bool EventRinne(WaitEvent waitEvent)
        {
            GameObject targetCard = waitEvent.m_trigger;
            Card card = targetCard.GetComponent<Card>();
            if(card.GetCardID() == "140001")
            {
                m_arrowNum = 0;
            }
            return true;
        }
        private bool EventExist(WaitEvent waitEvent)
        {
            GameObject targetCard = waitEvent.m_trigger;

            Card card = targetCard.GetComponent<Card>();
            PlayerType playerType = card.GetPlayerType();
            if (card.GetCardInfo().m_ID == m_leaderID[m_playerClique[playerType]][0])
            {
                m_leaderStayTime[m_playerClique[playerType]]++;
                if (IsWin(playerType))
                {
                    if(playerType == PlayerType.Player)
                    {
                        WindowCreator.CreateGameOverWindow("You Win");
                    }
                    else
                    {
                        WindowCreator.CreateGameOverWindow("You Lose");
                    }
                    GameObject.Find("Canvas").GetComponent<GameControl>().GameFinish();
                }
            }
            return true;
        }
        private bool EventSkill(WaitEvent waitEvent)
        {
            GameObject targetCard = waitEvent.m_trigger;
            Card card = targetCard.GetComponent<Card>();
            
            switch (card.GetCardID())
            {
                case "920105":
                    ShootSun(targetCard);
                    break;
                default:
                    break;
            }

            return true;
        }
        private bool EventStageReady(WaitEvent waitEvent)
        {
            if(m_lastTime > 0)
            {
                GameObject target = GetEpochCard("920104");
                if(waitEvent.m_origin.GetComponent<Card>().GetPlayerType() == target.GetComponent<Card>().GetPlayerType())
                {
                    m_lastTime--;
                }
            }

            return true;
        }


        private void ShootSun(GameObject targetCard)
        {
            if(targetCard.GetComponent<Card>().GetSkill(0).IsLaunchSuccess())
            {
                m_arrowNum++;
                if (m_arrowNum == 5)
                    m_lastTime = 2;
            }
        }

        private GameObject GetEpochCard(string id)
        {
            List<GameObject> list = GameListener.Instance().GetEpochCard();

            for(int i = 0; i < list.Count; i++)
            {
                Card card = list[i].GetComponent<Card>();
                if(card.GetCardID() == id)
                    return list[i];
            }

            return null;
        }

        public override string GetScreenPlayerName()
        {
            return "日月争辉";
        }
        public override Clique GetClique(PlayerType playerType)
        {
            return m_playerClique[playerType];
        }


        protected override void UpdateScoreBoard()
        {
            string playerText = "我方：" + Convert.ToString(m_leaderStayTime[m_playerClique[PlayerType.Player]]) + "/5";
            string enemyText = "敌方：" + Convert.ToString(m_leaderStayTime[m_playerClique[PlayerType.Enemy]]) + "/5";

            m_scoreBoard.transform.Find("PlayerRecord").GetComponent<Text>().text = playerText;
            m_scoreBoard.transform.Find("EnemyRecord").GetComponent<Text>().text = enemyText;
        }
    }

}
