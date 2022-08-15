using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;
using WuXingCore;

namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：修改盘属性
    /// </summary>
    public class SkillItem08 : SkillItem
    {
        private TargetItem m_targetCard;
        private SanCai m_owner;
        private WuXing m_property;
        private bool m_isBase;
        private string m_playerType;

        public SkillItem08()
        {
            m_skillItemName = "SkillItem08";

            m_targetCard = new TargetSimple();
            m_owner = SanCai.Sky;
            m_property = WuXing.Chaos;
            m_isBase = true;
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
                case "Target":
                    TargetSimple targetSimple = new TargetSimple();
                    targetSimple.Initial(skillInfoNode);
                    m_targetCard = targetSimple;
                    break;
                case "Owner":
                    m_owner = SkillTranslate.GetSanCai(value);
                    break;
                case "Property":
                    m_property = SkillTranslate.GetWuXing(value);
                    break;
                case "BasePara":
                    m_isBase = bool.Parse(value);
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
            if(m_property == WuXing.Chaos)
            {
                List<GameObject> select = new List<GameObject>();
                select.AddRange(GameListener.Instance().GetSiteCard());

                List<GameObject> site = new List<GameObject>();
                site.AddRange(GameListener.Instance().GetSiteSignal());
                for(int i = 0; i < site.Count; i++)
                {
                    if(site[i].transform.childCount == 1)
                        select.Add(site[i]);
                }

                for (int i = 0; i < select.Count; i++)
                {
                    if (IsTarget(origin, select[i]))
                    {
                        if (m_isBase)
                        {
                            m_owner = select[i].GetComponent<Card>().GetBaseOwner();
                            m_property = select[i].GetComponent<Card>().GetBaseProprty();
                        }
                        else
                        {
                            m_owner = select[i].GetComponent<Card>().GetRegardOwner()[0];
                            m_property = select[i].GetComponent<Card>().GetRegardProperty()[0];
                        }
                        break;
                    }
                }
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
                return true;
            }


            if (m_property != WuXing.Chaos)
            {
                CardPara cardPara = new CardPara();
                cardPara.m_owner.Add(m_owner);
                cardPara.m_property.Add(m_property);
                cardPara.m_playerType = SkillTranslate.GetPlayerType(m_playerType, origin.GetComponent<Card>().GetPlayerType());

                GameListener.Instance().ChainEventNotify(ChainEventID.evt_siteproperty, cardPara);
            }
            m_selectComplete = false;

            return true;
        }


        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            return m_targetCard.SetTargetPara(origin, trigger, target, select);
        }
        public override bool SetSelectCard(List<GameObject> target, bool isCompelete)
        {
            if(target.Count > 0)
            {
                if(m_isBase)
                {
                    m_owner = target[0].GetComponent<Card>().GetBaseOwner();
                    m_property = target[0].GetComponent<Card>().GetBaseProprty();
                }
                else
                {
                    m_owner = target[0].GetComponent<Card>().GetRegardOwner()[0];
                    m_property = target[0].GetComponent<Card>().GetRegardProperty()[0];
                }
            }
            
            m_selectComplete = isCompelete;
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

