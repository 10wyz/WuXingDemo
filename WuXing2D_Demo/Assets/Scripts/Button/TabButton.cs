using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WuXingWindow;
using WuXingCore;

namespace WuXingButton
{
    /// <summary>
    /// 标签按钮
    /// </summary>
    public class TabButton : ButtonBase
    {
        int m_index;
        string m_skillDescription;

        public void Start()
        {
            if (GameListener.Instance().GetGameNetworkManager() != null)
            {
                GameListener.Instance().GetGameNetworkManager().AddButton(this.gameObject);
            }
        }

        public void Initial(int index, string text)
        {
            m_index = index;
            m_skillDescription = text;
        }

        public int GetIndex()
        {
            return m_index;
        }
        public string GetDescription()
        {
            return m_skillDescription;
        }


        public override void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                if (GameListener.Instance().IsAnimation() || GameObject.Find("MenuWindow").transform.childCount > 0)
                    return;

                if (GameListener.Instance().GetGameNetworkManager() != null && GameListener.Instance().IsPlayerControl())
                {
                    GameObject window = this.gameObject.transform.parent.gameObject;
                    window.GetComponent<StateWindow>().Notify(this.gameObject, "TabButton");
                    GameListener.Instance().GetGameNetworkManager().SendClickObject(this.gameObject);
                }
            }
            base.OnPointerDown(eventData);
        }
        public override void Click()
        {
            GameObject window = this.gameObject.transform.parent.gameObject;
            window.GetComponent<StateWindow>().Notify(this.gameObject, "TabButton");
        }

        public void OnDestroy()
        {
            if (GameListener.Instance().GetGameNetworkManager() != null)
            {
                GameListener.Instance().GetGameNetworkManager().RemoveButton(this.gameObject);
            }
        }
    }
}

