using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using Hsys.Effect3DBase;
using Unity.VisualScripting;



[InitializeOnLoad]
public class ChangeHierarchy
{
    //===================================================================
    private static Hsys.Effect3DBase.Hsys3DEffectHierarchyMenu m_hierarchymenu = new Hsys3DEffectHierarchyMenu();
    private static Rect m_hierarchyrect = new Rect(10, 10, 10, 10);
    private static int m_timer;
    private static Transform[] effectlistall;
    private static GameObject obj;
    //===================================================================
    static ChangeHierarchy() 
    {
        //m_3deffectbase = Hsys.HsysGetYourNeadVarOfClass.GetMonoClassVar(typeof(Hsys3DEffectBase)) as Hsys3DEffectBase;

        EditorApplication.hierarchyWindowItemOnGUI = delegate (int instanceid, Rect selectionrec)
        {
            if(Selection.activeGameObject == null) { return; }
            obj = Selection.activeGameObject;
            if (obj.name == "3DEffectBase(Hsys)")
            {
                MyHierarchy(ref obj, ref selectionrec);
                //effectlistall = obj.GetComponentsInChildren<Transform>();
                /*for (int index = 0; index < effectlistall.Length; index += 1)
                {
                    if (effectlistall[index].name == "3DEffectList(Hsys)")
                    {
                        // effectlistall[index].
                    }
                }*/

            }
        };
    }


    private static void MyHierarchy(ref GameObject obj, ref Rect selectionrec)
    {
        m_timer += Time.frameCount;
        if (m_timer >= 5)
        {
            m_hierarchymenu.HierarchyShowTips(obj.gameObject, selectionrec);
        }
        m_timer = 0;
    }
}

namespace Hsys
{
    namespace Effect3DBase
    {
        public enum ShowHierarchyType
        {
            Effect3DBase,
            Effect3DList,
            None
        }

        public class Hsys3DEffectHierarchyMenu
        {
            private Hsys.Effect3DBase.ShowHierarchyType m_hierarchytype;
            private Hsys3DEffectHierarchyMenuItemEffect m_hsys3deffecthierarchymenuitemeffect;

            private bool[] m_changehierarchy;
            public Hsys3DEffectHierarchyMenu()
            {
                m_hsys3deffecthierarchymenuitemeffect = new Hsys3DEffectHierarchyMenuItemEffect();
                m_changehierarchy = new bool[8];
            }


            //===============================================
            private Rect newrect;
            private Rect newrect2;
            public void HierarchyShowTips(GameObject targetobjbase, Rect selectionrect)
            {

                //控制开关
                newrect = new Rect(selectionrect);
                newrect.x = newrect.width + 50;
                newrect.width = 15;

                m_hsys3deffecthierarchymenuitemeffect.tooltip.tooltip = "启用Hsys组件";
                m_changehierarchy[0] = targetobjbase.GetComponent<Hsys3DEffectBase>().enabled;
                m_changehierarchy[0] = GUI.Toggle(newrect, m_changehierarchy[0], m_hsys3deffecthierarchymenuitemeffect.tooltip);
                targetobjbase.GetComponent<Hsys3DEffectBase>().enabled = m_changehierarchy[0];


                GUIStyle style = new GUIStyle();

                style.normal.textColor = Color.yellow;
                style.hover.textColor = Color.green;

                //GUI.Label(newrect, "[Count]",style);
                //targetobjbase. = Color.yellow;
                //GUI.color = ;
                //GUI.Label(newrect, targetobjbase.name, style);
            }

            public class Hsys3DEffectHierarchyMenuItemEffect
            {
                public GUIContent tooltip = new GUIContent();
                public string showtext = string.Empty;
                public Color showcolor = Color.blue;

                public void CaseHierarchytype(ref ShowHierarchyType hierarchytype)
                {
                    switch (hierarchytype)
                    {
                        case Hsys.Effect3DBase.ShowHierarchyType.Effect3DBase:
                            Effect3DBase();
                            break;
                        case Hsys.Effect3DBase.ShowHierarchyType.Effect3DList:

                            break;
                        case Hsys.Effect3DBase.ShowHierarchyType.None:
                            break;
                    }
                }

                private void Effect3DBase()
                {

                }
            }
        }
    }
}