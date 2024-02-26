Shader "Hsys/ZToning/Level"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _IN_OUT ("In_Out", Vector) = (0.0,1.0,0.0,1.0)
        _Strength ("Strength", Float) = 1.0
    }

    CGINCLUDE 
            #pragma vertex vert
            #pragma fragment frag

            #include "../../../BaseLib/CgincLib/Postprocessing/Toning.cginc"
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _IN_OUT;
            half _Strength;
    ENDCG
    SubShader
    {
        Name "Toning_Level"
        ZTest Always Cull Off ZWrite Off 
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            struct v2f_level
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            

            v2f_level vert (appdata v)
            {
                v2f_level o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_level i) : SV_Target
            {
                return Hsys::Toning::Toning_Level(_MainTex, i.uv, _IN_OUT.x,_IN_OUT.y,_IN_OUT.z, _IN_OUT.w, _Strength);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            struct v2f_level_ar_vr
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            

            v2f_level_ar_vr vert (appdata v)
            {
                v2f_level_ar_vr o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_level_ar_vr i) : SV_Target
            {
                return Hsys::Toning::Toning_Level(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv,_MainTex_ST), _IN_OUT.x,_IN_OUT.y,_IN_OUT.z, _IN_OUT.w, _Strength,true);
            }
            ENDCG
        }
    }
}
