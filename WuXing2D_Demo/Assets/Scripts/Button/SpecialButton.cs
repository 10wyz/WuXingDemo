using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingWindow;
using UnityEngine.UI;

namespace WuXingButton
{
    /// <summary>
    /// 特殊类型按钮（滚动条，下拉窗等）
    /// </summary>
    public class SpecialButton : MonoBehaviour
    {
        public void ChangeValue()
        {
            GameObject window = this.transform.parent.gameObject;
            window.GetComponent<StateWindow>().Notify(this.gameObject, this.gameObject.name);
        }
    }
}

