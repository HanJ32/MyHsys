using Hsys.AO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HsysOutBorder : MonoBehaviour
{
    [Tooltip("OutBorder 处理队列")][SerializeField] private List<Hsys.OutBorder.OutBorderData> m_outborder;
    private Hsys.OutBorder.OutBorderEffects m_outbordereffects = new Hsys.OutBorder.OutBorderEffects();
    private int passnum = 0;
    private bool m_isjmpdefaultrender = false;
    private void OnEnable()
    {
        Hsys.HsysGetYourNeadVarOfClass.SetMonoClassVar(this);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        /*        if(buffer == null)
                {
                    buffer = RenderTexture.GetTemporary(source.width, source.height, 0);
                    Graphics.Blit(source, buffer);
                    buffer2 = RenderTexture.GetTemporary(source.width, source.height, 0);
                }*/
        if (m_outborder.Count == 0) { Graphics.Blit(source, destination); return; }
        for (int index = 0; index < m_outborder.Count; index += 1)
        {
            if (m_outborder[index].m_IsEnable == false)
            {
                Graphics.Blit(source, destination);
                //Graphics.Blit(buffer, buffer2);
                //Graphics.Blit(buffer, destination);
                continue;
            };
            //if (index != 0) { Graphics.CopyTexture(buffer2, buffer); }
            CaseTypeOfShader(ref index);
            switch (m_outborder[index].m_OutBorderType)
            {
                case Hsys.OutBorder.outbordertype.OutLine:
                    break;
            }
            //TODO:多处理效果
            if (!m_isjmpdefaultrender) { Graphics.Blit(source, destination, m_outborder[index]._Material, passnum); }
            //if (!m_isjmpdefaultrender) { Graphics.Blit(buffer, buffer2, m_toning[index]._Material, passnum); }
            m_isjmpdefaultrender = false;
            passnum = 0;

            //Graphics.Blit(buffer2, buffer);
        }
        //Graphics.Blit(buffer2, destination);
    }
    public void CaseTypeOfShader(ref int index)
    {
        switch (m_outborder[index].m_OutBorderType)
        {
            case Hsys.OutBorder.outbordertype.OutLine:
                if (m_outborder[index]._Material == null)
                {
                    m_outborder[index]._Material = new Material(Shader.Find("Hsys/ZOutBorder/OutLine"));
                }
                if (m_outborder[index]._Material.shader.name == "Hsys/ZOutBorder/OutLine") break;
                m_outborder[index]._Material.shader = Shader.Find("Hsys/ZOutBorder/OutLine");
                m_outborder[index]._Material.name = "Hsys/ZOutBorder/OutLine";
                break;
        }
    }
    private void OnDestroy()
    {
        m_outborder.Clear();
    }
    public void AddPushOutBorderData()
    {
        if(m_outborder.Count <= 8) { return; }
        Hsys.OutBorder.OutBorderData add_item = new Hsys.OutBorder.OutBorderData();
        add_item.m_OutBorderAccuracy = Hsys.GlobalSetting.accuracy.Half;
        m_outborder.Add(add_item);
        int index = m_outborder.Count - 1;
        CaseTypeOfShader(ref index);
    }

    public void DeletePopOutBorderData()
    {
        if (m_outborder.Count == 0) return;
        int index = m_outborder.Count - 1;
        DestroyImmediate(m_outborder[index]._Material);
        m_outborder.RemoveAt(index);
    }
    public ref List<Hsys.OutBorder.OutBorderData> GetOutBorderDataList()
    {
        return ref this.m_outborder;
    }
}

namespace Hsys
{
    namespace OutBorder
    {
        public enum outbordertype
        {
            OutLine

        }
        [System.Serializable]
        public class OutBorderData
        {
            public bool m_IsEnable;
            public outbordertype m_OutBorderType;
            public GlobalSetting.accuracy m_OutBorderAccuracy;
            public Hsys.GlobalSetting.quality m_Quality;


            public Material _Material;
            public bool is_Deal;
        }

        public class OutBorderEffects
        {
            public void CaseOutBorder(ref string _mode, ref HsysOutBorder outborder)
            {
                switch (_mode)
                {
                    case "OutLine":
                        Hsys.OutBorder.OutBorderData _OutLine = new Hsys.OutBorder.OutBorderData();
                        _OutLine.m_IsEnable = true;
                        _OutLine.m_OutBorderAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _OutLine.m_OutBorderType = Hsys.OutBorder.outbordertype.OutLine;
                        _OutLine.is_Deal = false;
                        outborder.GetOutBorderDataList().Add(_OutLine);
                        break;

                }
            }

            public void CaseOutBorderPass(ref string _mode, ref HsysOutBorder outborder, ref int index)
            {
                switch (_mode)
                {
                    case "OutLine":
                        outborder.GetOutBorderDataList()[index]._Material = new Material(Shader.Find("Hsys/ZOutBorder/OutLine"));
                        break;
                }
            }

            public void OutLine(ref List<Hsys.OutBorder.OutBorderData> outborderdata, ref int index)
            {
                //outborderdata[index]._Material.Set
            }
        }

        
    }
}
