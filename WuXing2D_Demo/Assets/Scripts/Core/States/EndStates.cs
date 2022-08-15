using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WuXingBase;
using WuXingTool;
using WuXingSkill;
using WuXingButton;
using WuXingWindow;


namespace WuXingCore
{
    public class EndStates : GameStates
    {
        private GameObject m_window;
        private GameObject m_clickSkill;

        public EndStates()
        {
            m_window = null;
            m_clickSkill = null;
        }


        public override bool PreProcess(GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);

            SetFinalCard(player);

            GameListener.Instance().GetSkillControl().AddWaitEvent(player.GetDeadSignal(), player.GetDeadSignal(), null, TriggerType.StageEnd);
            
            base.PreProcess(player1, player2);

            return true;
        }
        public override bool EndProcess(GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);

            CancelFinalCard(player);

            GameListener.Instance().GetSkillControl().SkillProcess();

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
        private bool EventCardClick(NotifyPara notifyPara, GamePlayer player)
        {
            if (m_window != null)
                return false;


            EventPara eventPara = notifyPara as EventPara;
            GameObject targetCard = eventPara.m_trigger;

            Card card = targetCard.GetComponent<Card>();

            switch (card.GetCardType())
            {
                case CardType.HandCard:
                    return ClickHandCard(targetCard, player);
                case CardType.SiteCard:
                    return ClickSiteCard(targetCard, player);
                default:
                    return false;
            }
        }
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
        /// 点击场地上的牌
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool ClickSiteCard(GameObject targetCard, GamePlayer player)
        {
            Card card = targetCard.GetComponent<Card>();

            if (player.GetPlayerType() == card.GetPlayerType())
            {
                if (m_selectCard.Count > 0)
                {
                    if (m_selectCard[0] == targetCard)
                    {
                        DisplayActor.HideSkillButton(targetCard);
                        m_selectCard.Clear();
                    }
                    else
                    {
                        DisplayActor.HideSkillButton(m_selectCard[0]);
                        m_selectCard.RemoveAt(0);
                        if (card.IsSkillEnable(null, null, TriggerType.Click).Count > 0)
                        {
                            DisplayActor.DisplaySkillButton(targetCard);
                            m_selectCard.Add(targetCard);
                        }
                    }
                }
                else
                {
                    if (card.IsSkillEnable(null, null, TriggerType.Click).Count > 0)
                    {
                        DisplayActor.DisplaySkillButton(targetCard);
                        m_selectCard.Add(targetCard);
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// 点击手中的牌
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool ClickHandCard(GameObject targetCard, GamePlayer player)
        {
            Card card = targetCard.GetComponent<Card>();

            if (player.GetPlayerType() == card.GetPlayerType())
            {
                if (m_selectCard.Count > 0)
                {
                    if (m_selectCard[0] == targetCard)
                    {
                        DisplayActor.HideSkillButton(targetCard);
                        m_selectCard.Clear();
                    }
                    else
                    {
                        DisplayActor.HideSkillButton(m_selectCard[0]);
                        m_selectCard.RemoveAt(0);
                        if (card.IsSkillEnable(null, null, TriggerType.Click).Count > 0)
                        {
                            DisplayActor.DisplaySkillButton(targetCard);
                            m_selectCard.Add(targetCard);
                        }
                    }
                }
                else
                {
                    if (card.IsSkillEnable(null, null, TriggerType.Click).Count > 0)
                    {
                        DisplayActor.DisplaySkillButton(targetCard);
                        m_selectCard.Add(targetCard);
                    }
                }
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
            List<int> index = card.IsSkillEnable(null, null, TriggerType.Click);
            if(index.Count == 1)
            {
                m_selectCard.RemoveAt(0);
                if(!targetCard.GetComponent<Card>().OnSkill(targetCard, null, WuXingSkill.TriggerType.Click, index[0]))
                {
                    return false;
                }
            }
            else
            {
                //技能选择
                m_window = WindowCreator.CreateSkillSelectWinodw(targetCard, index);
            }

            if (m_afterState == null)
                GameListener.Instance().GetSkillControl().SkillProcess();

            return true;
        }
        private bool ClickSkillSelectOkButton(GameObject targetButton)
        {
            int index = m_window.GetComponent<SkillSelectWindow>().GetIndex();

            GameObject.Destroy(m_window);
            m_window = null;

            GameObject targetCard = m_selectCard[0];
            m_selectCard.RemoveAt(0);

            if(!targetCard.GetComponent<Card>().OnSkill(targetCard, null, WuXingSkill.TriggerType.Click, index))
                return false;

            GameListener.Instance().GetSkillControl().SkillProcess();

            return true;
        }
        private bool ClickSkillSelectCancelButton(GameObject targetButton)
        {
            m_selectCard.RemoveAt(0);

            GameObject.Destroy(m_window);
            m_window = null;

            return true;
        }




        private bool SetFinalCard(GamePlayer player)
        {
            return player.SetFinalCard();
        }
        private bool CancelFinalCard(GamePlayer player)
        {
            return player.CancelFinalCard();
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


        public override bool IsChangeStageEnable()
        {
            return m_window == null;
        }
        public override bool IsChainEnable()
        {
            return true;
        }
    }



    public class PlayerEndStates : EndStates
    {
        public PlayerEndStates()
        {
            m_playerType = PlayerType.Player;
        }

        public override GameStates GetNextStates()
        {
            PlayerAbandonStates playerAbandonStates = new PlayerAbandonStates();
            return playerAbandonStates;
        }
        public override string GetCurrentStatusName()
        {
            return "我方结束阶段";
        }
    }

    public class EnemyEndStates : EndStates
    {
        public EnemyEndStates()
        {
            m_playerType = PlayerType.Enemy;
        }



        public override GameStates GetNextStates()
        {
            EnemyAbandonStates enemyAbandonStates = new EnemyAbandonStates();
            return enemyAbandonStates;
        }
        public override string GetCurrentStatusName()
        {
            return "敌方结束阶段";
        }
    }
}

