using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WuXingAssetManage
{
    public class ModuleWarehouse
    {
        private static ModuleWarehouse instance;
        public static ModuleWarehouse Instance()
        {
            if (instance == null)
                instance = new ModuleWarehouse();
            return instance;
        }


        private Dictionary<string, GameObject> m_modules;

        private ModuleWarehouse()
        {
            m_modules = new Dictionary<string, GameObject>();
        }

        /// <summary>
        /// 加载预制件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool LoadModule(string name)
        {
            string Path = "Prefabs/" + name;
            GameObject module = Resources.Load<GameObject>(Path);
            m_modules[name] = module;
            return true;
        }

        /// <summary>
        /// 获取预制件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject GetModule(string name)
        {
            if(m_modules.ContainsKey(name))
            {
                return m_modules[name];
            }
            else
            {
                if (LoadModule(name))
                    return m_modules[name];
                else
                    return null;
            }

        }
    }
}

