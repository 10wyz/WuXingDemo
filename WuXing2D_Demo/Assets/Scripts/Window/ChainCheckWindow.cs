using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingCore;

namespace WuXingWindow
{
    public class ChainCheckWindow : StateWindow
    {
        public override void Notify(GameObject child, string eventID)
        {
            switch (eventID)
            {
                case "确定":
                    EventOk(child);
                    break;
                case "取消":
                    EventCancel(child);
                    break;
                default:
                    break;
            }
        }

        private void EventOk(GameObject button)
        {
            GameObject.Destroy(this.gameObject);
        }
        private void EventCancel(GameObject button)
        {
            EventPara eventPara = new EventPara();
            eventPara.m_trigger = button;

            GameListener.Instance().ControlEventNotify(ControlEventID.evt_button_click, eventPara);
            GameObject.Destroy(this.gameObject);
        }

    }
}

