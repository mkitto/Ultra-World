using System;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Common.Data;

public class UIQuestSystem : UIWindow
{
    public Text title;
    public GameObject itemPrefab;

    public TabView Tabs;
    public ListView listMain;
    public ListView listBranch;

    public UIQuestInfo questInfo;

    //是否显示可接任务
    private bool showAvailableList = false;

    private void Start()
    {
        //事件绑定
        //当被LISTMAIN被选择时，代表任务被选择
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.listBranch.onItemSelected += this.OnQuestSelected;
        //当TABS被选择时
        this.Tabs.OnTabSelect += OnSelectTab;
        //刷新UI
        RefreshUI();
        //QuestManager.Instance.OnQuestChanged+=RefreshUI;

    }
    //点击可接任务和进行中按钮时
    void OnSelectTab(int idx)
    {
        //是否显示可接任务，如果是1是可接，否则是进行中
        showAvailableList = idx == 1;
        RefreshUI();
    }

    private void OnDestroy()
    {
       // QueststManager.Instance.OnQuestChnagecd -= RefreshUI;
    }
    void RefreshUI()
    {
        //清除任务列表
        ClearAllQuestList();
        // 初始化任务列表
        InitAllQuestItems();
    }

    /// <summary>
    /// 初始化任务列表
    /// </summary>
    void InitAllQuestItems()
    {
        //从任务管理管理器中 抓取所有可用的任务
        foreach (var kv in QuestManager.Instance.allQuests)
        {
            //如果要显示可用的
            if (showAvailableList)
            {
                //如果已经接了任务跳过
                if (kv.Value.Info != null) continue;
            }
            else
            {
                //没接的话
                if (kv.Value.Info == null) continue;
            }
            //实例化时判断是主线任务还是支线，并放置在对应的位置
            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            //每个任务要设置下任务信息
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);
            //判断是主线任务还是支线任务
            if (kv.Value.Define.Type == QuestType.Main)
                //加入列表
                this.listMain.AddItem(ui);

            else
                this.listBranch.AddItem(ui);
        }
    }

    /// <summary>
    /// 清除任务列表
    /// </summary>
    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }
    public void OnQuestSelected(ListView.ListViewItem item)//任务面板根据item显示信息
    {
        if (item.owner == listMain)
        {
            if (this.listBranch.SelectedItem)
            {
                this.listBranch.SelectedItem.onSelected(false);
                RefreshUI();
            }
        }
        if (item.owner == listBranch)
        {
            if(this.listMain.SelectedItem)
            {
                this.listMain.SelectedItem.onSelected(false);
                RefreshUI();
            }
        }
        UIQuestItem questItem = item as UIQuestItem;
        //当任务被选择时，设置任务信息
        this.questInfo.SetQuestInfo(questItem.quest);
    }
}
