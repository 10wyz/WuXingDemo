using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WuXingBase;
using UnityEngine.UI;

namespace WuXingDisplay
{
    public class FieldDisplay : MonoBehaviour
    {
        public List<GameObject> m_playerFieldProperty;
        public List<GameObject> m_enemyFieldProperty;
        public GameObject m_playerSignal;
        public GameObject m_enemySignal;

        public void DisplayFieldProperty(SanCai sanCai, WuXing wuXing, PlayerType playerType)
        {
            if(playerType == PlayerType.Player)
            {
                if(wuXing == WuXing.Chaos)
                {
                    for(int i = 0; i < 5; i++)
                    {
                        int index = (int)sanCai * 5 + i;
                        m_playerFieldProperty[index].transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 100/255f);
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int index = (int)sanCai * 5 + i;
                        m_playerFieldProperty[index].transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 100 / 255f);
                    }
                    int k = (int)sanCai * 5 + (int)wuXing;
                    m_playerFieldProperty[k].transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                }
            }
            else
            {
                if (wuXing == WuXing.Chaos)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int index = (int)sanCai * 5 + i;
                        m_enemyFieldProperty[index].transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 100 / 255f);
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int index = (int)sanCai * 5 + i;
                        m_enemyFieldProperty[index].transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 100 / 255f);
                    }
                    int k = (int)sanCai * 5 + (int)wuXing;
                    m_enemyFieldProperty[k].transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                }
            }
        }
        public void DisplayControlPlayer(PlayerType playerType)
        {
            if(playerType == PlayerType.Player)
            {
                m_playerSignal.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                m_enemySignal.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);

                m_playerSignal.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(0, 0, 1, 100 / 255f);
                m_enemySignal.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 0, 0, 0);

            }
            else
            {
                m_playerSignal.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                m_enemySignal.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);

                m_playerSignal.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(0, 0, 1, 0);
                m_enemySignal.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 0, 0, 100 / 255f);

            }
        }
    }
}

