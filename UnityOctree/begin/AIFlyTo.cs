using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIFlyTo : MonoBehaviour
{
    [SerializeField, Tooltip("速度")] public float speed = 10f;
    [SerializeField, Tooltip("加速度")] public float accuracy = 1f;
    [SerializeField, Tooltip("旋转速度")] public float rotspeed = 10f;

    [SerializeField, Tooltip("当前WP")] public int currentwp = 0;
    private Vector3 goal;
    [SerializeField, Tooltip("八叉树对象")] public GameObject octree;
    private Hsys.Graph graph;
    private Hsys.Octree ot;
    private List<Hsys.Node> pathList = new List<Hsys.Node>();

    public GameObject goalPosition;

    private void Start()
    {
        Invoke("Navigate", 1);
    }

    private void Navigate()
    {
        graph = octree.GetComponent<CreateTree>()._graph;
        ot = octree.GetComponent<CreateTree>()._ot;
    }

    private void NavigateTo(int _destination, Hsys.Node _finalgoal)
    {
        Hsys.Node destinationNode = graph.findNode(_destination);
        graph.AStar(graph.nodes[currentwp].octnode, destinationNode.octnode, pathList);
        currentwp = 0;
        pathList.Add(_finalgoal);
    }

    public int getPathLength()
    {
        return pathList.Count;
    }

    public Hsys.OctreeNode GetPathPoint(int _index)
    {
        return pathList[_index].octnode;
    }

    private void Update()
    {
        if (ot == null) { return; }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 pos = goalPosition.transform.position;
            int i = ot.AddDestination(pos);
            if (i == -1)
            {
                Debug.Log("Destination not found in Octreenode");
                return;
            }

            Hsys.Node finalGoal = new Hsys.Node(new Hsys.OctreeNode(new Bounds(pos, new Vector3(1f, 1f, 1f)), 1f, null));
            NavigateTo(i, finalGoal);
        }
    }

    private void LateUpdate()
    {
        if (graph == null) { return; }
        if (getPathLength() == 0 || currentwp == getPathLength()) return;

        if (Vector3.Distance(GetPathPoint(currentwp).m_nodebounds.center, this.transform.position) <= accuracy) { currentwp += 1; }

        if (currentwp < getPathLength())
        {
            goal = GetPathPoint(currentwp).m_nodebounds.center;
            Vector3 lookatgoal = new Vector3(goal.x, goal.y, goal.z);
            Vector3 destination = lookatgoal - this.transform.position;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(destination), Time.deltaTime * rotspeed);

            this.transform.Translate(0f, 0f, speed * Time.deltaTime);
        }
        else
        {
            if (getPathLength() == 0) return;
        }
    }
}

