using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;

namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：技能免疫
    /// </summary>
    public class SkillItem20 : SkillItem
    {
        private TargetItem m_targetCard;
        private List<GameObject> m_selectCard;//选择卡牌
        private TargetItem m_immuneCard;
        private int m_turn;
        private string m_targetSkillItem;

        public SkillItem20()
        {
            m_skillItemName = "SkillItem20";

            m_targetCard = new TargetSimple();
            m_selectCard = new List<GameObject>();
            m_immuneCard = new TargetSimple();
            m_turn = 0;
            m_targetSkillItem = "SkillItem";
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
                    TargetSimple targetSimple = new TargetSimple();
                    targetSimple.Initial(skillInfoNode);
                    m_targetCard = targetSimple;
                    break;
                case "ImmuneTarget":
                    TargetSimple immuneSimple = new TargetSimple();
                    immuneSimple.Initial(skillInfoNode);
                    m_immuneCard = immuneSimple;
                    break;
                case "Turn":
                    m_turn = int.Parse(value);
                    break;
                case "TargetSkillItem":
                    m_targetSkillItem = value;
                    break;
                default:
                    base.InitialSingle(skillInfoNode);
                    break;
            }
        }



        public override bool IsTarget(GameObject origin, GameObject target)
        {
            return base.IsTarget(origin, target) && m_targetCard.IsMatch(origin, target);
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
            List<GameObject> select = new List<GameObject>();
            select.AddRange(GameListener.Instance().GetSiteCard());
            select.AddRange(GameListener.Instance().GetHandCard());
            select.AddRange(GameListener.Instance().GetDeadCard());

            for (int i = 0; i < select.Count; i++)
            {
                if (IsTarget(origin, select[i]))
                    m_selectCard.Add(select[i]);
            }

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
                m_selectCard.Clear();
                return true;
            }

            while (m_selectCard.Count > 0)
            {
                GameObject target = m_selectCard[0];
                m_selectCard.RemoveAt(0);

                
                SkillImmuneItem skillImmuneItem = new SkillImmuneItem();
                skillImmuneItem.Initial(origin, m_immuneCard, m_targetSkillItem, m_turn);

                SkillImmune skillImmune = new SkillImmune();
                skillImmune.AddImmune(skillImmuneItem);

                ImmunePara immunePara = new ImmunePara();
                immunePara.m_target = target;
                immunePara.m_skillImmune = skillImmune;

                GameListener.Instance().ChainEventNotify(ChainEventID.evt_setskillimmune, immunePara);
            }

            m_selectComplete = false;
            m_selectCard.Clear();
            return true;
        }



        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            m_targetCard.SetTargetPara(origin, trigger, target, select);
            m_immuneCard.SetTargetPara(origin, trigger, target, select);
            return true;
        }
        public override bool SetSelectCard(List<GameObject> target, bool isComplete)
        {
            m_selectCard.AddRange(target);
            m_selectComplete = isComplete;
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

