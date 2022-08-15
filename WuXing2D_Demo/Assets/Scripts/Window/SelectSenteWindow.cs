using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingButton;
using UnityEngine.UI;
using WuXingCore;
using WuXingNetwork;

namespace WuXingWindow
{
    public class SelectSenteWindow : StateWindow
    {
        private string m_sente;
        void Start()
        {
            m_sente = "";
            this.transform.Find("Tip").GetComponent<Text>().text = "选择先后手";
        }
        public override void Notify(GameObject child, string eventID)
        {
            switch (eventID)
            {
                case "先手":
                    EventChooseSente(child);
                    break;
                case "后手":
                    EventNotChooseSente(child);
                    break;
                case "确定":
                    EventOk(child);
                    break;
                default:
                    break;
            }
        }

        private void EventChooseSente(GameObject button)
        {
            button.GetComponent<Image>().color = Color.red;
            this.transform.Find("后手").GetComponent<Image>().color = new Color(102 / 255f, 200 / 255f, 229 / 255f);
            m_sente = "先手";
        }
        private void EventNotChooseSente(GameObject button)
        {
            button.GetComponent<Image>().color = Color.red;
            this.transform.Find("先手").GetComponent<Image>().color = new Color(102 / 255f, 200 / 255f, 229 / 255f);
            m_sente = "后手";
        }
        private void EventOk(GameObject button)
        {
            if(m_sente != "")
            {
                if(m_sente == "先手")
                {
                    GameObject.Find("Canvas").GetComponent<GameControl>().SetSente(true);
                    GameObject.Find("NetworkControl").GetComponent<GameNetworkManager>().SendSente(false);
                }
                else
                {
                    GameObject.Find("Canvas").GetComponent<GameControl>().SetSente(false);
                    GameObject.Find("NetworkControl").GetComponent<GameNetworkManager>().SendSente(true);
                }
                

                GameObject.Destroy(this.gameObject);
            }
        }
    }
}

