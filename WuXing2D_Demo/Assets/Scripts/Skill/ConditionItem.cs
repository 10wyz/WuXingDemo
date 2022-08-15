using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using WuXingCore;

namespace WuXingSkill
{
    public class ConditionItem : MonoBehaviour
    {
        virtual public void Initial(SkillInfoNode skillInfoNode, PlayerType playerType)
        {
        }

        virtual public bool IsMatch(GameObject origin, GameObject trigger, GameObject target, TriggerType triggerType)
        {
            return true;
        }
    }
}

