using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;
using WuXingCore;

namespace WuXingSkill
{
    /// <summary>
    /// 技能组件：抽取卡牌
    /// </summary>
    public class SkillItem04 : SkillItem
    {
        struct DeckProperty
        {
            public WuXing wuXing;
            public SanCai sanCai;
        }

        private TargetItem m_targetCard;
        private Dictionary<DeckProperty, int> m_selectDeck;//选择卡组


        public SkillItem04()
        {
            m_skillItemName = "SkillItem04";

            m_targetCard = new TargetSimple();
            m_selectDeck = new Dictionary<DeckProperty, int>();
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
                default:
                    base.InitialSingle(skillInfoNode);
                    break;
            }
        }



        public override bool IsTarget(GameObject origin, GameObject targetCard)
        {

            if (m_targetCard.IsMatch(origin, targetCard))
            {
                DeckProperty deckProperty;
                deckProperty.sanCai = targetCard.GetComponent<Card>().GetBaseOwner();
                deckProperty.wuXing = targetCard.GetComponent<Card>().GetBaseProprty();
                int choosenum = m_selectDeck.ContainsKey(deckProperty) ? m_selectDeck[deckProperty] : 0;
                //Todo 判断卡组是否还有可以抽取的卡
                int decknum = GameListener.Instance().GetCardDeck().GetCardNum(deckProperty.sanCai, deckProperty.wuXing);

                if (decknum > choosenum)
                    return true;
                else
                    return false;
            }

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
                if(m_targetNum.GetNum(origin) != 0)
                {
                    ManualSelect(origin, trigger, target);
                    return false;
                }
                return true;
            }
        }
        private bool AutoSelect(GameObject origin, GameObject trigger, GameObject target)
        {
            PlayerType playerType = origin.GetComponent<Card>().GetPlayerType();
            List<GameObject> select = new List<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    SanCai sanCai = (SanCai)i;
                    WuXing wuXing = (WuXing)j;

                    GameObject gameObject = CardCreator.CreateDeckCard(sanCai, wuXing, playerType);
                    select.Add(gameObject);
                }
            }


            for (int i = 0; i < select.Count; i++)
            {
                if (IsTarget(origin, select[i]))
                {
                    DeckProperty deckProperty;
                    deckProperty.sanCai = select[i].GetComponent<Card>().GetBaseOwner();
                    deckProperty.wuXing = select[i].GetComponent<Card>().GetBaseProprty();
                    if (m_selectDeck.ContainsKey(deckProperty))
                        m_selectDeck[deckProperty]++;
                    else
                        m_selectDeck[deckProperty] = 1;
                }
                GameObject.Destroy(select[i]);
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

            //开启卡组选择板
            WindowPara windowPara = new WindowPara();
            windowPara.m_window = WindowCreator.CreateCardDeckWindow(GameListener.Instance().GetCardDeck(), GetTargetNum(origin));
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
                m_selectDeck.Clear();
                return true;
            }

            List<KeyValuePair<DeckProperty, int>> list = new List<KeyValuePair<DeckProperty, int>>(m_selectDeck);
            while (m_selectDeck.Count > 0)
            {
                DeckProperty deckProperty = list[0].Key;
                int num = list[0].Value;
                if(num == 1)
                {
                    m_selectDeck.Remove(deckProperty);
                    list.RemoveAt(0);
                }
                else
                {
                    list[0] = new KeyValuePair<DeckProperty, int>(deckProperty, num-1);
                    m_selectDeck[deckProperty]--;
                }
                EventPara eventPara = new EventPara();
                eventPara.m_origin = origin;
                eventPara.m_trigger = origin;
                eventPara.m_target = CardCreator.CreateDeckCard(deckProperty.sanCai, deckProperty.wuXing, GetExcutePlayer(origin));
                if (!GameListener.Instance().ChainEventNotify(ChainEventID.evt_drawrole, eventPara))
                {
                    GameObject.Destroy(eventPara.m_target);
                    return false;
                }
                else
                {
                    GameObject.Destroy(eventPara.m_target);
                }
            }
            m_selectComplete = false;
            m_selectDeck.Clear();
            return true;
        }


        public override bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            return m_targetCard.SetTargetPara(origin, trigger, target, select);
        }
        public override bool SetSelectCard(List<GameObject> target, bool isComplete)
        {
            m_selectDeck.Clear();

            for (int i = 0; i < target.Count; i++)
            {
                DeckProperty deckProperty;
                deckProperty.sanCai = target[i].GetComponent<Card>().GetBaseOwner();
                deckProperty.wuXing = target[i].GetComponent<Card>().GetBaseProprty();
                if (m_selectDeck.ContainsKey(deckProperty))
                    m_selectDeck[deckProperty]++;
                else
                    m_selectDeck[deckProperty] = 1;
            }
            
            m_selectComplete = isComplete;

            return true;
        }

        public override bool ResetSkillItem()
        {
            m_selectComplete = false;
            m_selectDeck.Clear();


            if (m_selectType == SelectType.Auto)
                return false;
            else
                return true;
        }
    }
}

