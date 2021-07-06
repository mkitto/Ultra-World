using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;

namespace Managers
{
    public class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();

        public event OnEquipChangeHandler OnEquipChanged;

        public Item[] Equips = new Item[(int) EquipSlot.SlotMax];

        byte[] Data;

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }

        public bool Contains(int equipId)
        {
            for (int i = 0; i < this.Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipId)
                    return true;
            }

            return false;
        }

        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int) slot];
        }

        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                    int itemId = *(int*) (pt + i * sizeof(int));
                    if (itemId > 0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else
                        Equips[i] = null;

                }
            }
        }

        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemId = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemId = 0;
                    else
                        *itemId = Equips[i].Id;
                }
            }

            return this.Data;

        }


        public void EquipItem(Item equip)
        {
            //发送穿装备请求
            ItemService.Instance.SendEquipItem(equip, true);
        }

        public void UnEquipItem(Item equip)
        {
            //发送脱装备请求
            ItemService.Instance.SendEquipItem(equip, false);
        }
        //收到穿装备请求
        public void OnEquipItem(Item equip)
        {
            //检查格子是否为空或已经穿上
            if(this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
            {
                return;
            }
            //从道具系统中拿出，添入装备格子
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

            if (OnEquipChanged != null)
                //通知装备改变了，在UICHAREQUIP中的事件
                OnEquipChanged();
        }
        //收到脱装备请求
        internal void OnUnEquipItem(EquipSlot slot)
        {
            if (this.Equips[(int) slot] != null)
            {
                this.Equips[(int) slot] = null;
                if (OnEquipChanged != null)
                    //通知装备改变了，在UICHAREQUIP中的事件
                    OnEquipChanged();
            }
        }

    }
}
