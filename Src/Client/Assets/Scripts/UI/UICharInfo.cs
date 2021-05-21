using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharInfo : MonoBehaviour
{
    public SkillBridge.Message.NCharacterInfo info;

    public UnityEngine.UI.Text charClass;
    public UnityEngine.UI.Text charName;
    public UnityEngine.UI.Image highlight;


    public bool Selected
    {
        get
        {
            return highlight.IsActive();
        }
        set
        {
            highlight.gameObject.SetActive(value);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (info!=null)
        {
            this.charClass.text = this.info.Class.ToString();
            this.charName.text = this.info.Name;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
