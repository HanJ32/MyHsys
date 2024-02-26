Shader "Hsys/ZBlur/TiltShift"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Blur("Blur", Float) = 1.0
        _Count("Count", Range(1,5)) = 1
        _Redius("Redius", Float) = 1.0
        _Params("Params", Vector) = (1.0,1.0,1.0,1.0)
        _Area ("Area", Float) = 1.0
        _Spread ("Spread", Float) = 1.0
        _Offset("Offset",Float) = 1.0
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
        uniform float4 _MainTex_TexelSize;
        half _Count;
        half _Blur;
        half _Redius;
        half _Spread;
        half _Area;
        half _Offset;
        half4 _Params;
    ENDCG
    SubShader
    {
        Name "TILTSHIFT"
        Tags { "RenderType"="Opaque" }
        LOD 3040
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct v2f_tiltshift_blur
            {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            };
            

            v2f_tiltshift_blur vert (appdata v)
            {
                v2f_tiltshift_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_tiltshift_blur i) : SV_Target
            {
                
                return Hsys::Blur::Blur_TiltShift(_MainTex, i.uv, _Redius*_Blur, _Offset,half2(_Area,_Spread*_Blur),1.0/_Params,_Count);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct v2f_tiltshift_blur
            {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            };

            v2f_tiltshift_blur vert (appdata v)
            {
                v2f_tiltshift_blur o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f_tiltshift_blur i) : SV_Target
            {
                
                return Hsys::Blur::Blur_TiltShift(_MainTex, _MainTex_TexelSize,i.uv,half2(_Blur,_Area),_Count);
            }
            ENDCG
        }
    }
}
