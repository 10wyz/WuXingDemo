using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WuXingWindow
{
    public class ImageSettingWindow : StateWindow
    {
        private int m_width;
        private int m_height;
        private FullScreenMode m_fullScreenMode;
        void Start()
        {
            m_width = Screen.width;
            m_height = Screen.height;
            m_fullScreenMode = Screen.fullScreenMode;

            GameObject dropdown1 = this.transform.Find("分辨率").gameObject;
            GameObject dropdown2 = this.transform.Find("全屏设置").gameObject;

            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            list.Add(new Dropdown.OptionData("800*600"));
            list.Add(new Dropdown.OptionData("1024*768"));
            list.Add(new Dropdown.OptionData("1280*720"));
            list.Add(new Dropdown.OptionData("1920*1080"));
            dropdown1.GetComponent<Dropdown>().AddOptions(list);
            switch(m_width)
            {
                case 800:
                    dropdown1.GetComponent<Dropdown>().SetValueWithoutNotify(0);
                    break;
                case 1024:
                    dropdown1.GetComponent<Dropdown>().SetValueWithoutNotify(1);
                    break;
                case 1280:
                    dropdown1.GetComponent<Dropdown>().SetValueWithoutNotify(2);
                    break;
                case 1920:
                    dropdown1.GetComponent<Dropdown>().SetValueWithoutNotify(3);
                    break;
            }

            List<Dropdown.OptionData> list2 = new List<Dropdown.OptionData>();
            list2.Add(new Dropdown.OptionData("全屏"));
            list2.Add(new Dropdown.OptionData("无边框窗口"));
            list2.Add(new Dropdown.OptionData("窗口"));
            dropdown2.GetComponent<Dropdown>().AddOptions(list2);

            switch (m_fullScreenMode)
            {
                case FullScreenMode.FullScreenWindow:
                    dropdown2.GetComponent<Dropdown>().SetValueWithoutNotify(0);
                    break;
                case FullScreenMode.MaximizedWindow:
                    dropdown2.GetComponent<Dropdown>().SetValueWithoutNotify(1);
                    break;
                case FullScreenMode.Windowed:
                    dropdown2.GetComponent<Dropdown>().SetValueWithoutNotify(2);
                    break;
            }
        }

        public override void Notify(GameObject child, string eventID)
        {
            switch(eventID)
            {
                case "分辨率":
                    EventResolution(child);
                    break;
                case "全屏设置":
                    EventFullScreenSet(child);
                    break;
                case "确定":
                    EventOk();
                    break;
                case "取消":
                    EventCancel();
                    break;
            }
        }


        public void EventResolution(GameObject dropdown)
        {
            string resolution = dropdown.GetComponent<Dropdown>().captionText.text;
            switch(resolution)
            {
                case "800*600":
                    m_width = 800;
                    m_height = 600;
                    break;
                case "1024*768":
                    m_width = 1024;
                    m_height = 768;
                    break;
                case "1280*720":
                    m_width = 1280;
                    m_height = 720;
                    break;
                case "1920*1080":
                    m_width = 1920;
                    m_height = 1080;
                    break;
                default:
                    break;
            }
        }
        public void EventFullScreenSet(GameObject dropdown)
        {
            string fullScreenMode = dropdown.GetComponent<Dropdown>().captionText.text;

            switch(fullScreenMode)
            {
                case "全屏":
                    m_fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case "无边框窗口":
                    m_fullScreenMode = FullScreenMode.MaximizedWindow;
                    break;
                case "窗口":
                    m_fullScreenMode = FullScreenMode.Windowed;
                    break;
                default:
                    break;
            }
        }
        public void EventOk()
        {
            Screen.SetResolution(m_width, m_height, m_fullScreenMode);
            if (this.transform.parent.GetComponent<StateWindow>() != null)
            {
                this.transform.parent.GetComponent<StateWindow>().ReEntryWindow();
            }

            GameObject.Destroy(this.gameObject);
        }
        private void EventCancel()
        {
            if(this.transform.parent.GetComponent<StateWindow>() != null)
            {
                this.transform.parent.GetComponent<StateWindow>().ReEntryWindow();
            }

            GameObject.Destroy(this.gameObject);
        }
    }
}

