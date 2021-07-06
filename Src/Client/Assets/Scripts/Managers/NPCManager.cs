using Common.Data;
using System.Collections.Generic;
using SkillBridge.Message;

namespace Managers
{
    class NPCManager:Singleton<NPCManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);//一个function对应一个委托 用委托来管理所有的事件

        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

        /// <summary>
        /// 注册事件
        /// </summary>
        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }
        }

        public NpcDefine GetNpcDefine(int npcId)
        {
            NpcDefine npc = null;
            DataManager.Instance.Npcs.TryGetValue(npcId, out npc);
            return npc;
        }

        public bool Interactive(int npcId)
        {
            //任何一个NPC想做交互的时候先判断这个NPC是否存在 如果存在执行交互
            if (DataManager.Instance.Npcs.ContainsKey(npcId))
            {
                var npc = DataManager.Instance.Npcs[npcId];
                return Interactive(npc);
            }

            return false;
        }

        //交互 结构参数
        public bool Interactive(NpcDefine npc)
        {
            //判断NPC类型，走对应的交互功能
            if (npc.Type == NpcType.Task)
            {
                return DoTaskInteractive(npc);
            }
            else if (npc.Type==NpcType.Functional)
            {
                return DoFuntionInteractive(npc);
            }

            return false;
        }

        /// <summary>
        /// 任务交互
        /// </summary>
        private bool DoTaskInteractive(NpcDefine npc)
        {
            MessageBox.Show("点击了NPC" + npc.Name, "NPC对话");
            return true;
        }
        /// <summary>
        /// 功能交互
        /// </summary>
        private bool DoFuntionInteractive(NpcDefine npc)
        {
            //查询事件表中是不是存在 如果存在直接传入表里的Function
            if (npc.Type != NpcType.Functional)
            {
                return false;
            }

            if (!eventMap.ContainsKey(npc.Function))
            {
                return false;
            }

            return eventMap[npc.Function](npc);
        }
        public void OnUnEquipItem(EquipSlot slot)
        {

        } 
    }
}