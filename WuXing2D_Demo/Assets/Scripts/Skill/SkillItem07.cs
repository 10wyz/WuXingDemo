using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingCore;
using WuXingBase;

namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：提供抗性
    /// </summary>
    public class SkillItem07 : SkillItem
    {
        private TargetItem m_targetCard;//选择的卡牌要求
        private List<GameObject> m_selectCard;//选择卡牌
        private int m_times;
        private int m_turn;
        private TargetItem m_resistCard;//抗性抵抗的卡牌
        public SkillItem07()
        {
            m_skillItemName = "SkillItem07";

            m_targetCard = new TargetSimple();
            m_selectCard = new List<GameObject>();
            m_times = 0;
            m_turn = 0;
            m_resistCard = new TargetSimple();
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
                case "Resist":
                    TargetSimple resistSimple = new TargetSimple();
                    resistSimple.Initial(skillInfoNode);
                    m_resistCard = resistSimple;
                    break;
                case "Time":
                    m_times = int.Parse(value);
                    break;
                case "Turn":
                    m_turn = int.Parse(value);
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

                ImmuneItem immuneItem = new ImmuneItem();
                immuneItem.Initial(origin, m_resistCard, m_times, m_turn);
                Immune immune = new Immune();
                immune.AddImmuneItem(immuneItem);

                Resistance resistance = new Resistance();
                resistance.Initial(new Shield(), immune);


                ResistPara resistPara = new ResistPara();
                resistPara.m_target = target;
                resistPara.m_resistance = resistance;

                GameListener.Instance().ChainEventNotify(ChainEventID.evt_setresistance, resistPara);
            }


            m_selectComplete = false;
            m_selectCard.Clear();
            return true;
        }




        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            m_targetCard.SetTargetPara(origin, trigger, target, select);
            m_resistCard.SetTargetPara(origin, trigger, target, select);
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
