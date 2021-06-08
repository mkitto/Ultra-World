using System.Collections;
using System.Collections.Generic;
using Entities;
using Managers;
using Models;
using UnityEngine;

public class GameObjectManager : MonoSingleton<GameObjectManager>
{
    //利用字典来做存储
    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();

    //单例的Mono对象中不能写Start 必须重载
    protected override void OnStart()
    {
        //如果游戏里已经有其他人了 我是最后一个进入的 走协程
        StartCoroutine(InitGameObjects());
        //角色管理器 如果有角色进入 就收到一个通知
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;

    }


    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    //创建角色对象
    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
    }
	//移除角色对象
    void OnCharacterLeave(Character character)
    {
        if (!Characters.ContainsKey(character.entityId))
        {
            return;
        }
		//如果在其他地方删除了角色则不执行
        if (Characters[character.entityId]!=null)
        {
            Destroy(Characters[character.entityId]);
            Characters.Remove(character.entityId);
        }
    }


    //通过协程查找当前场景中所有玩家角色，并分别创建角色对象
    IEnumerator InitGameObjects()
    {
        foreach(var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    private void CreateCharacterObject(Character character)
    {
        //判断当前角色ID是否被创建过
        if(!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null)
        {
            //通过角色表中导入角色资源
            Object obj = Resloader.Load<Object>(character.Define.Resource);
            //如果角色资源不存在则返回
            if(obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.",character.Define.TID, character.Define.Resource);
                return;
            }

            GameObject go = (GameObject)Instantiate(obj,this.transform);
            go.name = "Character_" + character.Info.Id + "_" + character.Info.Name;
            Characters[character.entityId] = go;

            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform,character);
        }
        this.InitGameObject(Characters[character.entityId],character);
    }
    public void InitGameObject(GameObject go,Character character)
    {
        //把这些坐标转换成世界坐标
        go.transform.position = GameObjectTool.LogicToWorld(character.position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
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
            if (character.Info.Id == Models.User.Instance.CurrentCharacter.Id)
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


    }
}
