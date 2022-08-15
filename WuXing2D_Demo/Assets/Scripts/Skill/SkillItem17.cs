using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;

namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：激活/关闭纪行卡牌
    /// </summary>
    public class SkillItem17 : SkillItem
    {
        private TargetItem m_targetEpoch;
        private List<GameObject> m_selectEpoch;//选择卡牌
        private bool m_isActive;//激活

        public SkillItem17()
        {
            m_skillItemName = "SkillItem17";

            m_targetEpoch = new TargetSimple();
            m_selectEpoch = new List<GameObject>();
            m_isActive = true;
        }


        public override void Initial(SkillInfoNode skillInfoNode)
        {
            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                InitialSingle(skillInfoNode.m_childNode[i]);
            }
        }
        protected override void InitialSingle(SkillInfoNode skillInfoNode)
        {
            string name = skillInfoNode.m_nodeName;
            string value = skillInfoNode.m_nodeValue;
            switch (name)
            {
                case "Target":
                    TargetSimple targetSimple = new TargetSimple();
                    targetSimple.Initial(skillInfoNode);
                    m_targetEpoch = targetSimple;
                    break;
                case "Active":
                    m_isActive = bool.Parse(value);
                    break;
                default:
                    base.InitialSingle(skillInfoNode);
                    break;
            }
        }



        public override bool IsTarget(GameObject origin, GameObject target)
        {
            return m_targetEpoch.IsMatch(origin, target);
        }

        public override bool SelectTarget(GameObject origin, GameObject trigger, GameObject target)
        {
            if (m_selectType == SelectType.Auto)
            {
                AutoSelect(origin, trigger, target);
                m_selectComplete = true;
                return true;
            }
            else
            {
                ManualSelect(origin, trigger, target);
                return false;
            }
        }
        private bool AutoSelect(GameObject origin, GameObject trigger, GameObject target)
        {
            List<GameObject> select = new List<GameObject>();
            select.AddRange(GameListener.Instance().GetEpochCard());

            for (int i = 0; i < select.Count; i++)
            {
                if (IsTarget(origin, select[i]))
                    m_selectEpoch.Add(select[i]);
            }

            return true;
        }
        private bool ManualSelect(GameObject origin, GameObject trigger, GameObject target)
        {
            m_selectEvent.m_origin = origin;
            m_selectEvent.m_trigger = trigger;
            m_selectEvent.m_target = target;

            EventPara eventPara = new EventPara();
            eventPara.m_origin = origin;
            eventPara.m_trigger = this.gameObject;
            eventPara.m_playerType = GetExcutePlayer(origin);

            GameListener.Instance().ChainEventNotify(ChainEventID.evt_startselect, eventPara);
            return true;
        }



        public override bool PreSkill(GameObject origin, GameObject trigger, GameObject target)
        {
            if (!m_selectComplete)
            {
                if (!SelectTarget(origin, trigger, target))
                    return false;
            }

            return true;
        }
        public override bool LaunchSkill(GameObject origin, SkillControl skillControl, bool free)
        {
            if (!m_selectComplete)
            {
                m_selectEpoch.Clear();
                return true;
            }

            while (m_selectEpoch.Count > 0)
            {
                GameObject target = m_selectEpoch[0];
                m_selectEpoch.RemoveAt(0);

                if(m_isActive)
                {
                    EventPara eventPara = new EventPara();
                    eventPara.m_origin = origin;
                    eventPara.m_trigger = origin;
                    eventPara.m_target = target;
                    if (GameListener.Instance().ChainEventNotify(ChainEventID.evt_epochactive, eventPara))
                    {
                        if (!free && !skillControl.AddWaitEvent(origin, origin, target, TriggerType.Come))
                            return false;
                    }
                }
                else
                {
                    EventPara eventPara = new EventPara();
                    eventPara.m_origin = origin;
                    eventPara.m_trigger = origin;
                    eventPara.m_target = target;
                    if (GameListener.Instance().ChainEventNotify(ChainEventID.evt_epochclose, eventPara))
                    {
                        if (!free && !skillControl.AddWaitEvent(origin, origin, target, TriggerType.Left))
                            return false;
                    }
                }
                

            }


            m_selectComplete = false;
            m_selectEpoch.Clear();
            return true;
        }




        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            return m_targetEpoch.SetTargetPara(origin, trigger, target, select);
        }
        public override bool SetSelectCard(List<GameObject> target, bool isComplete)
        {
            m_selectEpoch.AddRange(target);
            m_selectComplete = isComplete;
            return true;
        }


        public override List<GameObject> GetSelectCard()
        {
            return m_selectEpoch;
        }

        public override bool ResetSkillItem()
        {
            m_selectComplete = false;
            m_selectEpoch.Clear();
            if (m_selectType == SelectType.Auto)
                return false;
            else
                return true;
        }
    }
}

