using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateTree : MonoBehaviour
{
    [Tooltip("设置对象")] public GameObject[] obj;
    [Tooltip("树节点分割数")] public int node_number = 1;

    /*[SerializeField]*/ public Hsys.Octree _ot;
    /*[SerializeField]*/ public Hsys.Graph _graph;
    void Start()
    {
        _graph = new Hsys.Graph();
        _ot = new Hsys.Octree(obj, node_number, _graph);
    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            _ot.rootnode.Draw();
            _ot.navigationGraph.Draw();
        }
    }
}
