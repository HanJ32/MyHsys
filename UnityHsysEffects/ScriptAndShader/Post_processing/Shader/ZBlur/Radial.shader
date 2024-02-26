Shader "Hsys/ZBlur/Radial"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Blur("Blur", Float) = 1.0
        _Count("Count", Range(1,5)) = 1
        _Redius("Redius", Float) = 0.1
        _Params("Params", Vector) = (0,0,0,0)
        _Center("Params",vector) = (0,0,0,0)
    }

     CGINCLUDE
        #include "../../../BaseLib/CgincLib/Postprocessing/Blur.cginc"
        

        #include "UnityCG.cginc"
        
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        uniform sampler2D _MainTex;
        uniform float4 _MainTex_ST;
        half _Count;
        half _Blur;
        half _Redius;
        half4 _Params;
        half4 _Center;
    ENDCG
    SubShader
    {
        Name "RADIAL"
        Tags { "RenderType"="Opaque" }
        LOD 3100
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct v2f_radial_blur
            {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            };


            v2f_radial_blur vert (appdata v)
            {
                v2f_radial_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_radial_blur i) : SV_Target
            {

                return Hsys::Blur::Blur_Radial(_MainTex,i.uv,half4(_Blur*0.1,_Count,_Params.xy));
            }
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct v2f_radial_blur
            {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            };


            v2f_radial_blur vert (appdata v)
            {
                v2f_radial_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_radial_blur i) : SV_Target
            {

                return Hsys::Blur::Blur_Radial(_MainTex,i.uv,half4(_Blur,_Count,_Params.xy), _Redius, _Center);
            }
            ENDCG
        }
    }
}
