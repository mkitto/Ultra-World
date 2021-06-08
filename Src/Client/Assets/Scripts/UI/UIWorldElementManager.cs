using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

    public GameObject nameBarPrefab;

    //管理所有的世界元素
    private Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();

	// Use this for initialization
	protected override void OnStart()
	{
		nameBarPrefab.SetActive(false);
	}

	
	// Update is called once per frame
	void Update () {
		
	}


    public void AddCharacterNameBar(Transform owner, Character character)
    {
        //实例化对象 从上面取到绑定的脚本 给Owner跟随者赋值 血条知道角色 然后显示 加到管理器里面 owner是唯一的
        GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);
        goNameBar.name = "NameBar" + character.entityId;
        goNameBar.GetComponent<UIWorldElement>().owner = owner;
        goNameBar.GetComponent<UINameBar>().character = character;
        goNameBar.SetActive(true);
        this.elements[owner] = goNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        //移除之前 先判断一下是不是删过了 然后销毁掉
        if (this.elements.ContainsKey(owner))
        {
            Destroy(this.elements[owner]);
            this.elements.Remove(owner);
        }
    }
}
