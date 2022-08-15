using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingTool;
using WuXingBase;

namespace WuXingWindow
{
    public class ChainListWindow : MonoBehaviour
    {
        List<GameObject> m_chainCard;


        // Start is called before the first frame update
        void Start()
        {
            m_chainCard = new List<GameObject>();
        }


        public void AddCard(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            GameObject chainCard = CardCreator.CreateTempCard(card.GetCardInfo(), card.GetCardCategory(), card.GetPlayerType(), false);
            m_chainCard.Add(chainCard);

            float cardHeight = m_chainCard.Count>0 ? m_chainCard[0].GetComponent<RectTransform>().rect.height : 0;
            float height = m_chainCard.Count > 0 ? cardHeight * m_chainCard.Count + 0.1f * cardHeight * (m_chainCard.Count - 1) : 0;
            this.transform.Find("List").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            Allign();
        }
        public void RemoveCard()
        {
            GameObject.Destroy(m_chainCard[m_chainCard.Count - 1]);
            m_chainCard.RemoveAt(m_chainCard.Count - 1);

            float cardHeight = m_chainCard.Count > 0 ? m_chainCard[0].GetComponent<RectTransform>().rect.height : 0;
            float height = m_chainCard.Count > 0 ? cardHeight * m_chainCard.Count + 0.1f * cardHeight * m_chainCard.Count - 1 : 0;
            this.transform.Find("List").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            Allign();
        }


        private void Allign()
        {
            if (m_chainCard.Count == 0)
                return;


            float height = m_chainCard[0].GetComponent<RectTransform>().rect.height * 1.1f;
            for(int i = 0; i < m_chainCard.Count; i++)
            {
                float y = (i - (m_chainCard.Count-1) / 2f) * height - this.transform.Find("List").GetComponent<RectTransform>().rect.height/2f;

                
                m_chainCard[i].transform.SetParent(this.transform.Find("List"));
                
                
                m_chainCard[i].GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                m_chainCard[i].GetComponent<RectTransform>().localPosition = new Vector3(0, y, 0);
                m_chainCard[i].GetComponent<RectTransform>().localScale = Vector3.one;
            }

        }
    }
}

