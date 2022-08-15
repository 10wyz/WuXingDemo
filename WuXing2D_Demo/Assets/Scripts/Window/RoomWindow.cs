using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using WuXingCore;
using WuXingNetwork;

namespace WuXingWindow
{
    public class RoomWindow : StateWindow
    {
        public void UpdateInfo()
        {
            this.transform.Find("MasterName").GetComponent<Text>().text = "";
            this.transform.Find("PlayerName").GetComponent<Text>().text = "";

            this.transform.Find("MasterReady").GetComponent<Text>().text = "";
            this.transform.Find("PlayerReady").GetComponent<Text>().text = "";

            this.transform.Find("RoomName").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;

            Player[] players = PhotonNetwork.PlayerList;
            for(int i = 0; i < players.Length; i++)
            {
                if(players[i].IsMasterClient)
                {
                    this.transform.Find("MasterName").GetComponent<Text>().color = Color.black;
                    this.transform.Find("MasterName").GetComponent<Text>().text = players[i].NickName;
                    if(PhotonNetwork.LocalPlayer == players[i])
                        this.transform.Find("MasterName").GetComponent<Text>().color = Color.red;

                    this.transform.Find("MasterReady").GetComponent<Text>().color = Color.black;
                    this.transform.Find("MasterReady").GetComponent<Text>().text = "就绪";
                }
                else
                {
                    this.transform.Find("PlayerName").GetComponent<Text>().color = Color.black;
                    this.transform.Find("PlayerName").GetComponent<Text>().text = players[i].NickName;
                    if (PhotonNetwork.LocalPlayer == players[i])
                        this.transform.Find("PlayerName").GetComponent<Text>().color = Color.red;

                    this.transform.Find("PlayerReady").GetComponent<Text>().color = Color.black;
                    if(players[i].CustomProperties["准备"] == null || players[i].CustomProperties["准备"].Equals("false"))
                        this.transform.Find("PlayerReady").GetComponent<Text>().text = "未就绪";
                    else
                        this.transform.Find("PlayerReady").GetComponent<Text>().text = "就绪";

                }
            }

            if(PhotonNetwork.IsMasterClient)
            {
                GameObject button = this.transform.Find("开始游戏").gameObject;
                button.transform.Find("Text").GetComponent<Text>().text = "开始游戏";
            }
            else
            {
                GameObject button = this.transform.Find("开始游戏").gameObject;
                if (PhotonNetwork.LocalPlayer.CustomProperties["准备"] == null || PhotonNetwork.LocalPlayer.CustomProperties["准备"].Equals("false"))
                    button.transform.Find("Text").GetComponent<Text>().text = "准备";
                else
                    button.transform.Find("Text").GetComponent<Text>().text = "取消准备";
            }
        }

        public override void Notify(GameObject child, string eventID)
        {
            switch (eventID)
            {
                case "开始游戏":
                    EventStartGame();
                    break;
                case "退出房间":
                    EventLeftRoom();
                    break;
                default:
                    break;
            }
        }


        private void EventStartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Player[] players = PhotonNetwork.PlayerListOthers;

                if (players.Length < 1)
                    return;

                for (int i = 0; i < players.Length; i++)
                {
                    if(players[i].CustomProperties["准备"].Equals("false"))
                    {
                        return;
                    }
                }

                this.transform.parent.GetComponent<MenuControl>().CloseWindow();

                for(int i=0; i<PhotonNetwork.PlayerList.Length; i++)
                {
                    ExitGames.Client.Photon.Hashtable dictionaryEntries = PhotonNetwork.PlayerList[i].CustomProperties;
                    dictionaryEntries["准备"] = "false";
                    PhotonNetwork.PlayerList[i].SetCustomProperties(dictionaryEntries);
                }

                GameObject.Find("MenuNetworkControl").GetComponent<MenuNetworkManager>().SendStartGame();
            }
            else
            {
                Player player = PhotonNetwork.LocalPlayer;
                if (player.CustomProperties["准备"].Equals("false"))
                {
                    ExitGames.Client.Photon.Hashtable dictionaryEntries = player.CustomProperties;
                    dictionaryEntries["准备"] = "true";
                    player.SetCustomProperties(dictionaryEntries);
                    GameObject button = this.transform.Find("开始游戏").gameObject;
                    button.transform.Find("Text").GetComponent<Text>().text = "取消准备";
                }
                else
                {
                    ExitGames.Client.Photon.Hashtable dictionaryEntries = player.CustomProperties;
                    dictionaryEntries["准备"] = "false";
                    player.SetCustomProperties(dictionaryEntries);
                    GameObject button = this.transform.Find("开始游戏").gameObject;
                    button.transform.Find("Text").GetComponent<Text>().text = "准备";
                }
            }
        }
        private void EventLeftRoom()
        {
            Player[] players = PhotonNetwork.PlayerListOthers;
            if (PhotonNetwork.IsMasterClient && players.Length > 0)
            {
                PhotonNetwork.SetMasterClient(players[0]);
            }
            PhotonNetwork.LeaveRoom();

            this.transform.parent.GetComponent<MenuControl>().CloseWindow();
        }
    }
}

