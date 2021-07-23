using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Models;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public int npcID;
    SkinnedMeshRenderer renderer;
    Animator anim;
    Color orignColor;
    private bool inInteractive = false;

    private NpcDefine npc;

    NpcQuestStatus questStatus;
    void Start()
    {
        renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = this.gameObject.GetComponent<Animator>();
        orignColor = renderer.sharedMaterial.color;//初始化的时候，将当前的颜色先保存了一份
        npc = NPCManager.Instance.GetNpcDefine(npcID);
        this.StartCoroutine(Actions());
        RefreshNpcStatus();
        QuestManager.Instance.onQuestStatusChanged += OnQuestStatusChanged;
        
    }

    private void OnQuestStatusChanged(Quest quest)
    {
        this.RefreshNpcStatus();
    }

    private void RefreshNpcStatus()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.npcID);
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform,questStatus);
    }

    private void OnDestroy()
    {
        QuestManager.Instance.onQuestStatusChanged -= OnQuestStatusChanged;
        if (UIWorldElementManager.Instance!=null)
        {
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
        }
    }
    //随机动作 写死循环，永远不会卡住主线程
    IEnumerator Actions()
    {
        while (true)
        {
            if (inInteractive)
            {
                yield return new WaitForSeconds(2f);
            }
            else
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
            }

            this.Relax();
        }
    }

    void Relax()
    {
        anim.SetTrigger("Relax");
    }

    void Interactive()
    {//效验 防止重复点击 启动协程在协程里面执行交互
        if (!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }
    //交互
    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();//面向玩家 点击转身
        if (NPCManager.Instance.Interactive(npc))//把交互请求发送给了NPCManager
        {
            anim.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3f);
        inInteractive = false;

    }
    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        NPCManager.Instance.Interactive(npc);//通过npc的类型执行TestManager注册给NPCManager的字典的委托：打开不同种类的窗口
        Interactive();
        //Debug.LogError(this.name);
    }
    private void OnMouseOver()
    {
        Highlight(true);
    }
    private void OnMouseEnter()
    {
        Highlight(true);
    }
    private void OnMouseExit()
    {
        Highlight(false);
    }
    //高亮，知道鼠标移动到上面了
    void Highlight(bool highlight)
    {
        if (highlight)
        {
            if (renderer.sharedMaterial.color != Color.white)
            {
                renderer.sharedMaterial.color = Color.white;
            }
        }
        else
        {
            if (renderer.sharedMaterial.color != orignColor)
            {
                renderer.sharedMaterial.color = orignColor;
            }
        }
    }

}   

