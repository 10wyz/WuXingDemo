using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;

namespace WuXingSkill
{
    /// <summary>
    /// 条件组件：始牌，终牌
    /// </summary>
    public class ConditionItem03 : ConditionItem
    {
        private List<TriggerPhase> m_triggerPhases;

        public ConditionItem03()
        {
            m_triggerPhases = new List<TriggerPhase>();
        }

        public override void Initial(SkillInfoNode skillInfoNode, PlayerType playerType)
        {

            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;

                switch (name)
                {
                    case "TriggerPhase":
                        m_triggerPhases.Add(SkillTranslate.GetTriggerPhase(value));
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool IsMatch(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            Card card = origin.GetComponent<Card>();
            if (m_triggerPhases.Contains(card.GetTriggerPhase()))
                return true;

            return false;
        }


        public List<TriggerPhase> GetTriggerPhase()
        {
            return m_triggerPhases;
        }
    }
}

