using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WuXingBase;

namespace WuXingDisplay
{
    public class DetailCardDisplay : MonoBehaviour
    {
        public List<GameObject> m_owner;
        public List<GameObject> m_property;
        public GameObject m_campText;
        public GameObject m_campButton;
        private bool m_campEnable;

        void Start()
        {
            m_campEnable = false;
        }

        public void UpdateImage(GameObject targetCard)
        {
            m_campEnable = true;

            this.gameObject.GetComponent<Image>().sprite = targetCard.GetComponent<Image>().sprite;

            HideSignal();

            Card card = targetCard.GetComponent<Card>();
            if (card.GetPlayerType() == PlayerType.Enemy && card.GetCardType() == CardType.HandCard)
            {
                m_campEnable = false;
                HideCampDisplay();
                return;
            }
                


            DiaplayOwner(targetCard);
            DisplayProperty(targetCard);
            DisplayCamp(targetCard);
        }

        private void HideSignal()
        {
            for(int i = 0; i < m_owner.Count; i++)
            {
                m_owner[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 100 / 255f);
            }
            for(int i = 0; i < m_property.Count; i++)
            {
                m_property[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 100 / 255f);
            }
        }
        private void DiaplayOwner(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();

            List<SanCai> sanCais = card.GetRegardOwner();
            for(int i=0; i<sanCais.Count; i++)
            {
                switch(sanCais[i])
                {
                    case SanCai.Sky:
                        m_owner[0].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        break;
                    case SanCai.Earth:
                        m_owner[1].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        break;
                    case SanCai.Human:
                        m_owner[2].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        break;
                    default:
                        break;
                }
            }
        }
        private void DisplayProperty(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();

            List<WuXing> wuXings = card.GetRegardProperty();
            for (int i = 0; i < wuXings.Count; i++)
            {
                switch (wuXings[i])
                {
                    case WuXing.Metal:
                        m_property[0].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        break;
                    case WuXing.Wood:
                        m_property[1].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        break;
                    case WuXing.Water:
                        m_property[2].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        break;
                    case WuXing.Fire:
                        m_property[3].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        break;
                    case WuXing.Earth:
                        m_property[4].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        break;
                    default:
                        break;
                }
            }
        }
        private void DisplayCamp(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();

            GameObject campText = m_campText.transform.Find("Text").gameObject;

            string text = "阵营：\n";
            List<string> camp = card.GetCamp();
            for(int i=0; i< camp.Count; i++)
            {
                text = text + camp[i] + "\n";
            }

            campText.GetComponent<Text>().text = text;
        }
        private void HideCampDisplay()
        {
            m_campText.SetActive(false);
            m_campButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 100 / 255f);
        }
        private void ShowCampDisplay()
        {
            m_campText.SetActive(true);
            m_campButton.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }


        public void ClickCampButton()
        {
            if(m_campEnable)
            {
                if(m_campText.activeSelf)
                {
                    HideCampDisplay();
                }
                else
                {
                    ShowCampDisplay();
                }
            }
        }
    }
}

