using Hsys.Private_Hsys;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hsys
{
    namespace Private_Hsys
    {
        public enum isHalfEnum
        {
            NORMAL,
            HALF
        }
    }
    public class MenuItem : EditorWindow
    {
        [SerializeField] private static float _setRange = 50f;
        [SerializeField] private static CreateTree _octnode = null;
        [SerializeField] private static GameObject _myoctree = null;
        [UnityEditor.MenuItem("Octree/CreateTree"), Tooltip("Octree 创建树")]
        private static void CreateTree()
        {
/*            _myoctree = new GameObject("MyTree");
            _myoctree.transform.position = Vector3.zero;
            _octnode = _myoctree.AddComponent<CreateTree>();*/
            CreateOctreeObj();
            SetOctreeObjPosition();
        }
        [UnityEditor.MenuItem("Octree/SetRange"), Tooltip("设置范围")]
        private static void SetRange()
        {
            var window = EditorWindow.GetWindow(typeof(MenuItem), true, "Octree SetRange");
            window.Show();
            
        }
        [SerializeField] private float _getsetrange;
            
        [SerializeField] private isHalfEnum ihe = new isHalfEnum();


        private void OnGUI()
        {
            
            GUIContent content = new GUIContent();
            content.text = "Octree";
            GUIStyle mytitlestyle = new GUIStyle();
            mytitlestyle.fontSize = 32;
            mytitlestyle.alignment = TextAnchor.MiddleCenter;
            mytitlestyle.fontStyle = FontStyle.Bold;
            mytitlestyle.normal.textColor = Color.white;
            GUILayout.Label(content, mytitlestyle);

            GUILayout.Space(10);
            GUILayout.Label("Create Octree Mode");
            GUILayout.Space(10);
            ihe = (isHalfEnum)EditorGUILayout.EnumPopup("IsHalf", ihe);
            //ih = (isHalfEnum)EditorGUILayout.EnumFlagsField("IsHalf", ih);
            GUILayout.Space(10);
            if (GUILayout.Button("Create Octree"))
            {
                CreateOctreeObj(ihe);
            }

            GUILayout.Space(10);
            GUILayout.Label("Range");

            _getsetrange = EditorGUILayout.FloatField(_getsetrange);
            GUILayout.Space(10);
            if (GUILayout.Button("Show"))
            {
                //GetOctreeObjRange();
                //ShowOctreeObjRange();
            }


            GUILayout.Space(10);
            if(GUILayout.Button("Apply"))
            {
                SetObjRange();
            }

            GUILayout.Space(10);
            if(GUILayout.Button("ClearAll"))
            {
                DestroyOctreeObj();
                DestroyImmediate(_octnode);
                DestroyImmediate(_myoctree);
            }
            
        }

        private void SetObjRange()
        {
            _setRange = _getsetrange;
            SetOctreeObjName();
            SetOctreeObjPosition();
        }

        //------------------------------------------------------------------------------------
        [SerializeField] private bool _is_show_range = false;

        [SerializeField] private MeshRenderer[] mymeshrander;
        private void GetOctreeObjRange()
        {

            if (_octnode == null) return;
            for (int index = 0; index < _octnode.obj.Length; index += 1)
            {
                if (_octnode.obj[index] == null) break;
                if (_octnode.obj[index].GetComponent<MeshFilter>() != null && _octnode.obj[index].GetComponent<MeshRenderer>() != null) break;
                _octnode.obj[index].AddComponent<MeshFilter>();

                //_octnode.obj[index].GetComponent<MeshFilter>().mesh = mesh;
                _octnode.obj[index].AddComponent<MeshRenderer>();
            }

            for (int index = 0; index < _octnode.obj.Length; index += 1)
            {
                if (mymeshrander[index] == null)
                {
                    mymeshrander[index] = _octnode.obj[index].GetComponent<MeshRenderer>();
                }
            }

        }
        private void ShowOctreeObjRange()
        {
            _is_show_range = !_is_show_range;
            if(mymeshrander == null) { mymeshrander = new MeshRenderer[_octnode.obj.Length]; }
            if (_is_show_range)
            {
                for (int index = 0; index < _octnode.obj.Length; index += 1)
                {
                    if (_octnode.obj[index] == null) break;

                    if (mymeshrander[index] != null)
                    {
                        mymeshrander[index].enabled = true;
                    }
                    else { Debug.LogWarning("Hey, Not this component, were you want change it, that`s no way!!!"); }

                }
            }
            else
            {
                for (int index = 0; index < _octnode.obj.Length; index += 1)
                {
                    if (_octnode.obj[index] == null) break;

                    mymeshrander[index].enabled = false;
                }
            }
        }

        private static void CreateOctreeObj(isHalfEnum _ishalf = isHalfEnum.NORMAL)
        {
            string setRangestring = _setRange.ToString();

            _myoctree = new GameObject("MyTree");
            _myoctree.transform.position = Vector3.zero;
            _octnode = _myoctree.AddComponent<CreateTree>();

            switch (_ishalf)
            {
                case isHalfEnum.NORMAL:
                    _octnode.obj = new GameObject[]
                {
                    new GameObject("0_0_0"),
                    new GameObject("0_0_"+setRangestring),
                    new GameObject("0_"+setRangestring+"_0"),
                    new GameObject("0_"+setRangestring+"_"+setRangestring),
                    new GameObject(setRangestring+"_0_0"),
                    new GameObject(setRangestring+"_"+setRangestring+"_0"),
                    new GameObject(setRangestring+"_0_" + setRangestring),
                    new GameObject(setRangestring+"_"+setRangestring+"_"+setRangestring)
                };
                    SetOctreeObjPosition( _ishalf);
                    break;
                case isHalfEnum.HALF:

                    _octnode.obj = new GameObject[]
                {

                    //new GameObject("0_0_0"),
                    new GameObject("0_0_"+setRangestring),
                    new GameObject("0_"+setRangestring+"_0"),
                    //new GameObject("0_"+setRangestring+"_"+setRangestring),
                    new GameObject(setRangestring+"_0_0"),
                    //new GameObject(setRangestring+"_"+setRangestring+"_0"),
                    //new GameObject(setRangestring+"_0_" + setRangestring),
                    new GameObject(setRangestring+"_"+setRangestring+"_"+setRangestring)
                };
                    SetOctreeObjPosition( _ishalf);
                    break;
            }



            for (int index = 0; index < _octnode.obj.Length; index += 1)
            {
                _octnode.obj[index].transform.SetParent(_myoctree.transform);
                _octnode.obj[index].AddComponent<UnityEngine.BoxCollider>();
            }
        }



        private void SetOctreeObjName()
        {
            string setRangestring = _setRange.ToString();

            _octnode.obj[0].name = "0_0_0";
            _octnode.obj[1].name = "0_0_" + setRangestring;
            _octnode.obj[2].name = "0_" + setRangestring + "_0";
            _octnode.obj[3].name = "0_" + setRangestring + "_" + setRangestring;
            _octnode.obj[4].name = setRangestring + "_0_0";
            _octnode.obj[5].name = setRangestring + "_" + setRangestring + "_0";
            _octnode.obj[6].name = setRangestring + "_0_" + setRangestring;
            _octnode.obj[7].name = setRangestring + "_" + setRangestring + "_" + setRangestring;
        }

        private static void SetOctreeObjPosition(isHalfEnum _ishalf = isHalfEnum.NORMAL)
        {
            switch (_ishalf)
            {
                case isHalfEnum.NORMAL:
                    _octnode.obj[0].transform.position = new Vector3(0, 0, 0);
                    _octnode.obj[1].transform.position = new Vector3(0, 0, _setRange);
                    _octnode.obj[2].transform.position = new Vector3(0, _setRange, 0);
                    _octnode.obj[3].transform.position = new Vector3(0, _setRange, _setRange);
                    _octnode.obj[4].transform.position = new Vector3(_setRange, 0, 0);
                    _octnode.obj[5].transform.position = new Vector3(_setRange, _setRange, 0);
                    _octnode.obj[6].transform.position = new Vector3(_setRange, 0, _setRange);
                    _octnode.obj[7].transform.position = new Vector3(_setRange, _setRange, _setRange);
                    break;

                case isHalfEnum.HALF:
                    //_octnode.obj[0].transform.position = new Vector3(0, 0, 0);
                    _octnode.obj[0].transform.position = new Vector3(0, 0, _setRange);
                    _octnode.obj[1].transform.position = new Vector3(0, _setRange, 0);
                    //_octnode.obj[1].transform.position = new Vector3(0, _setRange, _setRange);
                    _octnode.obj[2].transform.position = new Vector3(_setRange, 0, 0);
                    //_octnode.obj[5].transform.position = new Vector3(_setRange, _setRange, 0);
                    //_octnode.obj[3].transform.position = new Vector3(_setRange, 0, _setRange);
                    _octnode.obj[3].transform.position = new Vector3(_setRange, _setRange, _setRange);
                    break;
            }
            
        }
        
        private void DestroyOctreeObj()
        {
            for(int index = 0; index < _octnode.obj.Length; index+=1)
            {
                DestroyImmediate(_octnode.obj[index]);
            }
        }
    }

    //没什么用写着玩
    public class MeshItem
    {
        public struct Border
        {
            public int x;
            public int y;
            public int z;
        }

        public Border m_border;
        public Mesh m_mesh;
        public MeshItem(int _x, int _y, int _z)
        {
            m_border.x = _x;
            m_border.y = _y;
            m_border.z = _z;
        }
        public void CreateMesh()
        {
            int v = 0;

            for (int x = 0; x <= m_border.x; x+=1)
            {
                m_mesh.vertices[v++] = new Vector3(x, 0, 0);
            }
            for (int z = 1; z <= m_border.z; z+=1)
            {
                m_mesh.vertices[v++] = new Vector3(m_border.x, 0, z);
            }
            for (int x = m_border.x - 1; x >= 0; x-=1)
            {
                m_mesh.vertices[v++] = new Vector3(x, 0, m_border.z);
            }
            for (int z = m_border.z - 1; z > 0; z-=1)
            {
                m_mesh.vertices[v++] = new Vector3(0, 0, z);
            }

            for (int z = 1; z < m_border.z; z++)
            {
                for (int x = 1; x < m_border.x; x++)
                {
                    m_mesh.vertices[v++] = new Vector3(x, m_border.y, z);
                }
            }
            for (int z = 1; z < m_border.z; z++)
            {
                for (int x = 1; x < m_border.x; x++)
                {
                    m_mesh.vertices[v++] = new Vector3(x, 0, z);
                }
            }
        }
        /*public void CreateT()
        {

            int quads = (m_border.x * m_border.y + m_border.x * m_border.z + m_border.y * m_border.z) * 2;
            int[] triangles = new int[quads * 6];
            int ring = (m_border.x + m_border.z) * 2;
            int t = 0, v = 0;

            for (int q = 0; q < ring; q++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
            }

            for (int y = 0; y < m_border.y; y++, v++)
            {
                for (int q = 0; q < ring - 1; q++, v++)
                {
                    t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
                }
                t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
            }

            m_mesh.triangles = triangles;


        }*/

/*        private static int SetQuad(int[] _triangles, int i, int v00, int v10, int v01, int v11)
        {
            _triangles[i] = v00;
            _triangles[i + 1] = _triangles[i + 4] = v01;
            _triangles[i + 2] = _triangles[i + 3] = v10;
            _triangles[i + 5] = v11;
            return i + 6;
        }*/
    }

}
