using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;
using WuXingTool;

namespace WuXingSkill
{
    /// <summary>
    /// 选择卡牌参数
    /// </summary>
    public class SkillItem18 : SkillItem
    {
        private List<GameObject> m_selectCard;//选择卡牌
        private List<string> m_options;
        private int m_type;
        private int m_minNum;
        private int m_maxNum;

        public SkillItem18()
        {
            m_skillItemName = "SkillItem19";

            m_options = new List<string>();
            m_selectCard = new List<GameObject>();
            m_type = 0;
            m_minNum = 0;
            m_maxNum = 0;
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
                case "Option":
                    m_options.Add(value);
                    break;
                case "TypeIndex":
                    m_type = int.Parse(value);
                    break;
                case "MinNum":
                    m_minNum = int.Parse(value);
                    break;
                case "MaxNum":
                    m_maxNum = int.Parse(value);
                    break;
                default:
                    base.InitialSingle(skillInfoNode);
                    break;
            }

        }



        public override bool IsTarget(GameObject origin, GameObject target)
        {
            return false;
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
            return false;
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


            WindowPara windowPara = new WindowPara();
            windowPara.m_window = WindowCreator.CreateOptionSelectWindow(m_options, m_maxNum, m_type);
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_setwindow, windowPara);

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
                for (int i = 0; i < m_selectCard.Count; i++)
                {
                    GameObject.Destroy(m_selectCard[i]);
                }
                m_selectCard.Clear();
                return true;
            }

            for(int i = 0; i < m_selectCard.Count; i++)
            {
                GameObject.Destroy(m_selectCard[i]);
            }

            m_selectComplete = false;
            m_selectCard.Clear();

            return true;
        }


        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            return true;
        }
        public override bool SetSelectCard(List<GameObject> target, bool isCompelete)
        {
            if(target.Count >= m_minNum && target.Count <= m_maxNum && isCompelete)
            {
                m_selectCard.AddRange(target);
                m_selectComplete = true;
            }
            else
            {
                for(int i = 0; i < target.Count; i++)
                {
                    GameObject.Destroy(target[i]);
                }
                m_selectComplete = false;
            }
            
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

