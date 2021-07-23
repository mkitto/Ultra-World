using Common.Data;
using Managers;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour, IPointerClickHandler//接口，指针点击处理器
{
    public Image icon;
    public Text title;
    public Text level;
    public Text limitClass;
    public Text limitCategory;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    //设置选中背景 
    private bool selected;

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            this.background.overrideSprite = selected ? selectedBg : normalBg;
        }
    }
    //索引
    public int index { get; set; }

    private UICharEquip owner;

    private Item item;

    void Start()
    {

    }

    bool isEquiped = false;

    public void SetEquipItem(int idx, Item item, UICharEquip owner, bool equiped)
    {
        this.owner = owner;
        this.index = idx;
        this.item = item;
        this.isEquiped = equiped;
       

        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) this.level.text = item.Define.Level.ToString();
        if (this.limitClass != null) this.limitClass.text = item.Define.LimitClass.ToString();
        if (this.limitCategory != null) this.limitCategory.text = item.Define.Category;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
      
    }
    //鼠标点击接口方法
    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isEquiped)
            //脱掉装备
            UnEquip();
        //如果是左面的没有装备上的列表
        else
        {
            //判断是否选中
            //再次点击时是选中状态，执行，代表双击穿装备
            if (this.selected)
            {
                //穿装备
                DoEquip();
                //关闭选中状态
                this.Selected = false;
            }
            //没有被选中时设置成选中状态
            else
                this.Selected = true;


        }
    }
    //穿装备
    private void DoEquip()
    {
        //是否穿当前装备
        var msg = MessageBox.Show(string.Format("要装备[{1}]吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        //点击确认的话
        msg.OnYes = () =>
        {
            //获取当前位置装备
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            //如果有装备
            if (oldEquip != null)
            {
                //询问是否替换    
                var newmsg = MessageBox.Show(string.Format("要替换掉[{0}]吗？", oldEquip.Define.Name), "确认", MessageBoxType.Confirm);
                //确认替换
                newmsg.OnYes = () =>
                {
                    this.owner.DoEquip(this.item);
                };

            }
            //没有装备的话直接穿装备
            else
            {
                this.owner.DoEquip(this.item);
            }
        };
    }  
    //脱装备
    private void UnEquip()
    {
        //弹出提示，MSG为返回值
        var msg = MessageBox.Show(string.Format("要取下装备[{0}]吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        //OnYes这种写法代表点击YES执行
        msg.OnYes = () =>
        {
            //调用UICharEquip中的UnEquip方法
            this.owner.UnEquip(this.item);
        };
    }


}

