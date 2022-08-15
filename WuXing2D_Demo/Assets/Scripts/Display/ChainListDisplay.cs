using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace WuXingDisplay
{
    public class ChainListDisplay : MonoBehaviour
    {
        public GameObject m_chainWindow;
        public GameObject m_list;

        public void ClickButton()
        {
            if(m_chainWindow.GetComponent<Image>().enabled)
            {
                m_chainWindow.GetComponent<Image>().enabled = false;
                m_list.SetActive(false);
            }
            else
            {
                m_chainWindow.GetComponent<Image>().enabled = true;
                m_list.SetActive(true);
            }
        }
    }
}

