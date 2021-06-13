using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    /// <summary>
    /// 背包，要去USERSERVICE中为角色创建背包
    /// </summary>
    class BagService :Singleton<BagService>
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(this.OnBagSave);
        }

        public void Init()
        {
        }
        /// <summary>
        /// 背包的保存
        /// </summary>
        void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("背包保存请求：角色：{0}：Unlocked{1}",character.Id,request.BagInfo.Unlocked);

            if (request.BagInfo != null)
            {
                //把协议里发送过来的背包赋值给角色的背包 
                character.Data.Bag.Items = request.BagInfo.Items;
                DBService.Instance.Save();
            }
        }
    }
}
