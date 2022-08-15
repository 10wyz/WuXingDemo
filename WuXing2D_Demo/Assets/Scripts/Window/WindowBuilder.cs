using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingAssetManage;
using UnityEngine.UI;
using WuXingTool;
using WuXingButton;

namespace WuXingWindow
{
    public class WindowBuilder
    {
        GameObject m_window;

        public WindowBuilder()
        {
            m_window = null;
        }

        public void SetMainWindow(string name, float width, float height, Color color, Sprite sprite = null)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Window/MainWindow");
            m_window = GameObject.Instantiate(module);
            m_window.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            m_window.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            m_window.GetComponent<Image>().color = color;
            m_window.GetComponent<Image>().sprite = sprite;

            m_window.name = name;
        }

        public void AddButton(string name,Vector2 size, Vector2 nomarPosition, Vector2 pivot, string text, ButtonFactory buttonFactory, ButtonTag buttonTag)
        {
            GameObject button = buttonFactory.GetButton(text, buttonTag);
            button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            button.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            button.transform.SetParent(m_window.transform);

            float windowWidth = m_window.GetComponent<RectTransform>().rect.width/2;
            float windowHeight = m_window.GetComponent<RectTransform>().rect.height/2;
            button.transform.localPosition =  new Vector3( nomarPosition.x* windowWidth, nomarPosition.y*windowHeight, 0);
            button.GetComponent<RectTransform>().pivot = pivot;

            button.name = name;
        }
        public void AddText(string name, Vector2 size, Vector2 nomarPosition, Vector2 pivot, string text, int fontsize=0, TextAnchor align= TextAnchor.MiddleCenter)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Window/MainText");
            GameObject mainText = GameObject.Instantiate(module);

            mainText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            mainText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            mainText.transform.SetParent(m_window.transform);

            float windowWidth = m_window.GetComponent<RectTransform>().rect.width / 2;
            float windowHeight = m_window.GetComponent<RectTransform>().rect.height / 2;
            mainText.transform.localPosition = new Vector3(nomarPosition.x * windowWidth, nomarPosition.y * windowHeight, 0);
            mainText.GetComponent<RectTransform>().pivot = pivot;

            mainText.GetComponent<Text>().text = text;
            mainText.name = name;

            if(fontsize != 0)
            {
                mainText.GetComponent<Text>().fontSize = fontsize;
            }
            mainText.GetComponent<Text>().alignment = align;
        }
        public void AddScrollbar(string name, Vector2 size, Vector2 nomarPosition, Vector2 pivot)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Window/Scrollbar");
            GameObject scrollbar = GameObject.Instantiate(module);

            scrollbar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            scrollbar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            scrollbar.transform.SetParent(m_window.transform);

            float windowWidth = m_window.GetComponent<RectTransform>().rect.width / 2;
            float windowHeight = m_window.GetComponent<RectTransform>().rect.height / 2;
            scrollbar.transform.localPosition = new Vector3(nomarPosition.x * windowWidth, nomarPosition.y * windowHeight, 0);
            scrollbar.GetComponent<RectTransform>().pivot = pivot;

            scrollbar.name = name;
        }
        public void AddDropdown(string name, Vector2 size, Vector2 nomarPosition, Vector2 pivot)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Window/Dropdown");
            GameObject dropdown = GameObject.Instantiate(module);

            dropdown.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            dropdown.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            dropdown.transform.SetParent(m_window.transform);

            float windowWidth = m_window.GetComponent<RectTransform>().rect.width / 2;
            float windowHeight = m_window.GetComponent<RectTransform>().rect.height / 2;
            dropdown.transform.localPosition = new Vector3(nomarPosition.x * windowWidth, nomarPosition.y * windowHeight, 0);
            dropdown.GetComponent<RectTransform>().pivot = pivot;

            dropdown.name = name;
        }


        public void AddInputField(string name, Vector2 size, Vector2 nomarPosition, Vector2 pivot)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Window/InputField");
            GameObject inputField = GameObject.Instantiate(module);

            inputField.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            inputField.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            inputField.transform.SetParent(m_window.transform);

            float windowWidth = m_window.GetComponent<RectTransform>().rect.width / 2;
            float windowHeight = m_window.GetComponent<RectTransform>().rect.height / 2;
            inputField.transform.localPosition = new Vector3(nomarPosition.x * windowWidth, nomarPosition.y * windowHeight, 0);
            inputField.GetComponent<RectTransform>().pivot = pivot;

            inputField.name = name;
        }



        public GameObject GetWindow()
        {
            return m_window;
        }
    }
}

