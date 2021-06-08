using System;
using Network;
using UnityEngine;
using Common.Data;
using Managers;
using SkillBridge.Message;
using Models;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        /// <summary>
        /// 当前地图ID
        /// </summary>
        public int CurrentMapId = 0;

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }

        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
            
            //遍历角色列表 将身边的玩家都填充进来
            foreach (var cha in response.Characters)
            {
                //判断当前角色的ID和列表的Id是不是一样的  再赋值相当于刷新一下本地数据
                if (User.Instance.CurrentCharacter == null || User.Instance.CurrentCharacter.Id == cha.Id)
                {
                    //当前角色切换地图
                    User.Instance.CurrentCharacter = cha;
                }
                //将角色加进角色管理器
                CharacterManager.Instance.AddCharacter(cha);
            }
            //判断是不是第一次进入地图 做是不是切换地图的判断
            if (CurrentMapId!=response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;

            }

        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave: CharID:{0}",response.characterId);
            if(response.characterId!=User.Instance.CurrentCharacter.Id)
                CharacterManager.Instance.RemoveCharacter(response.characterId);
            else
                CharacterManager.Instance.Clear();

        }

        /// <summary>
        /// 进入地图
        /// </summary>
        /// <param name="mapId"></param>
        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                //从表里面读出来要进入哪一个地图
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else
                Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
        }

        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity)
        {
            //Debug.LogFormat("MapEntitySyncResponse :ID:{0} 位置:{1} 方向:{2} 速度:{3}",entity.Id,entity.Position.String(),entity.Direction.ToString(),entity.Speed);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity
            };
            NetClient.Instance.SendMessage(message);

        }
        /// <summary>
        /// 同步地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            //添加到字符串里 一次性输出出来
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse:Entitys:{0}",response.entitySyncs.Count);
            sb.AppendLine();

            foreach (var entity in response.entitySyncs)
            {
                //收到Entity响应
                EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("  [{0}]evt:{1} entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }
            //Debug.Log(sb.ToString());
        }
        /// <summary>
        /// 发送传送
        /// </summary>
        /// <param name="iD"></param>
        internal void SendMapTeleport(int teleporterID)
        {
            Debug.LogFormat("MapTeleportRequst :telrportID:{0}",teleporterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterID;
            NetClient.Instance.SendMessage(message);
        }


    }
}