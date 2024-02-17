using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public ModelsOr3DEffectsItem()
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

//PostProcessingItem 数据项
public class PostProcessingItem
{
    private Camera m_camera;
    public List<object> myobj;
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
        HsysWaterBl waterbl = m_camera.gameObject.GetComponent<HsysWaterBl>();
        if (waterbl != null)
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
}