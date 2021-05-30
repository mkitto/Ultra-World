using System.Collections;
using System.Collections.Generic;
using Entities;
using Managers;
using Models;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    //利用字典来做存储
    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //如果游戏里已经有其他人了 我是最后一个进入的 走协程
        StartCoroutine(InitGameObjects());
        //角色管理器 如果有角色进入 就收到一个通知
        CharacterManager.Instance.OnCharacterEnter = OnCharacterEnter;
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter = null;

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
    }

    IEnumerator InitGameObjects()
    {
        //查找一下场景中的所有角色
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            //然后分别对每个角色来创建角色对象
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    //创建单个角色
    private void CreateCharacterObject(Character character)
    {
        //判断角色的Id是不是创建过了  是不是等于空
        if (!Characters.ContainsKey(character.Info.Id)||Characters[character.Info.Id]==null)
        {
            //从资源加载 把资源加载出来 判断是不是空的 实例化
            Object obj = Resloader.Load<Object>(character.Define.Resource);
            if (obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                return;
            }

            GameObject go = (GameObject)Instantiate(obj);
            go.name = "Character_" + character.Info.Id + "_" + character.Info.Name;

            //把这些坐标转换成世界坐标
            go.transform.position = GameObjectTool.LogicToWorld(character.position);
            go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
            Characters[character.Info.Id] = go;

            //从脚本中取到组件 分别赋值
            EntityController ec = go.GetComponent<EntityController>();
            if (ec != null)
            {
                ec.entity = character;
                ec.isPlayer = character.IsPlayer;
            }

            PlayerInputController pc = go.GetComponent<PlayerInputController>();
            if (pc != null)
            {
                if (character.Info.Id==Models.User.Instance.CurrentCharacter.Id)
                {
                    User.Instance.CurrentCharacterObject = go;
                    MainPlayerCamera.Instance.player = go;
                    pc.enabled = true;
                    pc.character = character;
                    pc.entityController = ec;
                }
                else
                {
                    pc.enabled = false;
                }
            }
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform,character);
        }
    }
}
