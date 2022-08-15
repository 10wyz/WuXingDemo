using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;
using WuXingCore;
using WuXingAssetManage;

namespace WuXingSkill
{
    public enum TriggerType {Null, Attack, Damage, Come, Exist, Left, ToHand, Replace, Abondan, Click, Skill, StageReady, StageDraw, StageMain, StageEnd, StageAbondan, Rinne,  }
    public enum TriggerPhase { Initial, Normal, Final }
    public enum SelectType { Auto, Manual }
    public enum SkillStage {NoChainSite, NoChainHand, OnSite, OnHand, FreeSite, FreeHand, OnDeadSite, OnDeadHand, FreeDeadSite, FreeDeadHand}
    
    public class Skill
    {
        private string m_skillDescription;//技能描述
        private List<GameObject> m_conditionItems;//组成该技能的发动条件组件
        private List<GameObject> m_skillItems;//组成该技能的技能组件
        private int m_currentSection;//当前技能阶段
        private int m_currentTimes;//技能当前可发动次数
        private int m_skillTimes;//技能初始可发动次数
        private int m_turnSkillTimes;//每轮技能可发动次数
        private SkillControl m_skillControl;
        private SkillStage m_skillStage;//技能触发所需的卡牌状态
        private TriggerType m_triggerType;//技能触发方式
        private bool m_isInnerSkill;//内部技能标志位（内部技能不能被其他卡牌改变发动次数）


        private bool m_skillAnimaFlag;//技能动画标志位
        public Skill()
        {
            m_skillDescription = "";
            m_conditionItems = new List<GameObject>();
            m_skillItems = new List<GameObject>();
            m_currentSection = -1;
            m_turnSkillTimes = 0;
            m_currentTimes = 0;
            m_skillTimes = 0;
            m_skillStage = SkillStage.NoChainSite;
            m_triggerType = TriggerType.Click;
            m_isInnerSkill = false;

            m_skillAnimaFlag = true;
        }

        public void Initial(SkillInfo skillInfo, PlayerType playerType, GameObject originCard, SkillControl skillControl)
        {
            m_skillControl = skillControl;
            
            GameObject skill = new GameObject();
            skill.name = "Skill";
            skill.GetComponent<Transform>().SetParent(originCard.GetComponent<Transform>());
            for(int i = 0; i < skillInfo.GetChildNodeNum(); i++)
            {
                SkillInfoNode skillInfoNode = skillInfo.GetChildNode(i);
                string name = skillInfoNode.m_nodeName;
                string value = skillInfoNode.m_nodeValue;
                switch (name)
                {
                    case "SkillDescription":
                        m_skillDescription = value;
                        break;
                    case "SkillItem":
                        m_skillItems.Add(CreateSkillItem(skill, skillInfoNode));
                        break;
                    case "ConditionItem":
                        m_conditionItems.Add(CreateConditionItem(skill, skillInfoNode, playerType));
                        break;
                    case "SkillTimes":
                        m_skillTimes = int.Parse(value);
                        m_currentTimes = m_skillTimes;
                        break;
                    case "TurnSkillTimes":
                        m_turnSkillTimes = int.Parse(value);
                        break;
                    case "SkillStage":
                        m_skillStage = SkillTranslate.GetSkillStage(value);
                        break;
                    case "TriggerType":
                        m_triggerType = SkillTranslate.GetTriggerType(value);
                        break ;
                    case "InnerSkill":
                        m_isInnerSkill = bool.Parse(value);
                        break;
                    default:
                        break;
                }
            }

            if(m_skillStage != SkillStage.NoChainSite && m_skillStage != SkillStage.NoChainHand)
                LoginSkill(originCard);

            if (IsChainFreeEnable())
                SetFreeSkillItemFlag();
        }


        private GameObject CreateSkillItem(GameObject origin, SkillInfoNode skillInfoNode)
        {
            GameObject skillItem = SundryCreator.CreateSkillItem(origin, skillInfoNode.m_nodeValue);
            skillItem.GetComponent<SkillItem>().Initial(skillInfoNode);

            return skillItem;
        }
        private GameObject CreateConditionItem(GameObject origin, SkillInfoNode skillInfoNode, PlayerType playerType)
        {
            GameObject conditionItem = SundryCreator.CreateConditionItem(origin, skillInfoNode.m_nodeValue);
            conditionItem.GetComponent<ConditionItem>().Initial(skillInfoNode, playerType);
            return conditionItem;
        }



        private bool LoginSkill(GameObject origin)
        {
            if(m_skillControl == null)
                return false;

            if (IsChainFreeEnable())
                m_skillControl.AddChainFreeSkill(origin, this, m_triggerType);
            else
                m_skillControl.AddChainSkill(origin, this, m_triggerType);
            return true;
        }


        public bool SetSkillNum(int num)
        {
            m_currentTimes = m_currentTimes + num;
            m_currentTimes = m_currentTimes > 0 ? m_currentTimes : 0;
            return true;
        }
        private void SetFreeSkillItemFlag()
        {
            for(int i = 0; i < m_skillItems.Count; i++)
            {
                m_skillItems[i].GetComponent<SkillItem>().SetFreeSkillItemFlag(true);
            }
        }

        public TriggerType GetTriggerType()
        {
            return m_triggerType;
        }
        public string GetSkillDescription()
        {
            return m_skillDescription;
        }

        public bool IsSkillEnable(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            if(triggerType == TriggerType.Skill)
            {
                int a = 1;
            }

            if (m_currentSection != -1)
                return false;

            CardType cardType = origin.GetComponent<Card>().GetCardType();
            if ( !(cardType == CardType.SiteCard && m_skillStage == SkillStage.OnSite) 
                && !(cardType == CardType.HandCard && m_skillStage == SkillStage.OnHand)
                && !(cardType == CardType.DeadSite && m_skillStage == SkillStage.OnDeadSite)
                && !(cardType == CardType.DeadHand && m_skillStage == SkillStage.OnDeadHand)
                && !(cardType == CardType.SiteCard && m_skillStage == SkillStage.FreeSite)
                && !(cardType == CardType.HandCard && m_skillStage == SkillStage.FreeHand)
                && !(cardType == CardType.DeadSite && m_skillStage == SkillStage.FreeDeadSite)
                && !(cardType == CardType.DeadHand && m_skillStage == SkillStage.FreeDeadHand)
                && ! (cardType == CardType.SiteCard && m_skillStage == SkillStage.NoChainSite)
                && !(cardType == CardType.HandCard && m_skillStage == SkillStage.NoChainHand))
                return false;

            if (m_currentSection == -1 && m_currentTimes == 0)
                return false;


            if (triggerType == TriggerType.Null)
                return false;

            SkillConstraint skillConstraint = origin.GetComponent<Card>().GetSkillConstraint();
            for (int i=0; i<m_conditionItems.Count; i++)
            {
                if(!IsChainFreeEnable())
                {
                    int flag = skillConstraint.GetConstraintEnable(m_conditionItems[i]);
                    if (flag == 0)
                        return false;
                    else if (flag == 1)
                        continue;
                }
                
                if (!m_conditionItems[i].GetComponent<ConditionItem>().IsMatch(origin, trigger, target, triggerType))
                    return false;
            }

            return true;
        }
        public bool IsContainSkillItem(GameObject skillItem)
        {
            return m_skillItems.Contains(skillItem);
        }
        public bool IsChainEnable()
        {
            if(m_skillStage == SkillStage.OnSite || m_skillStage == SkillStage.OnHand
                || m_skillStage == SkillStage.OnDeadSite || m_skillStage == SkillStage.OnDeadHand)

                return true;
            return false;
        }
        public bool IsChainFreeEnable()
        {
            if (m_skillStage == SkillStage.FreeSite || m_skillStage == SkillStage.FreeHand 
                || m_skillStage == SkillStage.FreeDeadSite || m_skillStage == SkillStage.FreeDeadHand)
                return true;
            return false;
        }
        public bool IsInnerSkill()
        {
            return m_isInnerSkill;
        }
        /// <summary>
        /// 是否正常发动技能（还是无效发动）
        /// </summary>
        /// <returns></returns>
        public bool IsLaunchSuccess()
        {
            if(m_currentSection == -1)
                return false;

            for (int i = m_currentSection; i < m_skillItems.Count; i++)
            {
                if (!m_skillItems[i].GetComponent<SkillItem>().IsSkillItemEnable())
                {
                    return false;
                }
            }

            return true;
        }


        public bool PreSkill(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            if (m_currentSection == -1 && !IsSkillEnable(origin, trigger, target, triggerType))
                return false;

            if (m_currentSection == -1 && m_currentTimes > 0)
            {
                m_currentTimes--;
                m_currentSection++;
            }

            while (m_currentSection < m_skillItems.Count)
            {
                UpdateTargetPara(origin, trigger, target);
                if (!m_skillItems[m_currentSection].GetComponent<SkillItem>().PreSkill(origin, trigger, target))
                {
                    return false;
                }
                UpdateSection();
            }


            m_currentSection = 0;

            SkillConstraint skillConstraint = origin.GetComponent<Card>().GetSkillConstraint();
            for (int i = 0; i < m_conditionItems.Count; i++)
            {
                skillConstraint.CostTime(m_conditionItems[i]);
            }

            if (IsChainFreeEnable())
            {
                m_skillControl.AddWaitFreeSkill(origin, this);
            }
            else
            {
                DisplayActor.DisplayHandCard(origin);

                m_skillControl.AddWaitSkill(origin, this);
                m_skillControl.AddWaitEvent(origin, origin, null, TriggerType.Skill);
            }

            return true;
        }
        public bool LauchSkill(GameObject origin)
        {
            bool skillEnable = true;
            //for (int i = m_currentSection; i < m_skillItems.Count; i++)
            //{
            //    if(!m_skillItems[i].GetComponent<SkillItem>().IsSkillItemEnable())
            //    {
            //        m_currentSection = -1;
            //        return true;
            //    }
            //}

            for (int i = m_currentSection; i < m_skillItems.Count; i++)
            {
                if (!m_skillItems[i].GetComponent<SkillItem>().IsSkillItemEnable())
                {
                    skillEnable = false;
                    break;
                }
            }

            if(!skillEnable)
            {
                for (int i = m_currentSection; i < m_skillItems.Count; i++)
                {
                    if (!m_skillItems[i].GetComponent<SkillItem>().IsCompelSkill())
                    {
                        m_skillItems[i].GetComponent<SkillItem>().ResetSkillItem();
                    }
                }
            }
            


            while (m_currentSection < m_skillItems.Count)
            {
                if (!m_skillItems[m_currentSection].GetComponent<SkillItem>().LaunchSkill(origin, m_skillControl, IsChainFreeEnable()))
                {
                    return false;
                }
                m_currentSection++;
            }

            m_currentSection = -1;

            return true;
        }



        public bool RollBackSkill(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            if(m_currentSection>=0)
            {
                m_skillItems[m_currentSection].GetComponent<SkillItem>().ResetSkillItem();
                RollBackSection();
            }


            while(m_currentSection >=0 && !m_skillItems[m_currentSection].GetComponent<SkillItem>().ResetSkillItem())
            {
                RollBackSection();
            }

            if(m_currentSection == -1)
            {
                m_currentTimes++;
                return true;
            }
            else
            {
                return false;
            }
            
        }



        private void UpdateTargetPara(GameObject origin, GameObject trigger, GameObject target)
        {
            int preSection = m_skillItems[m_currentSection].GetComponent<SkillItem>().GetPrePhase();
            List<KeyValuePair<GameObject, int>> select = new List<KeyValuePair<GameObject, int>>();
            for(int i = 0; i <= preSection; i++)
            {
                List<GameObject> list = m_skillItems[i].GetComponent<SkillItem>().GetSelectCard();
                for(int j=0; j < list.Count; j++)
                {
                    select.Add(new KeyValuePair<GameObject, int>(list[j], i));
                }
            }
            m_skillItems[m_currentSection].GetComponent<SkillItem>().SetTargetPara(origin, trigger, target, select);
        }
        private void UpdateSection()
        {
            int preSection = m_skillItems[m_currentSection].GetComponent<SkillItem>().GetPrePhase();
            int currentSection = m_currentSection;
            int nextSection = m_skillItems[m_currentSection].GetComponent<SkillItem>().GetNextPhase();
            m_currentSection = nextSection;
            if(m_currentSection < m_skillItems.Count)
            {
                if (m_skillItems[currentSection].GetComponent<SkillItem>().IsSelectComplete())
                    m_skillItems[m_currentSection].GetComponent<SkillItem>().SetPrePhase(currentSection);
                else
                    m_skillItems[m_currentSection].GetComponent<SkillItem>().SetPrePhase(preSection);
            }
                
        }
        private void RollBackSection()
        {
            int currentSection = m_currentSection;
            int preSection = m_skillItems[m_currentSection].GetComponent<SkillItem>().GetPrePhase();
            m_currentSection = preSection;
        }



        public bool FinishManualSelect()
        {
            UpdateSection();
            return true;
        }
        /// <summary>
        /// 每回合回复技能次数
        /// </summary>
        public void RecoverSkillTimeTurn()
        {
            if(m_turnSkillTimes >= 0)
            {
                m_currentTimes = m_turnSkillTimes;
            }
        }
        /// <summary>
        /// 重置技能状态
        /// </summary>
        public void ResetSkillTimes()
        {
            m_currentTimes = m_skillTimes;
        }
    }


}

