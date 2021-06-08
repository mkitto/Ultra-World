using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera camera;
    public Transform viewPoint;

    public GameObject player;
	

    private void LateUpdate()
    {
        //修正 如果角色还不存在，重新赋值
        if (player==null)
        {
            player = User.Instance.CurrentCharacterObject;
        }
        //如果玩家不等于空 将当前的位置等于玩家的位置 摄像机会跟着角色走
        if (player == null)
        {
            Debug.Log(" User.Instance.CurrentCharacterObject为空");
            return;
        }

        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
