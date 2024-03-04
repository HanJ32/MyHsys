using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Hsys
{

    //UIEffectsItem 数据项
    public class UIEffectsItem
    {
        public List<object> myobj;
        public UIEffectsItem()
        {
        }
        public void LoadScript<T>() where T : new()
        {
            if (myobj == null) { return; }
            for (int index = 0; index < myobj.Count; index += 1)
            {
                myobj.Add(new T());
            }
        }
    }

    //ImagesOr2DEffects 数据项
    public class ImagesOr2DEffectsItem
    {
        public List<object> myobj;
        public ImagesOr2DEffectsItem()
        {
        }
        public void LoadScript<T>() where T : new()
        {
            if (myobj == null) { return; }
            for (int index = 0; index < myobj.Count; index += 1)
            {
                myobj.Add(new T());
            }
        }
    }

    //ModelsOr3DEffects 数据项
    public class ModelsOr3DEffectsItem
    {
        public List<object> myobj;

        private Hsys.Effect3DBase.Effect3DBaseEffect m_effect3dbaseeffect = new Effect3DBase.Effect3DBaseEffect();

        private Hsys3DEffectBase hsys3DEffectbase;
        private HsysOutBorder hsysoutborder;

        //========================
        private GameObject __pr;
        private GameObject __pr_list;

        //===================================================================
        //private Hsys.Effect3DBase.Hsys3DEffectHierarchyMenu m_hierarchymenu;
        //===================================================================
        public void InitGameObject()
        {
            if (GameObject.Find("3DEffectBase(Hsys)") == null)
            {
                __pr = new GameObject("3DEffectBase(Hsys)");
                __pr_list = new GameObject("3DEffectList(Hsys)");
                __pr_list.transform.SetParent(__pr.transform);

            }
            else
            {
                if (GameObject.Find("3DEffectList(Hsys)") == null)
                {
                    __pr_list = new GameObject("3DEffectList(Hsys)");
                    __pr_list.transform.SetParent(__pr.transform);
                }
            }
            if (__pr == null)
            {
                __pr = GameObject.Find("3DEffectBase(Hsys)");
                __pr_list = GameObject.Find("3DEffectList(Hsys)");
            }
        }
        //=========================
        public void LoadScript<T>() where T : new()
        {
            if (myobj == null) { return; }
            for (int index = 0; index < myobj.Count; index += 1)
            {
                myobj.Add(new T());
            }
        }

        public void LoadEffect3DBase()
        {
            InitGameObject();
            if (myobj == null) { Debug.Log("未加载配置文件, 改为自加载"); }
            if (__pr == null) { Debug.Log("我不知道当前的组件在哪 Add Component ==> 3DEffectBase(Hsys) "); }
            if (__pr.gameObject.TryGetComponent<Hsys3DEffectBase>(out Hsys3DEffectBase effect3dbase))
            {
                Debug.Log("已存在该组件 Coponent ==>  Hsys3DEffectBase");
                return;
            }
            __pr.AddComponent<Hsys3DEffectBase>();
            hsys3DEffectbase = __pr.GetComponent<Hsys3DEffectBase>();
            if (hsys3DEffectbase == null) { Debug.Log("添加组件失败 Hsys3DEffectBase"); return; }
            //effect3dbase = __pr.GetComponent<Hsys3DEffectBase>();
            //effect3dbase.AddPush3DEffectData();
        }

        //====================================================================
        private HsysOutBorder[] __outborder;
        public void LoadOutBorderOurLine()
        {
            Debug.Log("LoadOutBorderOurLine");
            InitGameObject();
            if (myobj == null) { Debug.Log("未加载配置文件, 改为自加载"); }
            __outborder = __pr_list.gameObject.GetComponentsInChildren<HsysOutBorder>();
            if(__outborder == null) 
            { 
                GameObject mgobj = new GameObject();
                mgobj.AddComponent<HsysOutBorder>();
                return;
            }
            for (int index = 0; index < __outborder.Length; index += 1)
            {
                if (__outborder[index] == null)
                {
                    __outborder[index].AddComponent<HsysOutBorder>();
                    hsysoutborder = __outborder[index].GetComponent<HsysOutBorder>();
                    if (hsysoutborder == null) { Debug.LogWarning("无法添加该组件 Coponent ==>  OutBorderLine(HsysOutBorder)"); }
                }else
                {
                    Debug.Log("已存在该组件 Coponent ==>  HsysOutBorder");
                    continue;
                }
                
            }
        }
    }
    //PostProcessingItem 数据项
    public class PostProcessingItem
    {
        private Camera m_camera;
        public List<object> myobj;
        private Hsys.Blur.BlurEffects m_blureffect = new Hsys.Blur.BlurEffects();
        private Hsys.Bloom.BloomEffects m_bloomeffect = new Hsys.Bloom.BloomEffects();
        private Hsys.Toning.ToningEffects m_toningeffect = new Hsys.Toning.ToningEffects();
        private Hsys.Lens.LensEffects m_lenseffect = new Hsys.Lens.LensEffects();
        public PostProcessingItem(ref Camera _camera)
        {
            m_camera = _camera;
        }
        public void LoadScript<T>() where T : new()
        {
            if (myobj == null) { return; }
            for (int index = 0; index < myobj.Count; index += 1)
            {
                myobj.Add(new T());
            }
        }

        public void LoadWaterBl()
        {
            if (myobj == null) { Debug.Log("未加载配置文件, 改为自加载"); }
            if (m_camera == null) { Debug.Log("我不知道你的摄像机在哪 Add Component ==> Camera "); return; }

            if (m_camera.gameObject.TryGetComponent<HsysWaterBl>(out HsysWaterBl waterbl))
            {
                Debug.Log("已存在该组件 Coponent ==>  WaterBl");
                if (waterbl._Material == null)
                {
                    waterbl._Material = new Material(Shader.Find("Hsys/WaterBl"));
                }
                return;
            }
            m_camera.gameObject.AddComponent<HsysWaterBl>();
            waterbl = m_camera.gameObject.GetComponent<HsysWaterBl>();
            waterbl._Material = new Material(Shader.Find("Hsys/WaterBl"));
        }

        public void LoadDepthOfField()
        {
            if (myobj == null) { Debug.Log("未加载配置文件, 改为自加载"); }
            if (m_camera == null) { Debug.Log("我不知道你的摄像机在哪 Add Component ==> Camera "); return; }
            if (m_camera.gameObject.TryGetComponent<HsysDepthOfField>(out HsysDepthOfField depthoffield))
            {
                Debug.Log("已存在该组件 Coponent ==> DepthOfField");
                if (depthoffield._Material == null)
                {
                    depthoffield._Material = new Material(Shader.Find("Hsys/DepthOfField"));
                }
                return;
            }
            m_camera.gameObject.AddComponent<HsysDepthOfField>();
            depthoffield = m_camera.gameObject.GetComponent<HsysDepthOfField>();
            depthoffield._Material = new Material(Shader.Find("Hsys/DepthOfField"));
        }

        public void LoadBlur(string _mode)
        {
            if (myobj == null) { Debug.Log("未加载配置文件, 改为自加载"); }
            if (m_camera == null) { Debug.Log("我不知道你的摄像机在哪 Add Component ==> Camera "); return; }
            if (m_camera.gameObject.TryGetComponent<HsysBlur>(out HsysBlur blur))
            {
                Debug.Log("已存在该组件 Coponent ==> DepthOfField");
                if (blur.GetBlurDataList() == null)
                {
                    blur.GetBlurDataList() = new List<Hsys.Blur.BlurData>();
                    //TODO: 还要再重构的
                }
                m_blureffect.CaseBlur(ref _mode, ref blur);

                if (blur.GetBlurDataList().Count <= 0) return;

                for (int index = 0; index < blur.GetBlurDataList().Count; index += 1)
                {
                    if (blur.GetBlurDataList()[index]._Material == null)
                    {
                        m_blureffect.CaseBlurPass(ref _mode, ref blur, ref index);
                    }
                }

                return;
            }
            m_camera.gameObject.AddComponent<HsysBlur>();
            if (!m_camera.gameObject.TryGetComponent<HsysBlur>(out blur)) return;
            if (blur.GetBlurDataList() == null)
            {
                blur.GetBlurDataList() = new List<Hsys.Blur.BlurData>();
            }
            m_blureffect.CaseBlur(ref _mode, ref blur);
            if (blur.GetBlurDataList().Count <= 0) return;
            for (int index = 0; index < blur.GetBlurDataList().Count; index += 1)
            {

                m_blureffect.CaseBlurPass(ref _mode, ref blur, ref index);
            }
        }

        public void LoadBloom(string _mode)
        {
            if (myobj == null) { Debug.Log("未加载配置文件, 改为自加载"); }
            if (m_camera == null) { Debug.Log("我不知道你的摄像机在哪 Add Component ==> Camera "); return; }
            if (m_camera.gameObject.TryGetComponent<HsysBloom>(out HsysBloom bloom))
            {
                Debug.Log("已存在该组件 Coponent ==> DepthOfField");
                if (bloom.GetBloomDataList() == null)
                {
                    bloom.GetBloomDataList() = new List<Bloom.BloomData>();
                }

                if (bloom.GetBloomDataList().Count <= 0) return;
                for (int index = 0; index < bloom.GetBloomDataList().Count; index += 1)
                {
                    if (bloom.GetBloomDataList()[index]._Material == null)
                    {
                        m_bloomeffect.CaseBloomPass(ref _mode, ref bloom, ref index);
                    }
                }
                return;
            }
            m_camera.gameObject.AddComponent<HsysBloom>();
            if (!m_camera.gameObject.TryGetComponent<HsysBloom>(out bloom)) return;
            if (bloom.GetBloomDataList() == null)
            {
                bloom.GetBloomDataList() = new List<Bloom.BloomData>();
            }
            m_bloomeffect.CaseBloom(ref _mode, ref bloom);
            if (bloom.GetBloomDataList().Count <= 0) return;
            for (int index = 0; index < bloom.GetBloomDataList().Count; index += 1)
            {
                if (bloom.GetBloomDataList()[index]._Material == null)
                {
                    m_bloomeffect.CaseBloomPass(ref _mode, ref bloom, ref index);
                }
            }
        }

        public void LoadToning(string _mode)
        {
            if (myobj == null) { Debug.Log("未加载配置文件, 改为自加载"); }
            if (m_camera == null) { Debug.Log("我不知道你的摄像机在哪 Add Component ==> Camera "); return; }
            if (m_camera.gameObject.TryGetComponent<HsysToning>(out HsysToning toning))
            {
                Debug.Log("已存在该组件 Coponent ==> DepthOfField");
                if (toning.GetToningDataList() == null)
                {
                    toning.GetToningDataList() = new List<Toning.ToningData>();
                }

                if (toning.GetToningDataList().Count <= 0) return;
                for (int index = 0; index < toning.GetToningDataList().Count; index += 1)
                {
                    if (toning.GetToningDataList()[index]._Material == null)
                    {
                        m_toningeffect.CaseToningPass(ref _mode, ref toning, ref index);
                    }
                }
                return;
            }
            m_camera.gameObject.AddComponent<HsysToning>();
            if (!m_camera.gameObject.TryGetComponent<HsysToning>(out toning)) return;
            if (toning.GetToningDataList() == null)
            {
                toning.GetToningDataList() = new List<Toning.ToningData>();
            }
            m_toningeffect.CaseToning(ref _mode, ref toning);
            if (toning.GetToningDataList().Count <= 0) return;
            for (int index = 0; index < toning.GetToningDataList().Count; index += 1)
            {
                if (toning.GetToningDataList()[index]._Material == null)
                {
                    m_toningeffect.CaseToningPass(ref _mode, ref toning, ref index);
                }
            }
        }

        public void LoadLens(string _mode)
        {
            if (myobj == null) { Debug.Log("未加载配置文件, 改为自加载"); }
            if (m_camera == null) { Debug.Log("我不知道你的摄像机在哪 Add Component ==> Camera "); return; }
            if (m_camera.gameObject.TryGetComponent<HsysLens>(out HsysLens lens))
            {
                Debug.Log("已存在该组件 Coponent ==> DepthOfField");
                if (lens.GetLensDataList() == null)
                {
                    lens.GetLensDataList() = new List<Lens.LensData>();
                }

                if (lens.GetLensDataList().Count <= 0) return;
                for (int index = 0; index < lens.GetLensDataList().Count; index += 1)
                {
                    if (lens.GetLensDataList()[index]._Material == null)
                    {
                        m_lenseffect.CaseLensPass(ref _mode, ref lens, ref index);
                    }
                }
                return;
            }
            m_camera.gameObject.AddComponent<HsysLens>();
            if (!m_camera.gameObject.TryGetComponent<HsysLens>(out lens)) return;
            if (lens.GetLensDataList() == null)
            {
                lens.GetLensDataList() = new List<Lens.LensData>();
            }
            m_lenseffect.CaseLens(ref _mode, ref lens);
            if (lens.GetLensDataList().Count <= 0) return;
            for (int index = 0; index < lens.GetLensDataList().Count; index += 1)
            {
                if (lens.GetLensDataList()[index]._Material == null)
                {
                    m_lenseffect.CaseLensPass(ref _mode, ref lens, ref index);
                }
            }
        }
    }
}
