using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;

namespace WuXingSkill
{
    /// <summary>
    /// 条件组件：剧本标志
    /// </summary>
    public class ConditionItem05 : ConditionItem
    {
        private List<int> m_index;
        private bool m_reverse;//是否相反

        public ConditionItem05()
        {
            m_index = new List<int>();
            m_reverse = false;
        }

        public override void Initial(SkillInfoNode skillInfoNode, PlayerType playerType)
        {
            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;

                switch (name)
                {
                    case "Index":
                        m_index.Add(int.Parse(value));
                        break;
                    case "Reverse":
                        m_reverse = bool.Parse(value);
                        break;
                    default:
                        break;
                }
            }
        }


        public override bool IsMatch(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            bool result = true;
            for(int i = 0; i < m_index.Count; i++)
            {
                if(m_reverse)
                {
                    result = result && !GameListener.Instance().GetScreenEventFlag(m_index[i]);
                }
                else
                {
                    result = result && GameListener.Instance().GetScreenEventFlag(m_index[i]);
                }
            }

            return result;
        }
    }
}

