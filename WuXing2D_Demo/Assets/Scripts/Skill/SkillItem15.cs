using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;


namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：将卡牌放到场地上
    /// </summary>
    public class SkillItem15 : SkillItem
    {
        private TargetItem m_targetSite;
        private int m_targetIndex;
        private GameObject m_selectCard;//选择卡牌
        private GameObject m_selectSite;//选择场地
        private bool m_isSelectSite;
        private bool m_ignoreRule;//是否忽视放置规则

        public SkillItem15()
        {
            m_skillItemName = "SkillItem15";

            m_targetSite = new TargetSimple();
            m_selectCard = null;
            m_selectSite = null;
            m_isSelectSite = true;
            m_ignoreRule = false;
            m_targetIndex = -1;
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
                    m_targetSite = targetSimple;
                    break;
                case "TargetIndex":
                    m_targetIndex = int.Parse(value);
                    break;
                case "SelectSite":
                    m_isSelectSite = bool.Parse(value);
                    break;
                case "IgnoreRule":
                    m_ignoreRule = bool.Parse(value);
                    break;
                default:
                    base.InitialSingle(skillInfoNode);
                    break;
            }
        }



        public override bool IsTarget(GameObject origin, GameObject target)
        {
            if(m_isSelectSite)
            {
                GameObject site = target;
                if (target.GetComponent<Card>().GetCardType() == CardType.SiteCard)
                    site = target.transform.parent.gameObject;
                else if(target.GetComponent<Card>().GetCardType() == CardType.SiteSignal)
                {
                    if (site.transform.childCount > 1)
                        return false;
                }
                else
                {
                    return false;
                }

                GamePlayer player = GameListener.Instance().GetPlayer(m_selectCard);

                return base.IsTarget(origin, target) && m_targetSite.IsMatch(origin, target) && (player.IsCardToSiteFreeEnable(m_selectCard, site) || m_ignoreRule);
            }
            else
            {
                GamePlayer player = GameListener.Instance().GetPlayer(m_selectSite);

                return base.IsTarget(origin, target) && m_targetSite.IsMatch(origin, target) && (player.IsCardToSiteFreeEnable(target, m_selectSite) || m_ignoreRule);
            }
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
            select.AddRange(GameListener.Instance().GetSiteSignal());
            select.AddRange(GameListener.Instance().GetSiteCard());
            select.AddRange(GameListener.Instance().GetHandCard());

            for (int i = 0; i < select.Count; i++)
            {
                if(select[i].name == "wood")
                {
                    int kk = 1;
                }

                if (IsTarget(origin, select[i]))
                {
                    if(m_isSelectSite)
                        m_selectSite = select[i];
                    else
                        m_selectCard = select[i];
                    break;
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
                m_selectCard = null;
                m_selectSite = null;
                return true;
            }

            if(m_selectCard == null && m_selectType == SelectType.Auto)
            {
                AutoSelect(origin, null, null);
            }

            while(m_selectCard != null)
            {
                if (m_selectSite.transform.childCount > 1)
                {
                    GameObject siteCard = m_selectSite.transform.GetChild(1).gameObject;
                    EventPara eventPara = new EventPara();
                    eventPara.m_origin = origin;
                    eventPara.m_trigger = origin;
                    eventPara.m_target = siteCard;
                    if (GameListener.Instance().ChainEventNotify(ChainEventID.evt_left, eventPara))
                    {
                        if (!free && !skillControl.AddWaitEvent(origin, origin, siteCard, TriggerType.Left))
                            return false;
                    }
                }
                else
                {
                    GameObject targetCard = m_selectCard;
                    m_selectCard = null;

                    EventPara eventPara = new EventPara();
                    eventPara.m_origin = origin;
                    eventPara.m_trigger = targetCard;
                    eventPara.m_target = m_selectSite;
                    if (GameListener.Instance().ChainEventNotify(ChainEventID.evt_come, eventPara))
                    {
                        if (!free && !skillControl.AddWaitEvent(origin, origin, targetCard, TriggerType.Come))
                            return false;
                    }
                }
            }
            
            
            m_selectComplete = false;
            m_selectCard = null;
            m_selectSite = null;
            return true;
        }




        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            for(int i = 0; i < select.Count; i++)
            {
                if(m_targetIndex == select[i].Value)
                {
                    if (m_isSelectSite)
                        m_selectCard = select[i].Key;
                    else
                    {
                        if (select[i].Key.GetComponent<Card>().GetCardType() == CardType.SiteSignal)
                            m_selectSite = select[i].Key;
                        else
                            m_selectSite = select[i].Key.transform.parent.gameObject;
                    }
                    break;
                }
            }
            
            return m_targetSite.SetTargetPara(origin, trigger, target, select);
        }
        public override bool SetSelectCard(List<GameObject> target, bool isComplete)
        {
            if(isComplete)
            {
                Card card = target[0].GetComponent<Card>();
                if (m_isSelectSite)
                {
                    if (card.GetCardType() == CardType.SiteSignal)
                        m_selectSite = target[0];
                    else
                        m_selectSite = target[0].transform.parent.gameObject;
                }
                else
                    m_selectCard = target[0];
            }
            
            m_selectComplete = isComplete;
            return true;
        }


        public override List<GameObject> GetSelectCard()
        {
            List<GameObject> list = new List<GameObject>();
            list.Add(m_selectCard);
            return list;
        }

        public override bool ResetSkillItem()
        {
            m_selectComplete = false;

            m_selectCard = null;
            m_selectSite = null;

            if (m_selectType == SelectType.Auto)
                return false;
            else
                return true;
        }
    }
}

