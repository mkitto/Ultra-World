using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using SkillBridge.Message;

public class UICharacterSelect : MonoBehaviour
{
    public GameObject PanelCreate;
    public GameObject PanelSelect;

    public GameObject btnCreateCancel;
    public InputField charName;
    /// <summary>
    /// 角色类型
    /// </summary>
    CharacterClass charClass;

    /// <summary>
    /// 角色列表
    /// </summary>
    public Transform UICharList;
    public GameObject UICharInfo;

    public List<GameObject> UiChars = new List<GameObject>();

    public Image[] titles;
    public UnityEngine.UI.Text descs;
    public UnityEngine.UI.Text[] names;

    /// <summary>
    /// 选择角色的下标
    /// 索引
    /// </summary>
    private int selectCharacterIdx = -1;

    public UICharacterView characterView;


    void Start()
    {
        DataManager.Instance.Load();
        InitCharacterSelect(true);
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;
    }

    void Update()
    {

    }

    public void InitCharacterSelect(bool init)
    {
        PanelCreate.SetActive(false);
        PanelSelect.SetActive(true);

        //初始化角色，当前点击的是哪一个

        if (init)
        {
            //原来有没有 如果有就销毁掉
            foreach (var old in UiChars)
            {
                Destroy(old);
            }

            UiChars.Clear();


            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {
                //实例化一个对象
                //初始化 把角色名字类的信息都设置上去
               GameObject go= Instantiate(UICharInfo, this.UICharList);
               UICharInfo charInfo = go.GetComponent<UICharInfo>();
               charInfo.info = User.Instance.Info.Player.Characters[i];

               Button button = go.GetComponent<Button>();
               int idx = i;
                //当按下的时候 执行选择角色
               button.onClick.AddListener(() =>
               {
                   OnSelectCharacter(idx);
               });

               UiChars.Add(go);
               go.SetActive(true);
            }
        }

    }

    public void InitCharacterCreate()
    {
        PanelCreate.SetActive(true);
        PanelSelect.SetActive(false);
        OnSelectClass(1);
    }

    public void OnClickCreate()
    {
        if (string.IsNullOrEmpty(this.charName.text))
        {
            MessageBox.Show("请输入角色名称");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.charName.text, this.charClass);
    }

    /// <summary>
    /// 选择职业
    /// </summary>
    /// <param name="charClass"></param>
    public void OnSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;
        //赋值 将角色赋值给界面 -1 因为值是加1的
        characterView.CurrentCharacter = charClass - 1;

        //更新标题 
        for (int i = 0; i < 3; i++)
        {
            titles[i].gameObject.SetActive(i == charClass - 1);

            //读取配置表 获取角色职业
            names[i].text = DataManager.Instance.Characters[i + 1].Name;
        }
        //更新选择角色的描述
        descs.text = DataManager.Instance.Characters[charClass].Description;

    }

    void OnCharacterCreate(Result result, string message)
    {
        //如果角色创建成功 一个成功消息 一个失败消息 如果返回的是成功
        //初始化
        if (result==Result.Success)
        {
            InitCharacterSelect(true);
        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    /// <summary>
    /// 选择角色
    /// </summary>
    /// <param name="index"></param>
    public void OnSelectCharacter(int index)
    {
        this.selectCharacterIdx = index;
        var cha = User.Instance.Info.Player.Characters[index];
        Debug.LogFormat("Select Char：[{0}]{1}[{2}]",cha.Id,cha.Name,cha.Class);
        //User.Instance.CurrentCharacter = cha;
        //只有三个职业 应该用职业来计算索引
        characterView.CurrentCharacter = ((int) cha.Class - 1);

        //选择角色按钮图片高亮
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo ci = this.UiChars[i].GetComponent<UICharInfo>();
            ci.Selected = index == i;
        }
    }
    public void OnClickPlay()
    {
        if (selectCharacterIdx>=0)
        {
            //MessageBox.Show("进入游戏", "进入游戏", MessageBoxType.Confirm);
            UserService.Instance.SendGameEnter(selectCharacterIdx);
        }
    }
}
