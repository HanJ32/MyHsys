using Hsys.AO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;


public class HsysAO : MonoBehaviour
{
    [Tooltip("AO 处理队列")][SerializeField] private List<Hsys.AO.AOData> m_ao;
    public Hsys.AO.AOEffects m_aoeffects;
    private int passnum = 0;
    private bool m_isjmpdefaultrender = false;
    private RenderTexture m_rendertexter;
    private Camera m_camera;
    private void OnEnable()
    {
        Hsys.HsysGetYourNeadVarOfClass.SetMonoClassVar(this);
    }
    public void Start()
    {
        m_aoeffects = new AOEffects(GetComponent<Camera>());
    }
/*    private void OnPreRender()
    {
        m_rendertexter = RenderTexture.GetTemporary(m_camera.pixelWidth, m_camera.pixelHeight, 16);
        m_camera.targetTexture = m_rendertexter;
    }

    private void OnPostRender()
    {
        if (m_ao.Count == 0) { Graphics.Blit(m_rendertexter, destination); return; }
        for (int index = 0; index < m_ao.Count; index += 1)
        {
            if (m_ao[index].m_IsEnable == false)
            {
                Graphics.Blit(source, destination);
                //Graphics.Blit(buffer, buffer2);
                //Graphics.Blit(buffer, destination);
                continue;
            };
            //if (index != 0) { Graphics.CopyTexture(buffer2, buffer); }
            CaseTypeOfShader(ref index);
            switch (m_ao[index].m_AOType)
            {
                case Hsys.AO.aotype.SSAO:
                    break;
                case Hsys.AO.aotype.SSAO_URP:
                    break;
                case Hsys.AO.aotype.HBAO:
                    break;
            }
            //TODO:多处理效果
            if (!m_isjmpdefaultrender) { Graphics.Blit(source, destination, m_ao[index]._Material, passnum); }
            m_isjmpdefaultrender = false;
            passnum = 0;
        }
        m_camera.targetTexture = null;
        Graphics.Blit(m_rendertexter, null as RenderTexture, postProcess, passnum);
    }*/

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        /*        if(buffer == null)
                {
                    buffer = RenderTexture.GetTemporary(source.width, source.height, 0);
                    Graphics.Blit(source, buffer);
                    buffer2 = RenderTexture.GetTemporary(source.width, source.height, 0);
                }*/
        if (m_ao.Count == 0) { Graphics.Blit(source, destination); return; }
        for (int index = 0; index < m_ao.Count; index += 1)
        {
            if (m_ao[index].m_IsEnable == false)
            {
                Graphics.Blit(source, destination);
                //Graphics.Blit(buffer, buffer2);
                //Graphics.Blit(buffer, destination);
                continue;
            };
            //if (index != 0) { Graphics.CopyTexture(buffer2, buffer); }
            CaseTypeOfShader(ref index);
            switch (m_ao[index].m_AOType)
            {
                case Hsys.AO.aotype.SSAO:
                    break;
                case Hsys.AO.aotype.SSAO_URP:
                    break;
                case Hsys.AO.aotype.HBAO:
                    break;
            }
            //TODO:多处理效果
            if (!m_isjmpdefaultrender) { Graphics.Blit(source, destination, m_ao[index]._Material, passnum); }
            //if (!m_isjmpdefaultrender) { Graphics.Blit(buffer, buffer2, m_toning[index]._Material, passnum); }
            m_isjmpdefaultrender = false;
            passnum = 0;

            //Graphics.Blit(buffer2, buffer);
        }
        //Graphics.Blit(buffer2, destination);
    }

    public void CaseTypeOfShader(ref int index)
    {
        switch (m_ao[index].m_AOType)
        {
            case Hsys.AO.aotype.SSAO:
                if (m_ao[index]._Material == null)
                {
                    m_ao[index]._Material = new Material(Shader.Find("Hsys/ZAO/SSAO"));
                }
                if (m_ao[index]._Material.shader.name == "Hsys/ZAO/SSAO") break;
                m_ao[index]._Material.shader = Shader.Find("Hsys/ZAO/SSAO");
                m_ao[index]._Material.name = "Hsys/ZAO/SSAO";
                break;
        }
    }

    private void OnDestroy()
    {
        m_ao.Clear();
    }
    public void AddPushAOData()
    {
        if (m_ao.Count >= 8) { return; }
        Hsys.AO.AOData add_item = new Hsys.AO.AOData();
        add_item.m_AOAccuracy = Hsys.GlobalSetting.accuracy.Half;
        m_ao.Add(add_item);
        int index = m_ao.Count - 1;
        CaseTypeOfShader(ref index);
    }

    public void DeletePopAOData()
    {
        if (m_ao.Count == 0) return;
        int index = m_ao.Count - 1;
        DestroyImmediate(m_ao[index]._Material);
        m_ao.RemoveAt(index);
    }
    public ref List<Hsys.AO.AOData> GetAODataList()
    {
        return ref this.m_ao;
    }
}

namespace Hsys
{
    namespace AO
    {
        public enum aotype
        {
            SSAO,
            SSAO_URP,
            HBAO
        }

        [System.Serializable]
        public class AOData
        {
            public AOData()
            {
                Intensity = 1.0f;
                Radius = 1.2f;
                MaxRadiusPixels = 256;
                AngleBias = 0.05f;
                MaxDistance = 150f;
                Distance = 50f;
                Sharpness = 8.0f;
            }
            public bool m_IsEnable;
            public aotype m_AOType;
            public GlobalSetting.accuracy m_AOAccuracy;
            public Hsys.GlobalSetting.quality m_Quality;

            public float Intensity;
            public float Radius;
            public float MaxRadiusPixels;
            public float AngleBias;
            public float MaxDistance;
            public float Distance;

            public float Sharpness;

            //材质
            public Material _Material;

            public bool is_Deal;
        }

        public class AOEffects
        {
            public Camera active_camera;
            public AOEffects(Camera _active_camera)
            {
                this.active_camera = _active_camera;
            }
            public void CameraDepthTextureMode()
            {
                active_camera.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
            }

            public void CaseLens(ref string _mode, ref HsysAO lens)
            {
                switch (_mode)
                {
                    case "SSAO":
                        Hsys.AO.AOData _SSAO = new Hsys.AO.AOData();
                        _SSAO.m_IsEnable = true;
                        _SSAO.m_AOAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _SSAO.m_AOType = Hsys.AO.aotype.SSAO;
                        _SSAO.is_Deal = false;
                        lens.GetAODataList().Add(_SSAO);
                        break;
                    case "SSAO_URP":
                        Hsys.AO.AOData _SSAO_URP = new Hsys.AO.AOData();
                        _SSAO_URP.m_IsEnable = true;
                        _SSAO_URP.m_AOAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _SSAO_URP.m_AOType = Hsys.AO.aotype.SSAO_URP;
                        _SSAO_URP.is_Deal = false;
                        lens.GetAODataList().Add(_SSAO_URP);
                        break;
                    case "HBAO":
                        Hsys.AO.AOData _HBAO = new Hsys.AO.AOData();
                        _HBAO.m_IsEnable = true;
                        _HBAO.m_AOAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _HBAO.m_AOType = Hsys.AO.aotype.HBAO;
                        _HBAO.is_Deal = false;
                        lens.GetAODataList().Add(_HBAO);
                        break;
                }
            }
            public void CaseLensPass(ref string _mode, ref HsysAO ao, ref int index)
            {
                switch (_mode)
                {
                    case "SSAO":
                        ao.GetAODataList()[index]._Material = new Material(Shader.Find("Hsys/ZAO/SSAO"));
                        break;
                    case "SSAO_URP":
                        ao.GetAODataList()[index]._Material = new Material(Shader.Find("Hsys/ZAO/SSAO_URP"));
                        break;
                    case "HBAO":
                        ao.GetAODataList()[index]._Material = new Material(Shader.Find("Hsys/ZAO/HBAO"));
                        break;
                }
            }
            public bool SSAO()
            {
                return false;
            }

            public bool SSAO_URP()
            {
                return false;
            }

            public bool HBAO(ref List<Hsys.AO.AOData> lensdata, ref int index, ref RenderTexture source)
            {
                return false;
            }
        }
    }
}
