using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;

namespace WuXingSkill
{
    /// <summary>
    /// 条件组件：场上特定卡牌满足规定数量
    /// </summary>
    public class ConditionItem04 : ConditionItem
    {
        private TargetItem m_targetCard;
        private int m_minNum;
        private int m_maxNum;

        public ConditionItem04()
        {
            m_targetCard = new TargetSimple();
            m_minNum = 0;
            m_maxNum = int.MaxValue;
        }

        public override void Initial(SkillInfoNode skillInfoNode, PlayerType playerType)
        {
            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;

                switch (name)
                {
                    case "Target":
                        TargetSimple targetSimple = new TargetSimple();
                        targetSimple.Initial(skillInfoNode.m_childNode[i]);
                        m_targetCard = targetSimple;
                        break;
                    case "MaxNum":
                        m_maxNum = int.Parse(value);
                        break;
                    case "MinNum":
                        m_minNum = int.Parse(value);
                        break;
                    default:
                        break;
                }
            }
        }


        public override bool IsMatch(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            List<GameObject> list = new List<GameObject>();
            list.AddRange(GameListener.Instance().GetSiteCard());
            list.AddRange(GameListener.Instance().GetHandCard());
            list.AddRange(GameListener.Instance().GetDeadCard());
            list.AddRange(GameListener.Instance().GetEpochCard());
            int num = 0;
            for(int i = 0; i < list.Count; i++)
            {
                if(m_targetCard.IsMatch(origin, list[i]))
                    num++;
            }

            return num >= m_minNum && num <= m_maxNum;
        }
    }
}

