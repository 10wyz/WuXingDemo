using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;
using WuXingTool;


namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：修改玩家属性
    /// </summary>
    public class SkillItem12 : SkillItem
    {
        private List<string> m_camp;
        private bool m_isAdd;
        private string m_playerType;

        public SkillItem12()
        {
            m_skillItemName = "SkillItem12";

            m_camp = new List<string>();
            m_isAdd = true;
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
                case "Camp":
                    m_camp.Add(value);
                    break;
                case "Add":
                    m_isAdd = bool.Parse(value);
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


            PlayerPara playerPara = new PlayerPara();
            playerPara.m_camp.AddRange(m_camp);
            playerPara.m_playerType = SkillTranslate.GetPlayerType(m_playerType, origin.GetComponent<Card>().GetPlayerType());
            playerPara.m_isadd = m_isAdd;

            
            GameListener.Instance().ChainEventNotify(ChainEventID.evt_playerpara, playerPara);

            m_selectComplete = false;

            return true;
        }




        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            return true;
        }
        public override bool SetSelectCard(List<GameObject> target, bool isComplete)
        {
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

