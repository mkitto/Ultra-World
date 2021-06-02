﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using SkillBridge.Message;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChanged(Entity entity);
        void OnEntityEvent(EntityEvent @event);
    }

    class EntityManager:Singleton<EntityManager>
    {
        //维护客户端本地
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        Dictionary<int, IEntityNotify> notifies = new Dictionary<int, IEntityNotify>();  

        //用接口来实现一个事件
        //一个接收者，可以接收多种事件

        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            this.notifies[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity entity)
        {
            this.entities.Remove(entity.Id);
            if (notifies.ContainsKey(entity.Id))
            {
                notifies[entity.Id].OnEntityRemoved();
                notifies.Remove(entity.Id);
            }

        }

        internal void OnEntitySync(NEntitySync data)
        {
            Entity entity = null;
            entities.TryGetValue(data.Id, out entity);
            if (entity != null)
            {
                if (data.Entity != null)
                    entity.EntityData = data.Entity;
                if (notifies.ContainsKey(data.Id))
                {
                    notifies[entity.entityId].OnEntityChanged(entity);
                    notifies[entity.entityId].OnEntityEvent(data.Event);
                }
            }
        }
    }
}
