using Hsys.Effect3DBase;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Hsys3DEffectBase))]
public class Hsys3DEffectsBaseMenuItem : Editor
{
    private SerializedObject obj;
    private Hsys3DEffectBase m_3deffectbaseclass;
    private List<Effect3DData> m_3deffect;
    private GUIContent tooltip;
    private GUIStyle guistyle;
    private bool[] m_3deffect_changed;
    private bool[] m_3deffectmenu_changed;
    private int boolsize = 8;

    private void OnEnable()
    {
        //m_3deffect = obj.FindProperty("m_3deffectlist").GetUnderlyingValue() as List<Effect3DData>;
        m_3deffect_changed = new bool[boolsize];
        m_3deffectmenu_changed = new bool[boolsize];
        tooltip = new GUIContent();
        guistyle = new GUIStyle();
        obj = new SerializedObject(target);
        for (int index = 0; index < m_3deffect_changed.Length; index += 1)
        {
            m_3deffect_changed[index] = m_3deffectmenu_changed[index] = true;
            m_3deffectbaseclass = obj.targetObject as Hsys3DEffectBase;
        }
        guistyle.fontSize = 12;
        guistyle.fontStyle = FontStyle.Normal;
        guistyle.normal.textColor = Color.white;

    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //if (blurlist == null) return;
        //m_hierarchymenu.HierarchyShowTips()
        m_3deffect = obj.FindProperty("m_3deffectlist").GetUnderlyingValue() as List<Effect3DData>;
        if (m_3deffect == null) { Debug.Log("当前未找到 ==> 3DEffect ==> 3DEffectList"); return; }

        //EditorGUILayout.PropertyField(blurlist);
        tooltip.tooltip = "3D Effect 队列";
        tooltip.text = "3D Effect List";

        if(GUILayout.Button(tooltip)) { m_3deffectmenu_changed[0] = !m_3deffectmenu_changed[0];}
        if (m_3deffectmenu_changed[0])
        {
            for (int index = 0; index < m_3deffect.Count; index += 1)
            {

                EditorGUILayout.BeginVertical("box");
                m_3deffect_changed[index] = EditorGUILayout.Foldout(m_3deffect_changed[index], "List:" + m_3deffect[index].m_Model3DEffect.ToString());
                if (m_3deffect_changed[index])
                {
                    
                    EditorGUILayout.BeginVertical("box");
                    
                    tooltip.tooltip = "设置";
                    tooltip.text = "Setting";
                    GUILayout.Label(tooltip, guistyle);
                    tooltip.tooltip = "激活状态";
                    tooltip.text = "IsEnable";
                    m_3deffect[index].m_IsEnable = EditorGUILayout.Toggle(tooltip, m_3deffect[index].m_IsEnable);
                    tooltip.tooltip = "处理的类别";
                    tooltip.text = "3DEffectType";
                    m_3deffect[index].m_Model3DEffect = (Hsys.Effect3DBase.model3deffect)EditorGUILayout.EnumPopup(tooltip, m_3deffect[index].m_Model3DEffect);
                    tooltip.tooltip = "处理的对象";
                    tooltip.text = "EffectItem";
                    if (m_3deffect[index].m_EffectItem == null)
                    {
                        m_3deffect[index].m_EffectItem = EditorGUILayout.ObjectField(tooltip, null, null, true);
                    }else
                    {
                        System.Type type = m_3deffect[index].m_EffectItem.GetType();
                        m_3deffect[index].m_EffectItem = EditorGUILayout.ObjectField(tooltip,(Object)m_3deffect[index].m_EffectItem, type,true);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginVertical("box");
                    tooltip.tooltip = "属性";
                    tooltip.text = "Attribute";
                    GUILayout.Label(tooltip, guistyle);

                    SetShowValue(ref index);
                    EditorGUILayout.EndVertical();
                    
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("添加"))
            {
                if (m_3deffect.Count < 8)
                //ChangeBoolSize(true);
                {
                    m_3deffectbaseclass.AddPush3DEffectData();
                }
            }
            if (GUILayout.Button("删除"))
            {
                //ChangeBoolSize(false);
                m_3deffectbaseclass.DeletePop3DEffectData();

            }
            EditorGUILayout.EndHorizontal();
        }
        //Debug.Log(blurlist.GetEndProperty().name);
        obj.ApplyModifiedProperties();
    }

    //===================================================
    /*private bool[] __3deffect_changed;
    private bool changesize;
    private int bordersize = 6;
    private void ChangeBoolSize()
    {
        __3deffect_changed = new bool[m_3deffect_changed.Length];
        for (int index = 0; index < m_3deffect_changed.Length; index +=1)
        {
            __3deffect_changed[index] = m_3deffect_changed[index];
        }
        m_3deffect_changed = new bool[boolsize];

    }

    private void ChangeBoolSize(bool isadd)
    {
        if (changesize)
        {
            bordersize = (boolsize / 4) * 3;
            changesize = false;
        }
        if (isadd)
        {
            if (m_3deffect_changed.Length < bordersize || m_3deffectmenu_changed.Length < bordersize)
            {
                changesize = true;
                boolsize = bordersize;
                ChangeBoolSize();
            }
            Debug.Log("boolsize" + boolsize.ToString());
        }
        else
        {
            if (m_3deffect_changed.Length > bordersize || m_3deffectmenu_changed.Length > bordersize)
            {
                changesize = true;
                boolsize = bordersize * 2;
                ChangeBoolSize();

            }
            Debug.Log("boolsize" + boolsize.ToString());
        }
    }*/

    //=====================================================
    private void SetShowValue(ref int index)
    {
        switch (m_3deffect[index].m_Model3DEffect)
        {
            case model3deffect.OutBorder:
                //OutBorder(ref index);
                break;
        }
    }

    private bool OutBorder(ref List<Effect3DData> _3effect, ref int index)
    {
        HsysOutBorder _outborder = _3effect[index].m_EffectItem as HsysOutBorder;

        
        ref List<Hsys.OutBorder.OutBorderData> outborderlist = ref _outborder.GetOutBorderDataList();
        if(outborderlist == null) { Debug.LogWarning("未获取 HsysOutBorder 的数据 查看函数 ==> OutBorder(Hsys3DEffectsBaseMenuItem) => GetOutBorderDataList"); return false; }
        int outbordercount = outborderlist.Count;
        for (int index2 = 0; index2 < outbordercount; index2 +=1)
        {
            //outborderlist[index]._Material = new Material(Shader.Find("")
        }
        return false;
    }

    
}
