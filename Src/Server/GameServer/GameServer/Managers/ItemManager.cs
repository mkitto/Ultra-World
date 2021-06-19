using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ItemManager
    {
        //获取当前角色
        Character Owner;
        //创建字典，维护所有角色身上的道具
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        //传入角色，用于知道管理哪个角色的道具
        public ItemManager(Character owner)
        {
            this.Owner = owner;
            //遍历获得角色身上所有道具
            foreach(var item in owner.Data.Items)
            {
                //添加到字典中
                this.Items.Add(item.ItemID, new Item(item));
            }
        }
        //使用道具,count为每次使用数量
        public bool UseItem(int itemId, int count = 1)
        {
            Log.InfoFormat("[{0}]使用道具[{1}:{2}]",this.Owner.Data.ID,itemId,count);
            Item item = null;
            //判断道具是否存在，并且得到这个道具
            if(this.Items.TryGetValue(itemId,out item))
            {
                //判断道具数量是否足够
                if (item.Count < count)
                    return false;

                //T0DO:增加使用逻辑

                //使用后移除道具，只移除数量，并不移除道具存在，防止多次修改数据库
                item.Remove(count);
                return true;
            }
            return false;
        }
        //判断道具是否存在 
        public bool HasItem(int itemID)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemID,out item))
            {
                return item.Count > 0;
            }
            return false;
        }
        //获取道具
        public Item GetItem(int itemId)
        {
            Item item = null;
            this.Items.TryGetValue(itemId,out item);
            Log.InfoFormat("[{0}]获取道具[{1}:{2}]",this.Owner.Data.ID,itemId,item);
            return item;
        }

        public bool AddItem(int itemId, int count)
        {
            Item item = null;
            //先判断道具存不存在
            if (this.Items.TryGetValue(itemId,out item))
            {
                item.Add(count);
            }
            else
            {//不存在在DB中插入新数据
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.Data.ID;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);
                item = new Item(dbItem);
                this.Items.Add(itemId,item);
            }
            this.Owner.StatusManager.AddItemChange(itemId,count,StatusAction.Add);
            Log.InfoFormat("[{0}增加道具[{1}]]addCount:[2]",this.Owner.Data.ID,item,count);
            //DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int ItemId, int count)
        {
            if (!this.Items.ContainsKey(ItemId))
            {
                return false;
            }

            Item item = this.Items[ItemId];
            if (item.Count < count)
                return false;
            item.Remove(count);
            this.Owner.StatusManager.AddItemChange(ItemId,count,StatusAction.Delete);
            Log.InfoFormat("[{0}]移除道具[{1}]减少数量：{2}",this.Owner.Data.ID,item,count);
            //DBService.Instance.Save();
            return true;
        }
        //从内存数据转到网络数据中
        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach (var item in this.Items)
            {
                list.Add(new NItemInfo() {Id = item.Value.ItemID, Count = item.Value.Count});
            }
        }
    }
}
