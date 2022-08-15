using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;

namespace WuXingTool
{

    public class PropertyTool
    {
        /// <summary>
        /// 五行相生，返回由其所生属性
        /// </summary>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public static WuXing WuXingGeneration(WuXing wuXing)
        {
            switch (wuXing)
            {
                case WuXing.Metal:
                    return WuXing.Water;
                case WuXing.Wood:
                    return WuXing.Fire;
                case WuXing.Water:
                    return WuXing.Wood;
                case WuXing.Fire:
                    return WuXing.Earth;
                case WuXing.Earth:
                    return WuXing.Metal;
                default: 
                    return WuXing.Chaos;
            }
        }

        /// <summary>
        /// 五行相克，返回被其克制属性
        /// </summary>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public static WuXing WuXingRestriction(WuXing wuXing)
        {
            switch (wuXing)
            {
                case WuXing.Metal:
                    return WuXing.Wood;
                case WuXing.Wood:
                    return WuXing.Earth;
                case WuXing.Water:
                    return WuXing.Fire;
                case WuXing.Fire:
                    return WuXing.Metal;
                case WuXing.Earth:
                    return WuXing.Water;
                default:
                    return WuXing.Chaos;
            }
        }

        /// <summary>
        /// 五行转字符串
        /// </summary>
        /// <param name="wuXing"></param>
        /// <returns></returns>
        public static string WuXingToString(WuXing wuXing)
        {
            switch (wuXing)
            {
                case WuXing.Metal:
                    return "金";
                case WuXing.Wood:
                    return "木";
                case WuXing.Water:
                    return "水";
                case WuXing.Fire:
                    return "火";
                case WuXing.Earth:
                    return "土";
                case WuXing.Epoch:
                    return "纪";
                case WuXing.Spell:
                    return "术";
                default:
                    return "混沌";
            }
        }

        /// <summary>
        /// 三才转字符串
        /// </summary>
        /// <param name="sanCai"></param>
        /// <returns></returns>
        public static string SanCaiToString(SanCai sanCai)
        {
            switch(sanCai)
            {
                case SanCai.Sky:
                    return "天";
                case SanCai.Earth:
                    return "地";
                case SanCai.Human:
                    return "人";
                default :
                    return "";
            }
        }
    }

}
