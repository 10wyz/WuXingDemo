using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WuXingCore;
using WuXingNetwork;

namespace WuXingWindow
{
    public class JoinRoomWindow : StateWindow
    {
        public override void Notify(GameObject child, string eventID)
        {
            switch (eventID)
            {
                case "加入":
                    EventJoin();
                    break;
                case "取消":
                    EventCancel();
                    break;
                default:
                    break;
            }
        }

        private void EventJoin()
        {
            string roomName = this.transform.GetComponentInChildren<InputField>().textComponent.text;
            GameObject.Find("MenuNetworkControl").GetComponent<MenuNetworkManager>().JoinRoom(roomName);
        }
        private void EventCancel()
        {
            GameObject menu = this.transform.parent.gameObject;
            menu.GetComponent<MenuControl>().CloseWindow();
        }
    }
}

