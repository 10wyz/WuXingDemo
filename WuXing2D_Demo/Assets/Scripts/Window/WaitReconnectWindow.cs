using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace WuXingWindow
{
    public class WaitReconnectWindow : StateWindow
    {
        float m_time;
        int m_maxTime;

        void Start()
        {
            m_time = 0;
        }

        public void Initial(int maxtime)
        {
            m_maxTime = maxtime;
        }

        void FixedUpdate()
        {
            m_time += 0.02f;

            DisplayTimne();
        }

        private void DisplayTimne()
        {

            string text = Convert.ToString(Mathf.FloorToInt(m_time)) + "/" + Convert.ToString(m_maxTime);
            this.transform.Find("时间显示").GetComponent<Text>().text = text;
        }
    }
}

