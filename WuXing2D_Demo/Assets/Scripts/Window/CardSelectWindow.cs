using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WuXingAssetManage;

namespace WuXingWindow
{
    public class CardSelectWindow : StateWindow
    {
        List<GameObject> m_card;

        public CardSelectWindow()
        {
            m_card = new List<GameObject>();
        }


        public void Initial(List<GameObject> card)
        {
            m_card.AddRange(card);

            if (card.Count <= 7)
                this.gameObject.GetComponentInChildren<Scrollbar>().size = 1;
            else
                this.gameObject.GetComponentInChildren<Scrollbar>().size = 1 / (card.Count - 6);

            AllignCard(0);

            GameObject scrollbar = this.gameObject.transform.Find("滚动条").gameObject;
            scrollbar.GetComponent<Scrollbar>().numberOfSteps = Mathf.Max(card.Count - 6, 1);
            scrollbar.GetComponent<Scrollbar>().size = 1f/ scrollbar.GetComponent<Scrollbar>().numberOfSteps;
        }
        public bool AllignCard(int index)
        {
            if(m_card.Count == 0)
                return false;

            HideCard();
            float cardwidth = AssertManager.CardWidth;
            float cardheight = AssertManager.CardHeight;
            float width = cardwidth * 1.1f ;
            for(int i=0; i<Mathf.Min(7,m_card.Count); i++)
            {
                m_card[i + index].transform.SetParent(this.gameObject.transform);
                float x = (i - 3) * width;
                m_card[i + index].GetComponent<RectTransform>().pivot = Vector2.one / 2;
                m_card[i + index].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
                m_card[i + index].GetComponent<RectTransform>().localPosition = new Vector3(x, 0, 0);
                m_card[i + index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cardwidth);
                m_card[i + index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cardheight);
                m_card[i + index].GetComponent<RectTransform>().localScale = Vector3.one;
            }

            return true;
        }


        private bool HideCard()
        {
            for(int i=0; i<m_card.Count; i++)
            {
                if(m_card[i] != null)
                    m_card[i].transform.SetParent(null);
                //m_card[i].GetComponent<RectTransform>().localPosition = new Vector3 (1000, 1000, 0);
            }
            return true;
        }


        public void CloseAll()
        {
            HideCard();
            m_card.Clear();
        }

        public override void Notify(GameObject child, string eventID)
        {
            switch(eventID)
            {
                case "滚动条":
                    EventScrollbar(child);
                    break;
                case "关闭":
                    EventClose();
                    break;
                default:
                    break;
            }
        }


        private void EventScrollbar(GameObject scrollbar)
        {
            float value = scrollbar.GetComponent<Scrollbar>().value;
            int index = Mathf.RoundToInt(value * (scrollbar.GetComponent<Scrollbar>().numberOfSteps-1));

            HideCard();
            AllignCard(index);

        }
        private void EventClose()
        {
            GameObject.Destroy(this.gameObject);
        }


        public override bool IsModalWindow()
        {
            return false;
        }
    }

}
