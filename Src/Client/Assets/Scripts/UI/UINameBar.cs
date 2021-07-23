using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;
using UnityEngine.UI;

//信息条
public class UINameBar : MonoBehaviour
{
    public Text avaverName;


    public Character character;


    // Start is called before the first frame update
    void Start()
    {
        if (this.character!=null)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateInfo();
 
        //this.transform.forward = Camera.main.transform.forward;
        //this.transform.LookAt(Camera.main.transform,Vector3.up);

    }

    void UpdateInfo()
    {
        if (this.character != null)
        {
            //角色的名字和等级
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            //减少Update 引发重绘 为了性能
            if (name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }
        }

    }
}
