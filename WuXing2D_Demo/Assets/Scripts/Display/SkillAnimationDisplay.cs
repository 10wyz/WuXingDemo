using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingCore;

namespace WuXingDisplay
{
    public class SkillAnimationDisplay : AnimationDisplay
    {
        private int m_triggerTimes;
        public SkillAnimationDisplay()
        {
            m_state = null;
            m_triggerTimes = 0;
        }

        void Start()
        {
            Animator animator = this.gameObject.GetComponent<Animator>();
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == "SkillLaunch")
                {
                    //创建新事件
                    AnimationEvent m_animator = new AnimationEvent();
                    //对应事件触发相应函数的名称
                    m_animator.functionName = "EndAnimation";
                    //设定对应事件在相应动画上的触发时间点
                    m_animator.time = clips[i].length-0.1f;

                    clips[i].AddEvent(m_animator);
                }
            }
        }

        private void EndAnimation()
        {
            m_triggerTimes++;
            if(m_triggerTimes == 1)
            {
                StatesPara statesPara = new StatesPara();
                statesPara.m_gameStates = m_state;
                GameListener.Instance().ChainEventNotify(ChainEventID.evt_animaend, statesPara);
                GameObject.Destroy(this.gameObject);
            }
            
        }
    }
}

