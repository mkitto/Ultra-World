using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Entities;

namespace GameServer.Managers
{
    /// <summary>
    /// 角色管理器
    /// </summary>
    class CharacterManager :Singleton<CharacterManager>
    {
        //使用字典管理 便于查询  角色进入时要加入 退出移除
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();


        public CharacterManager()
        {

        }

        public void Dispose()
        {

        }

        public void Init()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public Character AddCharacter(TCharacter cha)
        {
            //根据数据库当中的角色创建出实体
            //保证游戏服务器中始终都是在线的角色
            Character character = new Character(CharacterType.Player, cha);
            this.Characters[cha.ID] = character;
            return character;
        }

        public void RemoveCharacter(int characterId)
        {
            this.Characters.Remove(characterId);
        }
    }
}
