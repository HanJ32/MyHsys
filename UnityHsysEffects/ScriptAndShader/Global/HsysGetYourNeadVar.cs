using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace Hsys
{


    [InitializeOnLoad]
    public class HsysGetYourNeadVarOfClass
    {
        private static object[] MonoVar = null;
        private static object nullobject = null;
        private static int lastnull = 0;
        
        static HsysGetYourNeadVarOfClass()
        {
            CreateMonoVarInit();
        }
        public static void CreateMonoVarInit()
        {
            if (MonoVar == null) { MonoVar = new object[Hsys.GlobalSetting.GlobalVar.Class_Size]; }
        }

        //TODO: ��Դ��������
        public static void SetMonoClassVar(object setvar)
        {
            MonoVar[lastnull] = setvar;
            lastnull += 1;
        }
        public static ref object GetMonoClassVar(System.Type classname)
        {
            for (int index = 0; index < MonoVar.Length; index += 1)
            {
                //if (MonoVar[index].GetType() == classname) { return ref MonoVar[index]; }
            }
            Debug.LogWarning("û���ҵ�����Ҫ�Ķ��� GetYourNeedClassVar ==>" + classname);
            return ref nullobject;
        }

    }
}

