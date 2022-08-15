using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingAssetManage;

namespace WuXingTool
{
    public class SundryCreator
    {
        public static GameObject CreateShield(GameObject parent)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Shield");
            GameObject shield = GameObject.Instantiate(module);
            shield.name = "Shield";
            shield.transform.SetParent(parent.transform, false);
            shield.transform.localPosition = new Vector3(0, 0, 0);

            return shield;
        }
        public static GameObject CreateSkillItem(GameObject parent, string name)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Skill/"+name);
            GameObject skillItem = GameObject.Instantiate(module);
            skillItem.GetComponent<Transform>().SetParent(parent.GetComponent<Transform>());
            skillItem.name = name;
            return skillItem;
        }
        public static GameObject CreateConditionItem(GameObject parent, string name)
        {
            GameObject module = ModuleWarehouse.Instance().GetModule("Condition/"+ name);
            GameObject conditionItem = GameObject.Instantiate(module);
            conditionItem.GetComponent<Transform>().SetParent(parent.GetComponent<Transform>());
            conditionItem.name = name;
            return conditionItem;
        }
    }
}

