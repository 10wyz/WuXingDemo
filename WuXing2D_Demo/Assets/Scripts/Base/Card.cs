using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WuXingCore;
using WuXingAssetManage;
using WuXingSkill;

namespace WuXingBase
{
    public enum CardType {DeckCard, HandCard, SiteCard, ExtraCard, SiteSignal, SiteCenter, CardDeck, DeadSite, DeadHand}
    public enum CardCategory {FakeCard, RoleCard, LeaderCard, SpellCard, ExtraCard, EpochCard}
    public enum PlayerType {Player, Enemy};
    public class Card : MonoBehaviour, IPointerDownHandler
    {
        private CardInfo m_cardInfo;//卡牌基本属性
        private Dictionary<SanCai, int> m_regardOwner;//卡牌被视为的三才从属
        private Dictionary<WuXing, int> m_regardProperty;//卡牌被视为的五行属性
        private HashSet<string> m_regardCamp;//卡牌被视为的阵营
        private CardType m_cardType;//卡牌类型
        private PlayerType m_playerType;//卡牌拥有者
        private CardCategory m_cardCategory;//卡牌分类
        private TriggerPhase m_triggerPhase;//卡牌触发阶段

        private List<Skill> m_skills;//技能对象
        private Resistance m_resistance;//破坏抗性
        private SkillConstraint m_skillConstraint;//技能约束
        private SkillImmune m_skillImmune;//技能免疫

        private SkillControl m_skillControl;

        private bool m_share;
        public Card()
        {
            m_regardOwner = new Dictionary<SanCai, int>();
            m_regardProperty = new Dictionary<WuXing, int>();
            m_regardCamp = new HashSet<string>();
            m_skills = new List<Skill>();
            m_resistance = new Resistance();
            m_skillConstraint = new SkillConstraint();
            m_skillImmune = new SkillImmune();

            m_share = false;
        }
        
        virtual public void Initial(CardInfo cardInfo, CardType cardType, PlayerType playerType, CardCategory cardCategory, SkillControl skillControl, bool share)
        {
            m_cardInfo = cardInfo;
            for(int i=0; i<3; i++)
            {
                m_regardOwner[(SanCai)i] = 0;
            }
            for(int i=0; i<5; i++)
            {
                m_regardProperty[(WuXing)i] = 0;
            }

            m_regardOwner[m_cardInfo.GetOwner()] = 1;
            m_regardProperty[m_cardInfo.GetProperty()] = 1;
            for(int i = 0; i < m_cardInfo.m_cardCamp.Count; i++)
            {
                m_regardCamp.Add(m_cardInfo.m_cardCamp[i]);
            }
            
            m_cardType = cardType;
            m_playerType = playerType;
            m_cardCategory = cardCategory;

            m_skillControl = skillControl;
            for(int i = 0; i < m_cardInfo.m_skillInfo.Count; i++)
            {
                Skill skill = new Skill();
                skill.Initial(m_cardInfo.m_skillInfo[i], m_playerType, this.gameObject, skillControl);
                m_skills.Add(skill);
            }


            m_share = share;
            if(GameListener.Instance().GetGameNetworkManager() != null && m_share)
                GameListener.Instance().GetGameNetworkManager().AddCard(this.gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                if (GameListener.Instance().IsAnimation() || GameObject.Find("MenuWindow").transform.childCount > 0)
                    return;

                if(m_share && GameListener.Instance().GetGameNetworkManager() != null && GameListener.Instance().IsPlayerControl())
                {
                    EventPara eventPara = new EventPara();
                    eventPara.m_trigger = this.gameObject;
                    GameListener.Instance().ControlEventNotify(ControlEventID.evt_card_click, eventPara);
                    GameListener.Instance().GetGameNetworkManager().SendClickObject(this.gameObject);
                }

                DisplayActor.SetFocusCard(this.gameObject);
            }
            else if(eventData.button == PointerEventData.InputButton.Right)
            {
                if (GameListener.Instance().IsAnimation() || GameObject.Find("MenuWindow").transform.childCount > 0)
                    return;
                DisplayActor.SetFocusCard(this.gameObject);
            }
        }
        public void Click()
        {
            EventPara eventPara = new EventPara();
            eventPara.m_trigger = this.gameObject;
            GameListener.Instance().ControlEventNotify(ControlEventID.evt_card_click, eventPara);
        }

        /// <summary>
        /// 获得卡牌ID
        /// </summary>
        /// <returns></returns>
        public string GetCardID()
        {
            return m_cardInfo.m_ID;
        }
        /// <summary>
        /// 获得卡牌类型
        /// </summary>
        /// <returns></returns>
        public CardType GetCardType()
        {
            return m_cardType;
        }
        /// <summary>
        /// 获得卡牌分类
        /// </summary>
        /// <returns></returns>
        public CardCategory GetCardCategory()
        {
            return m_cardCategory;
        }
        /// <summary>
        /// 返回基础属性
        /// </summary>
        /// <returns></returns>
        public WuXing GetBaseProprty()
        {
            return m_cardInfo.GetProperty();
        }
        /// <summary>
        /// 返回被视为属性
        /// </summary>
        /// <returns></returns>
        public List<WuXing> GetRegardProperty()
        {
            List<WuXing> list = new List<WuXing>();
            for(int i=0; i<5; i++)
            {
                if(m_regardProperty[(WuXing)i] > 0)
                {
                    list.Add((WuXing)i);
                }
            }
            return list;
        }
        /// <summary>
        /// 返回基础从属
        /// </summary>
        /// <returns></returns>
        public SanCai GetBaseOwner()
        {
            return m_cardInfo.GetOwner();
        }
        /// <summary>
        /// 返回被视为从属
        /// </summary>
        /// <returns></returns>
        public List<SanCai> GetRegardOwner()
        {
            List<SanCai> list = new List<SanCai>();
            for(int i=0; i<3; i++)
            {
                if (m_regardOwner[(SanCai)i] > 0)
                {
                    list.Add((SanCai)i);
                }
            }

            return list;
        }
        /// <summary>
        /// 返回卡牌所属玩家
        /// </summary>
        /// <returns></returns>
        public PlayerType GetPlayerType()
        {
            return m_playerType;
        }
        /// <summary>
        /// 返回卡牌基本信息
        /// </summary>
        /// <returns></returns>
        public CardInfo GetCardInfo()
        {
            return m_cardInfo;
        }
        /// <summary>
        /// 返回卡牌的阵营
        /// </summary>
        /// <returns></returns>
        public List<string> GetCamp()
        {
            List<string> list = new List<string>(m_regardCamp);
            return list;
        }
        public Skill GetSkill(GameObject skillItem)
        {
            for(int i = 0; i < m_skills.Count; i++)
            {
                if (m_skills[i].IsContainSkillItem(skillItem))
                    return m_skills[i];
            }
            return null;
        }
        public Skill GetSkill(int index)
        {
            if(index < m_skills.Count)
                return m_skills[index];

            return null;
        }
        public Resistance GetResistance()
        {
            return m_resistance;
        }
        public int GetSkillIndex(Skill skill)
        {
            return m_skills.IndexOf(skill);
        }
        public TriggerPhase GetTriggerPhase()
        {
            return m_triggerPhase;
        }
        public SkillConstraint GetSkillConstraint()
        {
            return m_skillConstraint;
        }
        public string GetSkillDecription(int index)
        {
            return m_skills[index].GetSkillDescription();
        }


        /// <summary>
        /// 设置卡牌类型
        /// </summary>
        /// <param name="cardType"></param>
        public void SetCardType(CardType cardType)
        {
            m_cardType = cardType;
        }
        /// <summary>
        /// 设置卡牌技能基本触发模式（始牌，终牌，普通牌）
        /// </summary>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public bool SetTriggerPhase(TriggerPhase triggerPhase)
        {
            m_triggerPhase = triggerPhase;
            return true;
        }
        /// <summary>
        /// 设置卡牌被视为的三才属性
        /// </summary>
        /// <param name = "sanCais" ></ param >
        /// < returns ></ returns >
        public bool SetRegardOwner(List<SanCai> sanCais)
        {
            for (int i = 0; i < 3; i++)
            {
                m_regardOwner[(SanCai)i] = 0;
            }
            for (int i = 0; i < sanCais.Count; i++)
            {
                m_regardOwner[sanCais[i]] = 1;
            }

            return true;
        }
        /// <summary>
        /// 设置卡牌被视为的五行属性
        /// </summary>
        /// <param name = "wuXings" ></ param >
        /// < returns ></ returns >
        public bool SetRegardProperty(List<WuXing> wuXings)
        {
            for (int i = 0; i < 5; i++)
            {
                m_regardProperty[(WuXing)i] = 0;
            }
            for (int i = 0; i < wuXings.Count; i++)
            {
                m_regardProperty[wuXings[i]] = 1;
            }
            return true;
        }
        /// <summary>
        /// 设置卡牌拥有者
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public bool SetPlayerType(PlayerType playerType)
        {
            m_playerType = playerType;
            return true;
        }
        /// <summary>
        /// 设置卡牌抗性
        /// </summary>
        /// <param name="resistance"></param>
        /// <returns></returns>
        public bool SetResistance(Resistance resistance)
        {
            m_resistance.SetResistance(resistance);
            DisplayActor.DisplayCardShield(this.gameObject);
            return true;
        }
        /// <summary>
        /// 设置技能使用次数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool SetSkillNum(int index, int num)
        {
            if (num == 0 || index >= m_skills.Count || index < -1)
                return false;

            if (index == -1)
            {
                for(int i=0; i<m_skills.Count; i++)
                {
                    if(!m_skills[i].IsInnerSkill())
                        m_skills[i].SetSkillNum(num);
                }
            }
            else
            {
                m_skills[index].SetSkillNum(num);
            }

            return true;
        }
        /// <summary>
        /// 设置技能约束
        /// </summary>
        /// <param name="skillConstraint"></param>
        /// <returns></returns>
        public bool SetSkillConstraint(SkillConstraint skillConstraint)
        {
            m_skillConstraint.SetConstraint(skillConstraint);
            return true;
        }
        public bool SetSkillImmune(SkillImmune skillImmune)
        {
            m_skillImmune.SetSkillImmune(skillImmune);
            return true;
        }


        /// <summary>
        /// 修改可视为的从属
        /// </summary>
        /// <param name="sanCais"></param>
        /// <returns></returns>
        public bool AddRegardOwner(List<SanCai> sanCais, int num)
        {
            for (int i = 0; i < sanCais.Count; i++)
            {
                m_regardOwner[sanCais[i]] += num;
            }

            return true;
        }
        /// <summary>
        /// 修改可视为的属性
        /// </summary>
        /// <param name="wuXings"></param>
        /// <returns></returns>
        public bool AddRegardProperty(List<WuXing> wuXings, int num)
        {
            for (int i = 0; i < wuXings.Count; i++)
            {
                m_regardProperty[wuXings[i]] += num;
            }
            return true;
        }



        /// <summary>
        /// 卡牌ID是否匹配
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool IsMatchID(string ID)
        {
            return m_cardInfo.m_ID == ID;
        }
        /// <summary>
        /// 阵营是否匹配
        /// </summary>
        /// <param name="camp"></param>
        /// <returns></returns>
        public bool IsMatchCamp(string camp)
        {
            return m_regardCamp.Contains(camp);
        }
        /// <summary>
        /// 固有从属是否匹配
        /// </summary>
        /// <param name="sanCai"></param>
        /// <returns></returns>
        public bool IsMatchOwnerBase(SanCai sanCai)
        {
            return m_cardInfo.GetOwner() == sanCai;
        }
        /// <summary>
        /// 被视为的从属是否匹配
        /// </summary>
        /// <returns></returns>
        public bool IsMatchOwner(SanCai sanCai)
        {
            return m_regardOwner[sanCai] > 0;
        }
        /// <summary>
        /// 固有属性是否匹配
        /// </summary>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public bool IsMatchPropertyBase(WuXing wuXing)
        {
            return m_cardInfo.GetProperty() == wuXing;
        }
        /// <summary>
        /// 被视为的属性是否匹配
        /// </summary>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public bool IsMatchProperty(WuXing wuXing)
        {
           if (wuXing == WuXing.Chaos || wuXing == WuXing.Epoch)
                return false;

            return m_regardProperty[wuXing] > 0;
        }
        /// <summary>
        /// 所属玩家是否匹配
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public bool IsMacthPlayerType(PlayerType playerType)
        {
            return playerType == m_playerType;
        }
        /// <summary>
        /// 卡牌类型是否匹配
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        public bool IsMatchCardType(CardType cardType)
        {
            return cardType == m_cardType;
        }
        public bool IsMatchCardCategory(CardCategory cardCategory)
        {
            return m_cardCategory == cardCategory;
        }
        /// <summary>
        /// 技能是否可发动
        /// </summary>
        /// <returns></returns>
        public List<int> IsSkillEnable(GameObject trigger, GameObject target, TriggerType triggerType)
        {
            List<int> index = new List<int>();
            for(int i=0; i<m_skills.Count; i++)
            {
                if (m_skills[i].IsSkillEnable(this.gameObject,trigger, target, triggerType))
                {
                    index.Add(i);
                }
            }
            return index;
        }
        /// <summary>
        /// 技能是否可连锁发动
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="target"></param>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public List<int> IsChainEnable(GameObject trigger, GameObject target, TriggerType triggerType)
        {
            List<int> index = new List<int>();
            for (int i = 0; i < m_skills.Count; i++)
            {
                if (m_skills[i].IsSkillEnable(this.gameObject, trigger, target, triggerType) && m_skills[i].IsChainEnable())
                {
                    index.Add(i);
                }
            }
            return index;
        }
        /// <summary>
        /// 技能是否可以自动发动（被动技能）
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="target"></param>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public List<int> IsChainFreeEnable(GameObject trigger, GameObject target, TriggerType triggerType)
        {
            List<int> index = new List<int>();
            for (int i = 0; i < m_skills.Count; i++)
            {
                if (m_skills[i].IsSkillEnable(this.gameObject, trigger, target, triggerType) && m_skills[i].IsChainFreeEnable())
                {
                    index.Add(i);
                }
            }
            return index;
        }
        /// <summary>
        /// 是否可以成为技能对象
        /// </summary>
        /// <param name="targetCard"></param>
        /// <param name="skilllItemName"></param>
        /// <returns></returns>
        public bool IsSkillTargetEnable(GameObject targetCard, string skilllItemName)
        {
            return !m_skillImmune.GetImmuneEnable(this.gameObject, targetCard, skilllItemName);
        }


        public bool BeOnHand(GameObject trigger)
        {
            //for (int i = 0; i < m_skills.Count; i++)
            //{
            //    m_skills[i].LoginSkill(this.gameObject, SkillStage.OnHand);
            //    m_skills[i].LogoutSkill(this.gameObject, SkillStage.OnSite);
            //}
            return true;
        }
        public bool BeCome(GameObject trigger)
        {
            //for(int i=0; i<m_skills.Count; i++)
            //{
            //    m_skills[i].LoginSkill(this.gameObject, SkillStage.OnSite);
            //    m_skills[i].LogoutSkill(this.gameObject, SkillStage.OnHand);
            //}
            return true;
        }
        public bool BeExist(GameObject trigger)
        {
            return true;
        }
        public bool BeDestroy(GameObject trigger)
        {   
            if(m_resistance.IsAttackValid(this.gameObject, trigger))
            {
                DisplayActor.DisplayCardShield(this.gameObject);
                return true;
            }
            else
            {
                DisplayActor.DisplayCardShield(this.gameObject);
                return false;
            }
        }
        public bool BeAbandon(GameObject trigger)
        {
            //for (int i = 0; i < m_skills.Count; i++)
            //{
            //    m_skills[i].LogoutSkill(this.gameObject, SkillStage.OnHand);
            //}
            return true;
        }



        /// <summary>
        /// 响应动作
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="target"></param>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public bool OnAction(GameObject trigger, GameObject target, TriggerType triggerType)
        {
            return m_skillControl.AddWaitEvent(this.gameObject, trigger, target, triggerType);
        }
        /// <summary>
        /// 发动技能
        /// </summary>
        /// <returns></returns>
        public bool OnSkill(GameObject trigger, GameObject target, TriggerType triggerType, int index)
        {
            return m_skills[index].PreSkill(this.gameObject, trigger, target, triggerType);
        }



        public bool ClearResistance()
        {
            m_resistance = new Resistance();
            return true;
        }
        public bool ClearSkillConstraint()
        {
            m_skillConstraint = new SkillConstraint();
            return true;
        }
        public bool ClearSkillTime()
        {
            for(int i=0; i<m_skills.Count; i++)
            {
                m_skills[i].ResetSkillTimes();
            }
            return true;
        }
        public bool ClearSkillImmune()
        {
            m_skillImmune = new SkillImmune();
            return true;
        }


        /// <summary>
        /// 每回合开始时更新卡牌状态（技能数，抗性等）
        /// </summary>
        public void UpdateCardState()
        {
            RecovertCardSkillTurn();
            m_resistance.TurnUpdate();
            m_skillConstraint.TurnUpdate();
            m_skillImmune.TurnUpdate();
        }

        private bool RecovertCardSkillTurn()
        {
            for(int i=0; i<m_skills.Count;i++)
            {
                m_skills[i].RecoverSkillTimeTurn();
            }
            return true;
        }




        public void OnDestroy()
        {
            if(m_skillControl != null)
            {
                if (m_cardType == CardType.HandCard || m_cardType == CardType.SiteCard)
                    m_skillControl.RemoveChainSkill(this.gameObject);
            }

            if(GameListener.Instance().GetGameNetworkManager() != null)
                GameListener.Instance().GetGameNetworkManager().RemoveCard(this.gameObject);
        }
    }
}

