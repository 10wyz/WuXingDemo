using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;

namespace WuXingSkill
{
    public class TargetNum
    {
        private int m_maxNum;
        private TargetItem m_targetCard;
        
        public TargetNum()
        {
            m_maxNum = 0;
            m_targetCard = new TargetSimple();
        }

        public void Initial(SkillInfoNode skillInfoNode)
        {
            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                InitialSingle(skillInfoNode.m_childNode[i]);
            }
        }

        protected void InitialSingle(SkillInfoNode skillInfoNode)
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
                case "MaxNum":
                    m_maxNum = int.Parse(value);
                    break;
                default:
                    break;
            }
        }

        public int GetNum(GameObject origin)
        {
            int matchNum = GetMatchNum(origin);

            return Mathf.Min(m_maxNum, matchNum);
        }

        private int GetMatchNum(GameObject origin)
        {
            if(m_targetCard.IsEmpty())
                return m_maxNum;

            List<GameObject> list = new List<GameObject>();
            list.AddRange(GameListener.Instance().GetHandCard());
            list.AddRange(GameListener.Instance().GetSiteCard());

            int num = 0;
            for(int i = 0; i < list.Count; i++)
            {
                if(m_targetCard.IsMatch(origin, list[i]))
                    num++;
            }

            return num;
        }
    }
}

