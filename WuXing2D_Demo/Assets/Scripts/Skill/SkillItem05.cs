using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;
using WuXingTool;

namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：抽取法术牌
    /// </summary>
    public class SkillItem05 : SkillItem
    {
        private int m_currentNum;
        private string m_playerType;

        public SkillItem05()
        {
            m_skillItemName = "SkillItem05";

            m_currentNum = 0;
            m_playerType = "Player";
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
                case "PlayerType":
                    m_playerType = value;
                    break;
                default:
                    base.InitialSingle(skillInfoNode);
                    break;
            }
        }



        public override bool IsTarget(GameObject origin, GameObject targetCard)
        {
            return true;
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
                if (m_targetNum.GetNum(origin) != 0)
                {
                    ManualSelect(origin, trigger, target);
                    return false;
                }
                return true;
            }
        }
        private bool AutoSelect(GameObject origin, GameObject trigger, GameObject target)
        {
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
                return true;
            }


            while(m_currentNum < m_targetNum.GetNum(origin))
            {
                m_currentNum++;
                EventPara eventPara = new EventPara();
                eventPara.m_playerType = SkillTranslate.GetPlayerType(m_playerType, origin.GetComponent<Card>().GetPlayerType());
                if (!GameListener.Instance().ChainEventNotify(ChainEventID.evt_drawspell, eventPara))
                    m_selectComplete = false;
            }

            m_currentNum = 0;
            return true;
        }



        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            return true;
        }
        public override bool SetSelectCard(List<GameObject> target, bool isComplete)
        {
            m_selectComplete = isComplete;

            return true;
        }


        public override List<GameObject> GetSelectCard()
        {
            return new List<GameObject>();
        }
        public override bool ResetSkillItem()
        {
            m_selectComplete = false;
            m_currentNum = 0;

            if (m_selectType == SelectType.Auto)
                return false;
            else
                return true;
        }
    }
}

