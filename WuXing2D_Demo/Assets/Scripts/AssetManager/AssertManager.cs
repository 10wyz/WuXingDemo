using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingCore;
using WuXingBase;
using System.Xml;

namespace WuXingAssetManage
{
    public class AssertManager
    {
        public static float CardWidth = 50;
        public static float CardHeight = 100;
        public static float OldScale = 1;
        // Start is called before the first frame update
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="CardData"></param>
        /// <returns></returns>
        public static List<CardInfo> LoadMonsterCardPack(TextAsset CardData)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(CardData.text);

            XmlNodeList nodeList = document.SelectSingleNode("MonsterCard").ChildNodes;
            List<CardInfo> result = new List<CardInfo>();

            for(int i = 0; i < nodeList.Count; i++)
            {
                XmlElement xmlElement = (XmlElement)nodeList[i];
                CardInfo cardInfo = new CardInfo();

                foreach (XmlElement element in xmlElement.ChildNodes)
                {
                    UpdateCardInfo(cardInfo, element);
                }
                result.Add(cardInfo);
            }

            return result;
        }
        public static List<CardInfo> LoadExtraCardPack(TextAsset CardData)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(CardData.text);

            XmlNodeList nodeList = document.SelectSingleNode("ExtraCard").ChildNodes;
            List<CardInfo> result = new List<CardInfo>();

            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlElement xmlElement = (XmlElement)nodeList[i];
                CardInfo cardInfo = new CardInfo();

                foreach (XmlElement element in xmlElement.ChildNodes)
                {
                    UpdateCardInfo(cardInfo, element);
                }
                result.Add(cardInfo);
            }

            return result;
        }
        public static List<CardInfo> LoadSpellCardPack(TextAsset CardData)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(CardData.text);

            XmlNodeList nodeList = document.SelectSingleNode("SpellCard").ChildNodes;
            List<CardInfo> result = new List<CardInfo>();

            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlElement xmlElement = (XmlElement)nodeList[i];
                CardInfo cardInfo = new CardInfo();

                foreach (XmlElement element in xmlElement.ChildNodes)
                {
                    UpdateCardInfo(cardInfo, element);
                }
                result.Add(cardInfo);
            }

            return result;
        }
        public static List<CardInfo> LoadEpochCardPack(TextAsset CardData, string screenName)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(CardData.text);

            XmlNodeList rootList = document.SelectSingleNode("EpochCard").ChildNodes;
            List<CardInfo> result = new List<CardInfo>();

            XmlNodeList nodeList = null;
            for (int i = 0; i < rootList.Count; i++)
            {
                XmlElement xmlElement = (XmlElement)rootList[i];
                if (xmlElement.GetAttribute("name") == screenName)
                {
                    nodeList = rootList[i].ChildNodes;
                }
            }

            if(nodeList != null)
            {
                for(int i = 0; i < nodeList.Count; i++)
                {
                    XmlElement xmlElement = (XmlElement)nodeList[i];
                    CardInfo cardInfo = new CardInfo();

                    foreach (XmlElement element in xmlElement.ChildNodes)
                    {
                        UpdateCardInfo(cardInfo, element);
                    }
                    result.Add(cardInfo);
                }
            }
            

            return result;
        }

        private static bool UpdateCardInfo(CardInfo cardInfo, XmlElement element)
        {
            switch (element.Name)
            {
                case "ID":
                    cardInfo.m_ID = element.InnerText;
                    break;
                case "Name":
                    cardInfo.m_cardName = element.InnerText;
                    break;
                case "Owner":
                    cardInfo.m_cardOwner = element.InnerText;
                    break;
                case "Property":
                    cardInfo.m_cardProperty = element.InnerText;
                    break;
                case "Camp":
                    cardInfo.m_cardCamp.Add(element.InnerText);
                    break;
                case "Skill":
                    cardInfo.m_skillInfo.Add(new SkillInfo(element));
                    break;
                default:
                    break;
            }

            return true;
        }


        public static Dictionary<SanCai, Dictionary<WuXing, GameObject>> GetSiteSignal(PlayerType playerType)
        {
            Dictionary<SanCai, Dictionary<WuXing, GameObject>> m_siteSignal = new Dictionary<SanCai, Dictionary<WuXing, GameObject>>();
            for (int i = 0; i < 3; i++)
            {
                m_siteSignal[(SanCai)i] = new Dictionary<WuXing, GameObject>();
                for(int j=0; j < 5; j++)
                {
                    m_siteSignal[(SanCai)i][(WuXing)j] = null;
                }
            }

            if (playerType == PlayerType.Player)
            {
                m_siteSignal[SanCai.Sky][WuXing.Metal] = GameObject.Find("Canvas/PlayerPart/SkyPart/metal");
                m_siteSignal[SanCai.Sky][WuXing.Wood] = GameObject.Find("Canvas/PlayerPart/SkyPart/wood");
                m_siteSignal[SanCai.Sky][WuXing.Water] = GameObject.Find("Canvas/PlayerPart/SkyPart/water");
                m_siteSignal[SanCai.Sky][WuXing.Fire] = GameObject.Find("Canvas/PlayerPart/SkyPart/fire");
                m_siteSignal[SanCai.Sky][WuXing.Earth] = GameObject.Find("Canvas/PlayerPart/SkyPart/earth");
                m_siteSignal[SanCai.Earth][WuXing.Metal] = GameObject.Find("Canvas/PlayerPart/EarthPart/metal");
                m_siteSignal[SanCai.Earth][WuXing.Wood] = GameObject.Find("Canvas/PlayerPart/EarthPart/wood");
                m_siteSignal[SanCai.Earth][WuXing.Water] = GameObject.Find("Canvas/PlayerPart/EarthPart/water");
                m_siteSignal[SanCai.Earth][WuXing.Fire] = GameObject.Find("Canvas/PlayerPart/EarthPart/fire");
                m_siteSignal[SanCai.Earth][WuXing.Earth] = GameObject.Find("Canvas/PlayerPart/EarthPart/earth");
                m_siteSignal[SanCai.Human][WuXing.Metal] = GameObject.Find("Canvas/PlayerPart/HumanPart/metal");
                m_siteSignal[SanCai.Human][WuXing.Wood] = GameObject.Find("Canvas/PlayerPart/HumanPart/wood");
                m_siteSignal[SanCai.Human][WuXing.Water] = GameObject.Find("Canvas/PlayerPart/HumanPart/water");
                m_siteSignal[SanCai.Human][WuXing.Fire] = GameObject.Find("Canvas/PlayerPart/HumanPart/fire");
                m_siteSignal[SanCai.Human][WuXing.Earth] = GameObject.Find("Canvas/PlayerPart/HumanPart/earth");
            }
            else
            {
                m_siteSignal[SanCai.Sky][WuXing.Metal] = GameObject.Find("Canvas/EnemyPart/SkyPart/metal");
                m_siteSignal[SanCai.Sky][WuXing.Wood] = GameObject.Find("Canvas/EnemyPart/SkyPart/wood");
                m_siteSignal[SanCai.Sky][WuXing.Water] = GameObject.Find("Canvas/EnemyPart/SkyPart/water");
                m_siteSignal[SanCai.Sky][WuXing.Fire] = GameObject.Find("Canvas/EnemyPart/SkyPart/fire");
                m_siteSignal[SanCai.Sky][WuXing.Earth] = GameObject.Find("Canvas/EnemyPart/SkyPart/earth");
                m_siteSignal[SanCai.Earth][WuXing.Metal] = GameObject.Find("Canvas/EnemyPart/EarthPart/metal");
                m_siteSignal[SanCai.Earth][WuXing.Wood] = GameObject.Find("Canvas/EnemyPart/EarthPart/wood");
                m_siteSignal[SanCai.Earth][WuXing.Water] = GameObject.Find("Canvas/EnemyPart/EarthPart/water");
                m_siteSignal[SanCai.Earth][WuXing.Fire] = GameObject.Find("Canvas/EnemyPart/EarthPart/fire");
                m_siteSignal[SanCai.Earth][WuXing.Earth] = GameObject.Find("Canvas/EnemyPart/EarthPart/earth");
                m_siteSignal[SanCai.Human][WuXing.Metal] = GameObject.Find("Canvas/EnemyPart/HumanPart/metal");
                m_siteSignal[SanCai.Human][WuXing.Wood] = GameObject.Find("Canvas/EnemyPart/HumanPart/wood");
                m_siteSignal[SanCai.Human][WuXing.Water] = GameObject.Find("Canvas/EnemyPart/HumanPart/water");
                m_siteSignal[SanCai.Human][WuXing.Fire] = GameObject.Find("Canvas/EnemyPart/HumanPart/fire");
                m_siteSignal[SanCai.Human][WuXing.Earth] = GameObject.Find("Canvas/EnemyPart/HumanPart/earth");
            }

            return m_siteSignal;
            
        }
        public static Dictionary<SanCai, GameObject> GetEpochSignal(PlayerType playerType)
        {
            Dictionary<SanCai, GameObject> m_epochSignal = new Dictionary<SanCai, GameObject>();
            for (int i = 0; i < 3; i++)
            {
                m_epochSignal[(SanCai)i] = null;
            }

            if (playerType == PlayerType.Player)
            {
                m_epochSignal[SanCai.Sky] = GameObject.Find("Canvas/PlayerPart/SkyPart/EpochCard");
                m_epochSignal[SanCai.Earth] = GameObject.Find("Canvas/PlayerPart/EarthPart/EpochCard");
                m_epochSignal[SanCai.Human] = GameObject.Find("Canvas/PlayerPart/HumanPart/EpochCard");
            }
            else
            {
                m_epochSignal[SanCai.Sky] = GameObject.Find("Canvas/EnemyPart/SkyPart/EpochCard");
                m_epochSignal[SanCai.Earth] = GameObject.Find("Canvas/EnemyPart/EarthPart/EpochCard");
                m_epochSignal[SanCai.Human] = GameObject.Find("Canvas/EnemyPart/HumanPart/EpochCard");
            }
            return m_epochSignal;
        }
    }
}

