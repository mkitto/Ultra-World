using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    //封装
    class Item
    {
        TCharacterItem dbItem;
        //道具ID
        public int ItemID;
        //道具数量
        public int Count;
        //道具初始化
        public Item(TCharacterItem item)
        {
            this.dbItem = item;
            this.ItemID = (short)item.ItemID;
            this.Count = (short)item.ItemCount;
        }
        //添加道具方法
        public void Add(int count)
        {
            this.Count += count;
            dbItem.ItemCount = this.Count;
        }
        //删除道具方法
        public void Remove(int count)
        {
            this.Count -= count;
            dbItem.ItemCount = this.Count;
        }
        //道具使用
        public bool Use(int count = 1)
        {

            return false;
        }
        //简化输出
        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.ItemID, this.Count);
        }
    }
}
