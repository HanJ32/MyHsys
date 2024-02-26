
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HsysBlur))]
public class HsysBlurMenuItem : Editor
{
    private SerializedObject obj;
    private HsysBlur m_hsysblurclass;
    private List<Hsys.Blur.BlurData> m_hsysblur;
    private GUIContent tooltip;
    private bool[] m_hsysblur_changed;
    private void OnEnable()
    {
        m_hsysblur_changed = new bool[8];
        tooltip = new GUIContent();
        obj = new SerializedObject(target);
        for(int index = 0; index < m_hsysblur_changed.Length; index+=1)
        {
            m_hsysblur_changed[index] = true;
            m_hsysblurclass = obj.targetObject as HsysBlur;
        }
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //if (blurlist == null) return;
        
        m_hsysblur = obj.FindProperty("m_blur").GetUnderlyingValue() as List<Hsys.Blur.BlurData>;
        if (m_hsysblur == null) return;
        //EditorGUILayout.PropertyField(blurlist);

        m_hsysblur_changed[3] = EditorGUILayout.Foldout(m_hsysblur_changed[3],"BlurList");
        if (m_hsysblur_changed[3])
        {
            for (int index = 0; index < m_hsysblur.Count; index += 1)
            {
                m_hsysblur_changed[4] = EditorGUILayout.Foldout(m_hsysblur_changed[4], "[Blur:" + index.ToString()+"] " + m_hsysblur[index].m_BlurType.ToString());
                if(m_hsysblur_changed[4])
                {
                    EditorGUILayout.BeginVertical("box");
                    SetShowValue(ref index);
                    m_hsysblurclass.CaseTypeOfShader(ref index);
                    EditorGUILayout.EndVertical();
                }
                
            }

            EditorGUILayout.BeginHorizontal("box");
            if(GUILayout.Button("添加"))
            {
                m_hsysblurclass.AddPushBlurData();
            }
            if(GUILayout.Button("删除"))
            {
                m_hsysblurclass.DeletePopBlurData();
            }
            EditorGUILayout.EndHorizontal();
        }
        //Debug.Log(blurlist.GetEndProperty().name);
        obj.ApplyModifiedProperties();
    }

    private void SetShowValue(ref int index)
    {
        EditorGUILayout.BeginVertical("box");
        m_hsysblur_changed[0] = EditorGUILayout.Foldout(m_hsysblur_changed[0], "Setting");
        if (m_hsysblur_changed[0])
        {
            tooltip.tooltip = "激活状态";
            tooltip.text = "IsEnable";
            m_hsysblur[index].m_IsEnable = EditorGUILayout.Toggle(tooltip, m_hsysblur[index].m_IsEnable);
            tooltip.tooltip = "Blur 类型";
            tooltip.text = "BlurType";
            m_hsysblur[index].m_BlurType = (Hsys.Blur.blurtype)EditorGUILayout.EnumPopup(tooltip, m_hsysblur[index].m_BlurType);
            tooltip.tooltip = "Blur 精度";
            tooltip.text = "BlurAccuracy";
            m_hsysblur[index].m_BlurAccuracy = (Hsys.GlobalSetting.accuracy)EditorGUILayout.EnumPopup(tooltip, m_hsysblur[index].m_BlurAccuracy);
            tooltip.tooltip = "Blur 质量";
            tooltip.text = "BlurQuality";
            m_hsysblur[index].m_Quality = (Hsys.GlobalSetting.quality)EditorGUILayout.EnumPopup(tooltip, m_hsysblur[index].m_Quality);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        m_hsysblur_changed[1] = EditorGUILayout.Foldout(m_hsysblur_changed[1], "Attribute");
        if (m_hsysblur_changed[1])
        {
            CaseTypeAttributeListShow(ref index);
            
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        m_hsysblur_changed[2] = EditorGUILayout.Foldout(m_hsysblur_changed[2], "Material");
        if (m_hsysblur_changed[2])
        {
            tooltip.tooltip = "材质";
            tooltip.text = "Material";
            m_hsysblur[index]._Material = EditorGUILayout.ObjectField(tooltip, m_hsysblur[index]._Material, typeof(Material), true) as Material;
        }
        EditorGUILayout.EndVertical();
    }

    private void CaseTypeAttributeListShow(ref int index)
    {
        switch(m_hsysblur[index].m_BlurType)
        {
            case Hsys.Blur.blurtype.BOX:
                BlurBoxMenu(ref index);
                break;
            case Hsys.Blur.blurtype.Bokeh:
                BlurBokehMenu(ref index);
                break;
            case Hsys.Blur.blurtype.TiltShift:
                BlurTiltShiftMenu(ref index);
                break;
            case Hsys.Blur.blurtype.Iris:
                BlurIrisMenu(ref index);
                break;
            case Hsys.Blur.blurtype.Grainy:
                BlurGrainyMenu(ref index);
                break;
            case Hsys.Blur.blurtype.Radial:
                BlurRadialMenu(ref index);
                break;
            case Hsys.Blur.blurtype.Directional:
                BlurDirectionalMenu(ref index);
                break;
        }
    }

    private void BlurBoxMenu(ref int index)
    {
        switch (m_hsysblur[index].m_Quality)
        {
            case Hsys.GlobalSetting.quality.Low:
                break;
            case Hsys.GlobalSetting.quality.Middle:
                break;
            case Hsys.GlobalSetting.quality.High:
                break;
        }

        tooltip.tooltip = "开启高采样";
        tooltip.text = "HighSampler";
        m_hsysblur[index].HighSampler = EditorGUILayout.Toggle(tooltip, m_hsysblur[index].HighSampler);
        tooltip.tooltip = "模糊程度";
        tooltip.text = "Blur";
        m_hsysblur[index].Blur = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Blur, 0.01f, 1f);

        //tooltip.tooltip = "幂次数";
        //tooltip.text = "PowCount";
        //m_hsysblur[index].Count = EditorGUILayout.IntSlider(tooltip, m_hsysblur[index].Count, 1, 5);
        tooltip.tooltip = "横向偏移";
        tooltip.text = "TexelSizeX";
        m_hsysblur[index].TexelSize.x = EditorGUILayout.Slider(tooltip, m_hsysblur[index].TexelSize.x, -1f, 1f);
        tooltip.tooltip = "纵向偏移";
        tooltip.text = "TexelSizeY";
        m_hsysblur[index].TexelSize.y = EditorGUILayout.Slider(tooltip, m_hsysblur[index].TexelSize.y, -1f, 1f);
    }
    private void BlurBokehMenu(ref int index)
    {
        switch (m_hsysblur[index].m_Quality)
        {
            case Hsys.GlobalSetting.quality.Low:
                break;
            case Hsys.GlobalSetting.quality.Middle:
                break;
            case Hsys.GlobalSetting.quality.High:
                break;
        }
        tooltip.tooltip = "模糊程度";
        tooltip.text = "Blur";
        m_hsysblur[index].Blur = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Blur, 0.01f, 1f);
        tooltip.tooltip = "迭代次数";
        tooltip.text = "Count";
        m_hsysblur[index].Count = EditorGUILayout.IntSlider(tooltip, m_hsysblur[index].Count, 1, 30);
        tooltip.tooltip = "模糊半径";
        tooltip.text = "Redius";
        m_hsysblur[index].Redius = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Redius, 0.01f, 10f);
        EditorGUILayout.BeginVertical("box");
        m_hsysblur_changed[5] = EditorGUILayout.Foldout(m_hsysblur_changed[5], "偏移");
        if (m_hsysblur_changed[5])
        {
            tooltip.tooltip = "旋转偏移";
            tooltip.text = "OffsetX";
            m_hsysblur[index].Offset.x = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Offset.x, -1f, 1f);
            tooltip.tooltip = "旋转偏移";
            tooltip.text = "OffsetY";
            m_hsysblur[index].Offset.y = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Offset.y, -1f, 1f);
            tooltip.tooltip = "旋转偏移";
            tooltip.text = "OffsetZ";
            m_hsysblur[index].Offset.z = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Offset.z, -1f, 1f);
            tooltip.tooltip = "旋转偏移";
            tooltip.text = "OffsetW";
            m_hsysblur[index].Offset.w = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Offset.w, -1f, 1f);
        }
        EditorGUILayout.EndVertical();
        tooltip.tooltip = "像素尺寸";
        tooltip.text = "PixelSize";
        m_hsysblur[index].PixelSize = EditorGUILayout.Slider(tooltip, m_hsysblur[index].PixelSize, -5f, 5f);
    }
    private void BlurTiltShiftMenu(ref int index)
    {
        switch (m_hsysblur[index].m_Quality)
        {
            case Hsys.GlobalSetting.quality.Low:
                tooltip.tooltip = "模糊程度";
                tooltip.text = "Blur";
                m_hsysblur[index].Blur = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Blur, 0.01f, 1f);
                tooltip.tooltip = "迭代次数";
                tooltip.text = "Count";
                m_hsysblur[index].Count = EditorGUILayout.IntSlider(tooltip, m_hsysblur[index].Count, 1, 30);
                tooltip.tooltip = "区域大小程度";
                tooltip.text = "AreaSize";
                m_hsysblur[index].AreaSize = EditorGUILayout.Slider(tooltip, m_hsysblur[index].AreaSize, 0.01f, 3f);
                break;
            case Hsys.GlobalSetting.quality.Middle:
                tooltip.tooltip = "模糊程度";
                tooltip.text = "Blur";
                m_hsysblur[index].Blur = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Blur, 0.01f, 1f);
                tooltip.tooltip = "迭代次数";
                tooltip.text = "Count";
                m_hsysblur[index].Count = EditorGUILayout.IntSlider(tooltip, m_hsysblur[index].Count, 1, 30);
                tooltip.tooltip = "区域大小程度";
                tooltip.text = "AreaSize";
                m_hsysblur[index].AreaSize = EditorGUILayout.Slider(tooltip, m_hsysblur[index].AreaSize, 0.01f, 3f);
                tooltip.tooltip = "扩散范围";
                tooltip.text = "Spread";
                m_hsysblur[index].Spread = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Spread, 0.01f, 10f);
                tooltip.tooltip = "模糊半径";
                tooltip.text = "Redius";
                m_hsysblur[index].Redius = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Redius, 0.01f, 10f);

                tooltip.tooltip = "偏移";
                tooltip.text = "Offset";
                m_hsysblur[index].Offset.x = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Offset.x, -1f, 1f);
                //tooltip.text = "ParamsX";
                //m_hsysblur[index].Params.x = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Params.x, -1f, 1f);
                m_hsysblur[index].Params.x = m_hsysblurclass.GetCamera().pixelWidth;
                m_hsysblur[index].Params.y = m_hsysblurclass.GetCamera().pixelHeight;
                //tooltip.text = "ParamsY";
                //m_hsysblur[index].Params.y = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Params.y, -1f, 1f);
                break;
            case Hsys.GlobalSetting.quality.High:
                break;
        }
       

    }
    private void BlurIrisMenu(ref int index)
    {
        switch (m_hsysblur[index].m_Quality)
        {
            case Hsys.GlobalSetting.quality.Low:
                break;
            case Hsys.GlobalSetting.quality.Middle:
                break;
            case Hsys.GlobalSetting.quality.High:
                break;
        }
        tooltip.tooltip = "模糊程度";
        tooltip.text = "Blur";
        m_hsysblur[index].Blur = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Blur, 0.01f, 1f);
        tooltip.tooltip = "区域大小程度";
        tooltip.text = "AreaSize";
        m_hsysblur[index].AreaSize = EditorGUILayout.Slider(tooltip, m_hsysblur[index].AreaSize, 0.01f, 1f);
        tooltip.tooltip = "迭代次数";
        tooltip.text = "Count";
        m_hsysblur[index].Count = EditorGUILayout.IntSlider(tooltip, m_hsysblur[index].Count, 1, 30);
        tooltip.tooltip = "偏移";
        tooltip.text = "ParamsX";
        m_hsysblur[index].Params.x = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Params.x, -1f, 1f);
        tooltip.text = "ParamsY";
        m_hsysblur[index].Params.y = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Params.y, -1f, 1f);
        tooltip.tooltip = "中心点";
        tooltip.text = "CenterX";
        m_hsysblur[index].Center.x = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Center.x, -1f, 1f);
        tooltip.text = "CenterY";
        m_hsysblur[index].Center.y = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Center.y, -1f, 1f);
        tooltip.tooltip = "缓冲半径";
        tooltip.text = "Radius";
        m_hsysblur[index].Redius = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Redius, 0f, 1f);
    }
    private void BlurGrainyMenu(ref int index)
    {
        switch (m_hsysblur[index].m_Quality)
        {
            case Hsys.GlobalSetting.quality.Low:
                break;
            case Hsys.GlobalSetting.quality.Middle:
                break;
            case Hsys.GlobalSetting.quality.High:
                break;
        }
        tooltip.tooltip = "粒度";
        tooltip.text = "Blur";
        m_hsysblur[index].Blur = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Blur, 0.01f, 1f);
        tooltip.tooltip = "平滑";
        tooltip.text = "Count";
        m_hsysblur[index].Count = EditorGUILayout.IntSlider(tooltip, m_hsysblur[index].Count, 1, 30);
        tooltip.tooltip = "影响系数";
        tooltip.text = "Effect";
        m_hsysblur[index].Effect = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Effect, 0.01f, 1f);
    }

    private void BlurRadialMenu(ref int index)
    {
        switch (m_hsysblur[index].m_Quality)
        {
            case Hsys.GlobalSetting.quality.Low:
                break;
            case Hsys.GlobalSetting.quality.Middle:
                break;
            case Hsys.GlobalSetting.quality.High:
                break;
        }
        EditorGUILayout.BeginVertical("box");
        tooltip.tooltip = "开启带圈半径";
        tooltip.text = "EnableRadius";
        m_hsysblur[index].HighSampler = EditorGUILayout.Toggle(tooltip, m_hsysblur[index].HighSampler);
        if (m_hsysblur[index].HighSampler)
        {
            tooltip.tooltip = "缓冲半径";
            tooltip.text = "Radius";
            m_hsysblur[index].Redius = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Redius, 0f, 1f);
            tooltip.tooltip = "中心点X";
            tooltip.text = "CenterX";
            m_hsysblur[index].Center.x = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Center.x, -1f, 1f);
            tooltip.tooltip = "中心点Y";
            tooltip.text = "CenterY";
            m_hsysblur[index].Center.y = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Center.y, -1f, 1f);
        }
        EditorGUILayout.EndVertical();
        tooltip.tooltip = "模糊程度";
        tooltip.text = "Blur";
        m_hsysblur[index].Blur = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Blur, 0.01f, 1f);
        tooltip.tooltip = "迭代次数";
        tooltip.text = "Count";
        m_hsysblur[index].Count = EditorGUILayout.IntSlider(tooltip, m_hsysblur[index].Count, 1, 30);
        tooltip.tooltip = "视角偏移X";
        tooltip.text = "ParamsX";
        m_hsysblur[index].Params.x = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Params.x, -1f, 1f);
        tooltip.tooltip = "视角偏移Y";
        tooltip.text = "ParamsY";
        m_hsysblur[index].Params.y = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Params.y, -1f, 1f);
    }
    private void BlurDirectionalMenu(ref int index)
    {
        switch (m_hsysblur[index].m_Quality)
        {
            case Hsys.GlobalSetting.quality.Low:
                break;
            case Hsys.GlobalSetting.quality.Middle:
                break;
            case Hsys.GlobalSetting.quality.High:
                break;
        }
        tooltip.tooltip = "模糊程度";
        tooltip.text = "Blur";
        m_hsysblur[index].Blur = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Blur, 0.01f, 10f);
        tooltip.tooltip = "迭代次数";
        tooltip.text = "Count";
        m_hsysblur[index].Count = EditorGUILayout.IntSlider(tooltip, m_hsysblur[index].Count, 1, 30);
        EditorGUILayout.BeginVertical("box");
        m_hsysblur_changed[6] = EditorGUILayout.Foldout(m_hsysblur_changed[6], "偏移");
        if (m_hsysblur_changed[6])
        {
            //tooltip.tooltip = "视角偏移";
            //tooltip.text = "ParamsX";
            //m_hsysblur[index].Params.x = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Params.x, -1f, 1f);
            tooltip.tooltip = "角度";
            tooltip.text = "Angle";
            m_hsysblur[index].Angle = EditorGUILayout.Slider(tooltip, m_hsysblur[index].Angle, 0f, 3f);
        }
        EditorGUILayout.EndVertical();
    }
}
