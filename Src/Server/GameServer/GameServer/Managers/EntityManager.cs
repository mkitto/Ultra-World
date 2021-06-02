using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;

namespace GameServer.Managers
{
    class EntityManager:Singleton<EntityManager>
    {
        private int idx = 0;
        //维护所有Entities的列表
        public List<Entity> AllEntities = new List<Entity>();
        //区分底图Entities是那些
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();


        public void AddEntity(int mapId, Entity entity)
        {
            AllEntities.Add(entity);
            //加入管理器唯一ID
            //总列表的索引作为Entities的Id 只有一个entity加入管理器 id才生成
            entity.EntityData.Id = ++this.idx;

            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapId, out entities))
            {//判断当前是那张底图 列表上不存在创建一个新的赋值进去如果已经存在就直接加进去
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
        }
        public void RemoveEntity(int mapId, Entity entity)
        {
            this.AllEntities.Remove(entity);
            this.MapEntities[mapId].Remove(entity);
        } 
    }
}
