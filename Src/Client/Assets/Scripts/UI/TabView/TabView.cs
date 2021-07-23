using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour
{

    public TabButton[] tabButtons;
    public GameObject[] tabPages;

    public UnityAction<int> OnTabSelect;

    public int index = -1;
    // Use this for initialization
    IEnumerator Start()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;
            tabButtons[i].tabIndex = i;
        }
        yield return new WaitForEndOfFrame();
        SelectTab(0);

    }
    public void SelectTab(int index)
    {
        //先判断当前是第几页
        if (this.index != index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == index);//变色
                if (i<tabPages.Length)// 进行中/可接任务 当选中的是进行中的任务时 
                                       //page[0]主页面 若不是第一个按钮则制成false
                                       //总之就是按第二个按钮 tabPages[0].SetActive(0==1);
                {
                  tabPages[i].SetActive(i == index);
                }              
            }
            if (OnTabSelect!=null)
            {
                OnTabSelect(index);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
