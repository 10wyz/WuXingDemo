using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;

namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：技能约束
    /// </summary>
    public class SkillItem16 : SkillItem
    {
        private TargetItem m_targetCard;//选择的卡牌要求
        private List<GameObject> m_selectCard;//选择卡牌
        private string m_name;
        private bool m_enable;
        private int m_time;
        private int m_turn;
        private TargetCondition m_condition;


        public SkillItem16()
        {
            m_skillItemName = "SkillItem16";

            m_targetCard = new TargetSimple();
            m_selectCard = new List<GameObject>();
            m_name = "";
            m_enable = true;
            m_time = 0;
            m_turn = 0;
            m_condition = new TargetCondition();
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
                    m_targetCard = targetSimple;
                    break;
                case "ConditionName":
                    m_name = value;
                    break;
                case "Enable":
                    m_enable = bool.Parse(value);
                    break;
                case "Time":
                    m_time = int.Parse(value);
                    break;
                case "Turn":
                    m_turn = int.Parse(value);
                    break;
                case "TargetCondition":
                    TargetCondition targetCondition = CreateTargetCondition(value);
                    targetCondition.Initial(skillInfoNode);
                    m_condition = targetCondition;
                    break;
                default:
                    base.InitialSingle(skillInfoNode);
                    break;
            }
        }

        private TargetCondition CreateTargetCondition(string name)
        {
            switch (name)
            {
                case "ConditionItem03":
                    return new TargetCondition03();
                default:
                    return null;
            }
        }

        public override bool IsTarget(GameObject origin, GameObject target)
        {
            return base.IsTarget(origin, target) && m_targetCard.IsMatch(origin, target);
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
            select.AddRange(GameListener.Instance().GetSiteCard());

            for (int i = 0; i < select.Count; i++)
            {
                if (IsTarget(origin, select[i]))
                    m_selectCard.Add(select[i]);
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
                m_selectCard.Clear();
                return true;
            }

            while (m_selectCard.Count > 0)
            {
                GameObject target = m_selectCard[0];
                m_selectCard.RemoveAt(0);


                SkillPara skillPara = new SkillPara();
                skillPara.m_target = target;
                skillPara.m_index = 0;
                skillPara.m_num = 0;

                
                SkillConstraintItem skillConstraintItem = new SkillConstraintItem();
                skillConstraintItem.m_origin = origin;
                skillConstraintItem.m_name = m_name;
                skillConstraintItem.m_enable = m_enable;
                skillConstraintItem.m_time = m_time;
                skillConstraintItem.m_turn = m_turn;
                skillConstraintItem.m_condition = m_condition;

                SkillConstraint skillConstraint = new SkillConstraint();
                skillConstraint.AddConstraint(skillConstraintItem);

                skillPara.m_skillConstraint = skillConstraint;

                GameListener.Instance().ChainEventNotify(ChainEventID.evt_skillconstraint, skillPara);
            }


            m_selectComplete = false;
            m_selectCard.Clear();
            return true;
        }




        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            m_targetCard.SetTargetPara(origin, trigger, target, select);
            return true;
        }
        public override bool SetSelectCard(List<GameObject> target, bool isComplete)
        {
            m_selectCard.AddRange(target);
            m_selectComplete = isComplete;
            return true;
        }

        public override List<GameObject> GetSelectCard()
        {
            return m_selectCard;
        }

        public override bool ResetSkillItem()
        {
            m_selectComplete = false;
            m_selectCard.Clear();
            if (m_selectType == SelectType.Auto)
                return false;
            else
                return true;
        }
    }
}


