using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingTool;
using WuXingCore;
using UnityEngine.UI;
using System;

namespace WuXingWindow
{
    public class CardDeckWindow : StateWindow
    {
        private Dictionary<SanCai, Dictionary<WuXing, GameObject>> m_cardDeck;//卡组

        public void Initial(CardDeck cardDeck, int num)
        {
            m_cardDeck = new Dictionary<SanCai, Dictionary<WuXing, GameObject>>();
            for (int i = 0; i < 3; i++)
            {
                SanCai sanCai = (SanCai)i;
                m_cardDeck[sanCai] = new Dictionary<WuXing, GameObject>();
                for (int j = 0; j < 5; j++)
                {
                    WuXing wuXing = (WuXing)j;
                    m_cardDeck[sanCai].Add(wuXing, null);
                }
            }
            PlayerType playerType = GameListener.Instance().IsPlayerControl() ? PlayerType.Player : PlayerType.Enemy;

            CreateCardDeck(playerType);
            AllignDeck(this.gameObject);

            CreateNumText(cardDeck, num);
        }

        public override List<GameObject> GetCard()
        {
            List<GameObject> list = new List<GameObject>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    SanCai sanCai = (SanCai)i;
                    WuXing wuXing = (WuXing)j;

                    if (m_cardDeck[sanCai][wuXing] != null)
                        list.Add(m_cardDeck[sanCai][wuXing]);
                }
            }

            return list;
        }


        /// <summary>
        /// 卡组布局
        /// </summary>
        /// <returns></returns>
        private bool AllignDeck(GameObject parent)
        {
            RectTransform parentRectTrans = parent.GetComponent<RectTransform>();
            float Width = parentRectTrans.rect.width;
            float Height = parentRectTrans.rect.height;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    SanCai sanCai = (SanCai)i;
                    WuXing wuXing = (WuXing)j;

                    GameObject deck = m_cardDeck[sanCai][wuXing];
                    RectTransform rect = deck.GetComponent<RectTransform>();

                    rect.SetParent(parentRectTrans);

                    rect.anchorMin = Vector2.one / 2;
                    rect.anchorMax = Vector2.one / 2;
                    rect.pivot = Vector2.one / 2;

                    float newScale = Mathf.Min(Width / 7.0f / rect.rect.width, Height / 3.8f / rect.rect.height);
                    float newWidth = rect.rect.width * newScale;
                    float newHeight = rect.rect.height * newScale;

                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);

                    float x = (j - 2) * newWidth * 1.4f;
                    float y = -(i - 1) * newHeight * 1.2f;

                    rect.localScale = Vector3.one;
                    rect.localPosition = new Vector3(x, y, 0);
                }
            }

            return true;
        }
        private void CreateNumText(CardDeck cardDeck, int num)
        {
            for(int i=0; i<3; i++)
            {
                for(int j=0; j<5; j++)
                {
                    SanCai sanCai = (SanCai)i;
                    WuXing wuXing = (WuXing)j;

                    GameObject numText = new GameObject();
                    GameObject parent = m_cardDeck[sanCai][wuXing];
                    float width = parent.GetComponent<RectTransform>().rect.width;
                    float height = parent.GetComponent<RectTransform>().rect.height;

                    numText.name = "DeckNum";
                    numText.AddComponent<Text>();
                    numText.transform.SetParent(parent.transform);
                    numText.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
                    numText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    numText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.2f*height);
                    numText.GetComponent<RectTransform>().localPosition = new Vector3(0, -height / 2f, 0);
                    numText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                    
                    numText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                    numText.GetComponent<Text>().fontSize = Mathf.FloorToInt(0.2f * height / 1.5f);
                    numText.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 10);
                    numText.GetComponent<Text>().fontStyle = FontStyle.Normal;


                    int deckNum = cardDeck.GetCardNum(sanCai, wuXing);
                    int foldNum = cardDeck.GetFoldNum(sanCai, wuXing);
                    numText.GetComponent<Text>().text = Convert.ToString(deckNum) + "/" + Convert.ToString(foldNum) + "/0";
                }
            }

            GameObject restText = new GameObject();
            float windowWidth = this.gameObject.GetComponent<RectTransform>().rect.width;
            float windoeHeight = this.gameObject.GetComponent<RectTransform>().rect.height;

            restText.name = "RestCardNum";
            restText.AddComponent<Text>();
            restText.transform.SetParent(this.transform);
            restText.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            restText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, windowWidth);
            restText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.2f * windoeHeight / 3.8f);
            restText.GetComponent<RectTransform>().localPosition = new Vector3(0, windoeHeight/2f, 0);
            restText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);


            restText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            restText.GetComponent<Text>().fontSize = Mathf.FloorToInt(0.2f * windoeHeight / 3.8f / 1.5f);
            restText.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 10);
            restText.GetComponent<Text>().fontStyle = FontStyle.Normal;

            restText.GetComponent<Text>().text = "抽卡数：0/" + Convert.ToString(num);

        }
        /// <summary>
        /// 创建卡组
        /// </summary>
        /// <returns></returns>
        private bool CreateCardDeck(PlayerType playerType)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    SanCai sanCai = (SanCai)i;
                    WuXing wuXing = (WuXing)j;
                    GameObject gameObject = CardCreator.CreateDeckCard(sanCai, wuXing, playerType);
                    m_cardDeck[sanCai][wuXing] = gameObject;
                }
            }

            return true;
        }


        /// <summary>
        /// 更新卡牌数量显示
        /// </summary>
        /// <param name="targetDeck"></param>
        /// <param name="cardDeck"></param>
        public void UpdateNumText(GameObject targetDeck, CardDeck cardDeck)
        {
            Card card = targetDeck.GetComponent<Card>();

            SanCai sanCai = card.GetBaseOwner();
            WuXing wuXing = card.GetBaseProprty();

            GameObject numText = m_cardDeck[sanCai][wuXing].transform.Find("DeckNum").gameObject;
            int deckNum = cardDeck.GetDeckNum(sanCai, wuXing);
            int foldNum = cardDeck.GetFoldNum(sanCai, wuXing);
            string oldtext = numText.GetComponent<Text>().text;
            int selectNum = int.Parse(oldtext.Substring(oldtext.LastIndexOf("/") + 1)) + 1;

            numText.GetComponent<Text>().text = Convert.ToString(deckNum) + "/" + Convert.ToString(foldNum) + "/" + Convert.ToString(selectNum);


            GameObject restText = this.transform.Find("RestCardNum").gameObject;
            string oldtext2 = restText.GetComponent<Text>().text;
            int index1 = oldtext2.IndexOf("：");
            int index2 = oldtext2.IndexOf("/");
            int selectNummAll = int.Parse(oldtext2.Substring(index1+1, index2-index1-1)) + 1;
            int maxNum = int.Parse(oldtext2.Substring(index2 + 1));

            restText.GetComponent<Text>().text = "抽卡数：" + Convert.ToString(selectNummAll) + "/" + Convert.ToString(maxNum);

        }

        /// <summary>
        /// 销毁卡组
        /// </summary>
        /// <returns></returns>
        private bool DestroyCardDeck()
        {
            Debug.Log("销毁卡组");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    SanCai sanCai = (SanCai)i;
                    WuXing wuXing = (WuXing)j;

                    GameListener.Instance().GetGameNetworkManager().RemoveCard(m_cardDeck[sanCai][wuXing]);
                    GameObject.Destroy(m_cardDeck[sanCai][wuXing]);
                    m_cardDeck[sanCai][wuXing] = null;
                }
            }


            return true;
        }
        public void OnDestroy()
        {
            DestroyCardDeck();
        }


        public override bool IsModalWindow()
        {
            return false;
        }

    }
}

