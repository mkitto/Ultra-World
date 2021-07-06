using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemID, bool isEquip)
        {
            //获得角色
            Character character = sender.Session.Character;
            //判断装备是否存在
            if(!character.ItemManager.Items.ContainsKey(itemID))
                return Result.Failed;
            
            UpdateEquip(character.Data.Equips, slot, itemID, isEquip);
            DBService.Instance.Save();
            return Result.Success;
        }
        //更新装备，要在GAMESERVER属性--生成中勾选 允许不安全代码
        unsafe void UpdateEquip(byte[] equipData, int slot, int itemID, bool isEquip)
        {
            //声明指针指向角色身上装备数组
            fixed(byte* pt = equipData)
            {
                //当前指针+当前格子ID*每个槽子占的大小
                int* slotid = (int*)(pt + slot * sizeof(int));
                if (isEquip)
                    //穿装备
                    *slotid = itemID;
                else
                    //脱装备
                    *slotid = 0;
            }
        }
    }
}
