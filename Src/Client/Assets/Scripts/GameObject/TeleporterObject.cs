using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 地图传送脚本
/// </summary>

public class TeleporterObject : MonoBehaviour
{
    //传送点ID
    public int ID;
    Mesh mesh = null;


    void Start()
    {
        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (this.mesh != null)
        {
            // Gizmos.DrawWireMesh(this.mesh, this.transform.position + Vector3.up * this.transform.localScale.y * .5f,
            //this.transform.rotation, this.transform.localScale);
            //让该物体在编辑器模式显示线框
            Gizmos.DrawWireMesh(mesh, transform.position + Vector3.up * transform.localScale.y * 0f, transform.rotation, transform.localScale);
        }        

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0,this.transform.position,this.transform.rotation,1f,EventType.Repaint);
    }
#endif


    void OnTriggerEnter(Collider other)
    {

        Debug.LogWarning(other.gameObject.name);
        PlayerInputController playerController = other.GetComponent<PlayerInputController>();

        //判断传入对象是否带有角色控制器，并且处于激活开启状态
        if (playerController != null && playerController.isActiveAndEnabled)
        {
            //拉取配置表 获得数据
            TeleporterDefine td = DataManager.Instance.Teleporters[this.ID];
            if (td == null)
            {
                Debug.LogErrorFormat("TeleporterObject: Character [{0}] Enter Teleporter [{1}], But TeleporterDefine not existed",playerController.character.Info.Name,this.ID);
                return;
            }
            Debug.LogFormat("TeleporterObject: Character [{0}] Enter Teleporter [{1}:{2}]",playerController.character.Info.Name,td.ID,td.Name);
            if (td.LinkTo > 0)
            {
                //先检查有效性，告诉地图做传送 判断目标LinkToID是不是存在的
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                    MapService.Instance.SendMapTeleport(this.ID);
                else
                    Debug.LogErrorFormat("Teleporter ID:{0} LinkID {1} error!",td.ID,td.LinkTo);
            }
        }
    }

}
