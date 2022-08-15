using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

namespace WuXingBase
{
    public class SkillInfoNode
    {
        public string m_nodeName;
        public string m_nodeValue;
        public List<SkillInfoNode> m_childNode;


        public SkillInfoNode(XmlElement element)
        {
            if(element.ChildNodes.Count == 0)
            {
                m_nodeName = element.Name;
                m_nodeValue = element.GetAttribute("value");
                m_childNode = new List<SkillInfoNode>();
            }
            else
            {
                m_nodeName = element.Name;
                m_nodeValue = element.GetAttribute("value");
                m_childNode = new List<SkillInfoNode>();
                foreach(XmlElement node in element.ChildNodes)
                {
                    m_childNode.Add(new SkillInfoNode(node));
                }
            }
        }
    }


    public class SkillInfo
    {
        private List<SkillInfoNode> m_skillNode;

        public SkillInfo(XmlElement element)
        {
            m_skillNode = new List<SkillInfoNode>();
            XmlNodeList xmlNodeList = element.ChildNodes;
            
            foreach (XmlElement node in element.ChildNodes)
            {
                m_skillNode.Add(new SkillInfoNode(node));
            }
        }


        public int GetChildNodeNum()
        {
            return m_skillNode.Count;
        }
        public SkillInfoNode GetChildNode(int index)
        {
            return m_skillNode[index];
        }
    }



}

