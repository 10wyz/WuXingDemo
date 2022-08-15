using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WuXingBase;
using WuXingCore;

namespace WuXingButton
{
    public enum ButtonTag {Normal, Tab, Skill, ChainCheckCancel, SkillSeleckOk, SkillSelectCancel, SkillRollBackOk, SkillRollBackForce, OptionSelectOk, OptionSelectCancel}
    public class ButtonBase : MonoBehaviour, IPointerDownHandler
    {
        public ButtonTag m_buttonTag;

        virtual public void OnPointerDown(PointerEventData eventData)
        {

        }
        virtual public void Click()
        {

        }

    }
}

