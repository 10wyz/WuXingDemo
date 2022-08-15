using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WuXingBase;
using WuXingAssetManage;

namespace WuXingTool
{
    public class CardCreator
    {
        public static GameObject CreateRoleCard(CardInfo cardInfo, CardType cardType, PlayerType playerType, SkillControl skillControl)
        {
            GameObject cardModule = ModuleWarehouse.Instance().GetModule("CardModule");
            GameObject newCard = GameObject.Instantiate(cardModule);
            string Path = "Picture/Card/" + cardInfo.m_cardProperty + "-" + cardInfo.m_cardName;
            Sprite sp = Resources.Load<Sprite>(Path);
            newCard.GetComponent<Image>().sprite = sp;
            newCard.name = cardInfo.m_cardProperty + "-" + cardInfo.m_cardName;

            newCard.AddComponent<Card>();
            newCard.GetComponent<Card>().Initial(cardInfo, cardType, playerType, CardCategory.RoleCard, skillControl, true);

            return newCard;
        }
        public static GameObject CreateLeaderCard(CardInfo cardInfo, CardType cardType, PlayerType playerType, SkillControl skillControl)
        {
            GameObject cardModule = ModuleWarehouse.Instance().GetModule("CardModule");
            GameObject newCard = GameObject.Instantiate(cardModule);
            string Path = "Picture/Card/" + cardInfo.m_cardProperty + "-" + cardInfo.m_cardName;
            Sprite sp = Resources.Load<Sprite>(Path);
            newCard.GetComponent<Image>().sprite = sp;
            newCard.name = cardInfo.m_cardProperty + "-" + cardInfo.m_cardName;

            newCard.AddComponent<Card>();
            newCard.GetComponent<Card>().Initial(cardInfo, cardType, playerType, CardCategory.LeaderCard, skillControl, true);

            return newCard;
        }
        public static GameObject CreateExtraCard(CardInfo cardInfo, CardType cardType, PlayerType playerType, SkillControl skillControl)
        {
            GameObject cardModule = ModuleWarehouse.Instance().GetModule("CardModule");
            GameObject newCard = GameObject.Instantiate(cardModule);
            string Path = "Picture/Card/" + cardInfo.m_cardProperty + "-" + cardInfo.m_cardName;
            Sprite sp = Resources.Load<Sprite>(Path);
            newCard.GetComponent<Image>().sprite = sp;
            newCard.name = cardInfo.m_cardProperty + "-" + cardInfo.m_cardName;

            newCard.AddComponent<Card>();
            newCard.GetComponent<Card>().Initial(cardInfo, cardType, playerType, CardCategory.ExtraCard, skillControl, true);

            return newCard;
        }
        public static GameObject CreateSpellCard(CardInfo cardInfo, CardType cardType, PlayerType playerType, SkillControl skillControl)
        {
            GameObject cardModule = ModuleWarehouse.Instance().GetModule("CardModule");
            GameObject newCard = GameObject.Instantiate(cardModule);
            string Path = "Picture/Spell/术-" + cardInfo.m_cardName;
            Sprite sp = Resources.Load<Sprite>(Path);
            newCard.GetComponent<Image>().sprite = sp;
            newCard.name = "术-" + cardInfo.m_cardName;

            newCard.AddComponent<Card>();
            newCard.GetComponent<Card>().Initial(cardInfo, cardType, playerType, CardCategory.SpellCard, skillControl, true);

            return newCard;
        }
        public static GameObject CreateEpochCard(CardInfo cardInfo, CardType cardType, PlayerType playerType, SkillControl skillControl, bool share=true)
        {
            GameObject cardModule = ModuleWarehouse.Instance().GetModule("CardModule");
            GameObject newCard = GameObject.Instantiate(cardModule);
            string Path = "Picture/Card/纪" + "-" + cardInfo.m_cardName;
            Sprite sp = Resources.Load<Sprite>(Path);
            newCard.GetComponent<Image>().sprite = sp;
            newCard.name = "纪-" + cardInfo.m_cardName;

            newCard.AddComponent<Card>();
            newCard.GetComponent<Card>().Initial(cardInfo, cardType, playerType, CardCategory.EpochCard, skillControl, share);

            newCard.GetComponent<RectTransform>().pivot = Vector2.one / 2;

            return newCard;
        }
        public static GameObject CreateTempCard(CardInfo cardInfo, CardCategory cardCategory, PlayerType playerType, bool share = true)
        {
            GameObject cardModule = ModuleWarehouse.Instance().GetModule("CardModule");
            GameObject newCard = GameObject.Instantiate(cardModule);

            string Path = "";
            switch (cardCategory)
            {
                case CardCategory.LeaderCard:
                case CardCategory.RoleCard:
                case CardCategory.ExtraCard:
                    Path = "Picture/Card/" + cardInfo.m_cardProperty + "-" + cardInfo.m_cardName;
                    break;
                case CardCategory.SpellCard:
                    Path = "Picture/Spell/术-" + cardInfo.m_cardName;
                    break;
                case CardCategory.EpochCard:
                    Path = "Picture/Card/纪" + "-" + cardInfo.m_cardName;
                    break;
                default:
                    break;
            }

            Sprite sp = Resources.Load<Sprite>(Path);
            newCard.GetComponent<Image>().sprite = sp;
            newCard.name = cardInfo.m_cardProperty + "-" + cardInfo.m_cardName;

            newCard.AddComponent<Card>();
            newCard.GetComponent<Card>().Initial(cardInfo, CardType.DeckCard, playerType, cardCategory, null, share);

            return newCard;
        }
        public static GameObject CreateFakeCard(string option, int type, PlayerType originplayer)
        {
            GameObject cardModule = ModuleWarehouse.Instance().GetModule("CardModule");
            GameObject newCard = GameObject.Instantiate(cardModule);
            
            CardInfo cardInfo = new CardInfo();
            CardType cardType = new CardType();
            PlayerType playerType = new PlayerType();
            CardCategory cardCategory = new CardCategory();

            switch(type)
            {
                case 0:
                    cardInfo.m_ID = option;
                    break;
                case 1:
                    cardInfo.m_cardName = option;
                    break;
                case 2:
                    cardInfo.m_cardOwner = option;
                    break;
                case 3:
                    cardInfo.m_cardProperty = option;
                    break;
                case 4:
                    cardInfo.m_cardCamp.Add(option);
                    break;
                case 5:
                    cardType = SkillTranslate.GetCardType(option);
                    break;
                case 6:
                    playerType = SkillTranslate.GetPlayerType(option, originplayer);
                    break;
                case 7:
                    cardCategory = SkillTranslate.GetCardCategory(option);
                    break;
                default:
                    break;
            }

            newCard.AddComponent<Card>();
            newCard.GetComponent<Card>().Initial(cardInfo, cardType, playerType, cardCategory, null, true);

            return newCard;
        }
        public static GameObject CreateDeckCard(SanCai sanCai, WuXing wuXing, PlayerType playerType)
        {
            GameObject cardModule = ModuleWarehouse.Instance().GetModule("CardModule");
            GameObject newDeck = GameObject.Instantiate(cardModule);
            string Path = "Picture/Back/卡背-" + PropertyTool.WuXingToString(wuXing);
            Sprite sp = Resources.Load<Sprite>(Path);
            newDeck.GetComponent<Image>().sprite = sp;
            newDeck.name = PropertyTool.SanCaiToString(sanCai) + "-" + PropertyTool.WuXingToString(wuXing);

            newDeck.AddComponent<Card>();
            CardInfo cardInfo = new CardInfo("", "", PropertyTool.SanCaiToString(sanCai), PropertyTool.WuXingToString(wuXing), new List<string>(), null);
            newDeck.GetComponent<Card>().Initial(cardInfo, CardType.CardDeck, playerType, CardCategory.FakeCard, null, true);

            return newDeck;
        }
    }
}

