Shader "Hsys/ZToning/ColorGrading"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Strength ("Strength", Float) = 1.0
        _HUE_Saturation_Value("HUE_Saturation_Value", Vector) = (1.0,1.0,1.0,1.0)
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
        half3 _HUE_Saturation_Value;
        half _Strength;
        
    ENDCG
    SubShader
    {
        Name "Color_Grading"
        ZTest Always Cull Off ZWrite Off 
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            struct v2f_colorgrading
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };



            v2f_colorgrading vert (appdata v)
            {
                v2f_colorgrading o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_colorgrading i) : SV_Target
            {

                return Hsys::Toning::ColorGrading(_MainTex,i.uv,_Strength,_HUE_Saturation_Value);
            }
            ENDCG
        }
    }
}
