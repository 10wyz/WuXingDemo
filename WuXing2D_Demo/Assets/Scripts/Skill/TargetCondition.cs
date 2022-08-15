using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;

namespace WuXingSkill
{
    public class TargetCondition
    {
        virtual public void Initial(SkillInfoNode skillInfoNode)
        {

        }
        virtual protected void InitialSingle(SkillInfoNode skillInfoNode)
        {

        }
        virtual public bool IsMatch(GameObject target)
        {
            return true;
        }

        virtual public bool IsEqual(TargetCondition other)
        {
            if(other.GetType() != this.GetType())
                return false;

            return true;
        }
    }

    public class TargetCondition03 : TargetCondition
    {
        List<TriggerPhase> m_triggerPhase;

        public TargetCondition03()
        {
            m_triggerPhase = new List<TriggerPhase>();
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
                case "TriggerPhase":
                    m_triggerPhase.Add(SkillTranslate.GetTriggerPhase(value));
                    break;
                default:
                    break;
            }
        }

        public override bool IsMatch(GameObject target)
        {
            if(target.name != "ConditionItem03")
                return false;

            ConditionItem03 conditionItem03 = target.GetComponent<ConditionItem03>();

            List<TriggerPhase> list = conditionItem03.GetTriggerPhase();
            for(int i = 0; i < list.Count; i++)
            {
                if(m_triggerPhase.Contains(list[i]))
                    return true;
            }

            return false;
        }

        public override bool IsEqual(TargetCondition other)
        {
            if(other.GetType() != this.GetType())
                return false;

            TargetCondition03 targetCondition03 = other as TargetCondition03;

            if (m_triggerPhase.Count != targetCondition03.m_triggerPhase.Count)
                return false;

            for(int i = 0; i < m_triggerPhase.Count; i++)
            {
                if (!targetCondition03.m_triggerPhase.Contains(m_triggerPhase[i]))
                    return false;
            }
            return true;
        }
    }
}

