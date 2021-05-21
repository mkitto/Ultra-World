using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour
{
    //三个角色的数组
    public GameObject[] characters;

    //当前角色
    private int currentCharacter = 0;
    

    public int CurrentCharacter
    {
        get
        {
            return currentCharacter;
        }
        set
        {
            //更新节点显示
            currentCharacter = value;
            this.UpdateCharacter();
        }
    }

    void UpdateCharacter()
    {
        //遍历 获取当前是哪个角色的显示
        for (int i = 0; i < 3; i++)
        {
            characters[i].SetActive(i==this.currentCharacter);
        }
    }
}
