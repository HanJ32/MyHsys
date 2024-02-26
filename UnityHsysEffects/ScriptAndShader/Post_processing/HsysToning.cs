using Hsys.Bloom;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HsysToning : MonoBehaviour
{
    [Tooltip("Toning 处理队列")][SerializeField] private List<Hsys.Toning.ToningData> m_toning;
    public Hsys.Toning.ToningEffects m_toningeffects = new Hsys.Toning.ToningEffects();
    private int passnum = 0;
    private bool m_isjmpdefaultrender = false;

    //private RenderTexture buffer = null;
    //private RenderTexture buffer2 = null;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
/*        if(buffer == null)
        {
            buffer = RenderTexture.GetTemporary(source.width, source.height, 0);
            Graphics.Blit(source, buffer);
            buffer2 = RenderTexture.GetTemporary(source.width, source.height, 0);
        }*/
        if (m_toning.Count == 0) { Graphics.Blit(source, destination); return; }
        for (int index = 0; index < m_toning.Count; index += 1)
        {
            if (m_toning[index].m_IsEnable == false)
            {
                Graphics.Blit(source, destination);
                //Graphics.Blit(buffer, buffer2);
                //Graphics.Blit(buffer, destination);
                continue;
            };
            //if (index != 0) { Graphics.CopyTexture(buffer2, buffer); }
            CaseTypeOfShader(ref index);
            switch (m_toning[index].m_ToningType)
            {
                case Hsys.Toning.toningtype.Brightness:
                    m_isjmpdefaultrender = m_toningeffects.ToningBrightness(ref m_toning,ref index);
                    break;
                case Hsys.Toning.toningtype.Saturation:
                    m_isjmpdefaultrender = m_toningeffects.ToningSaturation(ref m_toning,ref index);
                    break;
                case Hsys.Toning.toningtype.Vibrance:
                    m_isjmpdefaultrender = m_toningeffects.ToningVibrance(ref m_toning,ref index);
                    break;
                case Hsys.Toning.toningtype.Level:
                    m_isjmpdefaultrender = m_toningeffects.ToningLevel(ref m_toning,ref index, ref passnum);
                    break;
                case Hsys.Toning.toningtype.ContrastRatio:
                    m_isjmpdefaultrender = m_toningeffects.ToningContrastRatio(ref m_toning,ref index);
                    break;
                case Hsys.Toning.toningtype.ColorGrading:
                    m_isjmpdefaultrender = m_toningeffects.ToningColorGraying(ref m_toning,ref index);
                    break;
            }
            //TODO:多处理效果
            if (!m_isjmpdefaultrender) { Graphics.Blit(source, destination, m_toning[index]._Material, passnum); }
            //if (!m_isjmpdefaultrender) { Graphics.Blit(buffer, buffer2, m_toning[index]._Material, passnum); }
            m_isjmpdefaultrender = false;
            passnum = 0;
            
            //Graphics.Blit(buffer2, buffer);
        }
        //Graphics.Blit(buffer2, destination);
    }
    public void CaseTypeOfShader(ref int index)
    {
        switch (m_toning[index].m_ToningType)
        {
            case Hsys.Toning.toningtype.Brightness:
                if (m_toning[index]._Material == null)
                {
                    m_toning[index]._Material = new Material(Shader.Find("Hsys/ZToning/Brightness"));
                }
                if (m_toning[index]._Material.shader.name == "Hsys/ZToning/Brightness") break;
                m_toning[index]._Material.shader = Shader.Find("Hsys/ZToning/Brightness");
                m_toning[index]._Material.name = "Hsys/ZBloom/Brightness";
                break;
            case Hsys.Toning.toningtype.Saturation:
                if (m_toning[index]._Material == null)
                {
                    m_toning[index]._Material = new Material(Shader.Find("Hsys/ZToning/Saturation"));
                }
                if (m_toning[index]._Material.shader.name == "Hsys/ZToning/Saturation") break;
                m_toning[index]._Material.shader = Shader.Find("Hsys/ZToning/Saturation");
                m_toning[index]._Material.name = "Hsys/ZBloom/Saturation";
                break;
            case Hsys.Toning.toningtype.Vibrance:
                if (m_toning[index]._Material == null)
                {
                    m_toning[index]._Material = new Material(Shader.Find("Hsys/ZToning/Vibrance"));
                }
                if (m_toning[index]._Material.shader.name == "Hsys/ZToning/Vibrance") break;
                m_toning[index]._Material.shader = Shader.Find("Hsys/ZToning/Vibrance");
                m_toning[index]._Material.name = "Hsys/ZBloom/Vibrance";
                break;
            case Hsys.Toning.toningtype.Level:
                if (m_toning[index]._Material == null)
                {
                    m_toning[index]._Material = new Material(Shader.Find("Hsys/ZToning/Level"));
                }
                if (m_toning[index]._Material.shader.name == "Hsys/ZToning/Level") break;
                m_toning[index]._Material.shader = Shader.Find("Hsys/ZToning/Level");
                m_toning[index]._Material.name = "Hsys/ZBloom/Level";
                break;

            case Hsys.Toning.toningtype.ContrastRatio:
                if (m_toning[index]._Material == null)
                {
                    m_toning[index]._Material = new Material(Shader.Find("Hsys/ZToning/ContrastRatio"));
                }
                if (m_toning[index]._Material.shader.name == "Hsys/ZToning/ContrastRatio") break;
                m_toning[index]._Material.shader = Shader.Find("Hsys/ZToning/ContrastRatio");
                m_toning[index]._Material.name = "Hsys/ZBloom/ContrastRatio";
                break;
            case Hsys.Toning.toningtype.ColorGrading:
                if (m_toning[index]._Material == null)
                {
                    m_toning[index]._Material = new Material(Shader.Find("Hsys/ZToning/ColorGrading"));
                }
                if (m_toning[index]._Material.shader.name == "Hsys/ZToning/ColorGrading") break;
                m_toning[index]._Material.shader = Shader.Find("Hsys/ZToning/ColorGrading");
                m_toning[index]._Material.name = "Hsys/ZBloom/ColorGrading";
                break;
        }
    }
    private void OnDestroy()
    {
        m_toning.Clear();
    }
    public void AddPushBloomData()
    {
        Hsys.Toning.ToningData add_item = new Hsys.Toning.ToningData();
        add_item.m_ToningAccuracy = Hsys.GlobalSetting.accuracy.Half;
        m_toning.Add(add_item);
        int index = m_toning.Count - 1;
        CaseTypeOfShader(ref index);
    }

    public void DeletePopBloomData()
    {
        if (m_toning.Count == 0) return;
        int index = m_toning.Count - 1;
        DestroyImmediate(m_toning[index]._Material);
        m_toning.RemoveAt(index);
    }
    public ref List<Hsys.Toning.ToningData> GetToningDataList()
    {
        return ref this.m_toning;
    }
}

namespace Hsys
{
    namespace Toning
    {
        public enum toningtype
        {
            Brightness,
            Saturation,
            Vibrance,
            Level,
            ContrastRatio,
            ColorGrading
        }
        [System.Serializable]
        public class ToningData
        {
            public ToningData()
            {
                Strength = 1f;
                MinInput = 0.1f;
                MaxInput = 0.8f;
                MinOutput = 0.01f;
                MaxOutput = 0.8f;
            }
            public bool m_IsEnable;
            public toningtype m_ToningType;
            public Hsys.GlobalSetting.accuracy m_ToningAccuracy;
            public Hsys.GlobalSetting.quality Quality;
            //属性
            public bool is_VR_AR;
            public float Strength;
            public float Range;
            public float Contrast;
            public Private_Hsys.HUESaturationValue Hue_Saturation_Value;
            public float MinInput;
            public float MaxInput;
            public float MinOutput;
            public float MaxOutput;

            //材质
            public Material _Material;
            public bool is_Deal;
        }

        namespace Private_Hsys
        {
            [System.Serializable]
            public struct HUESaturationValue
            {
                public float HUE;
                public float Saturation;
                public float Value;
            }
        }

        public class ToningEffects
        {
            public void CaseToning(ref string _mode, ref HsysToning toning)
            {
                switch (_mode)
                {
                    case "Brightness":
                        Hsys.Toning.ToningData _Brightness = new Hsys.Toning.ToningData();
                        _Brightness.m_IsEnable = true;
                        _Brightness.m_ToningAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _Brightness.m_ToningType = Hsys.Toning.toningtype.Brightness;
                        _Brightness.is_Deal = false;
                        toning.GetToningDataList().Add(_Brightness);
                        break;
                    case "Saturation":
                        Hsys.Toning.ToningData _Saturation = new Hsys.Toning.ToningData();
                        _Saturation.m_IsEnable = true;
                        _Saturation.m_ToningAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _Saturation.m_ToningType = Hsys.Toning.toningtype.Saturation;
                        _Saturation.is_Deal = false;
                        toning.GetToningDataList().Add(_Saturation);
                        break;
                    case "Vibrance":
                        Hsys.Toning.ToningData _Vibrance = new Hsys.Toning.ToningData();
                        _Vibrance.m_IsEnable = true;
                        _Vibrance.m_ToningAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _Vibrance.m_ToningType = Hsys.Toning.toningtype.Vibrance;
                        _Vibrance.is_Deal = false;
                        toning.GetToningDataList().Add(_Vibrance);
                        break;
                    case "Level":
                        Hsys.Toning.ToningData _Level = new Hsys.Toning.ToningData();
                        _Level.m_IsEnable = true;
                        _Level.m_ToningAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _Level.m_ToningType = Hsys.Toning.toningtype.Level;
                        _Level.is_Deal = false;
                        toning.GetToningDataList().Add(_Level);
                        break;
                    case "ContrastRatio":
                        Hsys.Toning.ToningData _ContrastRatio = new Hsys.Toning.ToningData();
                        _ContrastRatio.m_IsEnable = true;
                        _ContrastRatio.m_ToningAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _ContrastRatio.m_ToningType = Hsys.Toning.toningtype.Level;
                        _ContrastRatio.is_Deal = false;
                        toning.GetToningDataList().Add(_ContrastRatio);
                        break;
                    case "ColorGrading":
                        Hsys.Toning.ToningData _ColorGrading = new Hsys.Toning.ToningData();
                        _ColorGrading.m_IsEnable = true;
                        _ColorGrading.m_ToningAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _ColorGrading.m_ToningType = Hsys.Toning.toningtype.Level;
                        _ColorGrading.is_Deal = false;
                        toning.GetToningDataList().Add(_ColorGrading);
                        break;
                }
            }

            public void CaseToningPass(ref string _mode, ref HsysToning toning, ref int index)
            {
                switch (_mode)
                {
                    case "Brightness":
                        toning.GetToningDataList()[index]._Material = new Material(Shader.Find("Hsys/ZToning/Brightness"));
                        break;
                    case "Saturation":
                        toning.GetToningDataList()[index]._Material = new Material(Shader.Find("Hsys/ZToning/Saturation"));
                        break;
                    case "Vibrance":
                        toning.GetToningDataList()[index]._Material = new Material(Shader.Find("Hsys/ZToning/Vibrance"));
                        break;
                    case "Level":
                        toning.GetToningDataList()[index]._Material = new Material(Shader.Find("Hsys/ZToning/Level"));
                        break;
                    case "ContrastRatio":
                        toning.GetToningDataList()[index]._Material = new Material(Shader.Find("Hsys/ZToning/ContrastRatio"));
                        break;
                    case "ColorGrading":
                        toning.GetToningDataList()[index]._Material = new Material(Shader.Find("Hsys/ZToning/ColorGrading"));
                        break;
                }
            }

            public bool ToningBrightness(ref List<Hsys.Toning.ToningData> toningdata, ref int index)
            {
                toningdata[index]._Material.SetFloat("_Strength", toningdata[index].Strength);
                return false;
            }
            public bool ToningSaturation(ref List<Hsys.Toning.ToningData> toningdata, ref int index)
            {
                return false;
            }

            public bool ToningVibrance(ref List<Hsys.Toning.ToningData> toningdata, ref int index)
            {
                toningdata[index]._Material.SetFloat("_Range", toningdata[index].Range);
                return false;
            }

            public bool ToningLevel(ref List<Hsys.Toning.ToningData> toningdata, ref int index,ref int passnum)
            {
                passnum = toningdata[index].is_VR_AR ? 1 : 0;
                toningdata[index]._Material.SetVector("_IN_OUT", new Vector4(toningdata[index].MinInput, toningdata[index].MaxInput, toningdata[index].MinOutput, toningdata[index].MaxOutput));
                toningdata[index]._Material.SetFloat("_Strength", toningdata[index].Strength);
                return false;
            }
            public bool ToningContrastRatio(ref List<Hsys.Toning.ToningData> toningdata, ref int index)
            {
                toningdata[index]._Material.SetFloat("_Contrast", toningdata[index].Contrast);
                return false;
            }

            public bool ToningColorGraying(ref List<Hsys.Toning.ToningData> toningdata, ref int index)
            {
                toningdata[index]._Material.SetFloat("_Strength", toningdata[index].Strength);
                toningdata[index]._Material.SetVector("_HUE_Saturation_Value", new Vector3(toningdata[index].Hue_Saturation_Value.HUE, toningdata[index].Hue_Saturation_Value.Saturation, toningdata[index].Hue_Saturation_Value.Value));
                return false;
            }
        }
    }
}