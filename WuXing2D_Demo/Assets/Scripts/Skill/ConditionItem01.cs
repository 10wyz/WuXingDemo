using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;
using WuXingTool;

namespace WuXingSkill
{
    /// <summary>
    /// 条件组件：阵营满足规定数量
    /// </summary>
    public class ConditionItem01 : ConditionItem
    {
        private PlayerType m_playerType;
        private string m_camp;
        private int m_minNum;
        private int m_maxNum;

        public ConditionItem01()
        {
            m_camp = "";
            m_minNum = 0;
            m_maxNum = int.MaxValue;
        }

        public override void Initial(SkillInfoNode skillInfoNode, PlayerType playerType)
        {
            for(int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;

                switch(name)
                {
                    case "Camp":
                        m_camp = value;
                        break;
                    case "MaxNum":
                        m_maxNum = int.Parse(value);
                        break;
                    case "MinNum":
                        m_minNum = int.Parse(value);
                        break;
                    case "PlayerType":
                        m_playerType = SkillTranslate.GetPlayerType(value, playerType);
                        break;
                    default:
                        break;
                }
            }
        }


        public override bool IsMatch(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            if (m_camp == "")
                return true;

            Dictionary<string, int> camp = GameListener.Instance().GetCampNum(m_playerType);
            int num = 0;
            if(camp.ContainsKey(m_camp))
            {
                num = camp[m_camp];
            }

            return num >= m_minNum && num <= m_maxNum;
        }
    }
}

