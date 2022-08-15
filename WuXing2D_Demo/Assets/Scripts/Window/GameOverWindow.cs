using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace WuXingWindow
{
    public class GameOverWindow : StateWindow
    {
        public override void Notify(GameObject child, string eventID)
        {
            switch (eventID)
            {
                case "确定":
                    EventOk();
                    break;
            }
        }

        private void EventOk()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        void Update()
        {
            if (Input.GetButtonDown("ESC") && GameObject.Find("MenuWindow").transform.childCount == 1)
            {
                EventOk();
            }
        }
    }
}

