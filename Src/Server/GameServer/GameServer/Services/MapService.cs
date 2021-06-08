using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapEntitySync: characterID:{0}:{1} Entity.Id:{2} Evt:{3} Entity:{4}",character.Id,character.Info.Name,request.entitySync.Id,request.entitySync.Event,request.entitySync.Entity.String());
            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }

        internal void SendEntityUpdate(NetConnection<NetSession> connection, NEntitySync entity)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entity);

            byte[] data = PackageHandler.PackMessage(message);
            connection.SendData(data,0,data.Length);
        }

        void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            //当前谁在请求传送
            Character character = sender.Session.Character;
            Log.InfoFormat("ON地图传送: 角色ID:{0}:{1} 传送点ID:{2}", character.Id, character.Data.Name, request.teleporterId);
            //校验
            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("进入的传送点{0} 不存在", request.teleporterId);
                return;
            }
            //如果传送点存在，读取数据 判断表里面的连接点是不是对的
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];
            if (source.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {
                Log.WarningFormat("进入的传送点ID{0} 链接ID{1} 不存在", request.teleporterId, source.LinkTo);
            }
            //根据LInkTo拉取传送目标
            TeleporterDefine target = DataManager.Instance.Teleporters[source.LinkTo];
            //玩家先离开老地图 并设置一下位置 玩家进入新地图
            MapManager.Instance[source.MapID].CharacterLeave(character);
            character.Position = target.Position;
            character.Direction = target.Direction;
            MapManager.Instance[target.MapID].CharacterEnter(sender, character);
        }

    }
}
