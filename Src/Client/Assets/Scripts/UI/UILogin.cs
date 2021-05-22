﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;

public class UILogin : MonoBehaviour
{
    public InputField username;
    public InputField password;
    public Button buttonLogin;
    public Button buttonRegister;

    void Start()
    {
        UserService.Instance.OnLogin = OnLogin;
    }
    void Update()
    {

    }

    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(this.username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }

        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }

        //进入游戏
        UserService.Instance.SendLogin(this.username.text,this.password.text);
    }

    void OnLogin(Result result,string message)
    {
        //如果结果等于成功
        if (result == Result.Success)
        {
            //登录成功，进入角色选择
            MessageBox.Show("登录成功,准备角色选择" + message,"提示", MessageBoxType.Information);
            SceneManager.Instance.LoadScene("CharSelect");

        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);

    }


}
