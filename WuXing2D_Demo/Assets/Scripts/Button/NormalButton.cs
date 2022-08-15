using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WuXingWindow;
using WuXingCore;

namespace WuXingButton
{
    /// <summary>
    /// 普通按钮（发送按键事件）
    /// </summary>
    public class NormalButton : ButtonBase
    {
        public void Start()
        {
            if (GameListener.Instance().GetGameNetworkManager() != null)
            {
                GameListener.Instance().GetGameNetworkManager().AddButton(this.gameObject);
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (GameListener.Instance().IsAnimation() || GameObject.Find("MenuWindow").transform.childCount > 0)
                    return;

                if (GameListener.Instance().GetGameNetworkManager() != null && GameListener.Instance().IsPlayerControl())
                {
                    GameObject window = this.gameObject.transform.parent.gameObject;
                    window.GetComponent<StateWindow>().Notify(this.gameObject, this.gameObject.name);
                    GameListener.Instance().GetGameNetworkManager().SendClickObject(this.gameObject);
                }
            }
            base.OnPointerDown(eventData);
        }
        public override void Click()
        {
            GameObject window = this.gameObject.transform.parent.gameObject;
            window.GetComponent<StateWindow>().Notify(this.gameObject, this.gameObject.name);
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

