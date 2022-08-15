using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingAssetManage;
using UnityEngine.UI;
using WuXingTool;
using WuXingButton;
using System;
using WuXingCore;

namespace WuXingWindow
{
    public class SkillSelectWindow : StateWindow
    {
        int m_index;//技能下标
        List<GameObject> m_skillFlag;
        
        public SkillSelectWindow()
        {
            m_skillFlag = new List<GameObject>();
        }

        public bool Initial(List<int> index, List<string> text)
        {
            for(int i = 0; i < index.Count; i++)
            {
                AddSkillFlag(index[i], text[i]);
            }
            AllignButton();

            m_index = index[0];
            SetText(text[0]);

            return true;
        }

        public int GetIndex()
        {
            return m_index;
        }
        private void SetIndex(int index)
        {
            m_index = index;
        }
        private void SetText(string text)
        {
            GameObject skillText = this.gameObject.transform.Find("Description").gameObject;
            skillText.GetComponentInChildren<Text>().text = text;

            //自适应文字大小
            //float width = skillText.GetComponent<RectTransform>().rect.width;
            //float height = skillText.GetComponent<RectTransform>().rect.height;
            //int n = text.Length;
            //Debug.Log(n);
            //int fontsize = Mathf.FloorToInt(Mathf.Sqrt(width * height / 1.2f / n));
            //int row = Mathf.CeilToInt(1.0f * n / Mathf.FloorToInt(width / fontsize));
            //skillText.GetComponentInChildren<Text>().fontSize = Mathf.FloorToInt(height / row / 1.2f);
        }

        private bool AddSkillFlag(int index, string text)
        {
            int showindex = m_skillFlag.Count + 1;
            TabButtonFatory buttonFatory = new TabButtonFatory();
            GameObject skillFlag = buttonFatory.GetButton("Tab", ButtonTag.Tab);//ButtonCreator.CreateTextButton(this.gameObject);

            skillFlag.transform.SetParent(this.gameObject.transform);
            skillFlag.GetComponent<TabButton>().Initial(index, text);
            skillFlag.transform.GetComponentInChildren<Text>().text = Convert.ToString(showindex);

            m_skillFlag.Add(skillFlag);
            return true;
        }

        private void AllignButton()
        {
            if(m_skillFlag.Count > 0)
            {
                GameObject skillText = this.gameObject.transform.Find("Description").gameObject;

                float width = skillText.GetComponent<RectTransform>().rect.width;
                width = width / (m_skillFlag.Count + (m_skillFlag.Count-1)*0.05f);
                float height = this.gameObject.GetComponent<RectTransform>().rect.height * 0.15f;

                for (int i = 0; i < m_skillFlag.Count; i++)
                {
                    m_skillFlag[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    m_skillFlag[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                    float x = ((i - (m_skillFlag.Count-1)/2.0f)) * width*1.05f;
                    float y = skillText.GetComponent<RectTransform>().rect.height / 2f;
                    m_skillFlag[i].GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                    m_skillFlag[i].GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
                    m_skillFlag[i].GetComponent<RectTransform>().localScale = Vector3.one;
                }
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
            int index = button.GetComponent<TabButton>().GetIndex();
            string text = button.GetComponent< TabButton>().GetDescription();
            SetIndex(index);
            SetText(text);
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

