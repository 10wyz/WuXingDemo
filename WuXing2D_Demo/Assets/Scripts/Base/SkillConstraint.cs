using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingSkill;

namespace WuXingBase
{
    public class SkillConstraintItem : IEquatable<SkillConstraintItem>
    {
        public GameObject m_origin;
        public string m_name;
        public bool m_enable;
        public int m_time;
        public int m_turn;
        public TargetCondition m_condition;

        public override bool Equals(object obj)
        {
            return Equals(obj as SkillConstraintItem);
        }
        public bool Equals(SkillConstraintItem other)
        {
            return other != null &&
                   m_origin == other.m_origin &&
                   m_name == other.m_name &&
                   m_enable == other.m_enable &&
                   m_condition.IsEqual(other.m_condition);
        }
        public override int GetHashCode()
        {
            int hashCode = 486633369;
            hashCode = hashCode * -1521134295 + EqualityComparer<GameObject>.Default.GetHashCode(m_origin);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_name);
            hashCode = hashCode * -1521134295 + m_enable.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TargetCondition>.Default.GetHashCode(m_condition);
            return hashCode;
        }
        public static bool operator ==(SkillConstraintItem left, SkillConstraintItem right)
        {
            return EqualityComparer<SkillConstraintItem>.Default.Equals(left, right);
        }
        public static bool operator !=(SkillConstraintItem left, SkillConstraintItem right)
        {
            return !(left == right);
        }
    }
    public class SkillConstraint
    {
        List<SkillConstraintItem> m_skillConstraints;

        public SkillConstraint()
        {
            m_skillConstraints = new List<SkillConstraintItem>();
        }

        public bool AddConstraint(SkillConstraintItem skillConstraintItem)
        {
            m_skillConstraints.Add(skillConstraintItem);
            return true;
        }
        public bool SetConstraint(SkillConstraint skillConstraint)
        {
            for(int i = 0; i < skillConstraint.m_skillConstraints.Count; i++)
            {
                int index = m_skillConstraints.IndexOf(skillConstraint.m_skillConstraints[i]);
                if(index >= 0)
                {
                    m_skillConstraints[index].m_time = skillConstraint.m_skillConstraints[i].m_time;
                    m_skillConstraints[index].m_turn = skillConstraint.m_skillConstraints[i].m_turn;

                    if(m_skillConstraints[index].m_time <=0 || m_skillConstraints[index].m_turn <= 0)
                        m_skillConstraints.RemoveAt(index);
                }
                else
                {
                    if (skillConstraint.m_skillConstraints[i].m_time > 0 && skillConstraint.m_skillConstraints[i].m_turn > 0)
                        m_skillConstraints.Add(skillConstraint.m_skillConstraints[i]);
                }
            }
            return true;
        }

        public int GetConstraintEnable(GameObject targetCondition)
        {
            bool isContain = false;
            bool result = true;
            for(int i = 0; i < m_skillConstraints.Count; i++)
            {
                if (m_skillConstraints[i].m_name == "ConditionItem00")
                {
                    if (m_skillConstraints[i].m_enable)
                        return 1;
                    else
                        return 0;
                }
                else if(m_skillConstraints[i].m_name == targetCondition.name)
                {
                    if(m_skillConstraints[i].m_condition.IsMatch(targetCondition))
                    {
                        isContain = true;
                        result = m_skillConstraints[i].m_enable && result;
                    }
                }
            }

            if(isContain)
            {
                if (result)
                    return 1;
                else
                    return 0;
            }

            return -1;
        }

        public bool CostTime(GameObject targetCondition)
        {
            List<SkillConstraintItem> list = new List<SkillConstraintItem>();
            for (int i = 0; i < m_skillConstraints.Count; i++)
            {
                if (m_skillConstraints[i].m_name == "ConditionItem00")
                {
                    m_skillConstraints[i].m_time--;
                    if(m_skillConstraints[i].m_time <= 0)
                        list.Add(m_skillConstraints[i]);
                }
                else if (m_skillConstraints[i].m_name == targetCondition.name)
                {
                    if (m_skillConstraints[i].m_condition.IsMatch(targetCondition))
                    {
                        m_skillConstraints[i].m_time--;
                        if (m_skillConstraints[i].m_time <= 0)
                            list.Add(m_skillConstraints[i]);
                    }
                }
            }

            for(int i = 0; i < list.Count; i++)
            {
                m_skillConstraints.Remove(list[i]);
            }

            return true;
        }
        public bool TurnUpdate()
        {
            List<SkillConstraintItem> list = new List<SkillConstraintItem>();

            for (int i=0; i < m_skillConstraints.Count; i++)
            {
                m_skillConstraints[i].m_turn--;
                if(m_skillConstraints[i].m_turn <= 0)
                {
                    list.Add(m_skillConstraints[i]);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                m_skillConstraints.Remove(list[i]);
            }

            return true;
        }
    }
}

