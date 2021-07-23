using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//做更新
public class UIWorldElement : MonoBehaviour {

    public Transform owner;  //跟随者 元素是属于谁的

    public float height = 2f;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        //做跟踪
        if (owner != null)
        {
            this.transform.position = owner.position + Vector3.up * height;
        }

        if (Camera.main != null)
            this.transform.forward = Camera.main.transform.forward;
    }
}
