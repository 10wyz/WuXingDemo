using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingButton;
using WuXingSkill;
using WuXingTool;
using WuXingWindow;

namespace WuXingCore
{
    public class MainStates : GameStates
    {
        private GameObject m_window;
        private List<GameObject> m_twinkleCard;
        private GameObject m_clickSkill;

        public MainStates()
        {
            m_window = null;
            m_twinkleCard = new List<GameObject>();

            m_clickSkill = null;
        }

        public override bool PreProcess(GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);

            GameListener.Instance().GetSkillControl().AddWaitEvent(player.GetDeadSignal(), player.GetDeadSignal(), null, TriggerType.StageMain);
            
            base.PreProcess(player1, player2);
            return true;
        }
        public override bool EndProcess(GamePlayer player1, GamePlayer player2)
        {
            RecoverSelectCard();
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
                case ControlEventID.evt_button_click:
                    return EventButtonClick(notifyPara, player);
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
        public bool EventCardClick(NotifyPara notifyPara, GamePlayer player)
        {
            if(m_window != null)
                return false;

            EventPara eventPara = notifyPara as EventPara;
            GameObject gameObject = eventPara.m_trigger;

            Card card = gameObject.GetComponent<Card>();
            switch (card.GetCardType())
            {
                case CardType.HandCard:
                    ClickHandCard(gameObject, player);
                    return true;
                case CardType.SiteSignal:
                    ClickSite(gameObject, player);
                    return true;
                case CardType.SiteCard:
                    ClickSiteCard(gameObject, player);
                    return true;
                default:
                    return true;
            }
        }
        /// <summary>
        /// 按钮点击响应
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool EventButtonClick(NotifyPara notifyPara, GamePlayer player)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject targetButton = eventPara.m_trigger;

            ButtonBase button = targetButton.GetComponent<ButtonBase>();
            switch (button.m_buttonTag)
            {
                case ButtonTag.Skill:
                    ClickSkillButton(targetButton.transform.parent.gameObject);
                    return true;
                case ButtonTag.SkillSeleckOk:
                    ClickSkillSelectOkButton(targetButton);
                    return true;
                case ButtonTag.SkillSelectCancel:
                    ClickSkillSelectCancelButton(targetButton);
                    return true;
                default:
                    return true;
            }
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
                if (m_selectCard.Count == 0)//之前没有选中手牌
                {
                    m_selectCard.Add(gameObject);
                    DisplayActor.ExtractHandCard(gameObject);
                    OpemTwinkleCard(gameObject, player);

                    if (card.IsSkillEnable(null, null, TriggerType.Click).Count > 0)
                    {
                        DisplayActor.DisplaySkillButton(gameObject);
                    }
                }
                else//之前已经选中牌
                {
                    if (m_selectCard[0] == gameObject)//选择了同一张
                    {
                        m_selectCard.RemoveAt(0);
                        DisplayActor.PutBackHandCard(gameObject);
                        CloseTwinkleCard();
                        DisplayActor.HideSkillButton(gameObject);
                    }
                    else if (m_selectCard[0].GetComponent<Card>().GetCardType() == CardType.HandCard)//之前是手牌，选择另一张
                    {
                        DisplayActor.PutBackHandCard(m_selectCard[0]);
                        CloseTwinkleCard();
                        DisplayActor.HideSkillButton(m_selectCard[0]);
                        m_selectCard.RemoveAt(0);
                        m_selectCard.Add(gameObject);
                        DisplayActor.ExtractHandCard(gameObject);
                        OpemTwinkleCard(gameObject, player);
                        if (card.IsSkillEnable(null, null, TriggerType.Click).Count > 0)
                        {
                            DisplayActor.DisplaySkillButton(gameObject);
                        }
                    }
                    else if (m_selectCard[0].GetComponent<Card>().GetCardType() == CardType.SiteCard)//之前是场地牌
                    {
                        DisplayActor.HideSkillButton(m_selectCard[0]);
                        m_selectCard.RemoveAt(0);
                        m_selectCard.Add(gameObject);
                        DisplayActor.ExtractHandCard(gameObject);
                        OpemTwinkleCard(gameObject, player);
                    }
                }
            }
            else
            {
                RecoverSelectCard();
            }

            return true;
        }
        /// <summary>
        /// 点击场地上的牌
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool ClickSiteCard(GameObject gameObject, GamePlayer player)
        {
            Card card = gameObject.GetComponent<Card>();
            Card site = gameObject.GetComponent<Transform>().parent.GetComponent<Card>();

            if (site.GetPlayerType() == player.GetPlayerType())
            {
                if (m_selectCard.Count == 0)
                {
                    if(card.IsSkillEnable(null, null, TriggerType.Click).Count > 0)
                    {
                        DisplayActor.DisplaySkillButton(gameObject);
                        m_selectCard.Add(gameObject);
                    }
                }
                else
                {
                    if (m_selectCard[0].GetComponent<Card>().GetCardType() == CardType.HandCard && player.IsCardToSiteEnable(m_selectCard[0], site.gameObject))
                    {
                        CardActor.HandReplaceSite(player, player, m_selectCard[0], gameObject);
                        CloseTwinkleCard();
                        //CardActor.SiteToFold(player, player, gameObject);
                        //CardActor.HandToSite(player, player, m_selectCard[0], site.gameObject);
                        m_selectCard.RemoveAt(0);
                    }
                    else if (m_selectCard[0].GetComponent<Card>().GetCardType() == CardType.SiteCard)
                    {
                        if (m_selectCard[0] == gameObject)
                        {
                            DisplayActor.HideSkillButton(gameObject);
                            m_selectCard.RemoveAt(0);
                        }
                        else
                        {
                            DisplayActor.HideSkillButton(m_selectCard[0]);
                            m_selectCard.RemoveAt(0);
                            if(card.IsSkillEnable(null, null, TriggerType.Click).Count > 0)
                            {
                                DisplayActor.DisplaySkillButton(gameObject);
                                m_selectCard.Add(gameObject);
                            }      
                        }
                    }
                }
            }
            else
            {
                RecoverSelectCard();
            }

            return true;
        }
        /// <summary>
        /// 点击场地
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool ClickSite(GameObject gameObject, GamePlayer player)
        {
            Card site = gameObject.GetComponent<Card>();
            if (site.GetPlayerType() == player.GetPlayerType())
            {
                if (m_selectCard.Count > 0)
                {
                    if (m_selectCard[0].GetComponent<Card>().GetCardType() == CardType.HandCard)
                    {
                        if (player.IsCardToSiteEnable(m_selectCard[0], site.gameObject))
                        {
                            CardActor.HandToSite(player, player, m_selectCard[0], site.gameObject);
                            CloseTwinkleCard();
                            m_selectCard.RemoveAt(0);
                        }
                    }
                    else if (m_selectCard[0].GetComponent<Card>().GetCardType() == CardType.SiteCard)
                    {
                        DisplayActor.HideSkillButton(m_selectCard[0]);
                        m_selectCard.RemoveAt(0);
                    }
                }
            }
            else
            {
                RecoverSelectCard();
            }

            return true;
        }



        /// <summary>
        /// 点击发动技能按钮
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool ClickSkillButton(GameObject targetCard)
        {
            DisplayActor.HideSkillButton(targetCard);
            
            if (m_clickSkill == null)
            {
                m_clickSkill = targetCard;
                DisplayActor.DisplayLaunchEmphasize(targetCard);
                return false;
            }
            m_clickSkill = null;


            Card card = targetCard.GetComponent<Card>();
            if (targetCard.GetComponent<Card>().GetCardType() == CardType.HandCard)
            {
                DisplayActor.PutBackHandCard(targetCard);
            }

            
            List<int> index = card.IsSkillEnable(null, null, TriggerType.Click);
            if(index.Count == 1)
            {
                m_selectCard.RemoveAt(0);
                if (!targetCard.GetComponent<Card>().OnSkill(targetCard, null, WuXingSkill.TriggerType.Click, index[0]))
                {
                    return false;
                }
                    
            } 
            else
            {
                //技能选择
                m_window = WindowCreator.CreateSkillSelectWinodw(targetCard, index);
                
            }

            if(m_afterState == null)
                GameListener.Instance().GetSkillControl().SkillProcess();

            return true;
        }
        private bool ClickSkillSelectOkButton(GameObject targetButton)
        {
            int index = m_window.GetComponent<SkillSelectWindow>().GetIndex();

            m_window = null;

            GameObject targetCard = m_selectCard[0];
            m_selectCard.RemoveAt(0);

            if (!targetCard.GetComponent<Card>().OnSkill(targetCard, null, WuXingSkill.TriggerType.Click, index))
                return false;

            GameListener.Instance().GetSkillControl().SkillProcess();

            return true;
        }
        private bool ClickSkillSelectCancelButton(GameObject targetButton)
        {
            m_selectCard.RemoveAt(0);

            m_window = null;

            return true;
        }



        public override bool ReEntry(GamePlayer player1, GamePlayer player2)
        {
            if (m_clickSkill != null)
            {
                ClickSkillButton(m_clickSkill);
            }
            else
            {
                if (m_afterState == null)
                    GameListener.Instance().GetSkillControl().SkillProcess();
            }

            base.ReEntry(player1, player2);
            return true;
        }

        /// <summary>
        /// 恢复所有手牌状态
        /// </summary>
        /// <returns></returns>
        private bool RecoverSelectCard()
        {
            for (int i = 0; i < m_selectCard.Count; i++)
            {
                switch (m_selectCard[i].GetComponent<Card>().GetCardType())
                {
                    case CardType.HandCard:
                        DisplayActor.PutBackHandCard(m_selectCard[i]);
                        CloseTwinkleCard();
                        DisplayActor.HideSkillButton(m_selectCard[i]);
                        break;
                    case CardType.SiteCard:
                        DisplayActor.HideSkillButton(m_selectCard[i]);
                        break;
                    default:
                        break;
                }

            }
            m_selectCard.Clear();
            return true;
        }


        public override bool IsChangeStageEnable()
        {
            return m_window == null;
        }
        public override bool IsChainEnable()
        {
            return true;
        }


        /// <summary>
        /// 显示可放置场地
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="player"></param>
        private void OpemTwinkleCard(GameObject targetCard, GamePlayer player)
        {
            List<GameObject> site = new List<GameObject>();
            site.AddRange(player.GetSiteSignal());

            for(int i = 0; i < site.Count; i++)
            {
                if(player.IsCardToSiteEnable(targetCard, site[i]))
                {
                    if(site[i].transform.childCount > 1)
                    {
                        site[i] = site[i].transform.GetChild(1).gameObject;
                    }

                    DisplayActor.OpenTwinkleCard(site[i]);
                    m_twinkleCard.Add(site[i]);
                }
            }

        }
        /// <summary>
        /// 关闭可放置场地
        /// </summary>
        private void CloseTwinkleCard()
        {
            for (int i = 0; i < m_twinkleCard.Count; i++)
            {
                if (m_twinkleCard[i] != null)
                    DisplayActor.CloseTwinkleCard(m_twinkleCard[i]);
            }
            m_twinkleCard.Clear();
        }
    }


    public class PlayerMainStates : MainStates
    {
        public PlayerMainStates()
        {
            m_playerType = PlayerType.Player;
        }

        public override GameStates GetNextStates()
        {
            PlayerEndStates playerEndStates = new PlayerEndStates();
            return playerEndStates;
        }
        public override string GetCurrentStatusName()
        {
            return "我方主要阶段";
        }
    }

    public class EnemyMainStates : MainStates
    {
        public EnemyMainStates()
        {
            m_playerType = PlayerType.Enemy;
        }


        public override GameStates GetNextStates()
        {
            EnemyEndStates enemyEndStates = new EnemyEndStates();
            return enemyEndStates;
        }
        public override string GetCurrentStatusName()
        {
            return "敌方主要阶段";
        }
    }
}

