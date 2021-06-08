using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : UIWindow
{
    public Text title;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetTitle(string title)
    {
        this.title.text = title;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
