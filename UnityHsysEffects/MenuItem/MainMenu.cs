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

        //shader 的计算复杂度 越高性能开销越大,质量越高 可调参数更多
        //中低级 在绝大场合适用
        public enum modellevel
        {
            None,
            LOW,
            MIDDLE,
            HEIGH
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
            public int id;              //加载的编号 // 4 //开发时追踪错误用的
            public int modellevel;      //效果与复杂度级别
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
        private Camera m_camera;

        private List<UseScriptObject> m_listofsptobj;

        //json配置 Default path
        private string m_path = GlobalSetting.GlobalVar.Default_path;
        private JsonLoadScriptInfo m_json_load_scriptinfo;


        private void OnEnable()
        {
            m_listofsptobj = new List<UseScriptObject>();
            m_json_load_scriptinfo = new JsonLoadScriptInfo(m_path, ref m_listofsptobj);

        }

        //[UnityEditor.MenuItem("Tools/Hsys/Effects/2D"), Tooltip("2D效果")]

        //[UnityEditor.MenuItem("Tools/Hsys/Effects/3D"), Tooltip("3D效果")]

        //[UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing"), Tooltip("后处理效果")]
        /*        private void PostProcessings()
                {
                    m_camera = GameObject.FindObjectOfType<Camera>();
                    if (m_camera == null) { return; }
                    if (Private_Hsys.GlobalVar.m_post_processing == null)
                    {
                        Private_Hsys.GlobalVar.m_post_processing = new PostProcessingItem(ref m_camera);
                    }
                }
        */
        [UnityEditor.MenuItem("Tools/Hsys/Effects/Setting"), Tooltip("设置")]
        private static void Setting()
        {
            EditorWindow mywindow = EditorWindow.GetWindow(typeof(MainMenu), true, "HsysEffects");
            //mywindow.maxSize = new Vector2(500f, 1000f);
            mywindow.minSize = new Vector2(400f, 500f);
            mywindow.Show();
        }

        //=====================================================
        //private SerializedObject _serializedObject = null;
        //private SerializedProperty _serializedProperty = null;
        private modellevel _modelLevel = modellevel.None;
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

            //GUILayout.Space(10);
            //_serializedObject.Update();
            //EditorGUI.BeginChangeCheck();
            //_serializedObject = new SerializedObject(this);
            //_serializedProperty = _serializedObject.FindProperty("m_load_script_info");
            //EditorGUILayout.PropertyField(_serializedProperty,true);

            //if (EditorGUI.EndChangeCheck())
            //{//提交修改
            //   _serializedObject.ApplyModifiedProperties();
            //}

            GUILayout.Space(10);
            GUILayout.Label("设置(Setting)");

            _modelLevel = (modellevel)EditorGUILayout.EnumPopup("效果与复杂度级别", _modelLevel);

            m_path = EditorGUILayout.TextField("配置文件路径", m_path);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            //GUIStyle mytitlestyle2 = new GUIStyle();
            //mytitlestyle2.alignment = TextAnchor.MiddleCenter;
            //GUILayout.BeginArea(new Rect(50,50,150,150), mytitlestyle2);
            /*            GUILayoutOption guiop;
                        guiop = GUILayout.MinHeight(50);
                        guiop = GUILayout.MaxHeight(50);
                        guiop = GUILayout.MaxWidth(50);
                        guiop = GUILayout.MinWidth(50);*/

            //GUILayout.BeginArea(new Rect(50,50,100,100));
            if (GUILayout.Button("SaveSetting"))
            {
                OnChilkSaveSetting();
            }
            if (GUILayout.Button("AddSetting"))
            {
                OnChilkAddSetting();
            }
            //GUILayout.EndArea();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            if(GUILayout.Button("生成噪声图"))
            {
                EditorWindow CreateNoise = EditorWindow.GetWindow(typeof(CreateNoiseWindow), true, "Noise");
                //mywindow.maxSize = new Vector2(500f, 1000f);
                CreateNoise.minSize = new Vector2(400f, 500f);
                CreateNoise.Show();

            }

            GUILayout.Space(10);
            if(GUILayout.Button("LUT图生成"))
            {
                EditorWindow CreateLUT = EditorWindow.GetWindow(typeof(CreateLUTWindow), true, "LUT");
                CreateLUT.minSize = new Vector2(400f, 500f);
                CreateLUT.Show();
            }

            GUILayout.Space(10);
            if(GUILayout.Button("光线追踪"))
            {
                EditorWindow ImportRealLight = EditorWindow.GetWindow(typeof(ImportRealLightWindow), true, "Light");
                ImportRealLight.minSize = new Vector2(400f, 500f);
                ImportRealLight.Show();
            }

            //GUILayout.BeginArea(new Rect(50, 50, 150, 150));
            //应用并刷新所有配置
            GUILayout.Space(10);
            if (GUILayout.Button("Apply"))
            {
                OnChilkApply();
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
        private void OnChilkSaveSetting()
        {

        }
        private void OnChilkAddSetting()
        {

        }
        private void OnChilkApply()
        {

        }
        private void OnChilkClearAll()
        {

        }

        //卸载掉文件
        private void OnDestroy()
        {
            if (m_listofsptobj != null)
            {
                m_listofsptobj.Clear();
            }
        }
    }

    public class UIEffectsMenuItem
    {
        private static UIEffectsItem m_ui_effects = null;
        private void CreateImagesOr2DEffects()
        {
            if (m_ui_effects == null)
            {
                m_ui_effects = new UIEffectsItem();
            }

            Debug.Log("ImagesOr2DEffects");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/UI/one"), Tooltip("test")]
        private static void One()
        {

        }
    }


    //ImagesOr2DEffects 菜单项   
    public class ImagesOr2DEffectsMenuItem
    {
        //加载的脚本
        public ImagesOr2DEffectsItem m_image_or_2d_effects;
        private void CreateImagesOr2DEffects()
        {
            if (m_image_or_2d_effects == null)
            {
                m_image_or_2d_effects = new ImagesOr2DEffectsItem();
            }

            Debug.Log("ImagesOr2DEffects");
        }


        [UnityEditor.MenuItem("Tools/Hsys/Effects/2D/one"), Tooltip("test")]
        private static void One()
        {
            Debug.Log("one");
        }
    }

    //ModelsOr3DEffects 菜单项
    public class ModelsOr3DEffectsMenuItem
    {
        public ModelsOr3DEffectsItem m_model_or_3d_effects = null;
        private void CreateModelsOr3DEffects()
        {
            if (m_model_or_3d_effects == null)
            {
                m_model_or_3d_effects = new ModelsOr3DEffectsItem();
            }
            Debug.Log("ModelsOr3DEffects");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/3D/one"), Tooltip("test")]
        private static void One()
        {
            Debug.Log("one");
        }
    }

    public class PostProcessingMenuItem
    {
        //静态实例
        private static PostProcessingItem m_post_processing = null;
        private static Camera m_camera = null;

        private static void CreatePostProcessing()
        {
            if (m_camera == null)
            {
                m_camera = GameObject.FindObjectOfType<Camera>();
            }
            if (m_post_processing == null)
            {
                m_post_processing = new PostProcessingItem(ref m_camera);
            }
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/WaterBl"), Tooltip("屏幕水波纹效果(后处理)")]
        private static void WaterBl()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadWaterBl();
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/DepthOfField"), Tooltip("景深(后处理)")]
        private static void DepthOfField()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadDepthOfField();
        }

        //============================================ BLUR ==================================================
        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Blur/Box"), Tooltip("方框模糊(后处理)")]
        private static void BoxBlur()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadBlur("Box");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Blur/Bokeh"), Tooltip("散景模糊(后处理)")]
        private static void BokeBlur()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadBlur("Boke");
        }
        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Blur/TiltShift"), Tooltip("移轴模糊(后处理)")]
        private static void TiltShiftBlur()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadBlur("TiltShift");
        }
        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Blur/Iris"), Tooltip("光圈模糊(后处理)")]
        private static void IrisBlur()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadBlur("Iris");
        }
        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Blur/Grainy"), Tooltip("粒状模糊(后处理)")]
        private static void GrainyBlur()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadBlur("Grainy");
        }
        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Blur/Radial"), Tooltip("径向模糊(后处理)")]
        private static void RadialBlur()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadBlur("Radial");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Blur/Directional"), Tooltip("方向模糊(后处理)")]
        private static void DirectionalBlur()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadBlur("Directional");
        }



        //==============================Bloom==============================================
        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Bloom/HDR"), Tooltip("HDR辉光(后处理)")]
        private static void HDRBloom()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadBloom("HDR");
        }
        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Bloom/NoHDR"), Tooltip("不带HDR辉光(后处理)")]
        private static void NoHDRBloom()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadBloom("NoHDR");
        }


        //===========================Toning===============================================
        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Toning/Brightness"), Tooltip("亮度(后处理)")]
        private static void BrightnessToning()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadToning("Brightness");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Toning/Saturation"), Tooltip("饱和度(后处理)")]
        private static void SaturationToning()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadToning("Saturation");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Toning/Vibrance"), Tooltip("自然饱和度(后处理)")]
        private static void VibranceToning()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadToning("Vibrance");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Toning/Level"), Tooltip("色阶(后处理)")]
        private static void LevelToning()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadToning("Level");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Toning/ContrastRatio"), Tooltip("对比度(后处理)")]
        private static void ContrastRatioToning()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadToning("ContrastRatio");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Toning/ColorGraying"), Tooltip("色调分离(后处理)")]
        private static void ColorGrayingToning()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadToning("ColorGraying");
        }

        //=====================================Lens========================================
        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Lens/Twirl"), Tooltip("扭曲(后处理)")]
        private static void TwirlLens()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadLens("Twirl");
        }

        [UnityEditor.MenuItem("Tools/Hsys/Effects/PostProcessing/Lens/Twist"), Tooltip("旋转(后处理)")]
        private static void TwistLens()
        {
            CreatePostProcessing();
            if (m_post_processing == null) { Debug.LogWarning("[Hsys Effects PostProcessing Warning] 我不知道你的摄像机在哪 Add Component ==> Camera"); return; }
            m_post_processing.LoadLens("Twist");
        }
    }

    //==========================================================================================================

    //全局Json LoadScriptInfo 管理
    public class JsonLoadScriptInfo
    {

        //当前被激活的文件路径
        private string m_active_path = null;
        private string json_format = null;
        private LoadScriptInfo m_load_script_info;
        public JsonLoadScriptInfo(string _path, ref List<UseScriptObject> _description)
        {
            if (_path == null || _path == "" || _path == " " || _path == string.Empty) { Debug.Log("文件配置未加载 NULL"); return; }
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
            if (m_load_script_info == null) { return; }
            UseScriptObject u = new UseScriptObject();
            for (int index = 0; index < m_load_script_info.info.Count; index += 1)
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

    //================================================================================
    public class CreateNoiseWindow : EditorWindow
    {
        //文件存储路径
        private string m_path;
        private void OnGUI()
        {
            GUILayout.Label("预览图");
            GUILayout.Space(10);
            m_path = EditorGUILayout.TextField("文件存储路径", m_path);
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成"))
            {
                Debug.Log("生成");
            }
            if(GUILayout.Button("保存"))
            {
                Debug.Log("保存");
            }
            EditorGUILayout.EndHorizontal();
        }

    }


    namespace Private_Hsys
    {
        public enum lutsize
        {
            _256X256,
            _512X512,
            _1024X1024,
            _2048X2048
        };
    }
    public class CreateLUTWindow : EditorWindow
    {
        private string m_path;
        private lutsize m_preview_size;
        private lutsize m_save_size;
        private void OnGUI()
        {
            GUILayout.Label("预览图");
            GUILayout.Space(10);
            m_path = EditorGUILayout.TextField("文件存储路径", m_path);
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            m_preview_size = (lutsize)EditorGUILayout.EnumPopup("预览分辨率", m_preview_size);
            m_save_size = (lutsize)EditorGUILayout.EnumPopup("保存分辨率", m_save_size);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成"))
            {
                Debug.Log("生成");
            }
            if (GUILayout.Button("保存"))
            {
                Debug.Log("保存");
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public class ImportRealLightWindow : EditorWindow
    {
        private void OnGUI()
        {
            GUILayout.Label("光线追踪");

            GUILayout.Space(10);
            
        }
    }

    //默认配置路径
    namespace Private_Hsys
    {
        public class IsJsonOnLoadDefault
        {
            [SerializeField] private LoadScriptInfo m_default;
            public void Create()
            {
                Init();
                if (m_default == null) { Debug.LogWarning("Not This Json File"); return; }
                ScriptInfo myscript = new ScriptInfo();
                myscript.name = "xxx";
                myscript.path = "xxx";
                myscript.id = 0;
                myscript.classification = 0;
                m_default.info.Add(myscript);



                System.IO.File.WriteAllText(GlobalSetting.GlobalVar.Default_path, JsonUtility.ToJson(m_default));
            }

            private void Init()
            {
                if (m_default == null)
                {
                    m_default = new LoadScriptInfo();
                }
            }
        }
        
        
    }
}