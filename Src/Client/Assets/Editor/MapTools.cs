using System.Collections;
using System.Collections.Generic;
using Common.Data;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 编辑器扩展，保存传送点位置信息至配置表
/// </summary>

public class MapTool
{
    //定义一个菜单项
    [MenuItem("Map Tools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();
        //记录当前场景的地图
        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }

        List<TeleporterObject> allTeleporters = new List<TeleporterObject>();
        //遍历地图，生成地图原始路劲
        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} 不存在！", sceneFile);
                continue;
            }
            //打开场景
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);
            //找到地图中的所有传送点 遍历每一个传送点
            TeleporterObject[] teleporters = GameObject.FindObjectsOfType<TeleporterObject>();
            foreach (var teleporter in teleporters)
            {//检查传送点中配置的Id在表中存不存在
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图:{0} 中配置的传送点:{1}不存在", map.Value.Resource, teleporter.ID), "确定");
                    return;
                }
                //Mapid对不对
                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if (def.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图:{0} 中配置的传送点:{1} 地图ID{2}错误", map.Value.Resource, teleporter.ID, def.MapID), "确定");
                    return;
                }
                //把地图传送点转换成配置表坐标
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }
        //保存配置文件
        DataManager.Instance.SaveTeleporters();
        //恢复最早打开的场景
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");
    }
}
