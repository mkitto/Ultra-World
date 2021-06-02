﻿using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour
{
    public Text avatarName;
    public Text avatarLeve1;


    // Start is called before the first frame update
    void Start()
    {
        this.UpdateAvatar();   
    }

    void UpdateAvatar()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name,
            User.Instance.CurrentCharacter.Id);
        this.avatarLeve1.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }
}
