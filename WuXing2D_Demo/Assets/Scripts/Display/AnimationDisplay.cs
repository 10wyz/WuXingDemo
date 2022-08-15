using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingCore;

namespace WuXingDisplay
{
    public class AnimationDisplay : MonoBehaviour
    {
        protected GameStates m_state;
        
        public void Initial(GameStates state)
        {
            m_state = state;
        }
    }
}

