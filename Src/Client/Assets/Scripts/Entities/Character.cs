using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public class Character : Entity
    {
        public NCharacterInfo Info;

        public Common.Data.CharacterDefine Define;  //表里面读出来的配置

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }

        public bool IsPlayer
        {
            //如果角色的ID和当前的Id相等说明是我本人 否则就是其他玩家
            get { return this.Info.Id == Models.User.Instance.CurrentCharacter.Id; }
        }

        public Character(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.Tid];
        }

        /// <summary>
        /// 向前移动
        /// </summary>
        public void MoveForward()
        {
            Debug.LogFormat("MoveForward");
            this.speed = this.Define.Speed;
        }
        /// <summary>
        /// 向后移动
        /// </summary>
        public void MoveBack()
        {
            Debug.LogFormat("MoveBack");
            this.speed = -this.Define.Speed;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            Debug.LogFormat("Stop");
            this.speed = 0;
        }
        /// <summary>
        /// 设置方向
        /// </summary>
        /// <param name="direction"></param>
        public void SetDirection(Vector3Int direction)
        {
            Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }
        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector3Int position)
        {
            Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }

    }
}
