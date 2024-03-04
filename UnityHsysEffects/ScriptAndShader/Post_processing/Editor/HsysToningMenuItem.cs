using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(HsysToning))]
public class HsysToningMenuItem : Editor
{
    private SerializedObject obj;
    private HsysToning m_hsystoningclass;
    private List<Hsys.Toning.ToningData> m_hsystoning;
    private GUIContent tooltip;
    private bool[] m_hsystoning_changed;

    private void OnEnable()
    {
        tooltip = new GUIContent();
        obj = new SerializedObject(target);
        m_hsystoning_changed = new bool[8];
        for (int index = 0; index < m_hsystoning_changed.Length; index += 1)
        {
            m_hsystoning_changed[index] = true;
            m_hsystoningclass = obj.targetObject as HsysToning;
        }
    }

    public override void OnInspectorGUI()
    {
        m_hsystoning = obj.FindProperty("m_toning").GetUnderlyingValue() as List<Hsys.Toning.ToningData>;
        if (m_hsystoningclass == null) return;
        m_hsystoning_changed[0] = EditorGUILayout.Foldout(m_hsystoning_changed[0], "ToningList");
        if (m_hsystoning_changed[0])
        {
            for (int index = 0; index < m_hsystoning.Count; index += 1)
            {
                m_hsystoning_changed[1] = EditorGUILayout.Foldout(m_hsystoning_changed[1], "[Toning:" + index.ToString() + "] " + m_hsystoning[index].m_ToningType.ToString());
                if (m_hsystoning_changed[1])
                {
                    EditorGUILayout.BeginVertical("box");
                    SetShowValue(ref index);
                    m_hsystoningclass.CaseTypeOfShader(ref index);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("添加"))
            {
                m_hsystoningclass.AddPushBloomData();
            }
            if (GUILayout.Button("删除"))
            {
                m_hsystoningclass.DeletePopBloomData();
            }
            EditorGUILayout.EndHorizontal();
        }
        obj.ApplyModifiedProperties();
    }


    private void SetShowValue(ref int index)
    {
        EditorGUILayout.BeginVertical("box");
        m_hsystoning_changed[2] = EditorGUILayout.Foldout(m_hsystoning_changed[2], "Setting");
        if (m_hsystoning_changed[2])
        {
            tooltip.tooltip = "激活状态";
            tooltip.text = "IsEnable";
            m_hsystoning[index].m_IsEnable = EditorGUILayout.Toggle(tooltip, m_hsystoning[index].m_IsEnable);
            tooltip.tooltip = "Toning 类型";
            tooltip.text = "BlurType";
            m_hsystoning[index].m_ToningType = (Hsys.Toning.toningtype)EditorGUILayout.EnumPopup(tooltip, m_hsystoning[index].m_ToningType);
            tooltip.tooltip = "Toning 精度";
            tooltip.text = "BlurAccuracy";
            m_hsystoning[index].m_ToningAccuracy = (Hsys.GlobalSetting.accuracy)EditorGUILayout.EnumPopup(tooltip, m_hsystoning[index].m_ToningAccuracy);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        m_hsystoning_changed[3] = EditorGUILayout.Foldout(m_hsystoning_changed[3], "Attribute");
        if (m_hsystoning_changed[3])
        {
            CaseTypeAttributeListShow(ref index);

        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        m_hsystoning_changed[4] = EditorGUILayout.Foldout(m_hsystoning_changed[4], "Material");
        if (m_hsystoning_changed[4])
        {
            tooltip.tooltip = "材质";
            tooltip.text = "Material";
            m_hsystoning[index]._Material = EditorGUILayout.ObjectField(tooltip, m_hsystoning[index]._Material, typeof(Material), true) as Material;
        }
        EditorGUILayout.EndVertical();
    }


    private void CaseTypeAttributeListShow(ref int index)
    {
        switch (m_hsystoning[index].m_ToningType)
        {
            case Hsys.Toning.toningtype.Brightness:
                ToninBrightnessMenu(ref index);
                break;
            case Hsys.Toning.toningtype.Saturation:
                ToningSaturationMenu(ref index);
                break;
            case Hsys.Toning.toningtype.Vibrance:
                ToningVibranceMenu(ref index);
                break;
            case Hsys.Toning.toningtype.Level:
                ToningLevelMenu(ref index);
                break;
            case Hsys.Toning.toningtype.ContrastRatio:
                ToningContrastRatioMenu(ref index);
                break;
            case Hsys.Toning.toningtype.ColorGrading:
                ToningColorGradingMenu(ref index);
                break;
            case Hsys.Toning.toningtype.ColorEqualizer:
                TongingColorEqualizer(ref index);
                break;
        }
    }

    private void ToninBrightnessMenu(ref int index)
    {
        tooltip.tooltip = "强度";
        tooltip.text = "Strength";
        m_hsystoning[index].Strength = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Strength, 0f, 5f);
    }

    private void ToningSaturationMenu(ref int index)
    {
        tooltip.tooltip = "强度";
        tooltip.text = "Strength";
        m_hsystoning[index].Strength = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Strength, 0.01f, 5f);
    }

    private void ToningVibranceMenu(ref int index)
    {
        tooltip.tooltip = "范围";
        tooltip.text = "Range";
        m_hsystoning[index].Range = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Range, 0.01f, 5f);
    }

    private void ToningLevelMenu(ref int index)
    {
        tooltip.tooltip = "是否应用VRAR中";
        tooltip.text = "Is_VR_AR";
        m_hsystoning[index].is_VR_AR = EditorGUILayout.Toggle(tooltip, m_hsystoning[index].is_VR_AR);
        tooltip.tooltip = "伽马值";
        tooltip.text = "Strength";
        m_hsystoning[index].Strength = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Strength, 0.01f, 3f);
        tooltip.tooltip = "最大最小输入输出";
        tooltip.text = "MinInput";
        m_hsystoning[index].MinInput = EditorGUILayout.Slider(tooltip, m_hsystoning[index].MinInput, 0.01f, 1f);
        tooltip.text = "MaxInput";
        m_hsystoning[index].MaxInput = EditorGUILayout.Slider(tooltip, m_hsystoning[index].MaxInput, 0.01f, 1f);
        tooltip.text = "MinOutput";
        m_hsystoning[index].MinOutput = EditorGUILayout.Slider(tooltip, m_hsystoning[index].MinOutput, 0.01f, 1f);
        tooltip.text = "MaxOutput";
        m_hsystoning[index].MaxOutput = EditorGUILayout.Slider(tooltip, m_hsystoning[index].MaxOutput, 0.01f, 1f);
    }
    private void ToningContrastRatioMenu(ref int index)
    {
        tooltip.tooltip = "强度";
        tooltip.text = "Strength";
        m_hsystoning[index].Contrast = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Contrast, 0.01f, 5f);
    }

    private void ToningColorGradingMenu(ref int index)
    {
        tooltip.tooltip = "强度";
        tooltip.text = "Strength";
        m_hsystoning[index].Strength = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Strength, 0.0f, 1f);
        tooltip.tooltip = "Hue_Saturation_Value";
        tooltip.text = "Hue_Saturation_Value(HUE)";
        m_hsystoning[index].Hue_Saturation_Value.HUE = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Hue_Saturation_Value.HUE, 0.01f, 1f);
        tooltip.text = "Hue_Saturation_Value(Saturation)";
        m_hsystoning[index].Hue_Saturation_Value.Saturation = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Hue_Saturation_Value.Saturation, 0.01f, 2f);
        tooltip.text = "Hue_Saturation_Value(Value)";
        m_hsystoning[index].Hue_Saturation_Value.Value = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Hue_Saturation_Value.Value, 0.01f, 1f);
    }


    private enum redgreenblue
    {
        RED, GREEN, BLUE
    }
    private redgreenblue is_redgreenblue;
    private void TongingColorEqualizer(ref int index)
    {
        EditorGUILayout.BeginVertical("box");
        tooltip.tooltip = "RGB通道";
        tooltip.text = "RGB Cennel";
        is_redgreenblue = (redgreenblue)EditorGUILayout.EnumPopup(tooltip, is_redgreenblue);
        switch(is_redgreenblue)
        {
            case redgreenblue.RED:
                tooltip.text = "RGB Red(X)";
                m_hsystoning[index].Red.x = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Red.x, -1f, 1f);
                tooltip.text = "RGB Red(Y)";
                m_hsystoning[index].Red.y = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Red.y, -1f, 1f);
                tooltip.text = "RGB Red(Z)";
                m_hsystoning[index].Red.z = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Red.z, -1f, 1f);
                tooltip.text = "RGBC(X)";
                m_hsystoning[index].RGBC.x = EditorGUILayout.Slider(tooltip, m_hsystoning[index].RGBC.x, -1f, 1f);
                break;
            case redgreenblue.GREEN:
                tooltip.text = "RGB Green(X)";
                m_hsystoning[index].Green.x = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Green.x, -1f, 1f);
                tooltip.text = "RGB Green(Y)";
                m_hsystoning[index].Green.y = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Green.y, -1f, 1f);
                tooltip.text = "RGB Green(Z)";
                m_hsystoning[index].Green.z = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Green.z, -1f, 1f);
                tooltip.text = "RGBC(Y)";
                m_hsystoning[index].RGBC.y = EditorGUILayout.Slider(tooltip, m_hsystoning[index].RGBC.y, -1f, 1f);
                break;
            case redgreenblue.BLUE:
                tooltip.text = "RGB Blue(X)";
                m_hsystoning[index].Blue.x = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Blue.x, -1f, 1f);
                tooltip.text = "RGB Blue(Y)";
                m_hsystoning[index].Blue.y = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Blue.y, -1f, 1f);
                tooltip.text = "RGB Blue(Z)";
                m_hsystoning[index].Blue.z = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Blue.z, -1f, 1f);
                tooltip.text = "RGBC(Z)";
                m_hsystoning[index].RGBC.z = EditorGUILayout.Slider(tooltip, m_hsystoning[index].RGBC.z, -1f, 1f);
                break;
        }
        EditorGUILayout.EndVertical();
        tooltip.tooltip = "强度";
        tooltip.text = "Strength";
        m_hsystoning[index].Strength = EditorGUILayout.Slider(tooltip, m_hsystoning[index].Strength, 0.01f, 1f);
        tooltip.text = "RGB Blue(Y)";


    }
}
