using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        { 
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void SendBuyItem(int shopId, int ShopItemId)
        {
            Debug.Log("发送购买道具");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopId = shopId;
            message.Request.itemBuy.shopItemId = ShopItemId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show("购买结果：" + message.Result + "\n" + message.Errormsg, "购买完成");

            UIBag uiBag = GameObject.FindObjectOfType<UIBag>();
            if (uiBag != null)
            {
                Debug.Log("SetTitle");

                uiBag.SetTitle();
            }

        }

    }
}
