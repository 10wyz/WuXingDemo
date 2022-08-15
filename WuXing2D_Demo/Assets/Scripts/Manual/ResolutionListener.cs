using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingManual;
using WuXingAssetManage;
using UnityEngine.UI;

public class ResolutionListener : UnityEngine.EventSystems.UIBehaviour
{
    protected override void OnRectTransformDimensionsChange()
    {
        if(Screen.width / 1400f > Screen.height / 800f)
            this.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        else
            this.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;

        base.OnRectTransformDimensionsChange();

        //SizeControl[] list = this.transform.GetComponentsInChildren<SizeControl>();

        //for (int i = 0; i < list.Length; i++)
        //{
        //    list[i].Resize();
        //}
    }
}
