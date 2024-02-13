using Hsys.Private_Hsys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Hsys
{
    namespace Private_Hsys
    {
        //模块分类
        public enum loadmodelclassification
        {
            EFFECTS_2D,
            EFFECTS_3D,
            EFFECTS_POStPROCESSING
        }

        //使用了什么模块
        public enum usingwhatmodels
        {

        }

        //存储加载Script的信息，Json加载
        [Serializable]
        public class LoadScriptInfo
        {
            public List<ScriptInfo> info;
        }

        //Json使用的子存储序列
        [Serializable]
        public class ScriptInfo
        {                               //x32
            public int id;              //加载的编号 // 4
            public int classification;  //所述分类 0:EFFECTS_2D     1:EFFECTS_3D    2:EFFECTS_POStPROCESSING //4
            public string name;         //文件名 // 8
            public string path;         //文件所在地//8
        }

        //主要使用的
        public struct UseScriptObject
        {
            public loadmodelclassification classifimation;
            //绑定到的对象
            public GameObject m_obj;
        }
    }

    //菜单栏
    public class MainMenu : EditorWindow
    {
        //正在激活的相机对象，给后处理使用
        private static Camera m_camera;

        private List<UseScriptObject> m_listofsptobj;
        //加载的脚本
        private static ImagesOr2DEffectsItem m_image_or_2d_effects;
        private static ModelsOr3DEffectsItem m_model_or_3d_effects;
        private static PostProcessingItem m_post_processing;

        private void OnEnable()
        {
            m_listofsptobj = new List<UseScriptObject>();

        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/2D"), Tooltip("2D效果")]
        private static void ImagesOr2DEffects()
        {
            if (m_image_or_2d_effects == null)
            {
                m_image_or_2d_effects = new ImagesOr2DEffectsItem();
            }

            Debug.Log("ImagesOr2DEffects");
        }
        [UnityEditor.MenuItem("Tools/Hsys/Effects/3D"), Tooltip("3D效果")]
        private static void ModelsOr3DEffects()
        {
            if (m_model_or_3d_effects == null)
            {
                m_model_or_3d_effects = new ModelsOr3DEffectsItem();
            }
            Debug.Log("ModelsOr3DEffects");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing"), Tooltip("后处理效果")]
        private static void PostProcessings()
        {
            m_camera = GameObject.FindObjectOfType<Camera>();
            if (m_camera == null) { return; }
            if (m_post_processing == null)
            {
                m_post_processing = new PostProcessingItem();
            }
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/Setting"), Tooltip("设置")]
        private static void Setting()
        {
            EditorWindow mywindow = EditorWindow.GetWindow(typeof(MainMenu), true, "HsysEffects");
            //mywindow.maxSize = new Vector2(500f, 1000f);
            mywindow.minSize = new Vector2(400f, 500f);
            mywindow.Show();
        }

        //=====================================================
        private SerializedObject _serializedObject = null;
        private SerializedProperty _serializedProperty = null;
        //=====================================================
        //绘制插件内容
        private void OnGUI()
        {
            GUIContent content = new GUIContent();
            content.text = "HsysEffects";
            GUIStyle mytitlestyle = new GUIStyle();
            mytitlestyle.alignment = TextAnchor.MiddleCenter;
            mytitlestyle.fontSize = 32;
            mytitlestyle.fontStyle = FontStyle.Bold;
            mytitlestyle.normal.textColor = Color.white;
            GUILayout.Label(content, mytitlestyle);

            GUILayout.Space(10);
            //_serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            _serializedObject = new SerializedObject(this);
            _serializedProperty = _serializedObject.FindProperty("m_load_script_info");
            EditorGUILayout.PropertyField(_serializedProperty,true);

            if (EditorGUI.EndChangeCheck())
            {//提交修改
                _serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(10);
            GUILayoutOption guiop;
            guiop = GUILayout.MinHeight(80);
            guiop = GUILayout.MaxHeight(80);
            if (GUILayout.Button("SaveSetting", guiop))
            {

            }
            if (GUILayout.Button("AddSetting", guiop))
            {

            }



            //应用并刷新所有配置
            GUILayout.Space(10);
            if (GUILayout.Button("Apply"))
            {
                Debug.Log("Apply");
            }

            //删除所有被绑定的脚本或对象
            GUILayout.Space(10);
            if (GUILayout.Button("ClearAll"))
            {
                Debug.Log("ClearAll");
                OnChilkClearAll();
            }
        }

        //==========================按键实现==================================
        private void OnChilkClearAll()
        {
            
        }
    }

    public class ImagesOr2DEffectsItem
    {
        public ImagesOr2DEffectsItem()
        {
            LoadScript();
        }
        private void LoadScript()
        {

        }

    }

    public class ModelsOr3DEffectsItem
    {

        public ModelsOr3DEffectsItem()
        {
            LoadScript();
        }
        private void LoadScript()
        {

        }
    }


    public class PostProcessingItem
    {
        public PostProcessingItem()
        {
            LoadScript();
        }
        private void LoadScript()
        {

        }
    }




    //全局Json LoadScriptInfo 管理
    public class JsonLoadScriptInfo
    {

        //当前被激活的文件路径
        private string m_active_path = null;
        private string json_format = null;
        private LoadScriptInfo m_load_script_info;
        public JsonLoadScriptInfo(string _path, ref List<UseScriptObject> _description)
        {
            m_active_path = _path;
            //读取到的JSON内容
            json_format = System.IO.File.ReadAllText(_path);
            //解码
            m_load_script_info = JsonUtility.FromJson<LoadScriptInfo>(json_format);
            InitUseScriptObject(ref _description);
        }

        public void JsonSave(ref LoadScriptInfo _save)
        {
            //IO FILE WRITEALLTEXT
            System.IO.File.WriteAllText(m_active_path, JsonUtility.ToJson(_save));
        }

        //==========================================================
        public string GetMyActiveFilePath()
        {
            return m_active_path;
        }

        private void InitUseScriptObject(ref List<UseScriptObject> _description)
        {
            if(m_load_script_info == null) { return; }
            UseScriptObject u = new UseScriptObject() ;
            for (int index = 0; index < m_load_script_info.info.Count; index+=1)
            {
                if (m_load_script_info.info[index] == null) { continue; }

                switch (m_load_script_info.info[index].classification)
                {
                    case 0:
                        u = new UseScriptObject();
                        u.classifimation = loadmodelclassification.EFFECTS_2D;
                        _description.Add(u);
                        break;
                    case 1:
                        u = new UseScriptObject();
                        u.classifimation = loadmodelclassification.EFFECTS_3D;
                        _description.Add(u);
                        break;
                    case 2:
                        u = new UseScriptObject();
                        u.classifimation = loadmodelclassification.EFFECTS_POStPROCESSING;
                        _description.Add(u);
                        break;
                }
            }
        }
    }
}