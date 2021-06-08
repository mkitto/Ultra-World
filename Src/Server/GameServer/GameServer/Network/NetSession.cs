using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    /// <summary>
    /// 网络会话
    /// </summary>
    class NetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }

        internal void Disconnected()
        {
            if(Character!=null)
                UserService.Instance.CharacterLeave(Character);
        }
    }
}
