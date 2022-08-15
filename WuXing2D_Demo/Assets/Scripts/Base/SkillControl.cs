using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingSkill;
using WuXingCore;
using WuXingWindow;

namespace WuXingBase
{
    public class SkillControl
    {
        ScreenPlay m_screenPlay;

        Stack<WaitEvent> m_waitEvents;//待响应事件（队列）
        Stack<WaitSkill> m_waitSkills;//待发动技能（栈）
        List<ChainSkill> m_chainSkills;//可以连锁的技能列表
        List<ChainSkill> m_chainFreeSkills;//不产生连锁，不消耗事件的连锁技能列表
        Stack<WaitSkill> m_waitFreeSkills;//待发动的非连锁技能（不会产生事件）

        ChainListWindow m_chainListWindow;

        bool m_skillAnimaFlag = true;
        public SkillControl(ScreenPlay screenPlay)
        {
            m_screenPlay = screenPlay;

            m_waitEvents = new Stack<WaitEvent>();
            m_waitSkills = new Stack<WaitSkill>();
            m_chainSkills = new List<ChainSkill>();
            m_chainFreeSkills = new List<ChainSkill>();
            m_waitFreeSkills = new Stack<WaitSkill>();

            m_skillAnimaFlag = true;

            m_chainListWindow = GameObject.Find("ChainListTip").GetComponent<ChainListWindow>();
        }


        public bool SkillProcess()
        {
            if (!HandleWaitEvent())
                return false;
            if (!HandleWaitSkill())
                return false;
            if (!HandleWaitDestroyCard())
                return false;

            return true;
        }
        public bool CardProcess()
        {
            HandleWaitDestroyCard();
            return true;
        }



        /// <summary>
        /// 处理待处理事件
        /// </summary>
        /// <returns>处理完成返回true，还有事件残留返回false</returns>
        private bool HandleWaitEvent()
        {
            if(m_waitEvents.Count > 0 && m_waitEvents.Peek().m_continueResponse == 0)
            {
                m_screenPlay.Notify(m_waitEvents.Peek());
                HandleFreeSkill(m_waitEvents.Peek());
                if (m_waitEvents.Peek().IsFreeEvent())
                    m_waitEvents.Pop();
            }


            if(!GameListener.Instance().IsChainEnable())
            {
                if(m_waitEvents.Count > 0)
                    m_waitEvents.Pop();
                return false;
            }
                

            while(m_waitEvents.Count > 0)
            {
                WaitEvent waitEvent = m_waitEvents.Peek();



                if(waitEvent.m_continueResponse == 0)
                {
                    waitEvent.m_continueResponse = 1;

                    //切换到对方进行响应
                    if (HasChainSkill(waitEvent, "Enemy"))
                    {
                        Debug.Log("敌方响应");

                        EventPara eventPara = new EventPara();
                        eventPara.m_origin = waitEvent.m_origin;
                        eventPara.m_target = waitEvent.m_target;
                        eventPara.m_trigger = waitEvent.m_trigger;
                        eventPara.m_triggerType = waitEvent.m_triggerType;
                        if(eventPara.m_origin.GetComponent<Card>().GetPlayerType() == PlayerType.Player)
                            eventPara.m_playerType = PlayerType.Enemy;
                        else
                            eventPara.m_playerType = PlayerType.Player;
                        GameListener.Instance().ChainEventNotify(ChainEventID.evt_startchain, eventPara);
                        return false;
                    }
                }

                if(waitEvent.m_continueResponse == 1)
                {
                    //切换到我方进行响应
                    if (HasChainSkill(waitEvent, "Player"))
                    {
                        waitEvent.m_continueResponse = 2;

                        EventPara eventPara = new EventPara();
                        eventPara.m_origin = waitEvent.m_origin;
                        eventPara.m_target = waitEvent.m_target;
                        eventPara.m_trigger = waitEvent.m_trigger;
                        eventPara.m_triggerType = waitEvent.m_triggerType;
                        if (eventPara.m_origin.GetComponent<Card>().GetPlayerType() == PlayerType.Player)
                            eventPara.m_playerType = PlayerType.Player;
                        else
                            eventPara.m_playerType = PlayerType.Enemy;
                        GameListener.Instance().ChainEventNotify(ChainEventID.evt_startchain, eventPara);
                        return false;
                    }
                }

                RemoveWaitEvent();
            }

            return true;
        }
        /// <summary>
        /// 处理待发动技能
        /// </summary>
        /// <returns></returns>
        //private bool HandleWaitSkill(WaitSkill waitSkill)
        //{
        //    while(m_waitSkills.Contains(waitSkill))
        //    {
        //        WaitSkill tempWaitSkill = m_waitSkills.Peek();
        //        if(tempWaitSkill.LaunchSkill())
        //        {
        //            m_waitSkills.Pop();
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }

        //    if(m_waitSkills.Count == 0 && m_waitEvents.Count == 0)
        //        HandleWaitDestroyCard();

        //    return true;
        //}
        private bool HandleWaitSkill()
        {
            while (m_waitSkills.Count>0)
            {
                WaitSkill tempWaitSkill = m_waitSkills.Peek();
                if (m_skillAnimaFlag)
                {
                    m_skillAnimaFlag = false;

                    DisplayActor.DisplaySkillLaunchCard(tempWaitSkill.m_origin);
                    return false;
                }


                if (tempWaitSkill.LaunchSkill())
                {
                    m_waitSkills.Pop();
                    m_chainListWindow.RemoveCard();
                    m_skillAnimaFlag = true;
                }
                else
                {
                    m_skillAnimaFlag = true;
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// 处理待破坏卡牌
        /// </summary>
        /// <returns></returns>
        private bool HandleWaitDestroyCard()
        {
            //List<GameObject> list = new List<GameObject>(m_waitDestroyCard);
            //while(list.Count > 0)
            //{
            //    GameObject gameObject = list[0];
            //    list.RemoveAt(0);
            //    GameObject.Destroy(gameObject);
            //}
            //m_waitDestroyCard.Clear();
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_clear_deadcard, new EventPara());


            return true;
        }
        /// <summary>
        /// 处理（发动）自由连锁技能
        /// </summary>
        /// <returns></returns>
        private bool HandleFreeSkill(WaitEvent waitEvent)
        {
            for (int i = 0; i < m_chainFreeSkills.Count; i++)
            {
                if (m_chainFreeSkills[i].IsSkillActivate(waitEvent, "Player") || m_chainFreeSkills[i].IsSkillActivate(waitEvent, "Enemy"))
                {
                    Card card = m_chainFreeSkills[i].m_origin.GetComponent<Card>();
                    int index = card.GetSkillIndex(m_chainFreeSkills[i].m_skill);
                    card.GetComponent<Card>().OnSkill(waitEvent.m_trigger, waitEvent.m_target, waitEvent.m_triggerType, index);

                    HandleWaitFreeSkill();
                }
            }

            return true;
        }
        private bool HandleWaitFreeSkill()
        {
            while (m_waitFreeSkills.Count > 0)
            {
                WaitSkill tempWaitSkill = m_waitFreeSkills.Peek();
                if (tempWaitSkill.LaunchSkill())
                {
                    m_waitFreeSkills.Pop();
                }
            }

            return true;
        }


        /// <summary>
        /// 添加待响应事件
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="trigger"></param>
        /// <param name="target"></param>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public bool AddWaitEvent(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            WaitEvent waitEvent = new WaitEvent(origin, trigger, target, triggerType);
            m_waitEvents.Push(waitEvent);
            //m_waitEvents.Enqueue(waitEvent);

            return HandleWaitEvent();
        }
        /// <summary>
        /// 添加待执行技能
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public bool AddWaitSkill(GameObject origin, Skill skill)
        {
            WaitSkill waitSkill = new WaitSkill(origin, skill);
            m_waitSkills.Push(waitSkill);
            m_chainListWindow.AddCard(waitSkill.m_origin);

            return true;
            //return HandleWaitSkill(waitSkill);
        }
        /// <summary>
        /// 添加可连锁技能
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="skill"></param>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public bool AddChainSkill(GameObject origin, Skill skill, TriggerType triggerType)
        {
            ChainSkill chainSkill = new ChainSkill(origin, skill, triggerType);
            m_chainSkills.Add(chainSkill);
            return true;
        }
        /// <summary>
        /// 添加自由连锁技能
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="skill"></param>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public bool AddChainFreeSkill(GameObject origin, Skill skill, TriggerType triggerType)
        {
            ChainSkill chainSkill = new ChainSkill(origin, skill, triggerType);
            m_chainFreeSkills.Add(chainSkill);
            return true;
        }
        /// <summary>
        /// 添加待发动自由技能
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public bool AddWaitFreeSkill(GameObject origin, Skill skill)
        {
            WaitSkill waitSkill = new WaitSkill(origin, skill);
            m_waitFreeSkills.Push(waitSkill);

            return true;
        }


        public bool ReAddWaitEvent(EventPara eventPara, int flag)
        {
            //待响应事件队列为空 或 最前面的事件从未被响应过 才执行重新加入
            //（说明被响应的事件已经被删除， 之前点的确定，然后又右键取消了响应该事件）
            if(m_waitEvents.Count == 0 || m_waitEvents.Peek().m_continueResponse == 0)
            {
                WaitEvent waitEvent = new WaitEvent(eventPara.m_origin, eventPara.m_trigger, eventPara.m_target, eventPara.m_triggerType);
                waitEvent.m_continueResponse = flag;

                m_waitEvents.Push(waitEvent);
                //Queue<WaitEvent> temp = new Queue<WaitEvent>();

                //temp.Enqueue(waitEvent);
                //while (m_waitEvents.Count > 0)
                //{
                //    temp.Enqueue(m_waitEvents.Dequeue());
                //}
                //m_waitEvents = temp;
            }
            return true;
        }


        /// <summary>
        /// 移除等待事件
        /// </summary>
        /// <returns></returns>
        public bool RemoveWaitEvent()
        {
            m_waitEvents.Pop();
            //m_waitEvents.Dequeue();
            return true;
        }
        /// <summary>
        /// 移除可连锁技能
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool RemoveChainSkill(GameObject origin)
        {
            List< ChainSkill > list = new List<ChainSkill> ();
            for(int i = 0; i < m_chainSkills.Count; i++)
            {
                if(m_chainSkills[i].IsBelongCard(origin))
                    list.Add(m_chainSkills[i]);
            }

            for(int i = 0; i < list.Count; i++)
            {
                m_chainSkills.Remove(list[i]);
            }


            List<ChainSkill> list2 = new List<ChainSkill>();
            for (int i = 0; i < m_chainFreeSkills.Count; i++)
            {
                if (m_chainFreeSkills[i].IsBelongCard(origin))
                    list2.Add(m_chainFreeSkills[i]);
            }

            for (int i = 0; i < list2.Count; i++)
            {
                m_chainFreeSkills.Remove(list2[i]);
            }

            return true;
        }


        /// <summary>
        /// 是否有可连锁技能
        /// </summary>
        /// <param name="waitEvent"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool HasChainSkill(WaitEvent waitEvent, string player)
        {
            for(int i = 0; i < m_chainSkills.Count; i++)
            {
                if (m_chainSkills[i].IsSkillActivate(waitEvent, player))
                    return true;
            }    
            return false;
        }
    } 


    public class WaitEvent
    {
        public GameObject m_origin;//事件发起者
        public GameObject m_trigger;//事件主动对象
        public GameObject m_target;//事件被动者
        public TriggerType m_triggerType;//事件代码
        public int m_continueResponse;//是否可以继续响应

        public WaitEvent(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            m_origin = origin;
            m_trigger = trigger;
            m_target = target;
            m_triggerType = triggerType;
            m_continueResponse = 0;
        }

        public bool IsFreeEvent()
        {
            if (m_triggerType == TriggerType.StageAbondan || m_triggerType == TriggerType.StageEnd ||
                m_triggerType == TriggerType.StageMain)
                return true;

            return false;
        }
    }
    public class WaitSkill
    {
        public GameObject m_origin;//技能拥有者
        public Skill m_skill;//待发动技能

        public WaitSkill(GameObject origin, Skill skill)
        {
            m_origin = origin;
            m_skill = skill;
        }


        /// <summary>
        /// 激活技能
        /// </summary>
        /// <returns></returns>
        public bool LaunchSkill()
        {
            return m_skill.LauchSkill(m_origin);
        }
    }
    public class ChainSkill
    {
        public GameObject m_origin;//技能拥有者
        public Skill m_skill;//注册技能
        public TriggerType m_triggerType;//触发条件

        public ChainSkill(GameObject origin, Skill skill, TriggerType triggerType)
        {
            m_origin = origin;
            m_skill = skill;
            m_triggerType = triggerType;
        }


        /// <summary>
        /// 技能可以被事件激活
        /// </summary>
        /// <param name="waitEvent"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsSkillActivate(WaitEvent waitEvent, string player)
        {
            if(IsEqual(waitEvent.m_triggerType, m_triggerType))
            {
                switch(player)
                {
                    case "Player":
                        if (waitEvent.m_origin.GetComponent<Card>().GetPlayerType() == m_origin.GetComponent<Card>().GetPlayerType())
                        {
                            return m_skill.IsSkillEnable(m_origin, waitEvent.m_trigger, waitEvent.m_target, waitEvent.m_triggerType);
                        }
                        return false;
                    case "Enemy":
                        if (waitEvent.m_origin.GetComponent<Card>().GetPlayerType() != m_origin.GetComponent<Card>().GetPlayerType())
                        {
                            return m_skill.IsSkillEnable(m_origin, waitEvent.m_trigger, waitEvent.m_target, waitEvent.m_triggerType);
                        }
                        return false;
                    default:
                        return false;
                }
            }

            return false;
        }
        public bool IsBelongCard(GameObject origin)
        {
            return m_origin == origin;
        }


        private bool IsEqual(TriggerType waitEvent, TriggerType skillEvent)
        {
            switch (skillEvent)
            {
                case TriggerType.Left:
                    return waitEvent == TriggerType.Left || waitEvent == TriggerType.Damage;
                default:
                    return skillEvent == waitEvent;
            }
        }
    }

}

