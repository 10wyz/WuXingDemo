using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WuXingCore;
using WuXingNetwork;

namespace WuXingWindow
{
    public class SelectCliqueWindow : StateWindow
    {
        private string m_clique;

        void Start()
        {
            m_clique = "";
            if (GameObject.Find("Canvas").GetComponent<GameControl>().GetSente())
            {
                this.transform.Find("Tip").GetComponent<Text>().text = "你是先手,选择阴阳派系";
            }
            else
            {
                this.transform.Find("Tip").GetComponent<Text>().text = "你是后手,选择阴阳派系";
            }
            
        }
        public override void Notify(GameObject child, string eventID)
        {
            switch (eventID)
            {
                case "阳":
                    EventChooseYang(child);
                    break;
                case "阴":
                    EventChooseYin(child);
                    break;
                case "确定":
                    EventOk(child);
                    break;
                default:
                    break;
            }
        }

        private void EventChooseYang(GameObject button)
        {
            button.GetComponent<Image>().color = Color.red;
            this.transform.Find("阴").GetComponent<Image>().color = new Color(102 / 255f, 200 / 255f, 229 / 255f);
            m_clique = "阳";
        }
        private void EventChooseYin(GameObject button)
        {
            button.GetComponent<Image>().color = Color.red;
            this.transform.Find("阳").GetComponent<Image>().color = new Color(102 / 255f, 200 / 255f, 229 / 255f);
            m_clique = "阴";
        }
        private void EventOk(GameObject button)
        {
            if (m_clique != "")
            {
                if (m_clique == "阳")
                {
                    GameObject.Find("Canvas").GetComponent<GameControl>().SetClique(true);
                    GameObject.Find("Canvas").GetComponent<GameControl>().GameInitial();
                    GameObject.Find("NetworkControl").GetComponent<GameNetworkManager>().SendClique(false);
                }
                else
                {
                    GameObject.Find("Canvas").GetComponent<GameControl>().SetClique(false);
                    GameObject.Find("Canvas").GetComponent<GameControl>().GameInitial();
                    GameObject.Find("NetworkControl").GetComponent<GameNetworkManager>().SendClique(true);
                }

                GameObject.Destroy(this.gameObject);
            }
        }
    }
}

