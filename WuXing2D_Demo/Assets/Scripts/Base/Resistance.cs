using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingSkill;

namespace WuXingBase
{

    public class Shield
    {
        private int m_shieldNum;
        private WuXing m_shieldProperty;

        public Shield()
        {
            m_shieldNum = 0;
            m_shieldProperty = WuXing.Chaos;
        }

        public void Initial(int num, WuXing property)
        {
            m_shieldNum = num;
            m_shieldProperty = property;
        }

        public void SetShield(WuXing wuXing, int num)
        {
            m_shieldNum = num;
            m_shieldProperty = wuXing;
        }
        public bool UseShield(GameObject target)
        {
            Card card = target.GetComponent<Card>();
            if(card.GetBaseProprty() == m_shieldProperty || m_shieldProperty == WuXing.Chaos)
            {
                if(m_shieldNum > 0)
                {
                    m_shieldNum--;
                    return true;
                }
            }

            return false;
        }

        public bool SetShield(Shield shield)
        {
            if(shield.m_shieldNum > 0 )
            {
                if (shield.m_shieldProperty == m_shieldProperty)
                {
                    m_shieldNum = m_shieldNum + shield.m_shieldNum;
                }
                else
                {
                    m_shieldProperty = shield.m_shieldProperty;
                    m_shieldNum = shield.m_shieldNum;
                }
            }
            else
            {
                if (shield.m_shieldProperty == m_shieldProperty || shield.m_shieldProperty == WuXing.Chaos)
                {
                    m_shieldNum = m_shieldNum + shield.m_shieldNum > 0 ? m_shieldNum + shield.m_shieldNum : 0;
                }
            }
            
            return true;
        }

        public int GetNum()
        {
            return m_shieldNum;
        }
        public WuXing GetShieldProperty()
        {
            return m_shieldProperty;
        }
    }

    public class ImmuneItem : IEquatable<ImmuneItem>
    {
        public GameObject m_origin;//抗性来源
        public TargetItem m_targetCard;//抗性对象
        public int m_restTime;//剩余次数
        public int m_restTurn;//剩余回合数

        public ImmuneItem()
        {
            m_origin = null;
            m_targetCard = null;
            m_restTime = 0;
            m_restTurn = 0;
        }

        
        public void Initial(GameObject origin, TargetItem targetItem, int time, int turn)
        {
            m_origin = origin;
            m_targetCard = targetItem;
            m_restTime = time;
            m_restTurn = turn;
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as ImmuneItem);
        }
        public bool Equals(ImmuneItem other)
        {
            return other != null &&
                   EqualityComparer<GameObject>.Default.Equals(m_origin, other.m_origin) &&
                   EqualityComparer<TargetItem>.Default.Equals(m_targetCard, other.m_targetCard);
        }
        public override int GetHashCode()
        {
            int hashCode = -901132371;
            hashCode = hashCode * -1521134295 + EqualityComparer<GameObject>.Default.GetHashCode(m_origin);
            hashCode = hashCode * -1521134295 + EqualityComparer<TargetItem>.Default.GetHashCode(m_targetCard);
            return hashCode;
        }
        public static bool operator ==(ImmuneItem left, ImmuneItem right)
        {
            return EqualityComparer<ImmuneItem>.Default.Equals(left, right);
        }
        public static bool operator !=(ImmuneItem left, ImmuneItem right)
        {
            return !(left == right);
        }
    }
    public class Immune
    {
        private List<ImmuneItem> m_immuneItems;

        public Immune()
        {
            m_immuneItems = new List<ImmuneItem>();
        }

        

        public bool UseImmune(GameObject origin, GameObject target)
        {
            for(int i = 0; i < m_immuneItems.Count; i++)
            {
                if(m_immuneItems[i].m_targetCard.IsMatch(origin, target))
                {
                    m_immuneItems[i].m_restTime--;
                    if(m_immuneItems[i].m_restTime <= 0)
                        m_immuneItems.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
        public bool CostTurn()
        {
            for (int i = 0; i < m_immuneItems.Count; i++)
            {
                m_immuneItems[i].m_restTurn--;
                if (m_immuneItems[i].m_restTurn <= 0)
                    m_immuneItems.RemoveAt(i);
            }
            return true;
        }

        public bool SetImmune(Immune immune)
        {
            for(int i=0; i < immune.m_immuneItems.Count; i++)
            {
                if(m_immuneItems.Contains(immune.m_immuneItems[i]))
                {
                    int k = m_immuneItems.IndexOf(immune.m_immuneItems[i]);
                    if (immune.m_immuneItems[i].m_restTime > 0 && immune.m_immuneItems[i].m_restTurn > 0)
                    {
                        m_immuneItems[k].m_restTime += immune.m_immuneItems[i].m_restTime;
                        m_immuneItems[k].m_restTime += immune.m_immuneItems[i].m_restTurn;
                    }
                    else
                    {
                        m_immuneItems[k].m_restTime -= immune.m_immuneItems[i].m_restTime;
                        m_immuneItems[k].m_restTime -= immune.m_immuneItems[i].m_restTurn;
                        if (m_immuneItems[k].m_restTime > 0 || m_immuneItems[k].m_restTurn > 0)
                            m_immuneItems.RemoveAt(k);
                    }
                }
                else
                {
                    if(immune.m_immuneItems[i].m_restTime>0 && immune.m_immuneItems[i].m_restTurn > 0)
                        m_immuneItems.Add(immune.m_immuneItems[i]);
                }
            }
            return true;
        }
        public void AddImmuneItem(ImmuneItem immuneItem)
        {
            m_immuneItems.Add(immuneItem);
        }
    }



    public class Resistance
    {
        private Shield m_shield;
        private Immune m_immune;

        public Resistance()
        {
            m_shield = new Shield();
            m_immune = new Immune();
        }
        public void Initial(Shield shield, Immune immune)
        {
            m_shield = shield;
            m_immune = immune;
        }


        public bool SetResistance(Resistance resistance)
        {
            m_shield.SetShield(resistance.m_shield);
            m_immune.SetImmune(resistance.m_immune);
            return true;
        }

        public bool IsAttackValid(GameObject origin, GameObject target)
        {
            if(m_shield.UseShield(target))
                return false;

            if(m_immune.UseImmune(origin, target))
                return false;

            return true;
        }

        public bool TurnUpdate()
        {
            m_immune.CostTurn();
            return true;
        }

        public Shield GetShield()
        {
            return m_shield;
        }
    }
}

