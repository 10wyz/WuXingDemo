using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WuXingCore;

namespace WuXingButton
{
    /// <summary>
    /// 控制按钮（没有父窗口的按钮）
    /// </summary>
    public class ControlButton : ButtonBase
    {
        public ControlEventID m_eventID;

        public void Start()
        {
            if (GameListener.Instance().GetGameNetworkManager() != null)
            {
                GameListener.Instance().GetGameNetworkManager().AddButton(this.gameObject);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (GameListener.Instance().IsAnimation() || GameObject.Find("MenuWindow").transform.childCount > 0)
                    return;

                if (GameListener.Instance().GetGameNetworkManager() != null && GameListener.Instance().IsPlayerControl())
                {
                    EventPara eventPara = new EventPara();
                    eventPara.m_trigger = this.gameObject;
                    GameListener.Instance().ControlEventNotify(m_eventID, eventPara);
                    GameListener.Instance().GetGameNetworkManager().SendClickObject(this.gameObject);
                }
            }
            base.OnPointerDown(eventData);
        }
        public override void Click()
        {
            EventPara eventPara = new EventPara();
            eventPara.m_trigger = this.gameObject;
            GameListener.Instance().ControlEventNotify(m_eventID, eventPara);
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

