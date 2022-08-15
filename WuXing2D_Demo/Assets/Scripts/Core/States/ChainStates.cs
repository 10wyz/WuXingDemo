using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingButton;
using WuXingSkill;
using WuXingBase;
using WuXingAssetManage;
using WuXingTool;
using WuXingWindow;

namespace WuXingCore
{
    public class ChainStates : GameStates
    {
        protected GameObject m_window;//弹出的对话框

        protected GameObject m_origin;
        protected GameObject m_trigger;
        protected GameObject m_target;
        protected TriggerType m_triggerType;
        protected List<GameObject> m_twinkleCard;

        private GameObject m_clickSkill;
        public ChainStates(EventPara eventPara, GameStates gameStates)
        {
            m_origin = eventPara.m_origin;
            m_trigger = eventPara.m_trigger;
            m_target = eventPara.m_target;
            m_triggerType = eventPara.m_triggerType;

            m_beforeState = gameStates;

            m_window = null;
            m_clickSkill = null;

            m_twinkleCard = new List<GameObject>();
        }



        public override bool PreProcess(GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);

            m_window = WindowCreator.CreateChainCheckWindow();
            GameListener.Instance().GetSkillControl().RemoveWaitEvent();
            ShowSkillEnableCard(player);

            base.PreProcess(player1, player2);
            return true;
        }
        public override bool EndProcess(GamePlayer player1, GamePlayer player2)
        {
            HideSkillEnableCard(player1, player2);

            if (m_window != null)
            {
                GameObject.Destroy(m_window);
            }

            
            return base.EndProcess(player1, player2);
        }



        public override bool ControlEventNotify(ControlEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);

            switch (eventID)
            {
                case ControlEventID.evt_card_click:
                    return EventCardClick(notifyPara, player);
                case ControlEventID.evt_button_click:
                    EventButtonClick(notifyPara, player);
                    return true;
                case ControlEventID.evt_skill_cancel:
                    return EventSkillCancel(player);
                default:
                    return base.ControlEventNotify(eventID, notifyPara, player1, player2);
            }
        }



        private bool EventCardClick(NotifyPara notifyPara, GamePlayer player)
        {
            if (m_window != null && m_window.GetComponent<StateWindow>().IsModalWindow())
                return false;

            EventPara eventPara = notifyPara as EventPara;
            GameObject targetCard = eventPara.m_trigger;

            Card card = targetCard.GetComponent<Card>();

            switch (card.GetCardType())
            {
                case CardType.DeadSite:
                case CardType.SiteCard:
                    return ClickSiteCard(targetCard, player);
                case CardType.HandCard:
                    return ClickHandCard(targetCard, player);
                case CardType.ExtraCard:
                    return ClickExtraCard(targetCard, player);
                default:
                    return false;
            }
        }
        private bool EventButtonClick(NotifyPara notifyPara, GamePlayer player)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject targetButton = eventPara.m_trigger;

            ButtonBase button = targetButton.GetComponent<ButtonBase>();
            
            switch(button.m_buttonTag)
            {
                case ButtonTag.Skill:
                    return ClickSkillButton(targetButton.transform.parent.gameObject);
                case ButtonTag.ChainCheckCancel:
                    return ClickChainCheckCancelButton(targetButton, player);
                case ButtonTag.SkillSeleckOk:
                    return ClickSkillSelectOkButton(targetButton);
                case ButtonTag.SkillSelectCancel:
                    return ClickSkillSelectCancelButton(targetButton);
                default:
                    return false;
            }

        }
        private bool EventSkillCancel(GamePlayer player)
        {
            if (m_window != null)
                return false;

            if(m_selectCard.Count > 0)
            {
                DisplayActor.HideSkillButton(m_selectCard[0]);
                m_selectCard.RemoveAt(0);
            }

            
            EventPara eventPara = new EventPara();
            eventPara.m_origin = m_origin;
            eventPara.m_trigger = m_trigger;
            eventPara.m_target = m_target;
            eventPara.m_triggerType = m_triggerType;
            eventPara.m_playerType = player.GetPlayerType();

            if (m_origin.GetComponent<Card>().GetPlayerType() != player.GetPlayerType())
                GameListener.Instance().GetSkillControl().ReAddWaitEvent(eventPara, 1);
            else
                GameListener.Instance().GetSkillControl().ReAddWaitEvent(eventPara, 2);

            GameListener.Instance().ChainEventNotify(ChainEventID.evt_startchain, eventPara);


            StatesPara statesPara = new StatesPara();
            statesPara.m_gameStates = this;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_endchain, statesPara);

            return true;    
        }


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
                        if(card.IsChainEnable(m_trigger, m_target, m_triggerType).Count > 0)
                        {
                            DisplayActor.DisplaySkillButton(targetCard);
                            m_selectCard.Add(targetCard);
                        }
                    }
                }
                else
                {
                    if(card.IsChainEnable(m_trigger, m_target, m_triggerType).Count > 0)
                    {
                        DisplayActor.DisplaySkillButton(targetCard);
                        m_selectCard.Add(targetCard);
                    }
                }
            }

            return true;
        }
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
                        if (card.IsChainEnable(m_trigger, m_target, m_triggerType).Count > 0)
                        {
                            DisplayActor.DisplaySkillButton(targetCard);
                            m_selectCard.Add(targetCard);
                        }
                    }
                }
                else
                {
                    if (card.IsChainEnable(m_trigger, m_target, m_triggerType).Count > 0)
                    {
                        DisplayActor.DisplaySkillButton(targetCard);
                        m_selectCard.Add(targetCard);
                    }
                }
            }

            return true;
        }
        private bool ClickExtraCard(GameObject targetCard, GamePlayer player)
        {
            List<GameObject> list = new List<GameObject>();
            List<GameObject> chain = new List<GameObject>();
            list.AddRange(player.GetDeadCardAll());
            for (int i = 0; i < list.Count; i++)
            {
                Card card = list[i].GetComponent<Card>();
                if (card.IsChainEnable(m_trigger, m_target, m_triggerType).Count > 0)
                {
                    chain.Add(list[i]);
                }
            }
            if(chain.Count > 0)
            {
                if(m_window != null)
                    GameObject.Destroy(m_window);

                m_window = WindowCreator.CreateCardSelectWindow(chain);
            }

            return true;
        }


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
            List<int> index = card.IsChainEnable(m_trigger, m_target, m_triggerType);
            if (index.Count == 1)
            {
                m_selectCard.RemoveAt(0);
                targetCard.GetComponent<Card>().OnSkill(m_trigger, m_target, m_triggerType, index[0]);

                StatesPara statesPara = new StatesPara();
                statesPara.m_gameStates = this;
                GameListener.Instance().ChainEventNotify(ChainEventID.evt_endchain, statesPara);
            }
            else
            {
                //Todo 技能选择
                m_window = WindowCreator.CreateSkillSelectWinodw(targetCard, index);
            }
            
            

            return true;
        }
        private bool ClickChainCheckCancelButton(GameObject targetButton, GamePlayer player)
        {
            m_window = null;

            EventPara eventPara = new EventPara();
            eventPara.m_origin = m_origin;
            eventPara.m_trigger = m_trigger;
            eventPara.m_target = m_target;
            eventPara.m_triggerType = m_triggerType;

            if (m_origin.GetComponent<Card>().GetPlayerType() != player.GetPlayerType())
                GameListener.Instance().GetSkillControl().ReAddWaitEvent(eventPara, 1);
            else
                GameListener.Instance().GetSkillControl().ReAddWaitEvent(eventPara, 2);


            StatesPara statesPara = new StatesPara();
            statesPara.m_gameStates = this;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_endchain, statesPara);
            return true;
        }
        private bool ClickSkillSelectOkButton(GameObject targetButton)
        {
            int index = m_window.GetComponent<SkillSelectWindow>().GetIndex();

            m_window = null;

            GameObject targetCard = m_selectCard[0];
            m_selectCard.RemoveAt(0);

            targetCard.GetComponent<Card>().OnSkill(targetCard, null, WuXingSkill.TriggerType.Click, index);

            StatesPara statesPara = new StatesPara();
            statesPara.m_gameStates = this;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_endchain, statesPara);

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

            base.ReEntry(player1, player2);
            return true;
        }

        public override bool IsChangeStageEnable()
        {
            return false;
        }
        public override bool IsChainEnable()
        {
            return true;
        }


        private bool ShowSkillEnableCard(GamePlayer player)
        {
            List<GameObject> list = new List<GameObject>();
            list.AddRange(player.GetSiteCardAll());
            list.AddRange(player.GetHandCardAll());
            list.AddRange(player.GetEpochCard());

            for (int i = 0; i < list.Count; i++)
            {
                Card card = list[i].GetComponent<Card>();
                if (card.IsChainEnable(m_trigger, m_target, m_triggerType).Count > 0)
                {
                    DisplayActor.OpenTwinkleCard(list[i]);
                    m_twinkleCard.Add(list[i]);
                }
            }

            List<GameObject> list2 = new List<GameObject>();
            list2.AddRange(player.GetDeadCardAll());
            for(int i = 0; i < list2.Count; i++)
            {
                Card card = list2[i].GetComponent<Card>();
                if (card.IsChainEnable(m_trigger, m_target, m_triggerType).Count > 0)
                {
                    DisplayActor.OpenTwinkleCard(list2[i]);
                    m_twinkleCard.Add(list2[i]);

                    GameObject signal = player.GetDeadSignal();
                    if (!m_twinkleCard.Contains(signal))
                    {
                        DisplayActor.OpenTwinkleCard(signal);
                        m_twinkleCard.Add(signal);
                    }
                }
            }


            return true;
        }
        private bool HideSkillEnableCard(GamePlayer player1, GamePlayer player2)
        {
            for (int i = 0; i < m_twinkleCard.Count; i++)
            {
                if(m_twinkleCard[i] != null)
                {
                    DisplayActor.CloseTwinkleCard(m_twinkleCard[i]);
                }
            }
            m_twinkleCard.Clear();
            return true;
        }
    }

    public class PlayerChainStates : ChainStates
    {
        public PlayerChainStates(EventPara eventPara, GameStates gameStates)
            : base(eventPara, gameStates)
        {
            m_playerType = PlayerType.Player;
        }

        public override string GetCurrentStatusName()
        {
            return "我方连锁阶段";
        }
    }

    public class EnemyChainStates : ChainStates
    {
        public EnemyChainStates(EventPara eventPara, GameStates gameStates)
            : base(eventPara, gameStates)
        {
            m_playerType = PlayerType.Enemy;
        }

        public override string GetCurrentStatusName()
        {
            return "敌方连锁阶段";
        }
    }
}

