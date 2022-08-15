using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WuXingBase;
using WuXingTool;
using UnityEngine.UI;
using System;

namespace WuXingDisplay
{
    public class ShieldDisplay : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            GameObject gameObject = this.transform.parent.gameObject;
            this.transform.parent.GetComponent<Card>().OnPointerDown(eventData);
        }

        public void ShowShiled(Shield shield)
        {
            int num = shield.GetNum();

            if(num == 0)
            {
                GameObject.Destroy(this.gameObject);
                return;
            }    

            WuXing wuXing = shield.GetShieldProperty();

            GameObject image = this.transform.GetChild(0).gameObject;
            GameObject text = this.transform.GetChild(1).gameObject;

            string Path = "Picture/Other/" + PropertyTool.WuXingToString(wuXing) + "盾";
            Sprite sp = Resources.Load<Sprite>(Path);
            image.GetComponent<Image>().sprite = sp;

            text.GetComponent<Text>().text = Convert.ToString(num);
        }
    }
}

