using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace WuXingManual
{
    public class NikeName : MonoBehaviour
    {
        public string nikeName;

        public void ChangeName()
        {
            string name = this.GetComponent<InputField>().textComponent.text;
            if(name == "")
            {
                this.GetComponent<InputField>().textComponent.text = nikeName;
            }
            else
            {
                nikeName = name;
                PhotonNetwork.NickName = nikeName;
            }

        }
    }
}

