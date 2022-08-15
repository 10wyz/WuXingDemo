using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WuXingBase;
using WuXingAssetManage;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using WuXingTool;
using WuXingNetwork;

namespace WuXingCore
{
    public class GameControl : MonoBehaviour, IPointerClickHandler
    {

        public TextAsset m_text;//普通卡牌信息文件
        public TextAsset m_text2;//额外卡牌信息文件
        public TextAsset m_text3;//纪事卡牌信息文件
        public TextAsset m_text4;//法术卡牌信息文件

        public GameObject m_network;

        private GamePlayer m_player;//我方玩家
        private GamePlayer m_enemy;//敌方玩家

        //private List<CardInfo> m_cardPile;//全部卡牌
        //private CardDeck m_cardDeck;//卡组(包括弃牌堆和待抽卡组)

        private ScreenPlay m_screenPlay;//使用的剧本
        private SkillControl m_skillControl;//游戏当前连锁条目

        private bool m_sente;
        private bool m_yang;

        private bool m_gameOver;
        private bool m_disconnect;
        void Start()
        {
            m_gameOver = false;
            m_disconnect = false;
            if(PhotonNetwork.IsMasterClient)
            {
                bool choose = Random.Range(0, 2) == 0 ? false : true;
                //bool choose = false;
                if (choose)
                {
                    WindowCreator.CreateSelectSenteWindow();
                }
                else
                {
                    GameObject.Find("NetworkControl").GetComponent<GameNetworkManager>().SendCreateSelectSente();
                }
            }
        }

        public void GameInitial()
        {
            GameListener.Instance().ClearListener();


            List<CardInfo> cardPile1 = AssertManager.LoadMonsterCardPack(m_text);
            List<CardInfo> cardPile2 = AssertManager.LoadExtraCardPack(m_text2);
            List<CardInfo> cardPile4 = AssertManager.LoadSpellCardPack(m_text4);
            List<CardInfo> cardPile5 = AssertManager.LoadSpellCardPack(m_text4);

            if(m_yang)
                m_screenPlay = new ScreenPlay01(Clique.Yang);
            else
                m_screenPlay = new ScreenPlay01(Clique.Yin);

            List<CardInfo> cardPile3 = AssertManager.LoadEpochCardPack(m_text3, m_screenPlay.GetScreenPlayerName());

            CardDeck cardDeck = new CardDeck(m_screenPlay);
            cardDeck.LoadCardDeck(cardPile1);
            cardDeck.LoadExtraDeck(cardPile2);
            cardDeck.LoadSpellDeck(cardPile4, cardPile5);

            m_skillControl = new SkillControl(m_screenPlay);

            EpochCard epochCard1 = new EpochCard();
            EpochCard epochCard2 = new EpochCard();
            epochCard1.LoadCard(cardPile3, PlayerType.Player, m_screenPlay, m_skillControl);
            epochCard2.LoadCard(cardPile3, PlayerType.Enemy, m_screenPlay, m_skillControl);

            m_player = new GamePlayer(this.GetComponent<Transform>().GetChild(1).GetChild(3), PlayerType.Player, cardDeck, epochCard1, m_skillControl);
            m_enemy = new GamePlayer(this.GetComponent<Transform>().GetChild(2).GetChild(3), PlayerType.Enemy, cardDeck, epochCard2, m_skillControl);

            GameListener gameListener = GameListener.Instance();
            gameListener.Login(m_player, m_enemy, m_skillControl, m_screenPlay, m_sente);


            if (m_sente)
                this.GetComponent<Transform>().GetChild(3).GetComponentInChildren<Text>().text = "我方预备阶段";
            else
                this.GetComponent<Transform>().GetChild(3).GetComponentInChildren<Text>().text = "敌方预备阶段";


            
            GameObject.Find("NetworkControl").GetComponent<GameNetworkManager>().Initial();
            epochCard1.LoginNetwork();
            epochCard2.LoginNetwork();

            if (GameObject.Find("EpochBrowse") != null)
                GameObject.Find("EpochBrowse").GetComponent<Button>().enabled = true;



            //EventPara eventPara = new EventPara();
            //eventPara.m_playerType = PlayerType.Player;
            //gameListener.ChainEventNotify(ChainEventID.evt_drawspell, eventPara);
            //eventPara.m_playerType = PlayerType.Enemy;
            //gameListener.ChainEventNotify(ChainEventID.evt_drawspell, eventPara);
        }
        public void GameSynchronize()
        {
            m_player.GetCardDeck().CardShuffleInitial();
        }
        public void PreTurnOperation()
        {
            for(int i=0; i<3; i++)
            {
                EventPara eventPara = new EventPara();
                eventPara.m_playerType = PlayerType.Player;
                GameListener.Instance().ChainEventNotify(ChainEventID.evt_drawspell, eventPara);
                eventPara.m_playerType = PlayerType.Enemy;
                GameListener.Instance().ChainEventNotify(ChainEventID.evt_drawspell, eventPara);
            }
        }

        public void GameFinish()
        {
            m_gameOver = true;

            EventPara eventPara = new EventPara();
            eventPara.m_trigger = this.gameObject;
            GameListener.Instance().ControlEventNotify(ControlEventID.evt_game_finish, eventPara);

            DestroyChildWindow("GameWindow");
            DestroyChildWindow("TipWindow");
        }
        public void DisplayEpochBrowse()
        {
            if(GameObject.Find("MenuWindow").transform.childCount == 0 && GameObject.Find("TipWindow").transform.childCount == 0)
                WindowCreator.CreateEpochBrowseWindow(m_player, m_enemy);
        }


        private void DestroyChildWindow(string name)
        {
            GameObject gameWindow = GameObject.Find(name);
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < gameWindow.transform.childCount; i++)
            {
                list.Add(gameWindow.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < list.Count; i++)
            {
                GameObject.Destroy(list[i]);
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                if (GameListener.Instance().IsAnimation() || GameObject.Find("MenuWindow").transform.childCount > 0)
                    return;

                if (GameListener.Instance().GetGameNetworkManager() != null && GameListener.Instance().IsPlayerControl())
                {
                    GameListener.Instance().ControlEventNotify(ControlEventID.evt_skill_cancel, new EventPara());
                    GameListener.Instance().GetGameNetworkManager().SendClickObject(this.gameObject);
                }   
            }
        }
        public void Click()
        {
            GameListener.Instance().ControlEventNotify(ControlEventID.evt_skill_cancel, new EventPara());
        }

        public void SetSente(bool isSente)
        {
            m_sente = isSente;
        }
        public void SetClique(bool isYang)
        {
            m_yang = isYang;
        }
        public bool GetSente()
        {
            return m_sente;
        }
        public bool GetClique()
        {
            return m_yang;
        }

        public void Update()
        {
            if(Input.GetButtonDown("ESC"))
            {
                GameObject menu = GameObject.Find("MenuWindow").gameObject;
                if(menu.transform.childCount == 0 && GameObject.Find("TipWindow").transform.childCount == 0)
                    WindowCreator.CreateInGameMenuWindow(menu);
            }


            //if (PhotonNetwork.PlayerList.Length < 2 && PhotonNetwork.IsConnected && PhotonNetwork.InRoom && !m_gameOver )
            //{
            //    WindowCreator.CreateGameOverWindow("You Win");
            //    GameFinish();
            //}

            //if (PhotonNetwork.PlayerList.Length < 2 && (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom) && !m_gameOver )
            //{
            //    WindowCreator.CreateGameOverWindow("You Lose");
            //    GameFinish();
            //}



            if (PhotonNetwork.PlayerList.Length < 2 && PhotonNetwork.IsConnected && PhotonNetwork.InRoom && !m_gameOver && !m_disconnect)
            {
                int delaytime = 20;
                m_disconnect = true;
                WindowCreator.CreateWaitReconnectWindow(delaytime, "等待对方重连");
                Invoke("ReconnectFinish", delaytime);
            }

            if (PhotonNetwork.PlayerList.Length < 2 && (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom) && !m_gameOver && !m_disconnect)
            {
                int delaytime = 20;
                m_disconnect = true;
                WindowCreator.CreateWaitReconnectWindow(delaytime, "断开连接，正在重连");
                Invoke("ReconnectFailed", delaytime);
            }
        }

        public void ReconnectSucced()
        {
            m_disconnect = false;
            GameObject menu = GameObject.Find("MenuWindow").gameObject;
            if(menu.transform.Find("等待重连窗口") != null)
            {
                GameObject.Destroy(menu.transform.Find("等待重连窗口").gameObject);
            }
        }
        private void ReconnectFinish()
        {
            if (PhotonNetwork.PlayerList.Length < 2 && !m_gameOver && m_disconnect)
            {
                GameObject menu = GameObject.Find("MenuWindow").gameObject;
                GameObject.Destroy(menu.transform.Find("等待重连窗口").gameObject);
                WindowCreator.CreateGameOverWindow("You Win");
                GameFinish();
            }
        }
        private void ReconnectFailed()
        {
            if (PhotonNetwork.PlayerList.Length < 2 && !m_gameOver && m_disconnect)
            {
                GameObject menu = GameObject.Find("MenuWindow").gameObject;
                GameObject.Destroy(menu.transform.Find("等待重连窗口").gameObject);
                WindowCreator.CreateGameOverWindow("You Lose");
                GameFinish();
            }
        }
    }
}

