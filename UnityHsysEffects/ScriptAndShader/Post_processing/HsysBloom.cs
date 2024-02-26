using Hsys.Bloom;
using Hsys.Blur;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;
using static Unity.VisualScripting.Member;

public class HsysBloom : MonoBehaviour
{
    [Tooltip("Bloom处理队列")][SerializeField] private List<Hsys.Bloom.BloomData> m_bloom;
    public Hsys.Bloom.BloomEffects m_bloomeffects = new BloomEffects();
    private int passnum = 0;
    private bool m_isjmpdefaultrender = false;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_bloom.Count == 0) { Graphics.Blit(source, destination); return; }
        for (int index = 0; index < m_bloom.Count; index += 1)
        {
            if (m_bloom[index].m_IsEnable == false)
            {
                Graphics.Blit(source, destination);
                continue;
            };
            CaseTypeOfShader(ref index);
            switch (m_bloom[index].m_BloomType)
            {
                case bloomtype.HDR:
                    m_isjmpdefaultrender = m_bloomeffects.BloomHDR(ref m_bloom, ref index,ref source,ref destination);
                    break;
                case bloomtype.NoHDR:
                    m_isjmpdefaultrender = m_bloomeffects.BloomNoHDR(ref m_bloom, ref index, ref source, ref destination);
                    break;
            }
            //TODO:多处理效果
            if (!m_isjmpdefaultrender) { Graphics.Blit(source, destination, m_bloom[index]._Material, passnum); }
            m_isjmpdefaultrender = false;
        }
    }
    
    public void CaseTypeOfShader(ref int index)
    {
        switch(m_bloom[index].m_BloomType)
        {
            case Hsys.Bloom.bloomtype.HDR:
                if (m_bloom[index]._Material == null)
                {
                    m_bloom[index]._Material = new Material(Shader.Find("Hsys/ZBloom/HDR"));
                }
                if (m_bloom[index]._Material.shader.name == "Hsys/ZBloom/HDR") break;
                m_bloom[index]._Material.shader = Shader.Find("Hsys/ZBloom/HDR");
                m_bloom[index]._Material.name = "Hsys/ZBloom/HDR";
                break;
            case Hsys.Bloom.bloomtype.NoHDR:
                if (m_bloom[index]._Material == null)
                {
                    m_bloom[index]._Material = new Material(Shader.Find("Hsys/ZBloom/NoHDR"));
                }
                if (m_bloom[index]._Material.shader.name == "Hsys/ZBloom/NoHDR") break;
                m_bloom[index]._Material.shader = Shader.Find("Hsys/ZBloom/NoHDR");
                m_bloom[index]._Material.name = "Hsys/ZBloom/NoHDR";
                break;
        }
    }
    private void OnDestroy()
    {
        m_bloom.Clear();
    }
    public void AddPushBloomData()
    {
        Hsys.Bloom.BloomData add_item = new Hsys.Bloom.BloomData();
        add_item.m_BloomAcaccuracy = Hsys.GlobalSetting.accuracy.Half;
        m_bloom.Add(add_item);
        int index = m_bloom.Count - 1;
        CaseTypeOfShader(ref index);
    }

    public void DeletePopBloomData()
    {
        if (m_bloom.Count == 0) return;
        int index = m_bloom.Count - 1;
        DestroyImmediate(m_bloom[index]._Material);
        m_bloom.RemoveAt(index);
    }
    public ref List<Hsys.Bloom.BloomData> GetBloomDataList()
    {
        return ref this.m_bloom;
    }
}

namespace Hsys
{
    namespace Bloom
    {
        public enum bloomtype
        {
            HDR,
            NoHDR
        }

        [System.Serializable]
        public class BloomData
        {
            public BloomData()
            {
                m_BloomAcaccuracy = GlobalSetting.accuracy.Half;
                downbloom = false;
                Offset = Vector4.zero;
            }
            public bool m_IsEnable;
            public bloomtype m_BloomType;
            public Hsys.GlobalSetting.accuracy m_BloomAcaccuracy;

            public bool downbloom;
            //属性
            public float Strength;
            public float Luminance;
            public Color Color;
            public Vector4 Offset;
            //材质
            public Material _Material;
            public bool is_Deal;
        }

        public class BloomEffects
        {
            public void CaseBloom(ref string _mode, ref HsysBloom bloom)
            {
                switch (_mode)
                {
                    case "HDR":
                        Hsys.Bloom.BloomData _HDR = new Hsys.Bloom.BloomData();
                        _HDR.m_IsEnable = true;
                        _HDR.m_BloomAcaccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _HDR.m_BloomType = Hsys.Bloom.bloomtype.HDR;
                        _HDR.is_Deal = false;
                        bloom.GetBloomDataList().Add(_HDR);
                        break;
                    case "NoHDR":
                        Hsys.Bloom.BloomData _NoHDR = new Hsys.Bloom.BloomData();
                        _NoHDR.m_IsEnable = true;
                        _NoHDR.m_BloomAcaccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _NoHDR.m_BloomType = Hsys.Bloom.bloomtype.NoHDR     ;
                        _NoHDR.is_Deal = false;
                        bloom.GetBloomDataList().Add(_NoHDR);
                        break;
                }
            }

            public void CaseBloomPass(ref string _mode, ref HsysBloom bloom, ref int index)
            {

                switch (_mode)
                {
                    case "HDR":
                        bloom.GetBloomDataList()[index]._Material = new Material(Shader.Find("Hsys/ZBloom/HDR"));
                        break;
                    case "NoHDR":
                        bloom.GetBloomDataList()[index]._Material = new Material(Shader.Find("Hsys/ZBloom/NoHDR"));
                        break;
                }
            }

            public bool BloomHDR(ref List<Hsys.Bloom.BloomData> bloomdata, ref int index, ref RenderTexture source, ref RenderTexture dest)
            {
                //passnum = bloomdata[index].HighSampler ? 1 : 0;
                int weight = source.width;
                int height = source.height;
                RenderTexture buffer = RenderTexture.GetTemporary(weight, height, 0);
                bloomdata[index]._Material.SetColor("_HDRColor",bloomdata[index].Color);
                bloomdata[index]._Material.SetFloat("_Strength", bloomdata[index].Strength);
                bloomdata[index]._Material.SetFloat("_Luminance", bloomdata[index].Luminance);
                Graphics.Blit(source, buffer, bloomdata[index]._Material, 0);
                bloomdata[index]._Material.SetTexture("_BlurTex", buffer);
                bloomdata[index]._Material.SetVector("_Offset", bloomdata[index].Offset);
                Graphics.Blit(source, dest,bloomdata[index]._Material, 1);
                return true;
            }
            public bool BloomNoHDR(ref List<Hsys.Bloom.BloomData> bloomdata, ref int index, ref RenderTexture source, ref RenderTexture dest)
            {
                return false;
            }
        }


    }
}