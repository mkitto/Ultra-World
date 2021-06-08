using Common.Data;
using System.Collections.Generic;

namespace Manager
{
    class NPCManager:Singleton<NPCManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);

        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

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

        public bool Interactive(NpcDefine npc)
        {
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

        private bool DoTaskInteractive(NpcDefine npc)
        {
            MessageBox.Show("点击了NPC" + npc.Name, "NPC对话");
            return true;
        }

        private bool DoFuntionInteractive(NpcDefine npc)
        {
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
    }
}