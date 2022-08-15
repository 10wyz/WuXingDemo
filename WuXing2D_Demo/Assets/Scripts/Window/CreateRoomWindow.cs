using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingCore;
using UnityEngine.UI;
using WuXingTool;
using WuXingNetwork;

namespace WuXingWindow
{
    public class CreateRoomWindow : StateWindow
    {
        public override void Notify(GameObject child, string eventID)
        {
            switch (eventID)
            {
                case "创建":
                    EventOk();
                    break;
                case "取消":
                    EventCancel();
                    break;
                default:
                    break;
            }
        }

        private void EventOk()
        {
            string roomName = this.transform.GetComponentInChildren<InputField>().textComponent.text;
            GameObject.Find("MenuNetworkControl").GetComponent<MenuNetworkManager>().CreateRoom(roomName);
        }
        private void EventCancel()
        {
            GameObject menu = this.transform.parent.gameObject;
            menu.GetComponent<MenuControl>().CloseWindow();
        } 
    }
}

