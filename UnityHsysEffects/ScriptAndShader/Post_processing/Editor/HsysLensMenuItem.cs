using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HsysLens))]
public class HsysLensMenuItem : Editor
{
    private SerializedObject obj;
    private HsysLens m_hsyslensclass;
    private List<Hsys.Lens.LensData> m_hsyslens;
    private GUIContent tooltip;
    private bool[] m_hsyslens_changed;

    private void OnEnable()
    {
        tooltip = new GUIContent();
        obj = new SerializedObject(target);
        m_hsyslens_changed = new bool[8];
        for (int index = 0; index < m_hsyslens_changed.Length; index += 1)
        {
            m_hsyslens_changed[index] = true;
            m_hsyslensclass = obj.targetObject as HsysLens;
        }
    }

    public override void OnInspectorGUI()
    {
        m_hsyslens = obj.FindProperty("m_lens").GetUnderlyingValue() as List<Hsys.Lens.LensData>;
        if (m_hsyslensclass == null) return;
        m_hsyslens_changed[0] = EditorGUILayout.Foldout(m_hsyslens_changed[0], "LensList");
        if (m_hsyslens_changed[0])
        {
            for (int index = 0; index < m_hsyslens.Count; index += 1)
            {
                m_hsyslens_changed[1] = EditorGUILayout.Foldout(m_hsyslens_changed[1], "[Lens:" + index.ToString() + "] " + m_hsyslens[index].m_LensType.ToString());
                if (m_hsyslens_changed[1])
                {
                    EditorGUILayout.BeginVertical("box");
                    SetShowValue(ref index);
                    m_hsyslensclass.CaseTypeOfShader(ref index);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("添加"))
            {
                m_hsyslensclass.AddPushLensData();
            }
            if (GUILayout.Button("删除"))
            {
                m_hsyslensclass.DeletePopLensData();
            }
            EditorGUILayout.EndHorizontal();
        }
        obj.ApplyModifiedProperties();
    }

    private void SetShowValue(ref int index)
    {
        EditorGUILayout.BeginVertical("box");
        m_hsyslens_changed[2] = EditorGUILayout.Foldout(m_hsyslens_changed[2], "Setting");
        if (m_hsyslens_changed[2])
        {
            tooltip.tooltip = "激活状态";
            tooltip.text = "IsEnable";
            m_hsyslens[index].m_IsEnable = EditorGUILayout.Toggle(tooltip, m_hsyslens[index].m_IsEnable);
            tooltip.tooltip = "Lens 类型";
            tooltip.text = "LensType";
            m_hsyslens[index].m_LensType = (Hsys.Lens.lenstype)EditorGUILayout.EnumPopup(tooltip, m_hsyslens[index].m_LensType);
            tooltip.tooltip = "Lens 精度";
            tooltip.text = "LensAccuracy";
            m_hsyslens[index].m_LensAccuracy = (Hsys.GlobalSetting.accuracy)EditorGUILayout.EnumPopup(tooltip, m_hsyslens[index].m_LensAccuracy);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        m_hsyslens_changed[3] = EditorGUILayout.Foldout(m_hsyslens_changed[3], "Attribute");
        if (m_hsyslens_changed[3])
        {
            CaseTypeAttributeListShow(ref index);

        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        m_hsyslens_changed[4] = EditorGUILayout.Foldout(m_hsyslens_changed[4], "Material");
        if (m_hsyslens_changed[4])
        {
            tooltip.tooltip = "材质";
            tooltip.text = "Material";
            m_hsyslens[index]._Material = EditorGUILayout.ObjectField(tooltip, m_hsyslens[index]._Material, typeof(Material), true) as Material;
        }
        EditorGUILayout.EndVertical();
    }

    private void CaseTypeAttributeListShow(ref int index)
    {
        switch (m_hsyslens[index].m_LensType)
        {
            case Hsys.Lens.lenstype.Twirl:
                LensTwirlMenu(ref index);
                break;
            case Hsys.Lens.lenstype.Twist:
                LensTwistMenu(ref index);
                break;
        }
    }

    private void LensTwirlMenu(ref int index)
    {
        tooltip.tooltip = "中心点";
        tooltip.text = "CenterX";
        m_hsyslens[index].Center.x = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Center.x, -5f,5f);
        tooltip.text = "CenterY";
        m_hsyslens[index].Center.y = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Center.y, -5f,5f);

        tooltip.tooltip = "半径";
        tooltip.text = "RediusX";
        m_hsyslens[index].Redius.x = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Redius.x, -5f, 5f);
        tooltip.text = "RediusY";
        m_hsyslens[index].Redius.y = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Redius.y, -5f, 5f);

        tooltip.tooltip = "角度";
        tooltip.text = "Angle";
        m_hsyslens[index].Angle = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Angle, -180f, 180f);
    }

    private void LensTwistMenu(ref int index)
    {
        tooltip.tooltip = "中心点";
        tooltip.text = "CenterX";
        m_hsyslens[index].Center.x = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Center.x, -5f, 5f);
        tooltip.text = "CenterY";
        m_hsyslens[index].Center.y = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Center.y, -5f, 5f);

        tooltip.tooltip = "半径";
        tooltip.text = "RediusX";
        m_hsyslens[index].Redius.x = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Redius.x, -5f, 5f);
        tooltip.text = "RediusY";
        m_hsyslens[index].Redius.y = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Redius.y, -5f, 5f);

        tooltip.tooltip = "角度";
        tooltip.text = "Angle";
        m_hsyslens[index].Angle = EditorGUILayout.Slider(tooltip, m_hsyslens[index].Angle, -180f, 180f);
    }
}
