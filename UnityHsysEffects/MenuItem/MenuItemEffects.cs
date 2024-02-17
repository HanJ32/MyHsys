using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UIEffectsItem ������
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

//ImagesOr2DEffects ������
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

//ModelsOr3DEffects ������
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

//PostProcessingItem ������
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
        if (myobj == null) { Debug.Log("δ���������ļ�, ��Ϊ�Լ���"); }
        if (m_camera == null) { Debug.Log("�Ҳ�֪�������������� Add Component ==> Camera "); return; }
        HsysWaterBl waterbl = m_camera.gameObject.GetComponent<HsysWaterBl>();
        if (waterbl != null)
        {
            Debug.Log("�Ѵ��ڸ���� Coponent ==>  WaterBl");
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