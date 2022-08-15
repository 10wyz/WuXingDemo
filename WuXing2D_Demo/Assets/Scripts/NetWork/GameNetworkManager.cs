using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using WuXingCore;
using WuXingTool;
using WuXingBase;
using WuXingButton;
using ExitGames.Client.Photon;

namespace WuXingNetwork
{
    public class RPCEvent
    {
        public int m_ID;
        public string m_functionName;
        public RpcTarget m_target;
        public object[] m_parameters;
    }


    public class GameNetworkManager : MonoBehaviourPunCallbacks
    {
        public string m_roomName;
        public float m_reconnectTime;
        private int m_maxReconnectTime;
        private bool m_reconnectFlag;
        

        private Dictionary<PlayerType, List<GameObject>> m_cardList;
        public List<GameObject> m_playerCard;
        public List<GameObject> m_enemyCard;
        public List<GameObject> m_commonList;


        public List<RPCEvent> m_RPCEvents;
        private int m_currentID;
        private int m_otherID;

        private bool m_RPCEnable;
        public GameNetworkManager()
        {
            m_cardList = new Dictionary<PlayerType, List<GameObject>>();
            m_cardList[PlayerType.Player] = new List<GameObject>();
            m_cardList[PlayerType.Enemy] = new List<GameObject>();
            //m_commonList = new List<GameObject>();

        }


        void Awake()
        {
            m_roomName = PhotonNetwork.CurrentRoom.Name;
            m_maxReconnectTime = 10;
            m_RPCEvents = new List<RPCEvent>();
            m_currentID = 0;
            m_otherID = 0;
        }
        void Start()
        {
            
           
        }

        public void Initial()
        {
            m_cardList[PlayerType.Player].Clear();
            m_cardList[PlayerType.Player].AddRange(m_playerCard);
            m_cardList[PlayerType.Enemy].Clear();
            m_cardList[PlayerType.Enemy].AddRange(m_enemyCard);
        }
        public void AddCard(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            m_cardList[card.GetPlayerType()].Add(targetCard);
        }
        public void RemoveCard(GameObject targetCard)
        {
            Card card = targetCard.GetComponent<Card>();
            m_cardList[card.GetPlayerType()].Remove(targetCard);
        }
        public void AddButton(GameObject targetButton)
        {
            m_commonList.Add(targetButton);
        }
        public void RemoveButton(GameObject targetButton)
        {
            m_commonList.Remove(targetButton);
        }


        public void SendSente(bool sente)
        {
            //photonView.RPC("SetSente", RpcTarget.Others, sente);

            RPCEvent rPCEvent = new RPCEvent();
            rPCEvent.m_ID = m_currentID;
            m_currentID++;
            rPCEvent.m_functionName = "SetSente";
            rPCEvent.m_target = RpcTarget.Others;
            rPCEvent.m_parameters = new object[2];
            rPCEvent.m_parameters[0] = sente;
            rPCEvent.m_parameters[1] = rPCEvent.m_ID;
            m_RPCEvents.Add(rPCEvent);
        }
        public void SendClique(bool clique)
        {
            //photonView.RPC("SetClique", RpcTarget.Others, clique);

            RPCEvent rPCEvent = new RPCEvent();
            rPCEvent.m_ID = m_currentID;
            m_currentID++;
            rPCEvent.m_functionName = "SetClique";
            rPCEvent.m_target = RpcTarget.Others;
            rPCEvent.m_parameters = new object[2];
            rPCEvent.m_parameters[0] = clique;
            rPCEvent.m_parameters[1] = rPCEvent.m_ID;
            m_RPCEvents.Add(rPCEvent);
        }
        public void SendCreateSelectSente()
        {
            //photonView.RPC("CreateSelectSenteWindow", RpcTarget.Others);

            RPCEvent rPCEvent = new RPCEvent();
            rPCEvent.m_ID = m_currentID;
            m_currentID++;
            rPCEvent.m_functionName = "CreateSelectSenteWindow";
            rPCEvent.m_target = RpcTarget.Others;
            rPCEvent.m_parameters = new object[1];
            rPCEvent.m_parameters[0] = rPCEvent.m_ID;
            m_RPCEvents.Add(rPCEvent);
        }
        public void SendClickObject(GameObject target)
        {
            //if(m_commonList.Contains(target))
            //    photonView.RPC("ClickObject", RpcTarget.Others, m_commonList.IndexOf(target), 0);
            //else if(m_cardList[PlayerType.Player].Contains(target))
            //    photonView.RPC("ClickObject", RpcTarget.Others, m_cardList[PlayerType.Player].IndexOf(target), 2);
            //else if (m_cardList[PlayerType.Enemy].Contains(target))
            //    photonView.RPC("ClickObject", RpcTarget.Others, m_cardList[PlayerType.Enemy].IndexOf(target), 1);


            RPCEvent rPCEvent = new RPCEvent();
            rPCEvent.m_functionName = "ClickObject";
            rPCEvent.m_ID = m_currentID;
            m_currentID++;
            rPCEvent.m_target = RpcTarget.Others;


            if (m_commonList.Contains(target))
            {
                rPCEvent.m_parameters = new object[3];
                rPCEvent.m_parameters[0] = m_commonList.IndexOf(target);
                rPCEvent.m_parameters[1] = 0;
                rPCEvent.m_parameters[2] = rPCEvent.m_ID;
            }
            else if (m_cardList[PlayerType.Player].Contains(target))
            {
                rPCEvent.m_parameters = new object[3];
                rPCEvent.m_parameters[0] = m_cardList[PlayerType.Player].IndexOf(target);
                rPCEvent.m_parameters[1] = 2;
                rPCEvent.m_parameters[2] = rPCEvent.m_ID;
            }
            else if (m_cardList[PlayerType.Enemy].Contains(target))
            {
                rPCEvent.m_parameters = new object[3];
                rPCEvent.m_parameters[0] = m_cardList[PlayerType.Enemy].IndexOf(target);
                rPCEvent.m_parameters[1] = 1;
                rPCEvent.m_parameters[2] = rPCEvent.m_ID;
            }

            m_RPCEvents.Add(rPCEvent);

        }
        public void SendDeck(List<string> idList, int index)
        {
            string[] idArray = idList.ToArray();
            //photonView.RPC("SetDeck", RpcTarget.Others, idArray, index);


            RPCEvent rPCEvent = new RPCEvent();
            rPCEvent.m_ID = m_currentID;
            m_currentID++;
            rPCEvent.m_functionName = "SetDeck"; 
            rPCEvent.m_target = RpcTarget.Others;
            rPCEvent.m_parameters = new object[3];
            rPCEvent.m_parameters[0] = idArray;
            rPCEvent.m_parameters[1] = index;
            rPCEvent.m_parameters[2] = rPCEvent.m_ID;
            m_RPCEvents.Add(rPCEvent);
        }
        public void SendSurrender()
        {
            //photonView.RPC("Surrender", RpcTarget.Others);

            RPCEvent rPCEvent = new RPCEvent();
            rPCEvent.m_ID = m_currentID;
            m_currentID++;
            rPCEvent.m_functionName = "Surrender";
            rPCEvent.m_target = RpcTarget.Others;
            rPCEvent.m_parameters = new object[1];
            rPCEvent.m_parameters[0] = rPCEvent.m_ID;
            m_RPCEvents.Add(rPCEvent);
        }



        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            m_reconnectTime = 0;
            m_reconnectFlag = true;
        }
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("进入房间");


            GameObject.Find("Canvas").GetComponent<GameControl>().ReconnectSucced();
            //photonView.RPC("ReconnectSuccess", RpcTarget.Others);

            RPCEvent rPCEvent = new RPCEvent();
            rPCEvent.m_ID = m_currentID;
            m_currentID++;
            rPCEvent.m_functionName = "ReconnectSuccess";
            rPCEvent.m_target = RpcTarget.Others;
            rPCEvent.m_parameters = new object[1];
            rPCEvent.m_parameters[0] = rPCEvent.m_ID;
            m_RPCEvents.Add(rPCEvent);

        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            m_reconnectFlag = true;
            Debug.Log("加入房间失败");
        }


        void Update()
        {
            m_RPCEnable = true;
        }
        void FixedUpdate()
        {
            if ((!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom) && m_reconnectTime < m_maxReconnectTime)
            {
                m_reconnectTime += 0.02f;
                if (m_reconnectFlag)
                {
                    m_reconnectFlag = false;

                    if (PhotonNetwork.IsConnected)
                    {
                        PhotonNetwork.JoinRoom(m_roomName);
                    }
                    else
                        PhotonNetwork.ReconnectAndRejoin();
                }
            }


            for(int i = 0; i < m_RPCEvents.Count; i++)
            {
                photonView.RPC(m_RPCEvents[i].m_functionName, m_RPCEvents[i].m_target, m_RPCEvents[i].m_parameters);
            }
            //if (m_RPCEvents.Count > 0)
            //{
            //    photonView.RPC(m_RPCEvents[0].m_functionName, m_RPCEvents[0].m_target, m_RPCEvents[0].m_parameters);
            //}
        }



        #region PunRPC
        [PunRPC]
        public void SetSente(bool sente, int id)
        {
            if(m_RPCEnable)
            {
                if (id == m_otherID)
                {
                    m_otherID++;
                    m_RPCEnable = false;

                    GameObject canvas = GameObject.Find("Canvas");
                    canvas.GetComponent<GameControl>().SetSente(sente);
                    WindowCreator.CreateSelectCliqueWindow();
                }

                if(id < m_otherID)
                    photonView.RPC("ReceivedOrder", RpcTarget.Others, id);
            }
        }
        [PunRPC]
        public void SetClique(bool clique, int id)
        {
            if (m_RPCEnable)
            {
                if (id == m_otherID)
                {
                    m_otherID++;
                    m_RPCEnable = false;

                    GameObject canvas = GameObject.Find("Canvas");
                    canvas.GetComponent<GameControl>().SetClique(clique);
                    canvas.GetComponent<GameControl>().GameInitial();
                    canvas.GetComponent<GameControl>().GameSynchronize();


                    //photonView.RPC("PreTurnOperation", RpcTarget.Others);

                    RPCEvent rPCEvent = new RPCEvent();
                    rPCEvent.m_ID = m_currentID;
                    m_currentID++;
                    rPCEvent.m_functionName = "PreTurnOperation";
                    rPCEvent.m_target = RpcTarget.Others;
                    rPCEvent.m_parameters = new object[1];
                    rPCEvent.m_parameters[0] = rPCEvent.m_ID;
                    m_RPCEvents.Add(rPCEvent);


                    GameObject.Find("Canvas").GetComponent<GameControl>().PreTurnOperation();

                }
                if (id < m_otherID)
                    photonView.RPC("ReceivedOrder", RpcTarget.Others, id);
            }
        }
        [PunRPC]
        public void CreateSelectSenteWindow(int id)
        {
            if (m_RPCEnable)
            {
                if (id == m_otherID)
                {
                    m_otherID++;
                    m_RPCEnable = false;

                    WindowCreator.CreateSelectSenteWindow();
                }

                if (id < m_otherID)
                    photonView.RPC("ReceivedOrder", RpcTarget.Others, id);
            }
        }
        [PunRPC]
        public void ClickObject(int triggerIndex, int listIndex, int id)
        {
            if (m_RPCEnable)
            {
                if (id == m_otherID)
                {
                    m_otherID++;
                    m_RPCEnable = false;

                    GameObject trigger = null;
                    switch (listIndex)
                    {
                        case 0:
                            trigger = m_commonList[triggerIndex];
                            break;
                        case 1:
                            trigger = m_cardList[PlayerType.Player][triggerIndex];
                            break;
                        case 2:
                            trigger = m_cardList[PlayerType.Enemy][triggerIndex];
                            break;
                    }
                    Debug.Log(m_cardList[PlayerType.Enemy].Count);

                    Debug.Log("执行ClickObject:" + trigger.gameObject.name);

                    if (trigger.GetComponent<Card>() != null)
                    {
                        trigger.GetComponent<Card>().Click();
                        return;
                    }
                    if (trigger.GetComponent<ButtonBase>() != null)
                    {
                        trigger.GetComponent<ButtonBase>().Click();
                        return;
                    }
                    if (trigger.GetComponent<GameControl>() != null)
                    {
                        trigger.GetComponent<GameControl>().Click();
                        return;
                    }
                }

                if (id < m_otherID)
                    photonView.RPC("ReceivedOrder", RpcTarget.Others, id);
            }
        }
        [PunRPC]
        public void SetDeck(string[] idArray, int index, int id)
        {
            //if (m_RPCEnable)
            {
                if (id == m_otherID)
                {
                    m_otherID++;
                    m_RPCEnable = false;

                    List<string> idList = new List<string>();
                    for (int i = 0; i < idArray.Length; i++)
                    {
                        idList.Add(idArray[i]);
                    }
                    GameListener.Instance().GetPlayer().SetCardDeck(idList, index);
                }

                if (id < m_otherID)
                    photonView.RPC("ReceivedOrder", RpcTarget.Others, id);
            }
        }
        [PunRPC]
        public void Surrender(int id)
        {
            if (m_RPCEnable)
            {
                if (id == m_otherID)
                {
                    m_otherID++;
                    m_RPCEnable = false;

                    WindowCreator.CreateGameOverWindow("You Win");
                    GameObject.Find("Canvas").GetComponent<GameControl>().GameFinish();
                }

                if (id < m_otherID)
                    photonView.RPC("ReceivedOrder", RpcTarget.Others, id);
            }
        }
        [PunRPC]
        public void PreTurnOperation(int id)
        {
            if (m_RPCEnable)
            {
                if (id == m_otherID)
                {
                    m_otherID++;
                    m_RPCEnable = false;

                    GameObject.Find("Canvas").GetComponent<GameControl>().PreTurnOperation();
                }

                if (id < m_otherID)
                    photonView.RPC("ReceivedOrder", RpcTarget.Others, id);
            }
        }
        [PunRPC]
        public void ReconnectSuccess(int id)
        {
            if (m_RPCEnable)
            {
                if (id == m_otherID)
                {
                    m_otherID++;
                    m_RPCEnable = false;

                    GameObject.Find("Canvas").GetComponent<GameControl>().ReconnectSucced();
                }

                if (id < m_otherID)
                    photonView.RPC("ReceivedOrder", RpcTarget.Others, id);
            }
        }
        [PunRPC]
        public void ReceivedOrder(int id)
        {
            while (m_RPCEvents.Count > 0 && m_RPCEvents[0].m_ID <= id)
            {
                m_RPCEvents.RemoveAt(0);
            }
        }
        #endregion
    }
}

