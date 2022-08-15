using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WuXingCore;

namespace WuXingButton
{
    /// <summary>
    /// 技能按钮
    /// </summary>
    public class SkillButton : ButtonBase
    {
        public void Start()
        {
            if (GameListener.Instance().GetGameNetworkManager() != null)
            {
                GameListener.Instance().GetGameNetworkManager().AddButton(this.gameObject);
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                if (GameListener.Instance().IsAnimation() || GameObject.Find("MenuWindow").transform.childCount > 0)
                    return;

                if (GameListener.Instance().GetGameNetworkManager() != null && eventData.button == PointerEventData.InputButton.Left)
                {
                    if (GameListener.Instance().IsPlayerControl())
                    {
                        EventPara eventPara = new EventPara();
                        eventPara.m_trigger = this.transform.gameObject;

                        GameListener.Instance().ControlEventNotify(ControlEventID.evt_button_click, eventPara);
                        GameListener.Instance().GetGameNetworkManager().SendClickObject(this.gameObject);
                    }
                }
            }
        }
        public override void Click()
        {
            EventPara eventPara = new EventPara();
            eventPara.m_trigger = this.transform.gameObject;

            GameListener.Instance().ControlEventNotify(ControlEventID.evt_button_click, eventPara);
        }

        public void OnDestroy()
        {
            if (GameListener.Instance().GetGameNetworkManager() != null)
            {
                GameListener.Instance().GetGameNetworkManager().RemoveButton(this.gameObject);
            }
        }
    }
}

