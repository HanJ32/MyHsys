using Hsys.Effect3DBase;
using System.Collections.Generic;
using UnityEngine;

public class Hsys3DEffectBase : MonoBehaviour
{
    [Tooltip("3D Effect 处理队列")][SerializeField] private List<Effect3DData> m_3deffectlist = new List<Effect3DData>();
    private Effect3DBaseEffect m_3deffectbaseeffect;
    //private Hsys3DEffectHierarchyMenu m_hsys3deffecthierarchymenu;

    private void OnEnable()
    {
        Hsys.HsysGetYourNeadVarOfClass.SetMonoClassVar(this);
    }

    private void OnDestroy()
    {
        m_3deffectlist.Clear();
    }
    public void AddPush3DEffectData()
    {
        if(m_3deffectlist.Count >= 8) { return; }
        Hsys.Effect3DBase.Effect3DData add_item = new Hsys.Effect3DBase.Effect3DData();
        m_3deffectlist.Add(add_item);
        int index = m_3deffectlist.Count - 1;
        //CaseTypeOfShader(ref index);
    }

    public void DeletePop3DEffectData()
    {
        if (m_3deffectlist.Count == 0) { return; }
        int index = m_3deffectlist.Count - 1;
        //DestroyImmediate(m_3deffectlist[index]._Material);
        m_3deffectlist.RemoveAt(index);
    }
    public ref List<Effect3DData> Get3DEffectDataList()
    {
        return ref this.m_3deffectlist;
    }
}

namespace Hsys
{
    namespace Effect3DBase
    {
        public enum model3deffect
        {
            OutBorder
        }
        [System.Serializable]
        public class Effect3DData
        {
            public Effect3DData()
            {
                m_EffectItem = null;
            }
            public bool m_IsEnable;
            public model3deffect m_Model3DEffect;

            public object m_EffectItem;
        }

        public class Effect3DBaseEffect
        {
            public void Case3DEffect(ref List<Effect3DData> effect3ddata,  ref int index)
            {
                //TODO:Create New Effect
                switch(effect3ddata[index].m_Model3DEffect)
                {
                    case model3deffect.OutBorder:
                        OutBorder(ref effect3ddata, ref index);
                        break;
                }
            }

            private void OutBorder(ref List<Effect3DData> effect3ddata,ref int index)
            {
                HsysOutBorder newoutborder = new HsysOutBorder();
                effect3ddata[index].m_EffectItem = newoutborder;
            }
        }
    }
}