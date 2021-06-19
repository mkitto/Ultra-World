using System.Collections;
using System.Collections.Generic;
using Common.Data;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商店UI
/// </summary>
public class UIShop : UIWindow
{
    public Text title;

    public Text money;

    public GameObject ShopItem;
    
    ShopDefine shop;

    public Transform[] itemRoot;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitItems());
    }

    /// <summary>
    /// 初始化商店
    /// </summary>
    /// <returns></returns>
    IEnumerator InitItems()
    {
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                //拉出当前商店的所有物品
                GameObject go = Instantiate(ShopItem, itemRoot[0]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);
            }
        }

        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    public void SetMoney()
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    private UIShopItem selectedItem;

    public void SelectShopItem(UIShopItem item)
    {
        if (selectedItem != null)
        {
            selectedItem.Selected = false;
        }

        selectedItem = item;
    }

    public void OnClickBuy()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }
        if (!ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.ShopItemID))
        {

        }
        else
        {
            this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
            UIBag uiBag = GameObject.FindObjectOfType<UIBag>();
            if (uiBag != null)
            {
                uiBag.SetTitle();
            }
        }

    }
}
