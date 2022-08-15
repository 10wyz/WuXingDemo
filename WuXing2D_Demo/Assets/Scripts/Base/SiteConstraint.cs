using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingSkill;

namespace WuXingBase
{
    public class SiteConstraintItem : IEquatable<SiteConstraintItem>
    {
        public GameObject m_origin;
        public TargetItem m_target;
        public TargetItem m_site;
        public bool m_enable;
        public int m_index;
        public int m_time;
        public int m_turn;
        public PlayerType m_playerType;


        public bool IsMatch(GameObject targetCard, GameObject targetSite, int index)
        {
            return m_index == index && m_target.IsMatch(m_playerType, targetCard) && m_site.IsMatch(m_playerType, targetSite);
        }
        public bool IsMatch(GameObject targetCard, GameObject targetSite)
        {
            return m_target.IsMatch(m_playerType, targetCard) && m_site.IsMatch(m_playerType, targetSite);
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as SiteConstraintItem);
        }
        public bool Equals(SiteConstraintItem other)
        {
            return other != null &&
                   EqualityComparer<GameObject>.Default.Equals(m_origin, other.m_origin) &&
                   EqualityComparer<TargetItem>.Default.Equals(m_target, other.m_target) &&
                   EqualityComparer<TargetItem>.Default.Equals(m_site, other.m_site) &&
                   m_enable == other.m_enable &&
                   m_index == other.m_index;
        }
        public override int GetHashCode()
        {
            int hashCode = -916072274;
            hashCode = hashCode * -1521134295 + EqualityComparer<GameObject>.Default.GetHashCode(m_origin);
            hashCode = hashCode * -1521134295 + EqualityComparer<TargetItem>.Default.GetHashCode(m_target);
            hashCode = hashCode * -1521134295 + EqualityComparer<TargetItem>.Default.GetHashCode(m_site);
            hashCode = hashCode * -1521134295 + m_enable.GetHashCode();
            hashCode = hashCode * -1521134295 + m_index.GetHashCode();
            return hashCode;
        }
        public static bool operator ==(SiteConstraintItem left, SiteConstraintItem right)
        {
            return EqualityComparer<SiteConstraintItem>.Default.Equals(left, right);
        }
        public static bool operator !=(SiteConstraintItem left, SiteConstraintItem right)
        {
            return !(left == right);
        }
    }

    public class SiteConstraint
    {
        List<SiteConstraintItem> m_siteConstraints;
        
        public SiteConstraint()
        {
            m_siteConstraints = new List<SiteConstraintItem>();
        }

        public bool AddConstraint(SiteConstraintItem siteConstraintItem)
        {
            m_siteConstraints.Add(siteConstraintItem);
            return true;
        }

        /// <summary>
        /// 登场是否可行
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="targetSite"></param>
        /// <returns></returns>
        public int IsComeEnable(GameObject targetCard, GameObject targetSite, int index)
        {

            bool hasindex = false;
            bool enable = true;
            for(int i=0; i< m_siteConstraints.Count; i++)
            {
                if (m_siteConstraints[i].IsMatch(targetCard, targetSite, index))
                {
                    hasindex = true;
                    enable = enable && m_siteConstraints[i].m_enable;
                }
            }
            if(hasindex)
            {
                if (enable)
                    return 1;
                else
                    return -1;
            }

            return 0;
        }


        public bool SetConstraint(SiteConstraint siteConstraint)
        {
            for (int i = 0; i < siteConstraint.m_siteConstraints.Count; i++)
            {
                int index = m_siteConstraints.IndexOf(siteConstraint.m_siteConstraints[i]);
                if (index >= 0)
                {
                    m_siteConstraints[index].m_time -= siteConstraint.m_siteConstraints[i].m_time;
                    m_siteConstraints[index].m_turn -= siteConstraint.m_siteConstraints[i].m_turn;

                    if (m_siteConstraints[index].m_turn <= 0 || siteConstraint.m_siteConstraints[i].m_time<=0)
                        m_siteConstraints.RemoveAt(index);
                }
                else
                {
                    m_siteConstraints.Add(siteConstraint.m_siteConstraints[i]);
                }
            }
            return true;
        }
        public bool CostTime(GameObject targetCard, GameObject targetSite)
        {
            List<SiteConstraintItem> list = new List<SiteConstraintItem>();
            for (int i = 0; i < m_siteConstraints.Count; i++)
            {
                if (m_siteConstraints[i].IsMatch(targetCard, targetSite))
                {
                    m_siteConstraints[i].m_time--;
                    if (m_siteConstraints[i].m_time <= 0)
                        list.Add(m_siteConstraints[i]);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                m_siteConstraints.Remove(list[i]);
            }

            return true;
        }
        public bool TurnUpdate()
        {
            List<SiteConstraintItem> list = new List<SiteConstraintItem>();

            for (int i = 0; i < m_siteConstraints.Count; i++)
            {
                m_siteConstraints[i].m_turn--;
                if (m_siteConstraints[i].m_turn <= 0)
                {
                    list.Add(m_siteConstraints[i]);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                m_siteConstraints.Remove(list[i]);
            }

            return true;
        }
    }
}

