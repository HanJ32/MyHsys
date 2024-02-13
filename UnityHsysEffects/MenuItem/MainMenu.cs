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
        //ģ�����
        public enum loadmodelclassification
        {
            EFFECTS_2D,
            EFFECTS_3D,
            EFFECTS_POStPROCESSING
        }

        //ʹ����ʲôģ��
        public enum usingwhatmodels
        {

        }

        //�洢����Script����Ϣ��Json����
        [Serializable]
        public class LoadScriptInfo
        {
            public List<ScriptInfo> info;
        }

        //Jsonʹ�õ��Ӵ洢����
        [Serializable]
        public class ScriptInfo
        {                               //x32
            public int id;              //���صı�� // 4
            public int classification;  //�������� 0:EFFECTS_2D     1:EFFECTS_3D    2:EFFECTS_POStPROCESSING //4
            public string name;         //�ļ��� // 8
            public string path;         //�ļ����ڵ�//8
        }

        //��Ҫʹ�õ�
        public struct UseScriptObject
        {
            public loadmodelclassification classifimation;
            //�󶨵��Ķ���
            public GameObject m_obj;
        }
    }

    //�˵���
    public class MainMenu : EditorWindow
    {
        //���ڼ����������󣬸�����ʹ��
        private static Camera m_camera;

        private List<UseScriptObject> m_listofsptobj;
        //���صĽű�
        private static ImagesOr2DEffectsItem m_image_or_2d_effects;
        private static ModelsOr3DEffectsItem m_model_or_3d_effects;
        private static PostProcessingItem m_post_processing;

        private void OnEnable()
        {
            m_listofsptobj = new List<UseScriptObject>();

        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/2D"), Tooltip("2DЧ��")]
        private static void ImagesOr2DEffects()
        {
            if (m_image_or_2d_effects == null)
            {
                m_image_or_2d_effects = new ImagesOr2DEffectsItem();
            }

            Debug.Log("ImagesOr2DEffects");
        }
        [UnityEditor.MenuItem("Tools/Hsys/Effects/3D"), Tooltip("3DЧ��")]
        private static void ModelsOr3DEffects()
        {
            if (m_model_or_3d_effects == null)
            {
                m_model_or_3d_effects = new ModelsOr3DEffectsItem();
            }
            Debug.Log("ModelsOr3DEffects");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing"), Tooltip("����Ч��")]
        private static void PostProcessings()
        {
            m_camera = GameObject.FindObjectOfType<Camera>();
            if (m_camera == null) { return; }
            if (m_post_processing == null)
            {
                m_post_processing = new PostProcessingItem();
            }
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/Setting"), Tooltip("����")]
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
        //���Ʋ������
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
            {//�ύ�޸�
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



            //Ӧ�ò�ˢ����������
            GUILayout.Space(10);
            if (GUILayout.Button("Apply"))
            {
                Debug.Log("Apply");
            }

            //ɾ�����б��󶨵Ľű������
            GUILayout.Space(10);
            if (GUILayout.Button("ClearAll"))
            {
                Debug.Log("ClearAll");
                OnChilkClearAll();
            }
        }

        //==========================����ʵ��==================================
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




    //ȫ��Json LoadScriptInfo ����
    public class JsonLoadScriptInfo
    {

        //��ǰ��������ļ�·��
        private string m_active_path = null;
        private string json_format = null;
        private LoadScriptInfo m_load_script_info;
        public JsonLoadScriptInfo(string _path, ref List<UseScriptObject> _description)
        {
            m_active_path = _path;
            //��ȡ����JSON����
            json_format = System.IO.File.ReadAllText(_path);
            //����
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