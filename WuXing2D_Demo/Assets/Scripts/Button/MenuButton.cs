using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WuXingWindow;
using WuXingCore;

namespace WuXingButton
{
    /// <summary>
    /// 菜单按钮（不发送按键事件）
    /// </summary>
    public class MenuButton : ButtonBase
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                GameObject window = this.gameObject.transform.parent.gameObject;
                window.GetComponent<StateWindow>().Notify(this.gameObject, this.gameObject.name);
            }
            base.OnPointerDown(eventData);
        }
    }
}

