using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;
using WuXingTool;

namespace WuXingSkill
{
    public class SkillItem : MonoBehaviour
    {
        protected EventPara m_selectEvent;

        protected string m_skillItemName;

        protected int m_prePhase;//上个阶段的编号
        protected int m_nextPhaseSuccess;//当前阶段选取成功下个阶段编号
        protected int m_nextPhaseFailure;//当前阶段选取失败下个阶段编号

        protected TargetNum m_targetNum;
        protected SelectType m_selectType;//选择方式
        protected bool m_selectComplete;//卡牌选择完成

        protected bool m_isExcute;//卡牌持有者是否是该技能目标卡牌选择的执行者 
        protected bool m_skipEnable;//是否可以跳过
        protected bool m_compelEnable;//是否是必定发动的技能

        protected bool m_freeItemFlag;//是否是被动技能的组件
        public SkillItem()
        {
            m_prePhase = -1;
            m_nextPhaseSuccess = 0;
            m_nextPhaseFailure = 0;

            m_selectEvent = new EventPara();

            m_targetNum = new TargetNum();
            m_selectType = SelectType.Auto;
            m_selectComplete = false;

            m_isExcute = true;
            m_skipEnable = false;

            m_freeItemFlag = false;
            m_compelEnable = false;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="skillInfoNode"></param>
        virtual public void Initial(SkillInfoNode skillInfoNode)
        {

        }

        virtual protected void InitialSingle(SkillInfoNode skillInfoNode)
        {
            string name = skillInfoNode.m_nodeName;
            string value = skillInfoNode.m_nodeValue;
            switch (name)
            {
                case "Number":
                    TargetNum targetNum = new TargetNum();
                    targetNum.Initial(skillInfoNode);
                    m_targetNum = targetNum;
                    break;
                case "SelectType":
                    m_selectType = SkillTranslate.GetSelectType(value);
                    break;
                case "NextPhaseS":
                    m_nextPhaseSuccess = int.Parse(value);
                    break;
                case "NextPhaseF":
                    m_nextPhaseFailure = int.Parse(value);
                    break;
                case "Excutor":
                    m_isExcute = SkillTranslate.GetExcute(value);
                    break;
                case "SkipEnable":
                    m_skipEnable = bool.Parse(value);
                    break;
                case "CompelEnable":
                    m_compelEnable = bool.Parse(value);
                    break;
                default:
                    break;
            }
        }

        virtual public bool SelectTarget(GameObject origin, GameObject trigger, GameObject target)
        {
            return true;
        }

        virtual public bool PreSkill(GameObject origin, GameObject trigger, GameObject target)
        {
            return true;
        }
        virtual public bool LaunchSkill(GameObject origin, SkillControl skillControl, bool free)
        {
            return true;
        }


        /// <summary>
        /// 是否是该技能模块的目标
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        virtual public bool IsTarget(GameObject origin, GameObject target)
        {
            Card card = target.GetComponent<Card>();

            if (m_freeItemFlag)
                return true;


            return card.IsSkillTargetEnable(origin, m_skillItemName);
        }
        virtual public bool IsSelectComplete()
        {
            return m_selectComplete;
        }
        /// <summary>
        /// 技能模块是否可以执行
        /// </summary>
        /// <returns></returns>
        virtual public bool IsSkillItemEnable()
        {
            return m_skipEnable || m_selectComplete;
        }
        /// <summary>
        /// 是否是强制发动的技能
        /// </summary>
        /// <returns></returns>
        virtual public bool IsCompelSkill()
        {
            return m_compelEnable;
        }



        /// <summary>
        /// 返回目标数
        /// </summary>
        /// <returns></returns>
        virtual public int GetTargetNum(GameObject origin)
        {
            return m_targetNum.GetNum(origin);
        }
        virtual public EventPara GetSelectEvent()
        {
            return m_selectEvent;
        }
        virtual public bool SetSelectCard(List<GameObject> target, bool isCompelete)
        {
            return true;
        }
        virtual public bool SetTargetPara(GameObject origin, GameObject trigger, GameObject target, List<KeyValuePair<GameObject, int>> select)
        {
            return true;
        }
        virtual public void SetFreeSkillItemFlag(bool flag)
        {
            m_freeItemFlag = flag;
        }

        virtual public void SetPrePhase(int phase)
        {
            m_prePhase = phase;
        }
        virtual public int GetNextPhase()
        {
            if (m_selectComplete)
                return m_nextPhaseSuccess;
            else
                return m_nextPhaseFailure;
        }
        virtual public int GetPrePhase()
        {
            return m_prePhase;
        }
        virtual public PlayerType GetExcutePlayer(GameObject origin)
        {
            Card card = origin.GetComponent<Card>();
            if(m_isExcute)
            {
                return card.GetPlayerType();
            }
            else
            {
                if(card.GetPlayerType() == PlayerType.Player)
                    return PlayerType.Enemy;
                else
                    return PlayerType.Player;
            }
        }
        virtual public List<GameObject> GetSelectCard()
        {
            return new List<GameObject>();
        }


        /// <summary>
        /// 重置技能模块
        /// </summary>
        /// <returns></returns>
        virtual public bool ResetSkillItem()
        {
            return true;
        }
    }
}

