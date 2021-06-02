using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TeleporterObject : MonoBehaviour
{
    public int ID;
    Mesh mesh = null;


    void Start()
    {
        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }

    void Update()
    {

    }



#if UNITY_EDITOR
    [MenuItem()]
    void OnDrawGizwos()
    {
        Gizmos.color= Color.green;
        if (this.mesh != null)
        {
            Gizmos.DrawWireMesh(this.mesh,this.transform.position+Vector3.up*this.transform.localScale.y*.5f,transform.rotation,this.transform.localScale);
        }
        UnityEditor.Handles.color=Color.red;
        UnityEditor.Handles.ArrowHandleCap(0,this.transform.position,this.transform.rotation,1f,EventType.Repaint);
    }

#endif
}
