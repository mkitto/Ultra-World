using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包UI
/// </summary>

public class UIBag : UIWindow
{
    //金钱数量
    public Text money;
    //页数
    public Transform[] pages;
    //格子图标及文本
    public GameObject bagItem;
    //槽，代表空格子
    List<Image> slots;

    void Start()
    {
        if(slots == null)
        {
            //新建一个格子列表
            slots = new List<Image>();
            //遍历有几页背包
            for(int page = 0; page < this.pages.Length; page++)
            {
                //计算每页有几个格子
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        //初始化背包
        StartCoroutine(InitBags());
    }

    IEnumerator InitBags()
    {
        for (int i=0;i<BagManager.Instance.Items.Length;i++)
        {
            var Item = BagManager.Instance.Items[i];
            if (Item.ItemId>0)
            {
                GameObject go = Instantiate(bagItem,slots[i].transform);
                var ui = go.GetComponent<UIIconItem>();
                var def = ItemManager.Instance.Items[Item.ItemId].Define;
                ui.SetMainIcon(def.Icon, Item.Count.ToString());
            }
        }
        for (int i=BagManager.Instance.Items.Length;i<slots.Count;i++)
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }

    public void SetTitle()
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)
            {
                Destroy(slots[i].transform.GetChild(0).gameObject);
            }
        }
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
        this.Clear();
        StartCoroutine(InitBags());
    }
}
