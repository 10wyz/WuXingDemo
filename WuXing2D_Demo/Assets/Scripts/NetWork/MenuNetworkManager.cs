using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using WuXingCore;
using UnityEngine.UI;
using WuXingTool;
using UnityEngine.SceneManagement;


namespace WuXingNetwork
{
    public class MenuNetworkManager : MonoBehaviourPunCallbacks
    {
        public GameObject m_menu;
        string version = "1.0";

        float m_time;

        private void Awake()
        {
            m_time = 0;
            PhotonNetwork.AutomaticallySyncScene = false;
            if(!PhotonNetwork.IsConnected)
                Connect();
        }


        public bool Connect()
        {
            PhotonNetwork.NickName = m_menu.transform.Find("PlayerName").GetComponent<InputField>().textComponent.text;
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.GameVersion = version;//设置版本号
                if (PhotonNetwork.ConnectUsingSettings())//连接Photon服务器
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }


        void Update()
        {
            if(!PhotonNetwork.IsConnected && m_time > 1)
            {
                PhotonNetwork.Reconnect();
                m_time = 0;
            }
        }
        void FixedUpdate()
        {
            if(!PhotonNetwork.IsConnected)
            {
                m_time += 0.02f;
            }
        }


        public void CreateRoom(string roomName)
        {
            if (PhotonNetwork.IsConnected)
            {
                if (roomName == "")
                {
                    Debug.Log("房间名不合法");
                    m_menu.GetComponent<MenuControl>().CloseWindow();
                }
                else
                {
                    RoomOptions roomOptions = new RoomOptions();
                    roomOptions.MaxPlayers = 0;
                    PhotonNetwork.CreateRoom(roomName, roomOptions);
                }
            }
            else
            {
                Debug.Log("未连接服务器");
                m_menu.GetComponent<MenuControl>().CloseWindow();
            }
        }
        public void JoinRoom(string roomName)
        {
            if (PhotonNetwork.IsConnected)
            {
                if (roomName == "")
                {
                    Debug.Log("房间名不合法");
                    m_menu.GetComponent<MenuControl>().CloseWindow();
                }
                else
                {
                    PhotonNetwork.JoinRoom(roomName);
                }
            }
            else
            {
                Debug.Log("未连接到服务器");
                m_menu.GetComponent<MenuControl>().CloseWindow();
            }
        }
        public void SendStartGame()
        {
            photonView.RPC("StartGame", RpcTarget.Others);
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("离开房间");
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.Log("断开连接");
        }
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("成功加入房间");

            ExitGames.Client.Photon.Hashtable dictionaryEntries = PhotonNetwork.LocalPlayer.CustomProperties;
            dictionaryEntries["准备"] = "false";
            PhotonNetwork.LocalPlayer.SetCustomProperties(dictionaryEntries);

            m_menu.GetComponent<MenuControl>().CloseWindow();
            GameObject window = WindowCreator.CreateRoomWindow(m_menu);
            m_menu.GetComponent<MenuControl>().SetWindow(window);
            m_menu.GetComponent<MenuControl>().UpdateRoomWindow();
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            m_menu.GetComponent<MenuControl>().CloseWindow();
            Debug.Log("加入房间失败");
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            m_menu.GetComponent<MenuControl>().UpdateRoomWindow();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            m_menu.GetComponent<MenuControl>().UpdateRoomWindow();
        }
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("连接到服务器");
        }
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("创建房间");
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
            m_menu.GetComponent<MenuControl>().UpdateRoomWindow();
        }


        #region PunRPC
        [PunRPC]
        public void StartGame()
        {
            SceneManager.LoadScene("GamePlay", LoadSceneMode.Single);
            if (!PhotonNetwork.IsMasterClient)
                SendStartGame();
        }
        #endregion

    }
}

