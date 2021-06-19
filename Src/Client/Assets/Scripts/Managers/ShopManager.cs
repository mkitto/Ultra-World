using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using Services;

namespace Managers
{
    class ShopManager:Singleton<ShopManager>
    {
        public void Init()
        {
            //对接NPC系统 注册npc点击事件 打开商店
            NPCManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop,OnOpenShop);
        }

        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;
        }
        public void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId, out shop))
            {
                //打开UI
                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if (uiShop != null)
                {
                    //把商店的信息设置给UI
                    uiShop.SetShop(shop);
                }
            }
        }
        public bool BuyItem(int shopId,int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }
    }
}
