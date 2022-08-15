using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;

namespace WuXingSkill
{
    public enum TargetSource { Constant, Trigger, Target, Origin, Select }

    public class TargetItem
    {
        virtual public void Initial(SkillInfoNode skillInfoNode)
        {

        }
        virtual public bool IsMatch(GameObject origin, GameObject target)
        {
            return true;
        }
        virtual public bool IsMatch(PlayerType playerType, GameObject target)
        {
            return true;
        }
        virtual public bool IsMatch(CardInfo cardInfo)
        {
            return true;
        }
        virtual public bool SetTargetPara(GameObject m_origin, GameObject m_trigger, GameObject m_target, List<KeyValuePair<GameObject,int>> m_select)
        {
            return true;
        }
        virtual public bool IsEmpty()
        {
            return false;
        }
    }

    public class TargetBase : TargetItem, IEquatable<TargetBase>
    {
        private TargetSource m_targetSource;
        private int m_targetIndex;
        private List<string> m_ID;//卡片ID
        private List<SanCai> m_baseOwner;//基本从属
        private List<SanCai> m_regardOwner;//被视为从属
        private List<WuXing> m_baseProperty;//基本属性
        private List<WuXing> m_regardProperty;//被视为属性
        private List<string> m_camp;//阵营
        private List<string> m_playerType;//所属玩家
        private List<CardType> m_cardType;//卡牌类型
        private List<CardCategory> m_cardCategorie;//卡牌分类
        private List<int> m_specialIndex;//特殊条件索引
        public TargetBase()
        {
            m_targetSource = TargetSource.Constant;
            m_targetIndex = -1;
            m_ID = new List<string>();
            m_baseOwner = new List<SanCai>();
            m_regardOwner = new List<SanCai>();
            m_baseProperty = new List<WuXing>();
            m_regardProperty = new List<WuXing>();
            m_camp = new List<string>();
            m_playerType = new List<string>();
            m_cardType = new List<CardType>();
            m_cardCategorie = new List<CardCategory>();
            m_specialIndex = new List<int>();
        }


        public override void Initial(SkillInfoNode skillInfoNode)
        {
            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;
                switch (name)
                {
                    case "TargetSource":
                        m_targetSource = SkillTranslate.GetTargetSource(value);
                        break;
                    case "TargetIndex":
                        m_targetIndex = int.Parse(value);
                        break;
                    case "ID":
                        m_ID.Add(value);
                        break;
                    case "BaseOwner":
                        m_baseOwner.Add(SkillTranslate.GetSanCai(value));
                        break;
                    case "RegardOwner":
                        m_regardOwner.Add(SkillTranslate.GetSanCai(value));
                        break;
                    case "BaseProperty":
                        m_baseProperty.Add(SkillTranslate.GetWuXing(value));
                        break;
                    case "RegardProperty":
                        m_regardProperty.Add(SkillTranslate.GetWuXing(value));
                        break;
                    case "Camp":
                        m_camp.Add(value);
                        break;
                    case "PlayerType":
                        m_playerType.Add(value);
                        break;
                    case "CardType":
                        m_cardType.Add(SkillTranslate.GetCardType(value));
                        break;
                    case "CardCategory":
                        m_cardCategorie.Add(SkillTranslate.GetCardCategory(value));
                        break;
                    case "SpecialIndex":
                        m_specialIndex.Add(int.Parse(value));
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool IsMatch(GameObject origin, GameObject target)
        {
            if(target == null)
            {
                int aa = 1;
            }

            Card originCard = origin.GetComponent<Card>();
            Card targetCard = target.GetComponent<Card>();

            for (int i = 0; i < m_ID.Count; i++)
            {
                if (!targetCard.IsMatchID(m_ID[i]))
                    return false;
            }
            for (int i = 0; i < m_baseOwner.Count; i++)
            {
                if (!targetCard.IsMatchOwnerBase(m_baseOwner[i]))
                    return false;
            }
            for (int i = 0; i < m_regardOwner.Count; i++)
            {
                if (!targetCard.IsMatchOwner(m_regardOwner[i]))
                    return false;
            }
            for (int i = 0; i < m_baseProperty.Count; i++)
            {
                if (!targetCard.IsMatchPropertyBase(m_baseProperty[i]))
                    return false;
            }
            for (int i = 0; i < m_regardProperty.Count; i++)
            {
                if (!targetCard.IsMatchProperty(m_regardProperty[i]))
                    return false;
            }
            for (int i = 0; i < m_camp.Count; i++)
            {
                if (targetCard.IsMatchCamp(m_camp[i]))
                    break;

                if(i == m_camp.Count-1)
                    return false;
            }
            for (int i = 0; i < m_playerType.Count; i++)
            {
                if(m_playerType[i] == "Player")
                {
                    if(targetCard.GetPlayerType() != originCard.GetPlayerType())
                        return false;
                }
                else
                {
                    if (targetCard.GetPlayerType() == originCard.GetPlayerType())
                        return false;
                }
            }
            for (int i = 0; i < m_cardType.Count; i++)
            {
                if (!targetCard.IsMatchCardType(m_cardType[i]))
                    return false;
            }
            for (int i = 0; i < m_cardCategorie.Count; i++)
            {
                if (!targetCard.IsMatchCardCategory(m_cardCategorie[i]))
                    return false;
            }
            for(int i = 0; i < m_specialIndex.Count; i++)
            {
                if (!SpecialCardCondition.IsMatch(origin, target, m_specialIndex[i]))
                    return false;
            }
            return true;
        }
        public override bool IsMatch(PlayerType playerType, GameObject target)
        {
            Card targetCard = target.GetComponent<Card>();

            for (int i = 0; i < m_ID.Count; i++)
            {
                if (!targetCard.IsMatchID(m_ID[i]))
                    return false;
            }
            for (int i = 0; i < m_baseOwner.Count; i++)
            {
                if (!targetCard.IsMatchOwnerBase(m_baseOwner[i]))
                    return false;
            }
            for (int i = 0; i < m_regardOwner.Count; i++)
            {
                if (!targetCard.IsMatchOwner(m_regardOwner[i]))
                    return false;
            }
            for (int i = 0; i < m_baseProperty.Count; i++)
            {
                if (!targetCard.IsMatchPropertyBase(m_baseProperty[i]))
                    return false;
            }
            for (int i = 0; i < m_regardProperty.Count; i++)
            {
                if (!targetCard.IsMatchProperty(m_regardProperty[i]))
                    return false;
            }
            for (int i = 0; i < m_camp.Count; i++)
            {
                if (targetCard.IsMatchCamp(m_camp[i]))
                    break;

                if (i == m_camp.Count - 1)
                    return false;
            }
            for (int i = 0; i < m_playerType.Count; i++)
            {
                if (m_playerType[i] == "Player")
                {
                    if (targetCard.GetPlayerType() != playerType)
                        return false;
                }
                else
                {
                    if (targetCard.GetPlayerType() == playerType)
                        return false;
                }
            }
            for (int i = 0; i < m_cardType.Count; i++)
            {
                if (!targetCard.IsMatchCardType(m_cardType[i]))
                    return false;
            }
            for (int i = 0; i < m_cardCategorie.Count; i++)
            {
                if (!targetCard.IsMatchCardCategory(m_cardCategorie[i]))
                    return false;
            }
            for (int i = 0; i < m_specialIndex.Count; i++)
            {
                if (!SpecialCardCondition.IsMatch(null, target, m_specialIndex[i]))
                    return false;
            }
            return true;
        }
        public override bool IsMatch(CardInfo cardInfo)
        {
            for (int i = 0; i < m_ID.Count; i++)
            {
                if (cardInfo.m_ID != m_ID[i])
                    return false;
            }
            for (int i = 0; i < m_baseOwner.Count; i++)
            {
                if (cardInfo.GetOwner() != m_baseOwner[i])
                    return false;
            }
            for (int i = 0; i < m_baseProperty.Count; i++)
            {
                if (cardInfo.GetProperty() != m_baseProperty[i])
                    return false;
            }
            for (int i = 0; i < m_camp.Count; i++)
            {
                if (cardInfo.m_cardCamp.Contains(m_camp[i]))
                    break;

                if (i == m_camp.Count - 1)
                    return false;
            }
            for(int i=0; i < m_specialIndex.Count; i++)
            {
                if (!SpecialCardCondition.IsMatch(cardInfo, m_specialIndex[i]))
                    return false;
            }
            return true;
        }

        public override bool SetTargetPara(GameObject m_origin, GameObject m_trigger, GameObject m_target, List<KeyValuePair<GameObject, int>> m_select)
        {
            if(m_targetSource == TargetSource.Constant)
                return false;

            GameObject module = null;
            switch(m_targetSource)
            {
                case TargetSource.Origin:
                    module = m_origin;
                    break;
                case TargetSource.Trigger:
                    module = m_trigger;
                    break;
                case TargetSource.Target:
                    module = m_target;
                    break;
                case TargetSource.Select:
                    for(int i=0; i < m_select.Count; i++)
                    {
                        if(m_targetIndex == m_select[i].Value)
                        {
                            module = m_select[i].Key;
                            m_select.RemoveAt(i);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (module == null)
                return false;

            Card card = module.GetComponent<Card>();
            if (m_ID.Count != 0)
            {
                m_ID.Clear();
                m_ID.Add(card.GetCardID());
            }
            if (m_baseOwner.Count != 0)
            {
                m_baseOwner.Clear();
                m_baseOwner.Add(card.GetBaseOwner());
            }
            if (m_regardOwner.Count != 0)
            {
                m_regardOwner.Clear();
                m_regardOwner.AddRange(card.GetRegardOwner());
            }
            if (m_baseProperty.Count != 0)
            {
                m_baseProperty.Clear();
                m_baseProperty.Add(card.GetBaseProprty());
            }
            if (m_regardProperty.Count != 0)
            {
                m_regardProperty.Clear();
                m_regardProperty.AddRange(card.GetRegardProperty());
            }
            if (m_camp.Count != 0)
            {
                m_camp.Clear();
                m_camp.AddRange(card.GetCamp());
            }
            if (m_playerType.Count != 0)
            {
                m_playerType.Clear();
                if(m_origin.GetComponent<Card>().GetPlayerType() == card.GetPlayerType())
                {
                    m_playerType.Add("Player");
                }
                else
                {
                    m_playerType.Add("Enemy");
                }
            }
            if (m_cardType.Count != 0)
            {
                m_cardType.Clear();
                m_cardType.Add(card.GetCardType());
            }
            if (m_cardCategorie.Count != 0)
            {
                m_cardCategorie.Clear();
                m_cardCategorie.Add(card.GetCardCategory());
            }

            return true;
        }



        private static bool ListEqual<T>(List<T> left, List<T> right)
        {
            if(left.Count != right.Count)
                return false;

            for(int i = 0; i < left.Count; i++)
            {
                if(!right.Contains(left[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TargetBase);
        }
        public bool Equals(TargetBase other)
        {
            return other != null &&
                   m_targetSource == other.m_targetSource &&
                   m_targetIndex == other.m_targetIndex &&
                   ListEqual(m_ID, other.m_ID) &&
                   ListEqual(m_baseOwner, other.m_baseOwner) &&
                   ListEqual(m_regardOwner, other.m_regardOwner) &&
                   ListEqual(m_baseProperty, other.m_baseProperty) &&
                   ListEqual(m_regardProperty, other.m_regardProperty) &&
                   ListEqual(m_camp, other.m_camp) &&
                   ListEqual(m_playerType, other.m_playerType) &&
                   ListEqual(m_cardType, other.m_cardType) &&
                   ListEqual(m_cardCategorie, other.m_cardCategorie) &&
                   ListEqual(m_specialIndex, other.m_specialIndex);
        }
        public override int GetHashCode()
        {
            int hashCode = 944802846;
            hashCode = hashCode * -1521134295 + m_targetSource.GetHashCode();
            hashCode = hashCode * -1521134295 + m_targetIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(m_ID);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<SanCai>>.Default.GetHashCode(m_baseOwner);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<SanCai>>.Default.GetHashCode(m_regardOwner);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<WuXing>>.Default.GetHashCode(m_baseProperty);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<WuXing>>.Default.GetHashCode(m_regardProperty);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(m_camp);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(m_playerType);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<CardType>>.Default.GetHashCode(m_cardType);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<CardCategory>>.Default.GetHashCode(m_cardCategorie);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<int>>.Default.GetHashCode(m_specialIndex);
            return hashCode;
        }
        public static bool operator ==(TargetBase left, TargetBase right)
        {
            return EqualityComparer<TargetBase>.Default.Equals(left, right);
        }
        public static bool operator !=(TargetBase left, TargetBase right)
        {
            return !(left == right);
        }
    }
    public class TargetOr : TargetItem, IEquatable<TargetOr>
    {
        private List<TargetItem> m_targetItem;

        public override void Initial(SkillInfoNode skillInfoNode)
        {
            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;
                switch (name)
                {
                    case "ItemAnd":
                        TargetAnd targetAnd = new TargetAnd();
                        targetAnd.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem.Add(targetAnd);
                        break;
                    case "ItemOr":
                        TargetOr targetOr = new TargetOr();
                        targetOr.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem.Add(targetOr);
                        break;
                    case "ItemNot":
                        TargetNot targetNot = new TargetNot();
                        targetNot.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem.Add(targetNot);
                        break;
                    case "ItemBase":
                        TargetBase targetBase = new TargetBase();
                        targetBase.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem.Add(targetBase);
                        break;
                    default:
                        break;
                }
            }
        }
        public TargetOr()
        {
            m_targetItem = new List<TargetItem>();
        }
        public override bool IsMatch(GameObject origin, GameObject target)
        {
            for(int i = 0; i < m_targetItem.Count; i++)
            {
                if (m_targetItem[i].IsMatch(origin, target))
                    return true;
            }
            return false;
        }
        public override bool IsMatch(PlayerType playerType, GameObject target)
        {
            for (int i = 0; i < m_targetItem.Count; i++)
            {
                if (m_targetItem[i].IsMatch(playerType, target))
                    return true;
            }
            return false;
        }
        public override bool IsMatch(CardInfo cardInfo)
        {
            for (int i = 0; i < m_targetItem.Count; i++)
            {
                if (m_targetItem[i].IsMatch(cardInfo))
                    return true;
            }
            return false;
        }
        public override bool SetTargetPara(GameObject m_origin, GameObject m_trigger, GameObject m_target, List<KeyValuePair<GameObject, int>> m_select)
        {
            for(int i=0; i < m_targetItem.Count; i++)
            {
                m_targetItem[i].SetTargetPara(m_origin, m_trigger, m_target, m_select);
            }
            return true;
        }



        public override bool Equals(object obj)
        {
            return Equals(obj as TargetOr);
        }
        public bool Equals(TargetOr other)
        {
            return other != null &&
                   EqualityComparer<List<TargetItem>>.Default.Equals(m_targetItem, other.m_targetItem);
        }
        public override int GetHashCode()
        {
            return -2118140721 + EqualityComparer<List<TargetItem>>.Default.GetHashCode(m_targetItem);
        }
        public static bool operator ==(TargetOr left, TargetOr right)
        {
            return EqualityComparer<TargetOr>.Default.Equals(left, right);
        }
        public static bool operator !=(TargetOr left, TargetOr right)
        {
            return !(left == right);
        }
    }
    public class TargetAnd : TargetItem, IEquatable<TargetAnd>
    {
        private List<TargetItem> m_targetItem;

        public override void Initial(SkillInfoNode skillInfoNode)
        {
            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;
                switch (name)
                {
                    case "ItemAnd":
                        TargetAnd targetAnd = new TargetAnd();
                        targetAnd.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem.Add(targetAnd);
                        break;
                    case "ItemOr":
                        TargetOr targetOr = new TargetOr();
                        targetOr.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem.Add(targetOr);
                        break;
                    case "ItemNot":
                        TargetNot targetNot = new TargetNot();
                        targetNot.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem.Add(targetNot);
                        break;
                    case "ItemBase":
                        TargetBase targetBase = new TargetBase();
                        targetBase.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem.Add(targetBase);
                        break;
                    default:
                        break;
                }
            }
        }
        public TargetAnd()
        {
            m_targetItem = new List<TargetItem>();
        }
        public override bool IsMatch(GameObject origin, GameObject target)
        {
            for (int i = 0; i < m_targetItem.Count; i++)
            {
                if (!m_targetItem[i].IsMatch(origin, target))
                    return false;
            }
            return true;
        }
        public override bool IsMatch(PlayerType playerType, GameObject target)
        {
            for (int i = 0; i < m_targetItem.Count; i++)
            {
                if (!m_targetItem[i].IsMatch(playerType, target))
                    return false;
            }
            return true;
        }
        public override bool IsMatch(CardInfo cardInfo)
        {
            for (int i = 0; i < m_targetItem.Count; i++)
            {
                if (!m_targetItem[i].IsMatch(cardInfo))
                    return false;
            }
            return true;
        }
        public override bool SetTargetPara(GameObject m_origin, GameObject m_trigger, GameObject m_target, List<KeyValuePair<GameObject, int>> m_select)
        {
            for (int i = 0; i < m_targetItem.Count; i++)
            {
                m_targetItem[i].SetTargetPara(m_origin, m_trigger, m_target, m_select);
            }
            return true;
        }



        public override bool Equals(object obj)
        {
            return Equals(obj as TargetAnd);
        }
        public bool Equals(TargetAnd other)
        {
            return other != null &&
                   EqualityComparer<List<TargetItem>>.Default.Equals(m_targetItem, other.m_targetItem);
        }
        public override int GetHashCode()
        {
            return -2118140721 + EqualityComparer<List<TargetItem>>.Default.GetHashCode(m_targetItem);
        }
        public static bool operator ==(TargetAnd left, TargetAnd right)
        {
            return EqualityComparer<TargetAnd>.Default.Equals(left, right);
        }
        public static bool operator !=(TargetAnd left, TargetAnd right)
        {
            return !(left == right);
        }
    }
    public class TargetNot : TargetItem, IEquatable<TargetNot>
    {
        private TargetItem m_targetItem;
        public TargetNot()
        {
            m_targetItem = null;
        }

        
        public override void Initial(SkillInfoNode skillInfoNode)
        {
            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;
                switch (name)
                {
                    case "ItemAnd":
                        TargetAnd targetAnd = new TargetAnd();
                        targetAnd.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem = targetAnd;
                        break;
                    case "ItemOr":
                        TargetOr targetOr = new TargetOr();
                        targetOr.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem = targetOr;
                        break;
                    case "ItemNot":
                        TargetNot targetNot = new TargetNot();
                        targetNot.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem = targetNot;
                        break;
                    case "ItemBase":
                        TargetBase targetBase = new TargetBase();
                        targetBase.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem = targetBase;
                        break;
                    default:
                        break;
                }
            }
        }
        public override bool IsMatch(GameObject origin, GameObject target)
        {
            return !m_targetItem.IsMatch(origin, target);
        }
        public override bool IsMatch(PlayerType playerType, GameObject target)
        {
            return !m_targetItem.IsMatch(playerType, target);
        }
        public override bool IsMatch(CardInfo cardInfo)
        {
            return !m_targetItem.IsMatch(cardInfo);
        }
        public override bool SetTargetPara(GameObject m_origin, GameObject m_trigger, GameObject m_target, List<KeyValuePair<GameObject, int>> m_select)
        {
            m_targetItem.SetTargetPara(m_origin, m_trigger, m_target, m_select);

            return true;
        }




        public override bool Equals(object obj)
        {
            return Equals(obj as TargetNot);
        }
        public bool Equals(TargetNot other)
        {
            return other != null &&
                   EqualityComparer<TargetItem>.Default.Equals(m_targetItem, other.m_targetItem);
        }
        public override int GetHashCode()
        {
            return -2118140721 + EqualityComparer<TargetItem>.Default.GetHashCode(m_targetItem);
        }
        public static bool operator ==(TargetNot left, TargetNot right)
        {
            return EqualityComparer<TargetNot>.Default.Equals(left, right);
        }
        public static bool operator !=(TargetNot left, TargetNot right)
        {
            return !(left == right);
        }
    }


    public class TargetSimple : TargetItem, IEquatable<TargetSimple>
    {
        private TargetItem m_targetItem;

        public TargetSimple()
        {
            m_targetItem = null;
        }

        public override void Initial(SkillInfoNode skillInfoNode)
        {
            for (int i = 0; i < skillInfoNode.m_childNode.Count; i++)
            {
                string name = skillInfoNode.m_childNode[i].m_nodeName;
                string value = skillInfoNode.m_childNode[i].m_nodeValue;
                switch (name)
                {
                    case "ItemAnd":
                        TargetAnd targetAnd = new TargetAnd();
                        targetAnd.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem = targetAnd; 
                        break;
                    case "ItemOr":
                        TargetOr targetOr = new TargetOr();
                        targetOr.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem = targetOr;
                        break;
                    case "ItemNot":
                        TargetNot targetNot = new TargetNot();
                        targetNot.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem = targetNot;
                        break;
                    case "ItemBase":
                        TargetBase targetBase = new TargetBase();
                        targetBase.Initial(skillInfoNode.m_childNode[i]);
                        m_targetItem = targetBase;
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool IsMatch(GameObject origin, GameObject target)
        {
            return m_targetItem == null || m_targetItem.IsMatch(origin, target);
        }
        public override bool IsMatch(PlayerType playerType, GameObject target)
        {
            return m_targetItem == null || m_targetItem.IsMatch(playerType, target);
        }
        public override bool IsMatch(CardInfo cardInfo)
        {
            return m_targetItem == null || m_targetItem.IsMatch(cardInfo);
        }

        public override bool SetTargetPara(GameObject m_origin, GameObject m_trigger, GameObject m_target, List<KeyValuePair<GameObject, int>> m_select)
        {
            return m_targetItem == null || m_targetItem.SetTargetPara(m_origin, m_trigger, m_target, m_select);
        }

        public override bool IsEmpty()
        {
            return m_targetItem == null;
        }



        public override bool Equals(object obj)
        {
            return Equals(obj as TargetSimple);
        }
        public bool Equals(TargetSimple other)
        {
            return other != null &&
                   EqualityComparer<TargetItem>.Default.Equals(m_targetItem, other.m_targetItem);
        }
        public override int GetHashCode()
        {
            return -2118140721 + EqualityComparer<TargetItem>.Default.GetHashCode(m_targetItem);
        }
        public static bool operator ==(TargetSimple left, TargetSimple right)
        {
            return EqualityComparer<TargetSimple>.Default.Equals(left, right);
        }
        public static bool operator !=(TargetSimple left, TargetSimple right)
        {
            return !(left == right);
        }
    }
}

