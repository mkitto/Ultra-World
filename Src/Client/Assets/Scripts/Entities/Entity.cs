using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;

namespace Entities
{

    public class Entity
    {

        /// <summary>
        /// ID
        /// </summary>
        public int entityId;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3Int position;
        /// <summary>
        /// 方向
        /// </summary>
        public Vector3Int direction;
        /// <summary>
        /// 速度
        /// </summary>
        public int speed;


        private NEntity entityData;  //服务器上同步到客户端的信息  存数据
        public NEntity EntityData
        {
            get {
                return entityData;
            }
            set {
                entityData = value;
                this.SetEntityData(value);
            }
        }

        public Entity(NEntity entity)
        {
            this.entityId = entity.Id;
            this.entityData = entity;
            this.SetEntityData(entity);

        }

        public virtual void OnUpdate(float delta)
        {
            if (this.speed!=0)
            {
                //方向*速度
                //只要当前速度不为0就朝当前方向移动
                Vector3 dir = this.direction;
                this.position += Vector3Int.RoundToInt(dir * speed * delta / 100f);
            }
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
            entityData.Speed = this.speed;

        }

        public void SetEntityData(NEntity entity)
        {
            //把网络转换成本地位置  FromNVector3自己定义的方法实现类型转换
            this.position = this.position.FromNVector3(entity.Position);
            this.direction = this.direction.FromNVector3(entity.Direction);
            this.speed = entity.Speed;
        }

    }
}