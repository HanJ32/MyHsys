using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
namespace Hsys
{

    public class Node
    {
        public List<Edge> edgelist = new List<Edge>();
        public Node path = null;
        public OctreeNode octnode;
        public float f, g, h;
        public Node cameform;

        public Node(OctreeNode _octreenode)
        {
            octnode = _octreenode;
            path = null;
        }

        public OctreeNode getNode()
        {
            return octnode;
        }
    }

    public class Utils
    {
        public static int id_number = 0;
    }

    public class Edge
    {
        public Node m_startnode;
        public Node m_endnode;

        public Edge(Node _startnode, Node _endnode)
        {
            m_startnode = _startnode;
            m_endnode = _endnode;
        }
    }

    public struct OctreeSt
    {
        public Bounds bounds;
        public GameObject obj;

        public OctreeSt(GameObject _obj)
        {
            bounds = _obj.GetComponent<Collider>().bounds;
            obj = _obj;
        }
    }
    public class Octree
    {
        public OctreeNode rootnode;
        //public Bounds m_boundsD;
        public List<OctreeNode> emtryleaves = new List<OctreeNode>();
        public Hsys.Graph navigationGraph;
        public Octree(GameObject[] _gameobject, float _minnodesize, Hsys.Graph _navgraph)
        {
            Bounds bounds = new Bounds();
            navigationGraph = _navgraph;

            foreach (GameObject obj in _gameobject)
            {
                bounds.Encapsulate(obj.GetComponent<Collider>().bounds);
            }

            float maxsize = Mathf.Max(new float[] { bounds.size.x, bounds.size.y, bounds.size.z });
            Vector3 sizevector = new Vector3(maxsize, maxsize, maxsize) * 0.5f;
            bounds.SetMinMax(bounds.center - sizevector, bounds.center + sizevector);

            rootnode = new OctreeNode(bounds, _minnodesize, null);
            AddObjects(_gameobject);
            GetEmptyLeaves(rootnode);
            ProcessExtraConnections();
            Debug.Log("OctreeNode:" + emtryleaves.Count);

            Debug.Log("Edge:" + navigationGraph.edges.Count);
        }

        public void AddObjects(GameObject[] _gameobj)
        {
            foreach (GameObject obj in _gameobj)
            {
                rootnode.AddObject(obj);
            }
        }

        public int FindBindingNode(OctreeNode _node, Vector3 _position)
        {
            int found = -1;
            if (_node == null) { return -1; }

            if(_node.children == null)
            {
                if(_node.m_nodebounds.Contains(_position) && _node.continedobject.Count == 0)
                {
                    return _node.id;
                }
            }else
            {
                for (int i = 0; i< 8; ++i)
                {
                    found = FindBindingNode(_node.children[i], _position);
                    if(found != -1) break;
                }
            }
            return found;
        }

        public int AddDestination(Vector3 _destination)
        {
            return FindBindingNode(rootnode, _destination);
        }

        public void GetEmptyLeaves(OctreeNode _node)
        {
            if (_node == null) return;
            if (_node.children == null)
            {
                if (_node.continedobject.Count == 0)
                {
                    emtryleaves.Add(_node);
                    navigationGraph.AddNode(_node);
                    //Gizmos.color = new Color(0, 0, 1, 0.5f);
                    //Gizmos.DrawCube(node.children[0].m_nodebounds.center, m_nodebounds.size);
                }
            }
            else
            {
                for (int i = 0; i < 8; ++i)
                {
                    GetEmptyLeaves(_node.children[i]);
                    for (int s = 0; s < 8; ++s)
                    {
                        if (s != i)
                        {
                            navigationGraph.AddEdge(_node.children[i], _node.children[s]);
                        }
                    }
                }
            }
        }

        public void ProcessExtraConnections()
        {
            Dictionary<(int, int), int> subGraphConnections = new Dictionary<(int, int), int>();

            foreach (OctreeNode i in emtryleaves)
            {
                foreach (OctreeNode j in emtryleaves)
                {
                    if (i.id != j.id && i.parent.id != j.parent.id)
                    {
                        RaycastHit hitInfo;
                        Vector3 direction = j.m_nodebounds.center - i.m_nodebounds.center;
                        float accuracy = 1;
                        if (!Physics.SphereCast(i.m_nodebounds.center, accuracy, direction, out hitInfo))
                        {
                            if (subGraphConnections.TryAdd((i.parent.id, j.parent.id), 1))
                            {
                                navigationGraph.AddEdge(i, j);
                            }
                        }
                    }
                }
            }
        }

        public void ConnectLeafNodeNeighbours()
        {
            List<Vector3> rays = new List<Vector3>()
            {
                new Vector3(1f,0f,0f),
                new Vector3(-1f,0f,0f),
                new Vector3(0f,1f,0f),
                new Vector3(0f,-1f,0f),
                new Vector3(0f,0f,1f),
                new Vector3(0f,0f,-1f)
            };

            int numRays = 6;
            float epsilon = 0.01f;

            for(int index = 0; index< emtryleaves.Count;++index)
            {
                List<OctreeNode> neighbours = new List<OctreeNode>();
                for(int index2 = 0; index2 < numRays; ++index2)
                {
                    if(index != index2)
                    {
                        for (int index3 = 0; index3 < numRays; ++index3)
                        {
                            Ray ray = new Ray(emtryleaves[index].m_nodebounds.center, rays[index3]);
                            float maxlength = emtryleaves[index].m_nodebounds.size.y / 2f + epsilon;

                            float hitlength;

                            if (emtryleaves[index2].m_nodebounds.IntersectRay(ray, out hitlength))
                            {
                                if(hitlength < maxlength)
                                {
                                    neighbours.Add(emtryleaves[index2]);
                                }
                            }
                        }
                    }
                    
                }
                foreach(OctreeNode node in neighbours)
                {
                    navigationGraph.AddEdge(emtryleaves[index], node);
                }
            }
        }
    }

    public class OctreeNode
    {
        public int id;
        public Bounds m_nodebounds;
        Bounds[] childbounds;
        public OctreeNode[] children = null;
        float m_minsize;
        private bool dividing;

        public List<OctreeSt> continedobject = new List<OctreeSt>();

        public OctreeNode parent;

        public OctreeNode(Bounds _nodebounds, float _minsize, OctreeNode p)
        {
            m_nodebounds = _nodebounds;
            m_minsize = _minsize;
            id = Hsys.Utils.id_number;
            ++Hsys.Utils.id_number;
            parent = p;

            float quarter = m_nodebounds.size.y / 4f;
            float childlength = m_nodebounds.size.y / 2f;
            Vector3 childSize = new Vector3(childlength, childlength, childlength);

            childbounds = new Bounds[8];
            childbounds[0] = new Bounds(m_nodebounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
            childbounds[1] = new Bounds(m_nodebounds.center + new Vector3(quarter, quarter, -quarter), childSize);
            childbounds[2] = new Bounds(m_nodebounds.center + new Vector3(-quarter, quarter, quarter), childSize);
            childbounds[3] = new Bounds(m_nodebounds.center + new Vector3(quarter, quarter, quarter), childSize);
            childbounds[4] = new Bounds(m_nodebounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);
            childbounds[5] = new Bounds(m_nodebounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
            childbounds[6] = new Bounds(m_nodebounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
            childbounds[7] = new Bounds(m_nodebounds.center + new Vector3(quarter, -quarter, quarter), childSize);
        }

        public void AddObject(GameObject _gameobj)
        {
            DivideAndAdd(_gameobj);
        }

        public void DivideAndAdd(GameObject _gameobj)
        {
            OctreeSt octreeSt = new OctreeSt(_gameobj);
            if (m_nodebounds.size.y <= m_minsize)
            {
                continedobject.Add(octreeSt);
                return;
            }
            if (children == null)
            {
                children = new OctreeNode[8];
            }

            dividing = false;
            for (int i = 0; i < 8; ++i)
            {
                if (children[i] == null)
                {
                    children[i] = new OctreeNode(childbounds[i], m_minsize, this);
                }
                if (childbounds[i].Intersects(octreeSt.bounds))
                //if (childbounds[i].Contains(octreeSt.bounds.min)&& childbounds[i].Contains(octreeSt.bounds.max))
                {
                    dividing = true;
                    children[i].DivideAndAdd(_gameobj);
                }
            }
            if (dividing == false)
            {
                continedobject.Add(octreeSt);
                children = null;
            }
        }

        public void Draw()
        {
            Gizmos.color = new Color(1, 1, 0, 0.5f);
            Gizmos.DrawWireCube(m_nodebounds.center, m_nodebounds.size);
            Gizmos.color = new Color(1, 0, 0, 0.8f);
            foreach (OctreeSt obj in continedobject)
            {
                Gizmos.DrawCube(obj.bounds.center, obj.bounds.size + new Vector3(0.1f, 0.1f, 0.1f));
            }
            if (children != null)
            {
                for (int i = 0; i < 8; ++i)
                {
                    if (children[i] != null)
                    {
                        children[i].Draw();

                    }
                }
            }
            else if (continedobject.Count != 0)
            {
                Gizmos.color = new Color(0, 0, 1, 0.5f);
                Gizmos.DrawCube(m_nodebounds.center, m_nodebounds.size);
            }
        }
    }

    public class Graph
    {
        public List<Edge> edges = new List<Edge>();
        public List<Node> nodes = new List<Node>();
        //public List<Node> pathlist = new List<Node>();
        public Graph()
        {

        }

        public void AddNode(OctreeNode octnode)
        {
            if (findNode(octnode.id) == null)
            {
                //Debug.Log(octnode.id);
                Node node = new Node(octnode);
                nodes.Add(node);
            }
        }
        public void AddEdge(OctreeNode from, OctreeNode to)
        {
            Node _from = findNode(from.id);
            Node _to = findNode(to.id);

            if (_from != null && _to != null)
            {
                Edge e = new Edge(_from, _to);
                edges.Add(e);
                _from.edgelist.Add(e);
                Edge f = new Edge(_to, _from);
                edges.Add(f);
                _to.edgelist.Add(f);
            }
        }


        public Node findNode(int oct_id)
        {
            foreach (Node node in nodes)
            {
                if (node.getNode().id == oct_id)
                {
                    return node;
                }
            }
            return null;
        }


        public void Draw()
        {
            for (int i = 0; i < edges.Count; ++i)
            {
                Debug.DrawLine(edges[i].m_startnode.octnode.m_nodebounds.center,
                    edges[i].m_endnode.octnode.m_nodebounds.center, Color.red);
            }
            for (int i = 0; i < nodes.Count; ++i)
            {
                Gizmos.color = new Color(0, 1, 1);
                Gizmos.DrawWireSphere(nodes[i].octnode.m_nodebounds.center, 0.25f);
            }
        }

        public bool AStar(OctreeNode _startnode, OctreeNode _endnode, List<Node> _pathlist)
        {
            _pathlist.Clear();
            Node start = findNode(_startnode.id);
            Node end = findNode(_endnode.id);

            if (start == null || end == null)
            {
                return false;
            }

            List<Node> open = new List<Node>();
            List<Node> close = new List<Node>();

            float tentative_g_score = 0;
            bool tentative_b;
            start.g = 0;
            start.h = Vector3.SqrMagnitude(_startnode.m_nodebounds.center - _endnode.m_nodebounds.center);
            start.f = start.h;

            open.Add(start);
            while (open.Count > 0)
            {
                int i = loweatF(open);
                Node thisnode = open[i];

                if (thisnode.octnode.id == _endnode.id)
                {
                    recounstructPath(start, end, _pathlist);
                    return true;
                }
                /*recounstructPath()*/
                open.RemoveAt(i);
                close.Add(thisnode);

                Node neighbour;
                foreach (Edge ed in thisnode.edgelist)
                {
                    neighbour = ed.m_endnode;
                    neighbour.g = thisnode.g + Vector3.SqrMagnitude(thisnode.octnode.m_nodebounds.center - neighbour.octnode.m_nodebounds.center);

                    if (close.IndexOf(neighbour) > -1)
                    {
                        continue;
                    }

                    tentative_g_score = thisnode.g + Vector3.SqrMagnitude(thisnode.octnode.m_nodebounds.center - neighbour.octnode.m_nodebounds.center);

                    if (open.IndexOf(neighbour) == -1)
                    {
                        open.Add(neighbour);
                        tentative_b = true;
                    }
                    else if (tentative_g_score < neighbour.g)
                    {
                        tentative_b = true;
                    }
                    else
                    {
                        tentative_b = false;
                    }

                    if (tentative_b)
                    {
                        neighbour.cameform = thisnode;
                        neighbour.g = tentative_g_score;
                        neighbour.h = Vector3.SqrMagnitude(thisnode.octnode.m_nodebounds.center - _endnode.m_nodebounds.center);
                        neighbour.f = neighbour.g + neighbour.h;
                    }
                }
            }

            return false;
        }

        public void recounstructPath(Node _stard, Node _end, List<Node> _pathlist)
        {
            _pathlist.Clear();
            _pathlist.Add(_end);
            var p = _end.cameform;
            while (p != null && p != _stard)
            {
                _pathlist.Insert(0, _stard);
                p = p.cameform;
            }
            _pathlist.Insert(0, _stard);
        }
        public int loweatF(List<Node> _open)
        {
            float lowestf = 0;
            int count = 0;
            int iteratorcount = 0;

            for (int i = 0; i < _open.Count; i += 1)
            {
                if (i == 0)
                {
                    lowestf = _open[i].f;
                    iteratorcount = count;
                }
                else if (_open[i].f > lowestf)
                {
                    lowestf = _open[i].f;
                    iteratorcount = count;
                }
                count += 1;
            }
            return iteratorcount;
        }
    }
}