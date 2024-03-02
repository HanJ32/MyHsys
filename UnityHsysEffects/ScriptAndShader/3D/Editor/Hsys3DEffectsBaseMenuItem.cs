using Hsys.Effect3DBase;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Hsys3DEffectBase))]
public class Hsys3DEffectsBaseMenuItem : Editor
{
    private SerializedObject obj;
    private Hsys3DEffectBase m_3deffectbaseclass;
    private List<Effect3DData> m_3deffect;
    private GUIContent tooltip;
    private bool[] m_3deffect_changed;
    private void OnEnable()
    {
        m_3deffect_changed = new bool[8];
        tooltip = new GUIContent();
        obj = new SerializedObject(target);
        for (int index = 0; index < m_3deffect_changed.Length; index += 1)
        {
            m_3deffect_changed[index] = true;
            m_3deffectbaseclass = obj.targetObject as Hsys3DEffectBase;
        }
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //if (blurlist == null) return;
        m_3deffect = obj.FindProperty("m_3deffectlist").GetUnderlyingValue() as List<Effect3DData>;
        if (m_3deffect == null) return;

        //EditorGUILayout.PropertyField(blurlist);
        tooltip.tooltip = "3D Effect 队列";
        tooltip.text = "3D Effect List";

        if(GUILayout.Button(tooltip)) {m_3deffect_changed[0] = !m_3deffect_changed[0];}
        if (m_3deffect_changed[0])
        {
            for (int index = 0; index < m_3deffect.Count; index += 1)
            {

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginVertical("box");
                m_3deffect_changed[1] = EditorGUILayout.Foldout(m_3deffect_changed[1], "ListOne");
                if (m_3deffect_changed[0])
                {
                    tooltip.tooltip = "激活状态";
                    tooltip.text = "IsEnable";
                    m_3deffect[index].m_IsEnable = EditorGUILayout.Toggle(tooltip, m_3deffect[index].m_IsEnable);
                    tooltip.tooltip = "处理的类别";
                    tooltip.text = "Model3Deffect";
                    m_3deffect[index].m_Model3Deffect = (Hsys.Effect3DBase.model3deffect)EditorGUILayout.EnumPopup(tooltip, m_3deffect[index].m_Model3Deffect);
                    tooltip.tooltip = "处理的对象";
                    tooltip.text = "EffectItem";
                    //m_3deffect[index].m_EffectItem = EditorGUILayout.ObjectField(tooltip, m_3deffect[index].m_EffectItem, , true);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("添加"))
            {
                m_3deffectbaseclass.AddPush3DEffectData();
            }
            if (GUILayout.Button("删除"))
            {
                m_3deffectbaseclass.DeletePop3DEffectData();
            }
            EditorGUILayout.EndHorizontal();
        }
        //Debug.Log(blurlist.GetEndProperty().name);
        obj.ApplyModifiedProperties();
    }

    private void SetShowValue(ref int index)
    {
    }

    
}
