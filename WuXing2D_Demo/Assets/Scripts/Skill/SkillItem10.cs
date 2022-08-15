using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;
using WuXingTool;

namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：放置约束
    /// </summary>
    public class SkillItem10 : SkillItem
    {
        private TargetItem m_targetCard;
        private TargetItem m_targetSite;
        private bool m_enable;
        private int m_time;
        private int m_turn;
        private int m_conditionIndex;
        private string m_playerType;


        public SkillItem10()
        {
            m_skillItemName = "SkillItem10";

            m_targetCard = new TargetSimple();
            m_targetSite = new TargetSimple();
            m_turn = 0;
            m_playerType = "Enemy";
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
                    TargetSimple targetSimpleCard = new TargetSimple();
                    targetSimpleCard.Initial(skillInfoNode);
                    m_targetCard = targetSimpleCard;
                    break;
                case "Site":
                    TargetSimple targetSimpleSite = new TargetSimple();
                    targetSimpleSite.Initial(skillInfoNode);
                    m_targetSite = targetSimpleSite;
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
                case "ConditionIndex":
                    m_conditionIndex = int.Parse(value);
                    break;
                case "PlayerType":
                    m_playerType = value;
                    break;
                default:
                    base.InitialSingle(skillInfoNode);
                    break;
            }
        }



        public override bool IsTarget(GameObject origin, GameObject target)
        {
            return m_targetCard.IsMatch(origin, target);
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


            SiteConstraint siteConstraint = new SiteConstraint();
            SiteConstraintItem siteConstraintItem = new SiteConstraintItem();
            siteConstraintItem.m_origin = origin;
            siteConstraintItem.m_target = m_targetCard;
            siteConstraintItem.m_site = m_targetSite;
            siteConstraintItem.m_enable = m_enable;
            siteConstraintItem.m_index = m_conditionIndex;
            siteConstraintItem.m_time = m_time;
            siteConstraintItem.m_turn = m_turn;
            siteConstraintItem.m_playerType = origin.GetComponent<Card>().GetPlayerType();
            siteConstraint.AddConstraint(siteConstraintItem);

            SitePara sitePara = new SitePara();

            sitePara.m_siteConstraint = siteConstraint;
            sitePara.m_playerType = SkillTranslate.GetPlayerType(m_playerType, origin.GetComponent<Card>().GetPlayerType());


            GameListener.Instance().ChainEventNotify(ChainEventID.evt_siteconstraint, sitePara);



            m_selectComplete = false;

            return true;
        }




        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            m_targetCard.SetTargetPara(origin, trigger, target, select);
            m_targetSite.SetTargetPara(origin, trigger, target, select);
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
            if (m_selectType == SelectType.Auto)
                return false;
            else
                return true;
        }
    }
}

