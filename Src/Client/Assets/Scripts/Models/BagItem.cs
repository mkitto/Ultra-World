using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//背包物品结构体，为两个背包格子做交换用
namespace Models
{
    //属性，结构布局，结构体在内存中的存储格式
    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    struct BagItem
    {
        public ushort ItemId;
        public ushort Count;

        public static BagItem zero = new BagItem {ItemId =  0, Count = 0};

        public BagItem(int itemId, int count)
        {
            this.ItemId = (ushort) itemId;
            this.Count = (ushort) count;
        }

        public static bool operator ==(BagItem lhs, BagItem rhs)
        {
            return lhs.ItemId == rhs.ItemId && lhs.Count == rhs.Count;
        }

        public static bool operator !=(BagItem lhs, BagItem rhs)
        {
            return !(lhs == rhs);
        }
        /// <summary>
        /// 如果对象是相等的，返回true
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (other is BagItem)
            {
                return Equals((BagItem) other);
            }

            return false;
        }

        public bool Equals(BagItem other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ (Count.GetHashCode() << 2);
        }
    } 
}
