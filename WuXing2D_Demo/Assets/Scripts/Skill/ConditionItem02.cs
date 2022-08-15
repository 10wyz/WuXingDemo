using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;

namespace WuXingSkill
{
    /// <summary>
    /// 条件组件：满足触发动作
    /// </summary>
    public class ConditionItem02 : ConditionItem
    {
        private TargetItem m_trigger;//触发行为的卡牌的要求
        private TargetItem m_target;//协助触发行为的卡牌的要求
        private List<TriggerType> m_triggerType;//触发方式

        public ConditionItem02()
        {
            m_trigger = new TargetSimple();
            m_target = new TargetSimple();
            m_triggerType = new List<TriggerType>();
        }
        public override void Initial(SkillInfoNode skillInfoNode, PlayerType playerType)
        {
            
            for(int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;

                switch (name)
                {
                    case "Trigger":
                        TargetSimple targetSimple = new TargetSimple();
                        targetSimple.Initial(skillInfoNode.m_childNode[i]);
                        m_trigger = targetSimple;
                        break;
                    case "Target":
                        TargetSimple targetSimple2 = new TargetSimple();
                        targetSimple2.Initial(skillInfoNode.m_childNode[i]);
                        m_target = targetSimple2;
                        break;
                    case "TriggerType":
                        m_triggerType.Add(SkillTranslate.GetTriggerType(value));
                        break;
                    default:
                        break;
                }    
            }
        }

        public override bool IsMatch(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            return IsMatchTriggerType(triggerType) && IsMatchTrigger(origin, trigger) && IsMatchTarget(origin, target);
        }


        private bool IsMatchTrigger(GameObject origin, GameObject trigger)
        {
            return m_trigger.IsMatch(origin, trigger);
        }
        private bool IsMatchTarget(GameObject origin, GameObject target)
        {
            return m_target.IsMatch(origin, target);
        }
        private bool IsMatchTriggerType(TriggerType triggerType)
        {
            if (m_triggerType.Count == 0)
                return true;
            for (int i = 0; i < m_triggerType.Count; i++)
            {
                if (m_triggerType[i] == triggerType)
                    return true;
            }
            return false;
        }
    }
}

