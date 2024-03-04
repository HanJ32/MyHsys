using Hsys.Blur;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HsysBlur : MonoBehaviour
{
    [Tooltip("Blur处理队列")] [SerializeField] private List<Hsys.Blur.BlurData> m_blur;
    public Hsys.Blur.BlurEffects m_blureffects = new BlurEffects();
    private int passnum = 0;
    //叠加的效果图
    //private RenderTexture m_buffer = null;
    private bool m_isjmpdefaultrender = false;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_blur.Count == 0) { Graphics.Blit(source, destination);return; }
        for (int index = 0; index < m_blur.Count; index += 1)
        {
            if (m_blur[index].m_IsEnable == false)
            {
                Graphics.Blit(source, destination);
                continue;
            };
            CaseTypeOfShader(ref index);
            switch (m_blur[index].m_BlurType)
            {
                case Hsys.Blur.blurtype.BOX:
                    m_isjmpdefaultrender = m_blureffects.BlurBox(ref m_blur, ref index, ref passnum);
                    break;
                case Hsys.Blur.blurtype.Bokeh:
                    m_isjmpdefaultrender = m_blureffects.BlurBokeh(ref source, ref destination,ref m_blur ,ref index);
                    break;
                case Hsys.Blur.blurtype.TiltShift:
                    m_isjmpdefaultrender = m_blureffects.BlurTiltShift(ref m_blur, ref index);
                    break;
                case Hsys.Blur.blurtype.Iris:
                    m_isjmpdefaultrender = m_blureffects.BlurIris(ref m_blur, ref index, ref passnum);
                    break;
                case Hsys.Blur.blurtype.Grainy:
                    m_isjmpdefaultrender = m_blureffects.BlurGrainy(ref m_blur, ref index);
                    break;
                case Hsys.Blur.blurtype.Radial:
                    m_isjmpdefaultrender = m_blureffects.BlurRadial(ref m_blur, ref index,ref passnum);

                    break;
                case Hsys.Blur.blurtype.Directional:
                    m_isjmpdefaultrender = m_blureffects.BlurDirectional(ref m_blur, ref index);
                    break;
            }
            //TODO:多处理效果
            if (!m_isjmpdefaultrender) { Graphics.Blit(source, destination, m_blur[index]._Material, passnum); }
            m_isjmpdefaultrender = false;
        }
        
    }

    private void OnDestroy()
    {
        m_blur.Clear();
    }

    public void CaseTypeOfShader(ref int index)
    {
        switch (m_blur[index].m_BlurType)
        {
            case blurtype.BOX:
                if(m_blur[index]._Material == null)
                {
                    m_blur[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Box"));
                }
                if (m_blur[index]._Material.shader.name == "Hsys/ZBlur/Box") break;
                m_blur[index]._Material.shader = Shader.Find("Hsys/ZBlur/Box");
                m_blur[index]._Material.name = "Hsys/ZBlur/Box";
                break;
            case blurtype.Bokeh:
                if (m_blur[index]._Material == null)
                {
                    m_blur[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Bokeh"));
                }
                if (m_blur[index]._Material.shader.name == "Hsys/ZBlur/Bokeh") break;
                m_blur[index]._Material.shader = Shader.Find("Hsys/ZBlur/Bokeh");
                m_blur[index]._Material.name = "Hsys/ZBlur/Bokeh";
                break;
            case blurtype.TiltShift:
                if (m_blur[index]._Material == null)
                {
                    m_blur[index]._Material = new Material(Shader.Find("Hsys/ZBlur/TiltShift"));
                }
                if (m_blur[index]._Material.shader.name == "Hsys/ZBlur/TiltShift") break;
                m_blur[index]._Material.shader = Shader.Find("Hsys/ZBlur/TiltShift");
                m_blur[index]._Material.name = "Hsys/ZBlur/TiltShift";
                break;
            case blurtype.Iris:
                if (m_blur[index]._Material == null)
                {
                    m_blur[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Iris"));
                }
                if (m_blur[index]._Material.shader.name == "Hsys/ZBlur/Iris") break;
                m_blur[index]._Material.shader = Shader.Find("Hsys/ZBlur/Iris");
                m_blur[index]._Material.name = "Hsys/ZBlur/Iris";
                break;
            case blurtype.Grainy:
                if (m_blur[index]._Material == null)
                {
                    m_blur[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Grainy"));
                }
                if (m_blur[index]._Material.shader.name == "Hsys/ZBlur/Grainy") break;
                m_blur[index]._Material.shader = Shader.Find("Hsys/ZBlur/Grainy");
                m_blur[index]._Material.name = "Hsys/ZBlur/Grainy";
                break;
            case blurtype.Radial:
                if (m_blur[index]._Material == null)
                {
                    m_blur[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Radial"));
                }
                if (m_blur[index]._Material.shader.name == "Hsys/ZBlur/Radial") break;
                m_blur[index]._Material.shader = Shader.Find("Hsys/ZBlur/Radial");
                m_blur[index]._Material.name = "Hsys/ZBlur/Radial";
                break;

            case blurtype.Directional:
                if (m_blur[index]._Material == null)
                {
                    m_blur[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Directional"));
                }
                if (m_blur[index]._Material.shader.name == "Hsys/ZBlur/Directional") break;
                m_blur[index]._Material.shader = Shader.Find("Hsys/ZBlur/Directional");
                m_blur[index]._Material.name = "Hsys/ZBlur/Directional"; 
                break;
        }
    }

    public ref List<Hsys.Blur.BlurData> GetBlurDataList()
    {
        return ref this.m_blur;
    }

    public void AddPushBlurData()
    {
        if(m_blur.Count >= 8) { return; }
        Hsys.Blur.BlurData add_item = new Hsys.Blur.BlurData();
        add_item.m_BlurAccuracy = Hsys.GlobalSetting.accuracy.Half;
        m_blur.Add(add_item);
        int index = m_blur.Count - 1;
        CaseTypeOfShader(ref index);
    }

    public void DeletePopBlurData()
    {
        if (m_blur.Count == 0) return;
        int index = m_blur.Count - 1;
        DestroyImmediate(m_blur[index]._Material);
        m_blur.RemoveAt(index);
    }

    public Camera GetCamera()
    {
        return this.gameObject.GetComponent<Camera>();
    }
}

namespace Hsys
{
    namespace Blur
    {
        public enum blurtype
        {
            BOX,
            Bokeh,
            TiltShift,
            Iris,
            Grainy,
            Radial,
            Directional
        }
        [Serializable]
        public class BlurData
        {
            public BlurData()
            {
                m_BlurAccuracy = GlobalSetting.accuracy.Half;
                Redius = 0.5f;
                Blur = 0.02f;
                Count = 1;
                PixelSize = 0.5f;
                TexelSize = Vector4.zero;
                Params = Vector4.zero;
                Center = Vector4.zero;
                Offset = Vector4.zero;
                Gradient = Vector4.zero;
                HighSampler = false;
                AreaSize = 0.01f;
                Angle = 0f;
            }
            //[Header("Setting")]
            //[Tooltip("开启模糊")]
            public bool m_IsEnable;

            //[Tooltip("模糊类型")]
            public Hsys.Blur.blurtype m_BlurType;
            //[Tooltip("模糊精度")]
            public Hsys.GlobalSetting.accuracy m_BlurAccuracy;

            //质量
            public Hsys.GlobalSetting.quality m_Quality;
            //[Header("Attribute")]
            public bool HighSampler;
            //[Tooltip("模糊半径")] 
            public float Redius;
            public float Effect;
            public float AreaSize;
            public float Spread;
            public float Angle;
            //[Tooltip("模糊系数")]
            [Range(0.01f, 1f)] public float Blur;
            //[Tooltip("迭代次数")]
            [Range(1, 5)] public int Count;

            //[Tooltip("像素尺寸")] 
            public float PixelSize;
            //[Tooltip("边框")]
            public Vector4 TexelSize;
            public Vector4 Offset;
            public Vector4 Gradient;
            //[Tooltip("2X2矩阵核")]
            public Vector4 Params;
            //中心点
            public Vector4 Center;
            //[Header("Material")]
            //[Tooltip("材质")]
            public Material _Material;

            [HideInInspector] public bool is_Deal;
        }



        public class BlurEffects
        {
            public Camera m_camera;

            private float TimeX = 0f;
            public void CaseBlur(ref string _mode, ref HsysBlur blur)
            {
                //TODO: 还要再重构的
                switch (_mode)
                {
                    case "Box":
                        Hsys.Blur.BlurData _box = new Hsys.Blur.BlurData();
                        _box.m_IsEnable = true;
                        _box.m_BlurAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _box.m_BlurType = Hsys.Blur.blurtype.BOX;
                        _box.is_Deal = false;
                        blur.GetBlurDataList().Add(_box);

                        break;
                    case "Boke":
                        Hsys.Blur.BlurData _boke = new Hsys.Blur.BlurData();
                        _boke.m_IsEnable = true;
                        _boke.m_BlurAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _boke.m_BlurType = Hsys.Blur.blurtype.Bokeh;
                        _boke.is_Deal = false;
                        blur.GetBlurDataList().Add(_boke);

                        break;
                    case "TiltShift":
                        Hsys.Blur.BlurData _tiltshift = new Hsys.Blur.BlurData();
                        _tiltshift.m_IsEnable = true;
                        _tiltshift.m_BlurAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _tiltshift.m_BlurType = Hsys.Blur.blurtype.TiltShift;
                        _tiltshift.is_Deal = false;
                        blur.GetBlurDataList().Add(_tiltshift);

                        break;
                    case "Iris":
                        Hsys.Blur.BlurData _iris = new Hsys.Blur.BlurData();
                        _iris.m_IsEnable = true;
                        _iris.m_BlurAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _iris.m_BlurType = Hsys.Blur.blurtype.Iris;
                        _iris.is_Deal = false;
                        blur.GetBlurDataList().Add(_iris);
                        break;
                    case "Grainy":
                        Hsys.Blur.BlurData _grainy = new Hsys.Blur.BlurData();
                        _grainy.m_IsEnable = true;
                        _grainy.m_BlurAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _grainy.m_BlurType = Hsys.Blur.blurtype.Grainy;
                        _grainy.is_Deal = false;
                        blur.GetBlurDataList().Add(_grainy);
                        break;
                    case "Radial":
                        Hsys.Blur.BlurData _radial = new Hsys.Blur.BlurData();
                        _radial.m_IsEnable = true;
                        _radial.m_BlurAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _radial.m_BlurType = Hsys.Blur.blurtype.Radial;
                        _radial.is_Deal = false;
                        blur.GetBlurDataList().Add(_radial);
                        break;
                    case "Directional":
                        Hsys.Blur.BlurData _directional = new Hsys.Blur.BlurData();
                        _directional.m_IsEnable = true;
                        _directional.m_BlurAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _directional.m_BlurType = Hsys.Blur.blurtype.Directional;
                        _directional.is_Deal = false;
                        blur.GetBlurDataList().Add(_directional);
                        break;
                }
            }

            public void CaseBlurPass(ref string _mode, ref HsysBlur blur, ref int index)
            {
                switch (_mode)
                {
                    case "Box":
                        blur.GetBlurDataList()[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Box"));
                        break;
                    case "Boke":
                        blur.GetBlurDataList()[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Boke"));
                        break;
                    case "TiltShift":
                        blur.GetBlurDataList()[index]._Material = new Material(Shader.Find("Hsys/ZBlur/TiltShift"));
                        break;
                    case "Iris":
                        blur.GetBlurDataList()[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Iris"));
                        break;
                    case "Grainy":
                        blur.GetBlurDataList()[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Grainy"));
                        break;
                    case "Radial":
                        blur.GetBlurDataList()[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Radial"));
                        break;
                    case "Directional":
                        blur.GetBlurDataList()[index]._Material = new Material(Shader.Find("Hsys/ZBlur/Directional"));
                        break;
                }
            }

            public bool BlurBox(ref List<Hsys.Blur.BlurData> blurdata, ref int index, ref int passnum)
            {
                passnum = blurdata[index].HighSampler ? 1 : 0;

                blurdata[index]._Material.SetFloat("_Blur", blurdata[index].Blur);
                blurdata[index]._Material.SetFloat("_Count", blurdata[index].Count);
                blurdata[index]._Material.SetFloat("_TexelSizeX", blurdata[index].TexelSize.x);
                blurdata[index]._Material.SetFloat("_TexelSizeY", blurdata[index].TexelSize.y);
                return false;
            }
            public bool BlurBokeh(ref RenderTexture source, ref RenderTexture destination, ref List<Hsys.Blur.BlurData> blurdata, ref int index)
            {
               
                if (TimeX > 100) TimeX = 0;
                int width = source.width / blurdata[index].Count;
                int height = source.height / blurdata[index].Count;

                blurdata[index]._Material.SetFloat("_Blur", blurdata[index].Blur);
                blurdata[index]._Material.SetFloat("_Count", blurdata[index].Count);
                blurdata[index]._Material.SetFloat("_PixelSize", blurdata[index].PixelSize);
                blurdata[index]._Material.SetFloat("_Redius", blurdata[index].Redius);
                blurdata[index]._Material.SetVector("_Offset", blurdata[index].Offset);

                TimeX += Time.deltaTime;
                if (blurdata[index].Count > 1)
                {
                    RenderTexture buffer = RenderTexture.GetTemporary(width, height, 0);
                    Graphics.Blit(source, buffer, blurdata[index]._Material);
                    Graphics.Blit(buffer, destination);
                    RenderTexture.ReleaseTemporary(buffer);
                    return true;
                }
                return false;
            }
            public bool BlurTiltShift(ref List<Hsys.Blur.BlurData> blurdata, ref int index)
            {
                switch(blurdata[index].m_Quality)
                {
                    case GlobalSetting.quality.Low:
                        
                        blurdata[index]._Material.SetFloat("_Blur", blurdata[index].Blur);
                        blurdata[index]._Material.SetFloat("_Area", blurdata[index].AreaSize);
                        blurdata[index]._Material.SetFloat("_Count", blurdata[index].Count);
                        break;
                    case GlobalSetting.quality.Middle:
                        blurdata[index]._Material.SetFloat("_Blur", blurdata[index].Blur);
                        blurdata[index]._Material.SetFloat("_Count", blurdata[index].Count);
                        blurdata[index]._Material.SetVector("_Params", blurdata[index].Params);
                        blurdata[index]._Material.SetFloat("_Redius", blurdata[index].Redius);
                        blurdata[index]._Material.SetFloat("_Area", blurdata[index].AreaSize);
                        blurdata[index]._Material.SetFloat("_Spread", blurdata[index].Spread);
                        blurdata[index]._Material.SetFloat("_Offset", blurdata[index].Offset.x);
                        break;
                    case GlobalSetting.quality.High:
                        break;
                }
                
                return false;
            }

            public bool BlurIris(ref List<Hsys.Blur.BlurData> blurdata, ref int index, ref int passnum)
            {

                blurdata[index]._Material.SetFloat("_Blur", blurdata[index].Blur);
                blurdata[index]._Material.SetFloat("_Count", blurdata[index].Count);
                blurdata[index]._Material.SetFloat("_Redius", blurdata[index].Redius);
                //blurdata[index]._Material.SetVector("_Gradient", blurdata[index].Offset);
                blurdata[index]._Material.SetFloat("_AreaSize", blurdata[index].AreaSize);
                blurdata[index]._Material.SetVector("_Params", blurdata[index].Params);
                blurdata[index]._Material.SetFloat("_CenterX", blurdata[index].Center.x);
                blurdata[index]._Material.SetFloat("_CenterY", blurdata[index].Center.y);
                return false;
            }
            public bool BlurGrainy(ref List<Hsys.Blur.BlurData> blurdata, ref int index)
            {
                blurdata[index]._Material.SetFloat("_Blur", blurdata[index].Blur);
                blurdata[index]._Material.SetFloat("_Count", blurdata[index].Count);
                blurdata[index]._Material.SetFloat("_Effect", blurdata[index].Effect);
                return false;
            }
            public bool BlurRadial(ref List<Hsys.Blur.BlurData> blurdata, ref int index, ref int passnum)
            {
                passnum = blurdata[index].HighSampler ? 1 : 0;

                blurdata[index]._Material.SetFloat("_Blur", blurdata[index].Blur);
                blurdata[index]._Material.SetFloat("_Count", blurdata[index].Count);
                blurdata[index]._Material.SetFloat("_Redius", blurdata[index].Redius);
                blurdata[index]._Material.SetVector("_Params", blurdata[index].Params);
                blurdata[index]._Material.SetVector("_Center", blurdata[index].Center);
                return false;
            }
            public bool BlurDirectional(ref List<Hsys.Blur.BlurData> blurdata, ref int index)
            {
                float sinv = (Mathf.Sin(blurdata[index].Angle) * blurdata[index].Blur * 0.05f) / blurdata[index].Count;
                float cosv = (Mathf.Cos(blurdata[index].Angle) * blurdata[index].Blur * 0.05f) / blurdata[index].Count;
                blurdata[index].Params.y = sinv;
                blurdata[index].Params.z = cosv;
                blurdata[index]._Material.SetFloat("_Blur", blurdata[index].Blur);
                blurdata[index]._Material.SetFloat("_Count", blurdata[index].Count);
                blurdata[index]._Material.SetVector("_Params", blurdata[index].Params);
                return false;
            }

            public void SetCamera(ref Camera _camera)
            {
                m_camera = _camera;
            }

        }
    }
}