using Common.Data;
using SkillBridge.Message;

namespace Models
{
    public class Item
    {
        public int Id;
        public int Count;
        public ItemDefine Define;
        public EquipDefine EquipInfo;
        public Item(NItemInfo item):this(item.Id,item.Count){}
        public Item(int id ,int count)
        {
            this.Id = id;
            this.Count = count;
            //this.Define = DataManager.Instance.Items[this.Id];
            //当道具创建的时候，同时加载道具信息和装备信息
            DataManager.Instance.Items.TryGetValue(this.Id, out this.Define);
            DataManager.Instance.Equips.TryGetValue(this.Id, out this.EquipInfo);
        }

        public override string ToString()
        {
            return string.Format("Id:{0},Count:{1}", this.Id, this.Count);
        }
    }
}