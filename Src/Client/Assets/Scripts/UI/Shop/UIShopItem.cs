using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

 //要在游戏物体上加入Selectable组件，并调用接口
public class UIShopItem : MonoBehaviour,ISelectHandler
{
    public Image icon;

    public Text title;

    public Text price;

    public Text count;

    public Image background;

    public Sprite normalBg;
    public Sprite selectedBg;

    private  bool selected;
    public int ShopItemID { get; set; }
    private UIShop shop;

    private ItemDefine item;
    private ShopItemDefine ShopItem { get; set; }

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            this.background.overrideSprite = selected ? selectedBg : normalBg;
        }
    }

    internal void SetShopItem(int id, ShopItemDefine shopItem, UIShop owner)
    {
        this.shop = owner;
        this.ShopItemID = id;
        this.ShopItem = shopItem;
        this.item = DataManager.Instance.Items[this.ShopItem.ItemID];

        this.title.text = this.item.Name;
        this.count.text = ShopItem.Count.ToString();
        this.price.text = ShopItem.Price.ToString();
        this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
    }
    //实现接口，当鼠标选中时调用该方法
    public void OnSelect(BaseEventData eventData)
    {
        //标记为被选中状态
        this.Selected = true;
        //调用商店，告诉商店选中道具
        this.shop.SelectShopItem(this);
    }

}
