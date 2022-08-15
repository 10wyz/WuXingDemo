using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingTool;
using WuXingWindow;
using Photon.Pun;
using Photon.Realtime;

namespace WuXingCore
{
    public class MenuControl : StateWindow
    {
        private GameObject m_window;
        public List<GameObject> m_button;
        public MenuControl()
        {
            m_window = null;
        }

        void Start()
        {
            if(PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
            {
                GameObject window = WindowCreator.CreateRoomWindow(this.gameObject);
                window.GetComponent<RoomWindow>().UpdateInfo();
                SetWindow(window);
            }
        }

        public void CreateRoom()
        {
            m_window = WindowCreator.CreateNewRoomWindow(this.gameObject);
            for(int i = 0; i < m_button.Count; i++)
            {
                m_button[i].SetActive(false);
            }
        }
        public void JoinRoom()
        {
            m_window = WindowCreator.CreateJoinRoomWindow(this.gameObject);
            for (int i = 0; i < m_button.Count; i++)
            {
                m_button[i].SetActive(false);
            }
        }
        public void SetResolution()
        {
            m_window = WindowCreator.CreateImageSettingWindow(this.gameObject);
            for (int i = 0; i < m_button.Count; i++)
            {
                m_button[i].SetActive(false);
            }
        }
        public void QuitGame()
        {
            Application.Quit();
        }

        public override void ReEntryWindow()
        {
            for (int i = 0; i < m_button.Count; i++)
            {
                m_button[i].SetActive(true);
            }
        }
        public void CloseWindow()
        {
            if(m_window != null)
            {
                for(int i = 0; i < m_button.Count; i++)
                {
                    m_button[i].SetActive(true);
                }
                GameObject.Destroy(m_window);
            }
        }
        public void SetWindow(GameObject window)
        {
            m_window = window;
            for (int i = 0; i < m_button.Count; i++)
            {
                m_button[i].SetActive(false);
            }
        }
        public void UpdateRoomWindow()
        {
            if (m_window != null && m_window.GetComponent<RoomWindow>()!=null)
            {
                m_window.GetComponent<RoomWindow>().UpdateInfo();
            }
        }
    
        public void test()
        {
            m_window = WindowCreator.CreateInGameMenuWindow(this.gameObject);
        }
    }
}

