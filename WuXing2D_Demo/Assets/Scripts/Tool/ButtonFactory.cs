using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingAssetManage;
using WuXingButton;
using UnityEngine.UI;

namespace WuXingTool
{
    public class ButtonFactory
    {
        virtual public GameObject GetButton(string text, ButtonTag buttonTag)
        {
            return null;
        }
    }

    public class TabButtonFatory : ButtonFactory
    {
        public override GameObject GetButton(string text, ButtonTag buttonTag)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Window/RectButton");
            GameObject button = GameObject.Instantiate(module);
            button.GetComponentInChildren<Text>().text = text;
            button.AddComponent<TabButton>();
            button.GetComponent<TabButton>().m_buttonTag = buttonTag;
            return button;
        }
    }
    public class SkillButtonFactory : ButtonFactory
    {
        public override GameObject GetButton(string text, ButtonTag buttonTag)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Window/CircleButton");
            GameObject button = GameObject.Instantiate(module);
            button.AddComponent<SkillButton>();
            button.GetComponent<SkillButton>().m_buttonTag = buttonTag;

            GameObject image = new GameObject();
            image.AddComponent<Image>();
            Sprite sp = Resources.Load<Sprite>("Picture/Other/五行标");
            image.GetComponent<Image>().sprite = sp;
            image.transform.SetParent(button.transform);
            
            float width = button.GetComponent<RectTransform>().rect.width;
            float height = button.GetComponent<RectTransform>().rect.height;
            image.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            image.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            image.GetComponent<RectTransform>().localPosition = Vector3.zero;
            image.GetComponent<RectTransform>().localScale = Vector3.one;

            return button;
        }
    }

    public class NormalButtonFatory : ButtonFactory
    {
        private string m_shape;

        public NormalButtonFatory()
        {
            m_shape = "Rect";
        }
        public NormalButtonFatory(string shape)
        {
            m_shape = shape;
        }

        public override GameObject GetButton(string text, ButtonTag buttonTag)
        {
            GameObject module = GetModule();
            GameObject button = GameObject.Instantiate(module);
            button.GetComponentInChildren<Text>().text = text;
            button.AddComponent<NormalButton>();
            button.GetComponent<NormalButton>().m_buttonTag = buttonTag;
            return button;
        }

        private GameObject GetModule()
        {
            switch(m_shape)
            {
                case "Rect":
                    return ModuleWarehouse.Instance().GetModule("Window/RectButton");
                case "Circle":
                    return ModuleWarehouse.Instance().GetModule("Window/CircleButton");
                default:
                    return null;
            }
        }
    }
    public class MenuButtonFatory : ButtonFactory
    {
        private string m_shape;

        public MenuButtonFatory()
        {
            m_shape = "Rect";
        }
        public MenuButtonFatory(string shape)
        {
            m_shape = shape;
        }

        public override GameObject GetButton(string text, ButtonTag buttonTag)
        {
            GameObject module = GetModule();
            GameObject button = GameObject.Instantiate(module);
            button.GetComponentInChildren<Text>().text = text;
            button.AddComponent<MenuButton>();
            button.GetComponent<MenuButton>().m_buttonTag = buttonTag;
            return button;
        }

        private GameObject GetModule()
        {
            switch (m_shape)
            {
                case "Rect":
                    return ModuleWarehouse.Instance().GetModule("Window/RectButton");
                case "Circle":
                    return ModuleWarehouse.Instance().GetModule("Window/CircleButton");
                default:
                    return null;
            }
        }
    }


}

