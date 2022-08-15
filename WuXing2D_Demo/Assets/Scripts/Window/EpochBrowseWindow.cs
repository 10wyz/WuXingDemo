using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;
using WuXingTool;
using WuXingAssetManage;
using UnityEngine.UI;

namespace WuXingWindow
{
    public class EpochBrowseWindow : StateWindow
    {
        private List<GameObject> m_playerEpoch;
        private List<GameObject> m_enemyEpoch;

        public EpochBrowseWindow()
        {
            m_playerEpoch = new List<GameObject>();
            m_enemyEpoch = new List<GameObject>();
        }

        public void Initial(GamePlayer player, GamePlayer enemy)
        {
            List<GameObject> playerEpoch = player.GetEpochCard();
            List<GameObject> enemyEpoch = enemy.GetEpochCard();

            for (int i = 0; i < playerEpoch.Count; i++)
            {
                Card card = playerEpoch[i].GetComponent<Card>();
                GameObject epoch = CardCreator.CreateEpochCard(card.GetCardInfo(), CardType.HandCard, PlayerType.Player, null, false);
                m_playerEpoch.Add(epoch);
            }

            for (int i = 0; i < enemyEpoch.Count; i++)
            {
                Card card = enemyEpoch[i].GetComponent<Card>();
                GameObject epoch = CardCreator.CreateEpochCard(card.GetCardInfo(), CardType.HandCard, PlayerType.Player, null, false);
                m_enemyEpoch.Add(epoch);
            }


            AllignCard(0);

            GameObject scrollbar = this.gameObject.transform.Find("滚动条").gameObject;
            scrollbar.GetComponent<Scrollbar>().numberOfSteps = Mathf.Max(Mathf.Max(m_playerEpoch.Count - 6, m_enemyEpoch.Count - 6), 1);
            scrollbar.GetComponent<Scrollbar>().size = 1f / scrollbar.GetComponent<Scrollbar>().numberOfSteps;
        }

        public bool AllignCard(int index)
        {
            if (m_playerEpoch.Count == 0 && m_enemyEpoch.Count == 0)
                return false;

            HideCard();

            float windowWidth = this.gameObject.GetComponent<RectTransform>().rect.width * 0.8f;
            float windowheight = this.gameObject.GetComponent<RectTransform>().rect.height * 0.7f;

            float scale = Mathf.Min(windowWidth / 7.6f / AssertManager.CardWidth, windowheight / 2.1f / AssertManager.CardHeight);

            float cardwidth = AssertManager.CardWidth * scale;
            float cardheight = AssertManager.CardHeight * scale;
            float width = cardwidth * 1.1f;
            float height = cardheight * 1.1f;
            for (int i = 0; i < Mathf.Min(7, m_playerEpoch.Count); i++)
            {
                if (i + index >= m_playerEpoch.Count)
                    break;

                m_playerEpoch[i + index].transform.SetParent(this.gameObject.transform);
                float x = (i - 3) * width;
                float y = 0.5f * height - this.gameObject.GetComponent<RectTransform>().rect.height * 0.05f;
                m_playerEpoch[i + index].GetComponent<RectTransform>().pivot = Vector2.one / 2;
                m_playerEpoch[i + index].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
                m_playerEpoch[i + index].GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
                m_playerEpoch[i + index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cardwidth);
                m_playerEpoch[i + index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cardheight);
                m_playerEpoch[i + index].GetComponent<RectTransform>().localScale = Vector3.one;
            }

            for (int i = 0; i < Mathf.Min(7, m_enemyEpoch.Count); i++)
            {
                if (i + index >= m_enemyEpoch.Count)
                    break;

                m_enemyEpoch[i + index].transform.SetParent(this.gameObject.transform);
                float x = (i - 3) * width;
                float y = -0.5f * height - this.gameObject.GetComponent<RectTransform>().rect.height * 0.05f;
                m_enemyEpoch[i + index].GetComponent<RectTransform>().pivot = Vector2.one / 2;
                m_enemyEpoch[i + index].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
                m_enemyEpoch[i + index].GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
                m_enemyEpoch[i + index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cardwidth);
                m_enemyEpoch[i + index].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cardheight);
                m_enemyEpoch[i + index].GetComponent<RectTransform>().localScale = Vector3.one;
            }

            return true;
        }

        private bool HideCard()
        {
            for (int i = 0; i < m_playerEpoch.Count; i++)
            {
                if (m_playerEpoch[i] != null)
                    m_playerEpoch[i].GetComponent<RectTransform>().localPosition = new Vector3(1000, 1000, 0);
            }
            for (int i = 0; i < m_enemyEpoch.Count; i++)
            {
                if (m_enemyEpoch[i] != null)
                    m_enemyEpoch[i].GetComponent<RectTransform>().localPosition = new Vector3(1000, 1000, 0);
            }
            return true;
        }

        public override void Notify(GameObject child, string eventID)
        {
            switch (eventID)
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
            int index = Mathf.RoundToInt(value * (scrollbar.GetComponent<Scrollbar>().numberOfSteps - 1));

            HideCard();
            AllignCard(index);

        }
        private void EventClose()
        {
            GameObject.Destroy(this.gameObject);
        }

        void Update()
        {
            if (Input.GetButtonDown("ESC"))
            {
                EventClose();
            }
        }
        void OnDestroy()
        {
            for (int i = 0; i < m_playerEpoch.Count; i++)
            {
                GameObject.Destroy(m_playerEpoch[i]);
            }
            for (int i = 0; i < m_enemyEpoch.Count; i++)
            {
                GameObject.Destroy(m_enemyEpoch[i]);
            }
        }
    }
}


