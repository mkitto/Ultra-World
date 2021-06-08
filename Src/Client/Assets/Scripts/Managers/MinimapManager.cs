using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using UnityEngine;

namespace Managers
{
    class MinimapManager:Singleton<MinimapManager>
    {
        public UIMiniMap minimap;  //告诉管理器minimap是谁

        private Collider minimapBoundingBox;
        public Collider MinimapBoundingBox
        {
            get { return minimapBoundingBox; }
        }


        
        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                {
                    return null;               
                }
                return User.Instance.CurrentCharacterObject.transform;
            }
        }

        /// <summary>
        /// 加载当前的消息图
        /// </summary>
        /// <returns></returns>
        public Sprite LoadCurrentMinimap()
        {
            return Resloader.Load<Sprite>("UI/MiniMap/" + User.Instance.CurrentMapData.MiniMap);
        }

        //更新MiniMap
        public void UpdateMinimap(Collider minimapBoundingBox)
        {
            //管理器需要知道要更新地图了 再告诉小地图，地图需要更新了
            this.minimapBoundingBox = minimapBoundingBox;
            if (minimap!=null)
            {
                minimap.UpdateMap();
            }
        }
    }
}
