using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// 元素节点定义
    /// </summary>
    class UIElement
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public string Resources;  
        //要不要Cache，如果要把Instance存下来
        public bool Cache;       
        public GameObject Instance;
    }

    /// <summary>
    /// 保存定义UI的信息
    /// </summary>
    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        //第一次用到就要初始化完成，所以防在构造函数里面
        UIResources.Add(typeof(UITest),new UIElement(){Resources = "UI/UITest",Cache = true});
        UIResources.Add(typeof(UIBag),new UIElement(){Resources = "UI/UIBag",Cache = true});
    }

    ~UIManager()
    {
        
    }

    /// <summary>
    /// UI
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Show<T>()
    {
        //SoundManager.Instance.PlaySound("ui_open");
        Type type = typeof(T);
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                //判断游戏UIResources里面是不是已经有了类型，如果有拿出来 实例有没有有，如果有直接激活 如果没有从资源里加载Prefab
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab ==null)
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }
    public void Close(Type type)
    {
        //SoundManager.Instance.PlaySound("ui_close");
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Cache)
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }
}
