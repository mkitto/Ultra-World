using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Sprite activeImage;
    private Sprite normalImage;

    public TabView tabView;

    public int tabIndex = 0;
    public bool selected = false;

    private Image tabImage;
    // Use this for initialization
    void Start()
    {
        tabImage = this.GetComponent<Image>();
        normalImage = tabImage.sprite;
    }
    public void Select(bool select)
    {
        tabImage.overrideSprite = select ? activeImage : normalImage;
    }
   public void OnClick()
    {
        this.tabView.SelectTab(tabIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
