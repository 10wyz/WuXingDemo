using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingSkill;

namespace WuXingBase
{
    public class SkillImmuneItem : IEquatable<SkillImmuneItem>
    {
        public GameObject m_origin;//技能免疫来源
        public TargetItem m_targetCard;//技能免疫对象
        public string m_skillItemName;//免疫技能类型
        public int m_turn;//免疫回合数
        
        public void Initial(GameObject origin, TargetItem targetCard, string skillItemName, int turn)
        {
            m_origin = origin;
            m_targetCard = targetCard;
            m_skillItemName = skillItemName;
            m_turn = turn;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SkillImmuneItem);
        }
        public bool Equals(SkillImmuneItem other)
        {
            return other != null &&
                   EqualityComparer<GameObject>.Default.Equals(m_origin, other.m_origin) &&
                   EqualityComparer<TargetItem>.Default.Equals(m_targetCard, other.m_targetCard) &&
                   m_skillItemName == other.m_skillItemName;
        }
        public override int GetHashCode()
        {
            int hashCode = 879466347;
            hashCode = hashCode * -1521134295 + EqualityComparer<GameObject>.Default.GetHashCode(m_origin);
            hashCode = hashCode * -1521134295 + EqualityComparer<TargetItem>.Default.GetHashCode(m_targetCard);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_skillItemName);
            return hashCode;
        }
        public static bool operator ==(SkillImmuneItem left, SkillImmuneItem right)
        {
            return EqualityComparer<SkillImmuneItem>.Default.Equals(left, right);
        }
        public static bool operator !=(SkillImmuneItem left, SkillImmuneItem right)
        {
            return !(left == right);
        }
    }
    public class SkillImmune
    {
        List<SkillImmuneItem> m_skillImmuneItems;

        public SkillImmune()
        {
            m_skillImmuneItems = new List<SkillImmuneItem>();
        }

        public bool AddImmune(SkillImmuneItem skillImmuneItem)
        {
            m_skillImmuneItems.Add(skillImmuneItem);
            return true;
        }
        public bool SetSkillImmune(SkillImmune skillImmune)
        {
            for (int i = 0; i < skillImmune.m_skillImmuneItems.Count; i++)
            {
                int index = m_skillImmuneItems.IndexOf(skillImmune.m_skillImmuneItems[i]);
                if (index >= 0)
                {
                    m_skillImmuneItems[index].m_turn = skillImmune.m_skillImmuneItems[i].m_turn;

                    if (m_skillImmuneItems[index].m_turn <= 0)
                        m_skillImmuneItems.RemoveAt(index);
                }
                else
                {
                    if(skillImmune.m_skillImmuneItems[i].m_turn> 0)
                        m_skillImmuneItems.Add(skillImmune.m_skillImmuneItems[i]);
                }
            }
            return true;
        }


        /// <summary>
        /// 是否免疫该卡牌的该技能（不能成为技能对象）
        /// </summary>
        /// <param name="originCard"></param>
        /// <param name="targetCard"></param>
        /// <param name="skillItemName"></param>
        /// <returns></returns>
        public bool GetImmuneEnable(GameObject originCard, GameObject targetCard, string skillItemName)
        {
            for (int i = 0; i < m_skillImmuneItems.Count; i++)
            {
                if (m_skillImmuneItems[i].m_skillItemName == "SkillItem")
                {
                    return true;
                }
                else if (m_skillImmuneItems[i].m_skillItemName == skillItemName)
                {
                    if (m_skillImmuneItems[i].m_targetCard.IsMatch(originCard, targetCard))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public bool TurnUpdate()
        {
            List<SkillImmuneItem> list = new List<SkillImmuneItem>();

            for (int i = 0; i < m_skillImmuneItems.Count; i++)
            {
                m_skillImmuneItems[i].m_turn--;
                if (m_skillImmuneItems[i].m_turn <= 0)
                {
                    list.Add(m_skillImmuneItems[i]);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                m_skillImmuneItems.Remove(list[i]);
            }

            return true;
        }
    }
}

