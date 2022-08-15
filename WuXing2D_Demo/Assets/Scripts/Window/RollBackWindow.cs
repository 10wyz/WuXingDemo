using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingCore;

namespace WuXingWindow
{
    public class RollBackWindow : StateWindow
    {
        public override void Notify(GameObject child, string eventID)
        {
            switch(eventID)
            {
                case "回退":
                    EventBack(child);
                    break;
                case "继续发动":
                    EventLaunch(child);
                    break;
                case "取消":
                    EventCancel();
                    break;
                default:
                    break;
            }
        }

        private void EventBack(GameObject button)
        {
            EventPara eventPara = new EventPara();
            eventPara.m_trigger = button;
            GameListener.Instance().ControlEventNotify(ControlEventID.evt_button_click, eventPara);
            GameObject.Destroy(this.gameObject);
        }
        private void EventLaunch(GameObject button)
        {
            EventPara eventPara = new EventPara();
            eventPara.m_trigger = button;
            GameListener.Instance().ControlEventNotify(ControlEventID.evt_button_click, eventPara);
            GameObject.Destroy(this.gameObject);
        }
        private void EventCancel()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}

