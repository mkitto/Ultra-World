using System.Collections;
using System.Collections.Generic;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour
{
    public Collider minimapBoundingBox;
    public Image minimap;
    public Image arrow;
    public Text mapName;

    private Transform playertTransform; //缓存一下 提升性能

    void Start()
    {
       this.InitMap();
    }

    void InitMap()
    {
        this.mapName.text = User.Instance.CurrentMapData.Name;
        if (this.minimap.overrideSprite != null)
            this.minimap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();

        this.minimap.SetNativeSize();
        this.minimap.transform.localPosition=Vector3.zero;
        //获取当前角色的位置
        playertTransform = User.Instance.CurrentCharacterObject.transform;
    }

    void Update()
    {
        //拿到地图的宽高
        float realWidth = minimapBoundingBox.bounds.size.x;
        float realHeight = minimapBoundingBox.bounds.size.z;

        //相对坐标 角色在地图中的位置
        float realX = playertTransform.position.x - minimapBoundingBox.bounds.min.x;
        float realY = playertTransform.position.z - minimapBoundingBox.bounds.min.z;

        //相对位置 进行转换得到中心位置
        float pivotX = realX / realWidth;
        float pivotY = realY / realHeight;

        this.minimap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.minimap.rectTransform.localPosition=Vector3.zero;
        //小地图指针跟着角色旋转
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playertTransform.eulerAngles.y);

    }
}
