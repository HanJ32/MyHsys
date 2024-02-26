using Hsys.Lens;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.VisualScripting.Member;

//[ExecuteInEditMode]
public class HsysLens : MonoBehaviour
{
    [Tooltip("Lens 处理队列")][SerializeField] private List<Hsys.Lens.LensData> m_lens;
    public Hsys.Lens.LensEffects m_lenseffects = new LensEffects();
    private int passnum = 0;
    //叠加的效果图
    //private RenderTexture m_buffer = null;
    private bool m_isjmpdefaultrender = false;


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        /*        if(buffer == null)
                {
                    buffer = RenderTexture.GetTemporary(source.width, source.height, 0);
                    Graphics.Blit(source, buffer);
                    buffer2 = RenderTexture.GetTemporary(source.width, source.height, 0);
                }*/
        if (m_lens.Count == 0) { Graphics.Blit(source, destination); return; }
        for (int index = 0; index < m_lens.Count; index += 1)
        {
            if (m_lens[index].m_IsEnable == false)
            {
                Graphics.Blit(source, destination);
                //Graphics.Blit(buffer, buffer2);
                //Graphics.Blit(buffer, destination);
                continue;
            };
            //if (index != 0) { Graphics.CopyTexture(buffer2, buffer); }
            CaseTypeOfShader(ref index);
            switch (m_lens[index].m_LensType)
            {
                case Hsys.Lens.lenstype.Twirl:
                    m_isjmpdefaultrender = m_lenseffects.LensTwirl(ref m_lens, ref index, ref source);
                    break;
                case Hsys.Lens.lenstype.Twist:
                    m_isjmpdefaultrender = m_lenseffects.LensTwist(ref m_lens, ref index, ref source);
                    break;
            }
            //TODO:多处理效果
            if (!m_isjmpdefaultrender) { Graphics.Blit(source, destination, m_lens[index]._Material, passnum); }
            //if (!m_isjmpdefaultrender) { Graphics.Blit(buffer, buffer2, m_toning[index]._Material, passnum); }
            m_isjmpdefaultrender = false;
            passnum = 0;

            //Graphics.Blit(buffer2, buffer);
        }
        //Graphics.Blit(buffer2, destination);
    }
    public void CaseTypeOfShader(ref int index)
    {
        switch (m_lens[index].m_LensType)
        {
            case Hsys.Lens.lenstype.Twirl:
                if (m_lens[index]._Material == null)
                {
                    m_lens[index]._Material = new Material(Shader.Find("Hsys/ZLens/Twirl"));
                }
                if (m_lens[index]._Material.shader.name == "Hsys/ZLens/Twirl") break;
                m_lens[index]._Material.shader = Shader.Find("Hsys/ZLens/Twirl");
                m_lens[index]._Material.name = "Hsys/ZLens/Twirl";
                break;
            case Hsys.Lens.lenstype.Twist:
                if (m_lens[index]._Material == null)
                {
                    m_lens[index]._Material = new Material(Shader.Find("Hsys/ZLens/Twist"));
                }
                if (m_lens[index]._Material.shader.name == "Hsys/ZLens/Twist") break;
                m_lens[index]._Material.shader = Shader.Find("Hsys/ZLens/Twist");
                m_lens[index]._Material.name = "Hsys/ZLens/Twist";
                break;
        }
    }
    private void OnDestroy()
    {
        m_lens.Clear();
    }
    public void AddPushLensData()
    {
        Hsys.Lens.LensData add_item = new Hsys.Lens.LensData();
        add_item.m_LensAccuracy = Hsys.GlobalSetting.accuracy.Half;
        m_lens.Add(add_item);
        int index = m_lens.Count - 1;
        CaseTypeOfShader(ref index);
    }

    public void DeletePopLensData()
    {
        if (m_lens.Count == 0) return;
        int index = m_lens.Count - 1;
        DestroyImmediate(m_lens[index]._Material);
        m_lens.RemoveAt(index);
    }
    public ref List<Hsys.Lens.LensData> GetLensDataList()
    {
        return ref this.m_lens;
    }
}

namespace Hsys
{
    namespace Lens
    {
        public enum lenstype
        {
            Twirl,
            Twist
        }
        [System.Serializable]
        public class LensData
        {

            public LensData()
            {
                Center.x = Center.y = 0.5f;
                Redius.x = Redius.y = 0.3f;
                Ratation = Matrix4x4.zero;
            }
            //设置
            public bool m_IsEnable;
            public lenstype m_LensType;
            public Hsys.GlobalSetting.accuracy m_LensAccuracy;
            public Hsys.GlobalSetting.quality m_Quality;
            //属性
            public Vector2 Center;
            public Vector2 Redius;
            public Matrix4x4 Ratation;
            public float Angle;
            //材质
            public Material _Material;

            public bool is_Deal;
        }
        public class LensEffects
        {
            public void CaseLens(ref string _mode, ref HsysLens lens)
            {
                switch (_mode)
                {
                    case "Twirl":
                        Hsys.Lens.LensData _Twirl = new Hsys.Lens.LensData();
                        _Twirl.m_IsEnable = true;
                        _Twirl.m_LensAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _Twirl.m_LensType = Hsys.Lens.lenstype.Twirl;
                        _Twirl.is_Deal = false;
                        lens.GetLensDataList().Add(_Twirl);
                        break;
                    case "Twist":
                        Hsys.Lens.LensData _Twist = new Hsys.Lens.LensData();
                        _Twist.m_IsEnable = true;
                        _Twist.m_LensAccuracy = Hsys.GlobalSetting.accuracy.Half;
                        _Twist.m_LensType = Hsys.Lens.lenstype.Twirl;
                        _Twist.is_Deal = false;
                        lens.GetLensDataList().Add(_Twist);
                        break;
                        
                }
            }

            public void CaseLensPass(ref string _mode, ref HsysLens lens, ref int index)
            {
                switch (_mode)
                {
                    case "Twirl":
                        lens.GetLensDataList()[index]._Material = new Material(Shader.Find("Hsys/ZLens/Twirl"));
                        break;
                    case "Twist":
                        lens.GetLensDataList()[index]._Material = new Material(Shader.Find("Hsys/ZLens/Twist"));
                        break;
                }
            }

            public bool LensTwirl(ref List<Hsys.Lens.LensData> lensdata, ref int index, ref RenderTexture source)
            {
                bool invertY = source.texelSize.y < 0.0f;
                if (invertY)
                {
                    lensdata[index].Center.y = 1.0f - lensdata[index].Center.y;
                    lensdata[index].Angle = -lensdata[index].Angle;
                }
                lensdata[index].Ratation = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, lensdata[index].Angle), Vector3.one);
                lensdata[index]._Material.SetMatrix("_RotationM", lensdata[index].Ratation);

                lensdata[index]._Material.SetVector("_CenterRadius", new Vector4(lensdata[index].Center.x, lensdata[index].Center.y, lensdata[index].Redius.x, lensdata[index].Redius.y));

                return false;
            }
            public bool LensTwist(ref List<Hsys.Lens.LensData> lensdata, ref int index, ref RenderTexture source)
            {
                bool invertY = source.texelSize.y < 0.0f;
                if (invertY)
                {
                    lensdata[index].Center.y = 1.0f - lensdata[index].Center.y;
                    lensdata[index].Angle = -lensdata[index].Angle;
                }
                //lensdata[index].Ratation = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, lensdata[index].Angle), Vector3.one);
                //lensdata[index]._Material.SetMatrix("_RotationM", lensdata[index].Ratation);

                lensdata[index]._Material.SetFloat("_Angle", lensdata[index].Angle);
                lensdata[index]._Material.SetVector("_CenterRadius", new Vector4(lensdata[index].Center.x, lensdata[index].Center.y, lensdata[index].Redius.x, lensdata[index].Redius.y));


                return false;
            }
        }
    }
}