using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingTool;
using WuXingButton;
using UnityEngine.UI;
using WuXingCore;

namespace WuXingWindow
{
    public class OptionSelectWindow : StateWindow
    {
        private int m_num;
        private int m_type;
        private List<string> m_selectOption;
        private List<GameObject> m_optionTab;


        public OptionSelectWindow()
        {
            m_selectOption = new List<string>();
            m_optionTab = new List<GameObject>();
        }

        public void Initial(List<string> options, int num, int type)
        {
            m_num = num;
            m_type = type;
            for(int i = 0; i < options.Count; i++)
            {
                AddOptions(options[i]);
            }
            AllignButton();
        }

        public List<string> GetSelectOption()
        {
            return m_selectOption;
        }
        public int GetTypeIndex()
        {
            return m_type;
        }

        private void AddOptions(string option)
        {
            int showindex = m_optionTab.Count + 1;
            TabButtonFatory buttonFatory = new TabButtonFatory();
            GameObject optionTab = buttonFatory.GetButton("Tab", ButtonTag.Tab);//ButtonCreator.CreateTextButton(this.gameObject);

            optionTab.transform.SetParent(this.gameObject.transform);
            optionTab.GetComponent<TabButton>().Initial(showindex, option);
            optionTab.transform.GetComponentInChildren<Text>().text = option;

            m_optionTab.Add(optionTab);
        }

        private void AllignButton()
        {
            if (m_optionTab.Count > 0)
            {
                int row = Mathf.CeilToInt(m_optionTab.Count/5.0f);
                int col = 5;
                if (row == 1)
                    col = m_optionTab.Count;

                float windowWidth = this.gameObject.GetComponent<RectTransform>().rect.width * 0.8f;
                float windowHeight = this.gameObject.GetComponent<RectTransform>().rect.height * 0.6f;

                float width = Mathf.Min(windowWidth / (col + (col-1) * 0.05f), 80);
                float height = Mathf.Min(windowHeight / (row + (row - 1) * 0.05f), 40);

                for (int k = 0; k < m_optionTab.Count; k++)
                {
                    int i = k / col;
                    int j = k % col;

                    m_optionTab[k].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    m_optionTab[k].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                    float x = ((j - (col - 1) / 2.0f)) * width * 1.05f;
                    float y = -(i - (row - 1) / 2.0f) * height * 1.05f;
                    m_optionTab[k].GetComponent<RectTransform>().localPosition = new Vector3(x, y+0.05f* windowHeight, 0);
                    m_optionTab[k].GetComponent<RectTransform>().localScale = Vector3.one;
                }

                //GameObject skillText = this.gameObject.transform.Find("Description").gameObject;
                //float width = skillText.GetComponent<RectTransform>().rect.width;
                //width = width / (m_skillFlag.Count + (m_skillFlag.Count - 1) * 0.05f);
                //float height = skillText.GetComponent<RectTransform>().rect.height / 2
                //               + m_skillFlag[0].GetComponent<RectTransform>().rect.height / 2;


            }


        }


        public override void Notify(GameObject child, string eventID)
        {
            switch(eventID)
            {
                case "TabButton":
                    EventTabButton(child);
                    break;
                case "确定":
                    EventOk(child);
                    break;
                case "取消":
                    EventCancel(child);
                    break;
                default:
                    break;
            }
        }

        private void EventTabButton(GameObject button)
        {
            string text = button.GetComponent<TabButton>().GetDescription();

            if (m_selectOption.Contains(text))
            {
                m_selectOption.Remove(text);
                button.GetComponent<Image>().color = new Color(102/255f, 200/255f, 229/255f);
            }
            else if(m_selectOption.Count < m_num)
            {
                m_selectOption.Add(text);
                button.GetComponent<Image>().color = new Color(255/255f, 0/255f, 0/255f);
            }
        }
        private void EventOk(GameObject button)
        {
            EventPara eventPara = new EventPara();
            eventPara.m_trigger = button;

            GameListener.Instance().ControlEventNotify(ControlEventID.evt_button_click, eventPara);
            GameObject.Destroy(this.gameObject);
        }
        private void EventCancel(GameObject button)
        {
            EventPara eventPara = new EventPara();
            eventPara.m_trigger = button;

            GameListener.Instance().ControlEventNotify(ControlEventID.evt_button_click, eventPara);
            GameObject.Destroy(this.gameObject);
        }
    }
}

