using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Models;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        Dictionary<int, Map> Maps = new Dictionary<int, Map>();

        public void Init()
        {
            //遍历配置表获取地图Model
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapdefine);
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.Define.ID, map.Define.Name);
                this.Maps[mapdefine.ID] = map;
            }
        }



        public Map this[int key]
        {
            get
            {
                return this.Maps[key];
            }
        }

        //update 便于地图周期性的刷新怪
        //地图管理器存在自主服务
        public void Update()
        {
            foreach(var map in this.Maps.Values)
            {
                map.Update();
            }
        }
    }
}
