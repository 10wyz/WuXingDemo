using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingSkill;
using WuXingTool;
using WuXingWindow;
using WuXingButton;

namespace WuXingCore
{

    public class SelectState : GameStates
    {
        protected GameObject m_window;
        protected GameObject m_connectCard;//关联卡牌
        protected GameObject m_connectSkillItem;//关联技能
        protected int m_selectnum;//待选择卡牌数量
        protected bool m_selectCompelete;//选择完成
        protected List<GameObject> m_twinkleCard;
        protected List<GameObject> m_deckCard;//卡组中可选择的牌
        public SelectState(GameObject targetCard, GameObject targetSkillItem, GameStates gameStates)
        {
            m_connectSkillItem = targetSkillItem;
            m_connectCard = targetCard;
            m_selectnum = m_connectSkillItem.GetComponent<SkillItem>().GetTargetNum(m_connectCard);
            m_beforeState = gameStates;
            m_selectCompelete = false; 

            m_twinkleCard = new List<GameObject>();
            m_deckCard = new List<GameObject>();
        }


        

        public override bool PreProcess(GamePlayer player1, GamePlayer player2)
        {
            ShowSkillEnableCard(player1, player2);

            base.PreProcess(player1, player2);
            return true;
        }
        public override bool EndProcess(GamePlayer player1, GamePlayer player2)
        {
            HideSkillEnableCard(player1, player2);


            for(int i=0; i<m_deckCard.Count; i++)
            {
                GameObject.Destroy(m_deckCard[i]);
            }

            if(m_window != null)
            {
                GameObject.Destroy(m_window);
            }

            EventPara selectEvent = m_connectSkillItem.GetComponent<SkillItem>().GetSelectEvent();
            Skill connectSkill = m_connectCard.GetComponent<Card>().GetSkill(m_connectSkillItem);
            if (connectSkill != null)
                connectSkill.PreSkill(selectEvent.m_origin, selectEvent.m_trigger, selectEvent.m_target, TriggerType.Null);

            return true;
        }



        public override bool ControlEventNotify(ControlEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);

            switch (eventID)
            {
                case ControlEventID.evt_card_click:
                    return EventCardClick(notifyPara, player1, player2);
                case ControlEventID.evt_skill_cancel:
                    return EventSkillCancel(player);
                case ControlEventID.evt_button_click:
                    return EventButtonClick(notifyPara, player);
                default:
                    return base.ControlEventNotify(eventID, notifyPara, player1, player2);
            }
        }
        public override bool ChainEventNotify(ChainEventID eventID, NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            switch(eventID)
            {
                case ChainEventID.evt_setwindow:
                    return EventSetWindow(notifyPara);
                default:
                    return base.ChainEventNotify(eventID, notifyPara, player1, player2);
            }
            
        }


        /// <summary>
        /// 卡牌点击事件
        /// </summary>
        /// <param name="targetCard"></param>
        /// <returns></returns>
        private bool EventCardClick(NotifyPara notifyPara, GamePlayer player1, GamePlayer player2)
        {
            if (m_window != null && m_window.GetComponent<StateWindow>().IsModalWindow())
                return false;

            EventPara eventPara = notifyPara as EventPara;
            GameObject targetCard = eventPara.m_trigger;

            Card card = targetCard.GetComponent<Card>();

            switch (card.GetCardType())
            {
                case CardType.SiteCard:
                    return ClickSiteCard(targetCard);
                case CardType.HandCard:
                    return ClickHandCard(targetCard);
                case CardType.ExtraCard:
                    return ClickExtraCard(targetCard, player1, player2);
                case CardType.DeckCard:
                    return ClickDeckCard(targetCard);
                case CardType.CardDeck:
                    return ClickCardDeck(targetCard);
                case CardType.SiteSignal:
                    return ClickSiteSignal(targetCard);
                default:
                    return false;
            }

        }
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        /// <param name="notifyPara"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool EventButtonClick(NotifyPara notifyPara, GamePlayer player)
        {
            EventPara eventPara = notifyPara as EventPara;
            GameObject targetButton = eventPara.m_trigger;

            ButtonBase button = targetButton.GetComponent<ButtonBase>();

            switch (button.m_buttonTag)
            {
                case ButtonTag.SkillRollBackOk:
                    return ClickRollBackOkButton(targetButton, player);
                case ButtonTag.SkillRollBackForce:
                    return ClickRollBackForceButton(targetButton);
                case ButtonTag.OptionSelectOk:
                    return ClickOptionSelectOkButton(targetButton, player);
                case ButtonTag.OptionSelectCancel:
                    return ClickOptionSelectCancelButton(targetButton, player);
                default:
                    return false;
            }
        }
        /// <summary>
        /// 取消技能事件
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool EventSkillCancel(GamePlayer player)
        {
            if(m_window == null)
            {
                if(player.GetPlayerType() == m_connectCard.GetComponent<Card>().GetPlayerType())
                    m_window = WindowCreator.CreateRollBackWindow(0);
                else
                    m_window = WindowCreator.CreateRollBackWindow(1);
            }
            else if(m_window.GetComponent<CardDeckWindow>() != null)
            {
                GameObject.Destroy(m_window);
                if (player.GetPlayerType() == m_connectCard.GetComponent<Card>().GetPlayerType())
                    m_window = WindowCreator.CreateRollBackWindow(0);
                else
                    m_window = WindowCreator.CreateRollBackWindow(1);
            }

            return true;
        }
        


        /// <summary>
        /// 设置窗口
        /// </summary>
        /// <param name="notifyPara"></param>
        /// <returns></returns>
        private bool EventSetWindow(NotifyPara notifyPara)
        {
            if(m_window == null)
            {
                WindowPara windowPara = notifyPara as WindowPara;
                m_window = windowPara.m_window;

                List<GameObject> list = m_window.GetComponent<StateWindow>().GetCard();
                for (int i = 0; i < list.Count; i++)
                {
                    if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, list[i]))
                    {
                        DisplayActor.OpenTwinkleCard(list[i]);
                        m_twinkleCard.Add(list[i]);
                    }
                }

            }
            return true;
        }


        private bool ClickSiteCard(GameObject targetCard)
        {
            if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, targetCard))
            {
                if (m_selectCard.Contains(targetCard))
                {
                    m_selectCard.Remove(targetCard);
                    m_twinkleCard.Add(targetCard);
                    DisplayActor.OpenTwinkleCard(targetCard);
                }
                else
                {
                    m_selectCard.Add(targetCard);
                    m_twinkleCard.Remove(targetCard);
                    DisplayActor.CloseTwinkleCard(targetCard);
                    DisplayActor.DisplaySelectEmphasize(targetCard);
                    //CheckSelectComplete();
                }
            }
            return true;
        }
        private bool ClickHandCard(GameObject targetCard)
        {
            if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, targetCard) )
            {
                if(m_selectCard.Contains(targetCard))
                {
                    m_selectCard.Remove(targetCard);
                    m_twinkleCard.Add(targetCard);
                    DisplayActor.OpenTwinkleCard(targetCard);
                }
                else
                {
                    m_selectCard.Add(targetCard);
                    m_twinkleCard.Remove(targetCard);
                    DisplayActor.CloseTwinkleCard(targetCard);
                    DisplayActor.DisplaySelectEmphasize(targetCard);
                    //CheckSelectComplete();
                }
            }
            return true;
        }
        private bool ClickExtraCard(GameObject targetCard, GamePlayer player1, GamePlayer player2)
        {
            GamePlayer player = GetPlayer(player1, player2);
            GamePlayer enemy = GetEnemy(player1, player2);
            if(player.GetPlayerType() == targetCard.GetComponent<Card>().GetPlayerType())
            {
                List<GameObject> list = new List<GameObject>();
                List<GameObject> select = new List<GameObject>();
                list.AddRange(player.GetDeadCardAll());
                for (int i = 0; i < list.Count; i++)
                {
                    Card card = list[i].GetComponent<Card>();
                    if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, list[i]))
                    {
                        select.Add(list[i]);
                    }
                }
                select.AddRange(m_deckCard);
                if (select.Count > 0)
                {
                    if (m_window != null)
                        GameObject.Destroy(m_window);
                    m_window = WindowCreator.CreateCardSelectWindow(select);
                }
            }
            else
            {
                List<GameObject> list = new List<GameObject>();
                List<GameObject> select = new List<GameObject>();
                list.AddRange(enemy.GetDeadCardAll());
                for (int i = 0; i < list.Count; i++)
                {
                    Card card = list[i].GetComponent<Card>();
                    if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, list[i]))
                    {
                        select.Add(list[i]);
                    }
                }
                select.AddRange(m_deckCard);
                if (select.Count > 0)
                {
                    if(m_window != null)
                        GameObject.Destroy(m_window);
                    m_window = WindowCreator.CreateCardSelectWindow(select);
                }
            }

            return true;
        }
        private bool ClickDeckCard(GameObject targetCard)
        {
            if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, targetCard))
            {
                if (m_selectCard.Contains(targetCard))
                {
                    m_selectCard.Remove(targetCard);
                    m_twinkleCard.Add(targetCard);
                    DisplayActor.OpenTwinkleCard(targetCard);
                }
                else
                {
                    m_selectCard.Add(targetCard);
                    m_twinkleCard.Remove(targetCard);
                    DisplayActor.CloseTwinkleCard(targetCard);
                    DisplayActor.DisplaySelectEmphasize(targetCard);
                    //CheckSelectComplete();
                }
            }
            return true;
        }
        private bool ClickCardDeck(GameObject targetCard)
        {
            if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, targetCard))
            {
                m_selectCard.Add(targetCard);
                m_connectSkillItem.GetComponent<SkillItem>().SetSelectCard(m_selectCard, false);
                m_window.GetComponent<CardDeckWindow>().UpdateNumText(targetCard, GameListener.Instance().GetCardDeck());
                if(!m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, targetCard))
                {
                    m_twinkleCard.Remove(targetCard);
                    DisplayActor.CloseTwinkleCard(targetCard);
                }
                CheckSelectComplete();
            }
            return true;
        }
        private bool ClickSiteSignal(GameObject targetCard)
        {
            if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, targetCard))
            {
                if (m_selectCard.Contains(targetCard))
                {
                    m_selectCard.Remove(targetCard);
                    m_twinkleCard.Add(targetCard);
                    DisplayActor.OpenTwinkleCard(targetCard);
                }
                else
                {
                    m_selectCard.Add(targetCard);
                    m_twinkleCard.Remove(targetCard);
                    DisplayActor.CloseTwinkleCard(targetCard);
                    CheckSelectComplete();
                }
            }
            return true;
        }



        private bool ClickRollBackOkButton(GameObject targetButton, GamePlayer player)
        {
            m_window = null;

            EventPara selectEvent = m_connectSkillItem.GetComponent<SkillItem>().GetSelectEvent();
            Skill connectSkill = m_connectCard.GetComponent<Card>().GetSkill(m_connectSkillItem);
            selectEvent.m_triggerType = connectSkill.GetTriggerType();
            if (connectSkill.RollBackSkill(selectEvent.m_origin, selectEvent.m_trigger, selectEvent.m_target, selectEvent.m_triggerType))
            {
                selectEvent.m_playerType = m_connectCard.GetComponent<Card>().GetPlayerType();
                if (selectEvent.m_triggerType != TriggerType.Click)
                {
                    if (m_connectCard.GetComponent<Card>().GetPlayerType() != player.GetPlayerType())
                        GameListener.Instance().GetSkillControl().ReAddWaitEvent(selectEvent, 1);
                    else
                        GameListener.Instance().GetSkillControl().ReAddWaitEvent(selectEvent, 2);

                    GameListener.Instance().ChainEventNotify(ChainEventID.evt_startchain, selectEvent);
                }
            }

            StatesPara statesPara = new StatesPara();
            statesPara.m_gameStates = this;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_endselect, statesPara);

            return true;
        }
        private bool ClickRollBackForceButton(GameObject targetButton)
        {
            m_window = null;
            m_connectSkillItem.GetComponent<SkillItem>().SetSelectCard(m_selectCard, m_selectCompelete);

            Skill connectSkill = m_connectCard.GetComponent<Card>().GetSkill(m_connectSkillItem);
            connectSkill.FinishManualSelect();

            
            StatesPara statesPara = new StatesPara();
            statesPara.m_gameStates = this;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_endselect, statesPara);

            return true;
        }
        private bool ClickOptionSelectOkButton(GameObject targetButton, GamePlayer player)
        {
            List<string> options = m_window.GetComponent<OptionSelectWindow>().GetSelectOption();
            int type = m_window.GetComponent<OptionSelectWindow>().GetTypeIndex();

            m_window = null;

            for(int i = 0; i < options.Count; i++)
            {
                m_selectCard.Add(CardCreator.CreateFakeCard(options[i], type, player.GetPlayerType()));
            }

            m_selectCompelete = true;
            m_connectSkillItem.GetComponent<SkillItem>().SetSelectCard(m_selectCard, true);

            Skill connectSkill = m_connectCard.GetComponent<Card>().GetSkill(m_connectSkillItem);
            connectSkill.FinishManualSelect();

            StatesPara statesPara = new StatesPara();
            statesPara.m_gameStates = this;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_endselect, statesPara);

            return true;
        }
        private bool ClickOptionSelectCancelButton(GameObject targetButton, GamePlayer player)
        {
            m_window = null;

            EventPara selectEvent = m_connectSkillItem.GetComponent<SkillItem>().GetSelectEvent();
            Skill connectSkill = m_connectCard.GetComponent<Card>().GetSkill(m_connectSkillItem);
            selectEvent.m_triggerType = connectSkill.GetTriggerType();
            if (connectSkill.RollBackSkill(selectEvent.m_origin, selectEvent.m_trigger, selectEvent.m_target, selectEvent.m_triggerType))
            {
                selectEvent.m_playerType = m_connectCard.GetComponent<Card>().GetPlayerType();
                if (selectEvent.m_triggerType != TriggerType.Click)
                {
                    if (m_connectCard.GetComponent<Card>().GetPlayerType() != player.GetPlayerType())
                        GameListener.Instance().GetSkillControl().ReAddWaitEvent(selectEvent, 1);
                    else
                        GameListener.Instance().GetSkillControl().ReAddWaitEvent(selectEvent, 2);

                    GameListener.Instance().ChainEventNotify(ChainEventID.evt_startchain, selectEvent);
                }
            }

            StatesPara statesPara = new StatesPara();
            statesPara.m_gameStates = this;
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_endselect, statesPara);

            return true;
        }



        private bool ShowSkillEnableCard(GamePlayer player1, GamePlayer player2)
        {
            //手牌,场上卡牌,场地确定
            List<GameObject> list = new List<GameObject>();
            list.AddRange(player1.GetSiteCardAll());
            list.AddRange(player2.GetSiteCardAll());
            list.AddRange(player1.GetHandCardAll());
            list.AddRange(player2.GetHandCardAll());
            list.AddRange(player1.GetSiteSignal());
            list.AddRange(player2.GetSiteSignal());
            for (int i = 0; i < list.Count; i++)
            {
                if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, list[i]))
                {
                    DisplayActor.OpenTwinkleCard(list[i]);
                    m_twinkleCard.Add(list[i]);
                }
            }

            //我方死亡卡牌确定
            List<GameObject> list2 = new List<GameObject>();
            GamePlayer player = GetPlayer(player1, player2);
            list2.AddRange(player.GetDeadCardAll());
            for (int i = 0; i < list2.Count; i++)
            {
                if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, list2[i]))
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

            //敌方死亡卡牌确认
            List<GameObject> list3 = new List<GameObject>();
            GamePlayer enemy = GetEnemy(player1, player2);
            list3.AddRange(enemy.GetDeadCardAll());
            for (int i = 0; i < list3.Count; i++)
            {
                if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, list3[i]))
                {
                    DisplayActor.OpenTwinkleCard(list3[i]);
                    m_twinkleCard.Add(list3[i]);

                    GameObject signal = enemy.GetDeadSignal();
                    if (!m_twinkleCard.Contains(signal))
                    {
                        DisplayActor.OpenTwinkleCard(signal);
                        m_twinkleCard.Add(signal);
                    }
                }
            }

            //卡组中的角色牌确定
            List<CardInfo> list4 = new List<CardInfo>();
            list4.AddRange(player.GetDeckRoleCardAll());
            for (int i = 0; i < list4.Count; i++)
            {
                GameObject tempCard = CardCreator.CreateTempCard(list4[i], CardCategory.RoleCard, player.GetPlayerType());
                if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, tempCard))
                {
                    m_deckCard.Add(tempCard);
                    DisplayActor.OpenTwinkleCard(tempCard);
                    m_twinkleCard.Add(tempCard);

                    GameObject signal = player.GetDeadSignal();
                    if(!m_twinkleCard.Contains(signal))
                    {
                        DisplayActor.OpenTwinkleCard(signal);
                        m_twinkleCard.Add(signal);
                    }
                }
                else
                {
                    GameObject.Destroy(tempCard);
                }
            }

            //卡组中的额外牌确定
            List<CardInfo> list5 = new List<CardInfo>();
            list5.AddRange(player.GetDeckExtraCardAll());
            for (int i = 0; i < list5.Count; i++)
            {
                GameObject tempCard = CardCreator.CreateTempCard(list5[i], CardCategory.ExtraCard, player.GetPlayerType());
                if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, tempCard))
                {
                    m_deckCard.Add(tempCard);
                    DisplayActor.OpenTwinkleCard(tempCard);
                    m_twinkleCard.Add(tempCard);

                    GameObject signal = player.GetDeadSignal();
                    if (!m_twinkleCard.Contains(signal))
                    {
                        DisplayActor.OpenTwinkleCard(signal);
                        m_twinkleCard.Add(signal);
                    }
                }
                else
                {
                    GameObject.Destroy(tempCard);
                }
            }


            //卡组确认
            //List<GameObject> list6 = DeckDisplay.Instance().GetDeckAll();
            //for(int i = 0; i < list6.Count; i++)
            //{
            //    if (m_connectSkillItem.GetComponent<SkillItem>().IsTarget(m_connectCard, list6[i]))
            //    {
            //        CardActor.OpenTwinkleCard(list6[i]);
            //        m_twinkleCard.Add(list6[i]);
            //    }
            //}

            return true;
        }
        private bool HideSkillEnableCard(GamePlayer player1, GamePlayer player2)
        {
            for (int i = 0; i < m_twinkleCard.Count; i++)
            {
                if (m_twinkleCard[i] != null)
                    DisplayActor.CloseTwinkleCard(m_twinkleCard[i]);
            }
            m_twinkleCard.Clear();
            return true;
        }



        /// <summary>
        /// 检查是否完成卡牌选择
        /// </summary>
        /// <returns></returns>
        private bool CheckSelectComplete()
        {
            if (m_selectnum == m_selectCard.Count)
            {
                m_selectCompelete = true;
                m_connectSkillItem.GetComponent<SkillItem>().SetSelectCard(m_selectCard, true);

                Skill connectSkill = m_connectCard.GetComponent<Card>().GetSkill(m_connectSkillItem);
                connectSkill.FinishManualSelect();

                StatesPara statesPara = new StatesPara();
                statesPara.m_gameStates = this;
                GameListener.Instance().ChainEventNotify(ChainEventID.evt_endselect, statesPara);
            }
            return true;
        }


        public override bool ReEntry(GamePlayer player1, GamePlayer player2)
        {
            base.ReEntry(player1, player2);
            CheckSelectComplete();
            return true;
        }


        public override bool IsChangeStageEnable()
        {
            return false;
        }
        public override bool IsChainEnable()
        {
            return false;
        }
    }


    public class PlayerSelectStates : SelectState
    {
        public PlayerSelectStates(GameObject triggerCard, GameObject targetSkill, GameStates gameStates)
            : base(triggerCard, targetSkill, gameStates)
        {
            m_playerType = PlayerType.Player;
        }

        public override string GetCurrentStatusName()
        {
            return "我方选择阶段";
        }
    }

    public class EnemySelectStates : SelectState
    {
        public EnemySelectStates(GameObject triggerCard, GameObject targetSkill, GameStates gameStates)
            : base(triggerCard, targetSkill, gameStates)
        {
            m_playerType = PlayerType.Enemy;
        }

        public override string GetCurrentStatusName()
        {
            return "敌方选择阶段";
        }
    }

}
