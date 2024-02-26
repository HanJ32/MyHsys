using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HsysBloom))]
public class HsysBloomMenuItem : Editor
{
    private SerializedObject obj;
    private HsysBloom m_hsysbloomclass;
    private List<Hsys.Bloom.BloomData> m_hsysbloom;
    private GUIContent tooltip;
    private bool[] m_hsysbloom_changed;
    private void OnEnable()
    {
        tooltip = new GUIContent();
        obj = new SerializedObject(target);
        m_hsysbloom_changed = new bool[8];
        for (int index = 0; index < m_hsysbloom_changed.Length; index += 1)
        {
            m_hsysbloom_changed[index] = true;
            m_hsysbloomclass = obj.targetObject as HsysBloom;
        }
    }
    public override void OnInspectorGUI()
    {
        m_hsysbloom = obj.FindProperty("m_bloom").GetUnderlyingValue() as List<Hsys.Bloom.BloomData>;
        if (m_hsysbloomclass == null) return;
        m_hsysbloom_changed[0] = EditorGUILayout.Foldout(m_hsysbloom_changed[0], "BloomList");
        if (m_hsysbloom_changed[0])
        {
            for (int index = 0; index < m_hsysbloom.Count; index += 1)
            {
                m_hsysbloom_changed[1] = EditorGUILayout.Foldout(m_hsysbloom_changed[1], "[Bloom:" + index.ToString() + "] " + m_hsysbloom[index].m_BloomType.ToString());
                if (m_hsysbloom_changed[1])
                {
                    EditorGUILayout.BeginVertical("box");
                    SetShowValue(ref index);
                    m_hsysbloomclass.CaseTypeOfShader(ref index);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("添加"))
            {
                m_hsysbloomclass.AddPushBloomData();
            }
            if (GUILayout.Button("删除"))
            {
                m_hsysbloomclass.DeletePopBloomData();
            }
            EditorGUILayout.EndHorizontal();
        }
        obj.ApplyModifiedProperties();
    }


    private void SetShowValue(ref int index)
    {
        EditorGUILayout.BeginVertical("box");
        m_hsysbloom_changed[2] = EditorGUILayout.Foldout(m_hsysbloom_changed[2], "Setting");
        if (m_hsysbloom_changed[2])
        {
            tooltip.tooltip = "激活状态";
            tooltip.text = "IsEnable";
            m_hsysbloom[index].m_IsEnable = EditorGUILayout.Toggle(tooltip, m_hsysbloom[index].m_IsEnable);
            tooltip.tooltip = "Bloom 类型";
            tooltip.text = "BloomType";
            m_hsysbloom[index].m_BloomType = (Hsys.Bloom.bloomtype)EditorGUILayout.EnumPopup(tooltip, m_hsysbloom[index].m_BloomType);
            tooltip.tooltip = "Bloom 精度";
            tooltip.text = "BloomAccuracy";
            m_hsysbloom[index].m_BloomAcaccuracy = (Hsys.GlobalSetting.accuracy)EditorGUILayout.EnumPopup(tooltip, m_hsysbloom[index].m_BloomAcaccuracy);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        m_hsysbloom_changed[3] = EditorGUILayout.Foldout(m_hsysbloom_changed[3], "Attribute");
        if (m_hsysbloom_changed[3])
        {
            CaseTypeAttributeListShow(ref index);

        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        m_hsysbloom_changed[4] = EditorGUILayout.Foldout(m_hsysbloom_changed[4], "Material");
        if (m_hsysbloom_changed[4])
        {
            tooltip.tooltip = "材质";
            tooltip.text = "Material";
            m_hsysbloom[index]._Material = EditorGUILayout.ObjectField(tooltip, m_hsysbloom[index]._Material, typeof(Material), true) as Material;
        }
        EditorGUILayout.EndVertical();
    }


    private void CaseTypeAttributeListShow(ref int index)
    {
        switch(m_hsysbloom[index].m_BloomType)
        {
            case Hsys.Bloom.bloomtype.HDR:
                tooltip.tooltip = "阈值";
                tooltip.text = "Luminance";
                m_hsysbloom[index].Luminance = EditorGUILayout.Slider(tooltip, m_hsysbloom[index].Luminance, 0.01f, 1f);
                tooltip.tooltip = "强度";
                tooltip.text = "Strength";
                m_hsysbloom[index].Strength = EditorGUILayout.Slider(tooltip, m_hsysbloom[index].Strength, 0.01f,1f);
                tooltip.tooltip = "HDR颜色";
                tooltip.text = "HDRColor";
                m_hsysbloom[index].Color = EditorGUILayout.ColorField(tooltip, m_hsysbloom[index].Color,true,true,true);
                GUILayout.Label("Blur");
                tooltip.tooltip = "偏移";
                tooltip.text = "OffsetX";
                m_hsysbloom[index].Offset.x = EditorGUILayout.Slider(tooltip, m_hsysbloom[index].Offset.x, -1f, 1f);
                tooltip.text = "OffsetY";
                m_hsysbloom[index].Offset.y = EditorGUILayout.Slider(tooltip, m_hsysbloom[index].Offset.y, -1f, 1f);
                break;
            case Hsys.Bloom.bloomtype.NoHDR:
                break;
        }
    }
}
