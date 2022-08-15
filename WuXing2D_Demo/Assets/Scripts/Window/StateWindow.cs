using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WuXingWindow
{
    public class StateWindow : MonoBehaviour
    {
        virtual public void Notify(GameObject child, string eventID)
        {

        }
        virtual public List<GameObject> GetCard()
        {
            return new List<GameObject>();
        }
        virtual public bool IsModalWindow()
        {
            return true;
        }
        virtual public void ReEntryWindow()
        {

        }

    }
}

