using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingTool;
using UnityEngine.UI;
using WuXingCore;
using WuXingNetwork;

namespace WuXingWindow
{
    public class InGameMenuWindow : StateWindow
    {
        public override void Notify(GameObject child, string eventID)
        {
            switch(eventID)
            {
                case "返回游戏":
                    EventReturnGame();
                    break;
                case "图像设置":
                    EventImageSetting();
                    break;
                case "投降":
                    EventSurrender();
                    break;
                default:
                    break;
            }
        }
        public override void ReEntryWindow()
        {
            Display();
        }


        private void EventReturnGame()
        {
            GameObject.Destroy(this.gameObject);
        }
        private void EventImageSetting()
        {
            Hide();
            WindowCreator.CreateImageSettingWindow(this.gameObject);
            //this.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(1000, 1000, 0);
        }
        private void EventSurrender()
        {
            WindowCreator.CreateGameOverWindow("You Lose");
            GameObject.Find("Canvas").GetComponent<GameControl>().GameFinish();
            GameObject.Find("NetworkControl").GetComponent<GameNetworkManager>().SendSurrender();
            GameObject.Destroy(this.gameObject);
        }


        private void Hide()
        {
            this.GetComponent<Image>().enabled = false;
            this.transform.Find("返回游戏").gameObject.SetActive(false);
            this.transform.Find("图像设置").gameObject.SetActive(false);
            this.transform.Find("投降").gameObject.SetActive(false);
        }
        private void Display()
        {
            this.GetComponent<Image>().enabled = true;
            this.transform.Find("返回游戏").gameObject.SetActive(true);
            this.transform.Find("图像设置").gameObject.SetActive(true);
            this.transform.Find("投降").gameObject.SetActive(true);
        }


        void Update()
        {
            if (Input.GetButtonDown("ESC"))
            {
                EventReturnGame();
            }
        }
    }
}

